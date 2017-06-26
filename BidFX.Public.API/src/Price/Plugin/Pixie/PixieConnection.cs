using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Threading;
using BidFX.Public.API.Price.Plugin.Pixie.Fields;
using BidFX.Public.API.Price.Plugin.Pixie.Messages;
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

        public long SubscriptionInterval { get; set; }
        public bool CompressSubscriptions { get; set; }

        public PixieConnection(Stream stream, IProviderPlugin provider, PixieProtocolOptions protocolOptions)
        {
            _stream = stream;
            _provider = provider;
            _protocolOptions = protocolOptions;
            SubscriptionInterval = 250L;
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
                var gridHeaderRegistryByEdition = _subjectSetRegister.GetGridHeaderRegistryByEdition(subjectSet);
                priceSync.Visit(_dataDictionary, gridHeaderRegistryByEdition);
            }
            catch (Exception e)
            {
                Log.Warn("Could not process PricSync due to " + e);
                throw e;
            }
        }

        private MemoryStream ReadMessageFrame()
        {
            var frameLength = Varint.ReadU32(_stream);
            if (frameLength == 0) throw new IOException("unexpected empty Pixie message frame");
            var frameBuffer = new byte[frameLength];
            var totalRead = 0;
            while (totalRead < frameLength)
            {
                int got = _stream.Read(frameBuffer, totalRead, (int) frameLength - totalRead);
                if (got == -1)
                {
                    throw new IOException("end of message stream reached (perhaps the server closed the connection)");
                }
                totalRead += got;
            }
            return new MemoryStream(frameBuffer);
        }

        public void Subscribe(Subject.Subject subject, bool refresh = false)
        {
            _subjectSetRegister.Register(subject, refresh);
        }

        public void Unsubscribe(Subject.Subject subject)
        {
            _subjectSetRegister.Unregister(subject);
        }

        public void Close(string reason)
        {
//            throw new System.NotImplementedException();
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
                Log.Info("finished writing to Puffin"); // closed already called
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
            var timeSinceLastWrite = (DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond) - _lastWriteTime;
            if (timeSinceLastWrite > _protocolOptions.Heartbeat * 1000)
            {
                WriteFrame(new HeartbeatMessage());
            }
        }

        private void PeriodicallyCheckSubscriptions()
        {
            var timeNow = (DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond);
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
                if (subscriptionSync.IsChangedEdition() || _protocolOptions.Version >= 2)
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
            var encodedMessage = message.Encode((int) _protocolOptions.Version);
            var frameLength = Convert.ToInt32(encodedMessage.Length);
            var buffer = encodedMessage.ToArray();
            Varint.WriteU32(_stream, frameLength);
            _stream.Write(buffer, 0, frameLength);
            _stream.Flush();
            _lastWriteTime = (DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond);
        }
    }
}