using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using BidFX.Public.API.Price.Plugin.Pixie.Messages;
using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Price.Plugin.Pixie
{
    public class PixieConnection : ISubscriber
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private readonly AtomicBoolean _running = new AtomicBoolean(true);
        private readonly Stream _stream;
        private readonly IProviderPlugin _provider;
        private readonly BlockingCollection<AckData> _ackQueue = new BlockingCollection<AckData>();
        private volatile SubjectSetRegister _subjectSetRegister = new SubjectSetRegister();
        private long _lastSubscriptionCheckTime = 0;
        private long _lastWriteTime = 0;
        private long _subscriptionInterval = 250;
        private readonly PixieProtocolOptions _protocolOptions;
        
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
            var timeSinceLastWrite = DateTime.UtcNow.Millisecond - _lastWriteTime;
            if (timeSinceLastWrite > _protocolOptions.Heartbeat * 1000)
            {
                WriteFrame(new HeartbeatMessage());
            }
        }

        private void PeriodicallyCheckSubscriptions()
        {
            var timeNow = DateTime.UtcNow.Millisecond;
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
            var encodedMessage = message.Encode(_protocolOptions.Version);
            var frameLength = Convert.ToInt32(encodedMessage.Length);
            var buffer = encodedMessage.GetBuffer();
            Varint.WriteU32(_stream, frameLength);
            _stream.Write(buffer, 0, frameLength);
            _stream.Flush();
        }
    }
}