/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using BidFX.Public.API.Price.Plugin.Pixie.Fields;
using BidFX.Public.API.Price.Plugin.Pixie.Messages;
using BidFX.Public.API.Price.Plugin.Puffin;
using BidFX.Public.API.Price.Subject;
using BidFX.Public.API.Price.Tools;
using Serilog;
using Serilog.Events;

namespace BidFX.Public.API.Price.Plugin.Pixie
{
    internal class PixieConnection : ISubscriber
    {
        private static readonly ILogger Log = Logger.ForContext<PixieConnection>();
        
        private readonly AtomicBoolean _running = new AtomicBoolean(true);
        private readonly Stream _stream;
        private readonly IProviderPlugin _provider;
        private readonly BlockingCollection<AckData> _ackQueue = new BlockingCollection<AckData>();
        private readonly PixieProtocolOptions _protocolOptions;
        private readonly PriceSyncDecoder _priceSyncDecoder = new PriceSyncDecoder();
        private readonly GridCache _gridCache = new GridCache();

        private volatile SubjectSetRegister _subjectSetRegister;
        private IDataDictionary _dataDictionary = new ExtendableDataDictionary();
        private readonly List<Subject.Subject> _autoRefreshSubjects = new List<Subject.Subject>();
        private long _lastSubscriptionCheckTime;
        private long _lastWriteTime;
        private long _lastMsgRecTime = CurrentTimeMillis();

        private static long CurrentTimeMillis()
        {
            return DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        }

        public PixieConnection(Stream stream, IProviderPlugin provider, PixieProtocolOptions protocolOptions, string user)
        {
            _stream = stream;
            _provider = provider;
            _protocolOptions = protocolOptions;
            new Thread(SendOutgoingMessages)
            {
                Name = provider.Name + "-write",
                IsBackground = true
            }.Start();
            _subjectSetRegister = new SubjectSetRegister(user);
        }

        public void ProcessIncommingMessages()
        {
            try
            {
                while (_running.Value)
                {
                    MemoryStream message = ReadMessageFrame();
                    int msgType = message.ReadByte();
                    switch (msgType)
                    {
                        case PixieMessageType.DataDictionary:
                            DataDictionaryMessage dictionaryMessage = new DataDictionaryMessage(message);
                            OnDataDictionary(dictionaryMessage);
                            break;
                        case PixieMessageType.PriceSync:
                            long receivedTimeNanos = JavaTime.NanoTime();
                            long receivedTime = JavaTime.CurrentTimeMillis();
                            PriceSync priceSync = _priceSyncDecoder.DecodePriceSync(message);
                            OnPriceSync(priceSync);
                            _ackQueue.Add(new AckData
                            {
                                Revision = priceSync.Revision,
                                RevisionTime = priceSync.RevisionTime,
                                PriceReceivedTime = receivedTime,
                                HandlingStartNanoTime = receivedTimeNanos
                            });
                            break;
                        default:
                            Log.Debug("received message with type: {msgType}", (char) msgType);
                            break;
                    }
                }
            }
            catch (ThreadInterruptedException)
            {
                Log.Warning("thread interrupted while consuming");
            }
            catch (Exception e)
            {
                Log.Error("unexpected error reading from Pixie: " + e.Message);
            }
            finally
            {
                Close("the input stream from Pixie terminated");
            }
        }

        private void OnDataDictionary(DataDictionaryMessage dataDictionaryMessage)
        {
            if (dataDictionaryMessage.IsUpdate())
            {
                Log.Information("adding {@dataDictionaryMessage} to current DataDictionaryMessage", dataDictionaryMessage);
                DataDictionaryUtils.AddAllFields(_dataDictionary, dataDictionaryMessage.FieldDefs);
            }
            else
            {
                Log.Information("replacing DataDictionaryMessage with: {@dataDictionaryMessage}", dataDictionaryMessage);
                _dataDictionary = new ExtendableDataDictionary();
                DataDictionaryUtils.AddAllFields(_dataDictionary, dataDictionaryMessage.FieldDefs);
            }
        }

