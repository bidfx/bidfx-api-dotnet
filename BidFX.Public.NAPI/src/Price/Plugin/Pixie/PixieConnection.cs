using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using BidFX.Public.NAPI.Price.Plugin.Pixie.Messages;
using BidFX.Public.NAPI.Price.Tools;

namespace BidFX.Public.NAPI.Price.Plugin.Pixie
{
    public class PixieConnection : ISubscriber
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private readonly AtomicBoolean _running = new AtomicBoolean(true);
        private readonly Stream _stream;
        private readonly IProviderPlugin _provider;
        private readonly BlockingCollection<AckData> _ackQueue = new BlockingCollection<AckData>();
        
        public long SubscriptionInterval { get; set; }

        public PixieConnection(Stream stream, IProviderPlugin provider)
        {
            _stream = stream;
            _provider = provider;
            SubscriptionInterval = 250L;
            new Thread(SendOutgoingMessages)
            {
                Name = provider.Name + "-write",
                IsBackground = true
            }.Start();
        }
        
        public void Subscribe(string subject)
        {
            throw new System.NotImplementedException();
        }

        public void Unsubscribe(string subject)
        {
            throw new System.NotImplementedException();
        }

        public void Close(string reason)
        {
            throw new System.NotImplementedException();
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
                        
                    }
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

        private void WriteMesasge(IOutgoingPixieMessage message)
        {
////            var memoryStream = message.Encode(_negotiatedVersion);
//            var frameLength = Convert.ToInt32(memoryStream.Length);
//            var buffer = memoryStream.GetBuffer();
//            Varint.WriteU32(_stream, frameLength);
//            _stream.Write(buffer, 0, frameLength);
//            _stream.Flush();
        }
    }
}