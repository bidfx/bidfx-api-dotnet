using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using BidFX.Public.API.Price.Plugin.Pixie.Fields;
using BidFX.Public.API.Price.Plugin.Pixie.Messages;
using BidFX.Public.API.Price.Plugin.Puffin;
using BidFX.Public.API.Price.Tools;
using log4net;

namespace BidFX.Public.API.Price.Plugin.Pixie
{
    public class PixieConnection : ISubscriber
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly AtomicBoolean _running = new AtomicBoolean(true);
        private readonly Stream _stream;
        private readonly IProviderPlugin _provider;
        private readonly BlockingCollection<AckData> _ackQueue = new BlockingCollection<AckData>();
        private IDataDictionary _dataDictionary = new ExtendableDataDictionary();
        private volatile SubjectSetRegister _subjectSetRegister = new SubjectSetRegister();
        private long _lastSubscriptionCheckTime = 0;
        private long _lastWriteTime = 0;
        private long _subscriptionInterval = 250;
        private readonly PixieProtocolOptions _protocolOptions;
        private readonly PriceSyncDecoder _priceSyncDecoder = new PriceSyncDecoder();
        private readonly GridCache _gridCache = new GridCache();
        private long _lastMsgRecTime = CurrentTimeMillis();

        private static long CurrentTimeMillis()
        {
            return DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        }

        public bool CompressSubscriptions { get; set; }

        public PixieConnection(Stream stream, IProviderPlugin provider, PixieProtocolOptions protocolOptions)
        {
            _stream = stream;
            _provider = provider;
            _protocolOptions = protocolOptions;
            CompressSubscriptions = true;
            new Thread(SendOutgoingMessages)
            {
                Name = provider.Name + "-write",
                IsBackground = true
            }.Start();
        }

        public void ProcessIncommingMessages()
        {
            try
            {
                while (_running.Value)
                {
                    var message = ReadMessageFrame();
                    var msgType = message.ReadByte();
                    switch (msgType)
                    {
                        case PixieMessageType.DataDictionary:
                            var dictionaryMessage = new DataDictionaryMessage(message);
                            OnDataDictionary(dictionaryMessage);
                            break;
                        case PixieMessageType.PriceSync:
                            var receivedTimeNanos = JavaTime.NanoTime();
                            var receivedTime = JavaTime.CurrentTimeMillis();
                            var priceSync = _priceSyncDecoder.DecodePriceSync(message);
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
                            if (Log.IsDebugEnabled) Log.Debug("received message with type: " + (char) msgType);
                            break;
                    }
                }
            }
            catch (ThreadInterruptedException)
            {
                Log.Warn("thread interrupted while consuming");
            }
            catch (Exception e)
            {
                Log.Error("unexpected error reading from Puffin: " + e.Message);
            }
            finally
            {
                Close("the input stream from Puffin terminated");
            }
        }

        private void OnDataDictionary(DataDictionaryMessage dataDictionaryMessage)
        {
            if (dataDictionaryMessage.IsUpdate())
            {
                Log.Info("adding " + dataDictionaryMessage + " to current DataDictionaryMessage");
                DataDictionaryUtils.AddAllFields(_dataDictionary, dataDictionaryMessage.FieldDefs);
            }
            else
            {
                Log.Info("replacing DataDictionaryMessage with: " + dataDictionaryMessage);
                _dataDictionary = new ExtendableDataDictionary();
                DataDictionaryUtils.AddAllFields(_dataDictionary, dataDictionaryMessage.FieldDefs);
            }
        }

        private void OnPriceSync(PriceSync priceSync)
        {
            var edition = (int) priceSync.Edition;
            try
            {
                var subjectSet = _subjectSetRegister.SubjectSetByEdition(edition);
                if (subjectSet == null)
                {
                    Log.Error("tried to get edition " + edition +
                              " but SubjectSetRegister returned null. Cannot process PriceSync!");
                    throw new IllegalStateException("received PriceSync for edition " + edition +
                                                    " but it's not in the SubjectSetRegister.");
                }
                var visitor = new PriceSyncVisitor(subjectSet, this);
                var gridHeaderRegistryByEdition = _subjectSetRegister.GetGridHeaderRegistryByEdition(subjectSet);
                priceSync.Visit(_dataDictionary, gridHeaderRegistryByEdition, visitor);
            }
            catch (Exception e)
            {
                Log.Warn("Could not process PricSync due to " + e);
                throw e;
            }
        }

        public void Subscribe(Subject.Subject subject, bool refresh = false)
        {
            _gridCache.Add(subject);
            _subjectSetRegister.Register(subject, refresh);
        }

        public void Unsubscribe(Subject.Subject subject)
        {
            _gridCache.Remove(subject);
            _subjectSetRegister.Unregister(subject);
        }

        public void Close(string reason)
        {
            if (_running.CompareAndSet(true, false))
            {
                _gridCache.Reset();
                Log.Info("closing connection to Pixie (" + reason + ")");
                _stream.Close();
            }
        }