        private void OnPriceSync(PriceSync priceSync)
        {
            int edition = (int) priceSync.Edition;
            try
            {
                List<Subject.Subject> subjectSet = _subjectSetRegister.SubjectSetByEdition(edition);
                if (subjectSet == null)
                {
                    Log.Error("tried to get edition " + edition +
                              " but SubjectSetRegister returned null. Cannot process PriceSync!");
                    throw new IllegalStateException("received PriceSync for edition " + edition +
                                                    " but it's not in the SubjectSetRegister.");
                }

                PriceSyncVisitor visitor = new PriceSyncVisitor(subjectSet, this);
                IGridHeaderRegistry gridHeaderRegistryByEdition = _subjectSetRegister.GetGridHeaderRegistryByEdition(edition);
                priceSync.Visit(_dataDictionary, gridHeaderRegistryByEdition, visitor);
            }
            catch (Exception e)
            {
                Log.Warning("Could not process PriceSync due to {exception}", e);
                throw e;
            }
        }

        public void Subscribe(Subject.Subject subject, bool autoRefresh = false, bool refresh = false)
        {
            _gridCache.Add(subject);
            _subjectSetRegister.Register(subject, refresh);
            if (autoRefresh && "Quote".Equals(subject.GetComponent(SubjectComponentName.RequestFor)))
            {
                _autoRefreshSubjects.Add(subject);
            }
        }

        public void Unsubscribe(Subject.Subject subject)
        {
            _gridCache.Remove(subject);
            _subjectSetRegister.Unregister(subject);
            if (_autoRefreshSubjects.Contains(subject))
            {
                _autoRefreshSubjects.Remove(subject);
            }
        }

        public void Close(string reason)
        {
            if (_running.CompareAndSet(true, false))
            {
                _gridCache.Reset();
                Log.Information("closing connection to Pixie ({reason})", reason);
                _stream.Close();
            }
        }

        private void SendOutgoingMessages()
        {
            try
            {
                while (_running.Value)
                {
                    AckData ackData = PollNextAck();
                    if (ackData != null)
                    {
                        WriteFrame(ackData.ToAckMessage());
                    }
                    else
                    {
                        TryAndWriteHeartbeat();
                    }
                    PeriodicallyCheckSubscriptions();
                }
            }
            catch (OperationCanceledException)
            {
                Log.Information("finished writing to Pixie"); // closed already called
            }
            catch (Exception e)
            {
                Log.Error("unexpected error occured in the publisher thread.", e);
                Close(e.Message);
            }
        }

        private AckData PollNextAck()
        {
            AckData ackData;
            long timeout = Math.Max(1, _protocolOptions.SubscriptionInterval / 4);
            _ackQueue.TryTake(out ackData, TimeSpan.FromMilliseconds(timeout));
            return ackData;
        }

        private void TryAndWriteHeartbeat()
        {
            long timeSinceLastWrite = CurrentTimeMillis() - _lastWriteTime;
            if (timeSinceLastWrite > _protocolOptions.Heartbeat * 1000)
            {
                WriteFrame(new HeartbeatMessage());
            }
        }

        private void PeriodicallyCheckSubscriptions()
        {
            long timeNow = CurrentTimeMillis();
            if (timeNow - _lastSubscriptionCheckTime > _protocolOptions.SubscriptionInterval)
            {
                _lastSubscriptionCheckTime = timeNow;
                CheckAndSendSubscriptions();
            }
        }

        private void CheckAndSendSubscriptions()
        {
            SubscriptionSync subscriptionSync = _subjectSetRegister.NextSubscriptionSync();
            if (subscriptionSync != null)
            {
                if (subscriptionSync.IsChangedEdition() || _protocolOptions.Version >= PixieVersion.Version2)
                {
                    subscriptionSync.SetCompressed(_protocolOptions.CompressSubscriptions);
                    Log.Information("synchronising subscriptions with {subscriptionSync}",subscriptionSync.Summarize());
                    WriteFrame(subscriptionSync);
                }
            }
        }

        private void WriteFrame(IOutgoingPixieMessage message)
        {
            Log.Debug("sending message: {message}", message);

            MemoryStream encodedMessage = message.Encode(_protocolOptions.Version);
            int frameLength = Convert.ToInt32(encodedMessage.Length);
            byte[] buffer = encodedMessage.ToArray();
            Varint.WriteU32(_stream, frameLength);
            _stream.Write(buffer, 0, frameLength);
            _stream.Flush();
            _lastWriteTime = CurrentTimeMillis();
        }

        private MemoryStream ReadMessageFrame()
        {
            UpdateIdleTimeout();
            uint frameLength = Varint.ReadU32(_stream);
            if (frameLength == 0)
            {
                throw new IOException("unexpected empty Pixie message frame");
            }

            byte[] frameBuffer = new byte[frameLength];
            int totalRead = 0;
            while (totalRead < frameLength)
            {
                UpdateIdleTimeout();
                int got = _stream.Read(frameBuffer, totalRead, (int) frameLength - totalRead);
                if (got == -1)
                {
                    throw new IOException("end of message stream reached (perhaps the server closed the connection)");
                }
                totalRead += got;
            }
            _lastMsgRecTime = CurrentTimeMillis();
            return new MemoryStream(frameBuffer);
        }

        private void UpdateIdleTimeout()
        {
            _stream.ReadTimeout = CalculateTimeout();
        }

        private int CalculateTimeout()
        {
            int fullIdleTimeoutMs = _protocolOptions.Idle * 1000;
            long timeSinceRecLastMsgMs = CurrentTimeMillis() - _lastMsgRecTime;
            int remainingTimeout = (int) (fullIdleTimeoutMs - timeSinceRecLastMsgMs);
            return Math.Max(1, remainingTimeout);
        }

        public SubjectSetRegister SubjectSetRegister
        {
            get { return _subjectSetRegister; }
        }

        public GridCache GridCache
        {
            get { return _gridCache; }
        }

        public IApiEventHandler EventHandler
        {
            get { return _provider.InapiEventHandler; }
        }

        public List<Subject.Subject> AutoRefreshSubjects
        {
            get { return _autoRefreshSubjects; }
        }

        private class PriceSyncVisitor : ISyncable
        {
            private readonly List<Subject.Subject> _subjectSet;
            private readonly PixieConnection _pixieConnection;

            private Subject.Subject _gridSubject;
            private Grid _grid;

            internal PriceSyncVisitor(List<Subject.Subject> subjectSet, PixieConnection pixieConnection)
            {
                _subjectSet = Params.NotNull(subjectSet);
                _pixieConnection = pixieConnection;
            }

            public void PriceImage(int sid, Dictionary<string, object> price)
            {
                Log.Debug("received price image SID: {sid}, price: {price}", sid, price);

                HandlePriceUpdateEvent(sid, price, true);
            }

            public void PriceUpdate(int sid, Dictionary<string, object> price)
            {
                Log.Debug("received price update SID: {sid}, price: {price}", sid, price);

                HandlePriceUpdateEvent(sid, price, false);
            }

            private void HandlePriceUpdateEvent(int sid, Dictionary<string, object> price, bool replaceAllFields)
            {
                if (sid >= _subjectSet.Count)
                {
                    return;
                }

                Subject.Subject subject = _subjectSet[sid];

                if (!_pixieConnection.SubjectSetRegister.IsCurrentlySubscribed(subject))
                {
                    return;
                }

                CalculateHopLatency2(price);
                IPriceMap priceMap = CreatePriceMap(price);
                _pixieConnection.EventHandler.OnPriceUpdate(subject, priceMap, replaceAllFields);
            }

            private static IPriceMap CreatePriceMap(Dictionary<string, object> price)
            {
                PriceMap priceMap = new PriceMap();
                foreach (KeyValuePair<string, object> attribute in price)
                {
                    priceMap.SetField(attribute.Key, new PriceField(attribute.Value.ToString(), attribute.Value));
                }
                return priceMap;
            }

            private void CalculateHopLatency2(Dictionary<string, object> price)
            {
                try
                {
                    object sysTime = price["SystemTime"];
                    long sysTimeLong = Convert.ToInt64(sysTime);
                    {
                        long now = JavaTime.CurrentTimeMillis();
                        long hopLatency2 = now - (long) sysTimeLong;
                        price["HopLatency2"] = hopLatency2;
                    }
                }
                catch (Exception e)
                {
                    //no-op
                }
            }