        private void SendOutgoingMessages()
        {
            try
            {
                while (_running.Value)
                {
                    var ackData = PollNextAck();
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
                Log.Info("finished writing to Pixie"); // closed already called
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
            var timeout = Math.Max(1, SubscriptionInterval / 4);
            _ackQueue.TryTake(out ackData, TimeSpan.FromMilliseconds(timeout));
            return ackData;
        }

        private void TryAndWriteHeartbeat()
        {
            var timeSinceLastWrite = CurrentTimeMillis() - _lastWriteTime;
            if (timeSinceLastWrite > _protocolOptions.Heartbeat * 1000)
            {
                WriteFrame(new HeartbeatMessage());
            }
        }

        private void PeriodicallyCheckSubscriptions()
        {
            var timeNow = CurrentTimeMillis();
            if (timeNow - _lastSubscriptionCheckTime > _subscriptionInterval)
            {
                _lastSubscriptionCheckTime = timeNow;
                CheckAndSendSubscriptions();
            }
        }

        private void CheckAndSendSubscriptions()
        {
            var subscriptionSync = _subjectSetRegister.NextSubscriptionSync();
            if (subscriptionSync != null)
            {
                if (subscriptionSync.IsChangedEdition() || _protocolOptions.Version >= PixieVersion.Version2)
                {
                    subscriptionSync.SetCompressed(CompressSubscriptions);
                    Log.Info("syncronising subscriptions with " + subscriptionSync.Summarize());
                    WriteFrame(subscriptionSync);
                }
            }
        }

        private void WriteFrame(IOutgoingPixieMessage message)
        {
            if (Log.IsDebugEnabled) Log.Debug("sending message: " + message);
            var encodedMessage = message.Encode(_protocolOptions.Version);
            var frameLength = Convert.ToInt32(encodedMessage.Length);
            var buffer = encodedMessage.ToArray();
            Varint.WriteU32(_stream, frameLength);
            _stream.Write(buffer, 0, frameLength);
            _stream.Flush();
            _lastWriteTime = CurrentTimeMillis();
        }

        private MemoryStream ReadMessageFrame()
        {
            UpdateIdleTimeout();
            var frameLength = Varint.ReadU32(_stream);
            if (frameLength == 0) throw new IOException("unexpected empty Pixie message frame");
            var frameBuffer = new byte[frameLength];
            var totalRead = 0;
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
            var fullIdleTimeoutMs = _protocolOptions.Idle * 1000;
            var timeSinceRecLastMsgMs = CurrentTimeMillis() - _lastMsgRecTime;
            var remainingTimeout = (int) (fullIdleTimeoutMs - timeSinceRecLastMsgMs);
            return Math.Max(1, remainingTimeout);
        }

        public long SubscriptionInterval
        {
            get { return _subscriptionInterval; }
            set { _subscriptionInterval = Params.InRange(value, 25L, 5000L); }
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
                if (Log.IsDebugEnabled) Log.Debug("received price image SID: " + sid + ", price: " + price);
                HandlePriceUpdateEvent(sid, price, true);
            }

            public void PriceUpdate(int sid, Dictionary<string, object> price)
            {
                if (Log.IsDebugEnabled) Log.Debug("received price update SID: " + sid + ", price: " + price);
                HandlePriceUpdateEvent(sid, price, false);
            }

            private void HandlePriceUpdateEvent(int sid, Dictionary<string, object> price, bool replaceAllFields)
            {
                if (sid >= _subjectSet.Count) return;

                var subject = _subjectSet[sid];

                if (!_pixieConnection.SubjectSetRegister.IsCurrentlySubscribed(subject)) return;

                CalculateHopLatency2(price);
                var priceMap = CreatePriceMap(price);
                _pixieConnection.EventHandler.OnPriceUpdate(subject, priceMap, replaceAllFields);
            }

            private static IPriceMap CreatePriceMap(Dictionary<string, object> price)
            {
                var priceMap = new PriceMap();
                foreach (var attribute in price)
                {
                    priceMap.SetField(attribute.Key, new PriceField(attribute.Value.ToString(), attribute.Value));
                }
                return priceMap;
            }

            private void CalculateHopLatency2(Dictionary<string, object> price)
            {
                try
                {
                    var sysTime = price["SystemTime"];
                    var sysTimeLong = Convert.ToInt64(sysTime);
                    {
                        var now = JavaTime.CurrentTimeMillis();
                        var hopLatency2 = now - (long) sysTimeLong;
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
                if (Log.IsDebugEnabled)
                    Log.Debug("received price statis SID: " + sid + ", status: " + status + ", text: " + explanation);

                if (sid >= _subjectSet.Count) return;

                var subject = _subjectSet[sid];

                if (!_pixieConnection.SubjectSetRegister.IsCurrentlySubscribed(subject)) return;

                _pixieConnection.EventHandler.OnSubscriptionStatus(subject, status, explanation);
            }

            public void StartGridImage(int sid, int columnCount)
            {
                if (Log.IsDebugEnabled) Log.Debug("start grid image SID: " + sid + ", columnCount: " + columnCount);

                if (sid >= _subjectSet.Count) return;

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
                var map = new Dictionary<string, object>();
                var columnNames = _grid.ColumnNames;
                var numBidLevels = 0;
                var numAskLevels = 0;
                for (var i = 0; i < _grid.NumberOfColumns; i++)
                {
                    var columnName = columnNames[i];
                    var column = _grid.GetColumn(i);
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
                var priceMap = CreatePriceMap(map);
                _pixieConnection.EventHandler.OnPriceUpdate(_gridSubject, priceMap, false);
            }

            private void AddColumn(Dictionary<string, object> map, string columnName, IColumn column)
            {
                for (var i = 0; i < column.Size(); i++)
                {
                    var rowNum = i + 1;
                    map[columnName + rowNum] = column.Get(i);
                }
            }

            public void StartGridUpdate(int sid, int columnCount)
            {
                if (Log.IsDebugEnabled) Log.Debug("start grid update SID: " + sid + ", columnCount: " + columnCount);

                if (sid >= _subjectSet.Count) return;

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