            public void PriceStatus(int sid, SubscriptionStatus status, string explanation)
            {
                if (Log.IsEnabled(LogEventLevel.Debug))
                {
                    Log.Debug("received price status SID: {sid}, status: {status}, text: {text}", sid, status, explanation);
                }

                if (sid >= _subjectSet.Count)
                {
                    return;
                }

                Subject.Subject subject = _subjectSet[sid];

                if (status.Equals(SubscriptionStatus.CLOSED) && _pixieConnection.AutoRefreshSubjects.Contains(subject))
                {
                    _pixieConnection.Subscribe(subject, true);
                }

                if (!_pixieConnection.SubjectSetRegister.IsCurrentlySubscribed(subject))
                {
                    return;
                }

                _pixieConnection.EventHandler.OnSubscriptionStatus(subject, status, explanation);
            }

            public void StartGridImage(int sid, int columnCount)
            {
                if (Log.IsEnabled(LogEventLevel.Debug))
                {
                    Log.Debug("start grid image SID: {sid}, columnCount: {columnCount}", sid, columnCount);
                }

                if (sid >= _subjectSet.Count)
                {
                    return;
                }

                _gridSubject = _subjectSet[sid];
                _grid = _pixieConnection.GridCache.Get(_gridSubject);
                if (_grid != null)
                {
                    _grid.StartGridImage(sid, columnCount);
                }
            }

            public void ColumnImage(string name, int rowCount, object[] columnValues)
            {
                if (_grid != null)
                {
                    _grid.ColumnImage(name, rowCount, columnValues);
                }
            }

            public void EndGridImage()
            {
                if (_grid != null)
                {
                    _grid.EndGridImage();
                    PublishGrid();
                }
            }

            private void PublishGrid()
            {
                Dictionary<string, object> map = new Dictionary<string, object>();
                string[] columnNames = _grid.ColumnNames;
                int numBidLevels = 0;
                int numAskLevels = 0;
                for (int i = 0; i < _grid.NumberOfColumns; i++)
                {
                    string columnName = columnNames[i];
                    IColumn column = _grid.GetColumn(i);
                    AddColumn(map, columnName, column);
                    if ("Bid".Equals(columnName))
                    {
                        numBidLevels = _grid.GetColumn(i).Size();
                    }
                    else if ("Ask".Equals(columnName))
                    {
                        numAskLevels = _grid.GetColumn(i).Size();
                    }
                }

                map["BidLevels"] = numBidLevels;
                map["AskLevels"] = numAskLevels;
                IPriceMap priceMap = CreatePriceMap(map);
                _pixieConnection.EventHandler.OnPriceUpdate(_gridSubject, priceMap, false);
            }

            private void AddColumn(Dictionary<string, object> map, string columnName, IColumn column)
            {
                for (int i = 0; i < column.Size(); i++)
                {
                    int rowNum = i + 1;
                    map[columnName + rowNum] = column.Get(i);
                }
            }

            public void StartGridUpdate(int sid, int columnCount)
            {
                Log.Debug("start grid update SID: {sid}, columnCount: {columnCount}", sid, columnCount);

                if (sid >= _subjectSet.Count)
                {
                    return;
                }

                _gridSubject = _subjectSet[sid];
                _grid = _pixieConnection.GridCache.Get(_gridSubject);
                if (_grid != null)
                {
                    _grid.StartGridUpdate(sid, columnCount);
                }
            }

            public void FullColumnUpdate(string name, int cid, int rowCount, object[] columnValues)
            {
                if (_grid != null)
                {
                    _grid.FullColumnUpdate(name, cid, rowCount, columnValues);
                }
            }

            public void PartialColumnUpdate(string name, int cid, int rowCount, object[] columnValues, int[] rids)
            {
                if (_grid != null)
                {
                    _grid.PartialColumnUpdate(name, cid, rowCount, columnValues, rids);
                }
            }

            public void PartialTruncatedColumnUpdate(string name, int cid, int rowCount, object[] columnValues,
                int[] rids,
                int truncatedRid)
            {
                if (_grid != null)
                {
                    _grid.PartialTruncatedColumnUpdate(name, cid, rowCount, columnValues, rids, truncatedRid);
                }
            }

            public void EndGridUpdate()
            {
                if (_grid != null)
                {
                    _grid.EndGridUpdate();
                    PublishGrid();
                }
            }
        }
    }
}