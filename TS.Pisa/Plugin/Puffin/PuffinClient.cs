using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TS.Pisa.Tools;

namespace TS.Pisa.Plugin.Puffin
{
    public class PuffinClient : IPuffinRequestor
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly AtomicBoolean _running = new AtomicBoolean(false);
        private readonly Thread _consumerThread;
        private readonly Stream _stream;
        private readonly string _name;
        private readonly BlockingCollection<string> _messageQueue = new BlockingCollection<string>();
        private readonly PuffinMessageReader _puffinMessageReader;
        private readonly PuffinMessageReceiver _messageReceiver = new PuffinMessageReceiver();

        public delegate void OnHeartbeatListener(long interval, long transmitTime, long receiveTime, bool clockSync);

        public PuffinClient(Stream stream, string name, EventHandler<PriceUpdateEventArgs> priceUpdate)
        {
            _stream = stream;
            _name = name;
            _consumerThread = new Thread(RunningLoop) {Name = _name + "-read"};
            _puffinMessageReader = new PuffinMessageReader(stream);
            _messageReceiver.OnHeartbeatListener = HandleHeartbeat;
            _messageReceiver.PriceUpdate = priceUpdate;
        }

        public void Start()
        {
            if (_running.CompareAndSet(false, true))
            {
                _consumerThread.Start();
            }
            Publish();
        }

        private void RunningLoop()
        {
            while (_running.Value)
            {
                Consume();
            }
        }

        /// <summary>
        /// Consume and process the input from the socket.
        /// </summary>
        private void Consume()
        {
            var message = _puffinMessageReader.ReadMessage();
            if (Log.IsDebugEnabled) Log.Debug("received: ");
            _messageReceiver.OnMessage(message);
        }

        /// <summary>
        /// Publish queued messaged to the socket.
        /// </summary>
        private void Publish()
        {
            while (_running.Value)
            {
                var message = _messageQueue.Take();
                if (Log.IsDebugEnabled) Log.Debug(_name + " was sent: " + message);
                _stream.Write(Encoding.ASCII.GetBytes(message), 0, message.Length);
                _stream.Flush();
            }
        }

        /// <summary>
        /// Place a message on the socket's outgoing message queue.
        /// This method will block if the message queue is full.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public void QueueMessage(string message)
        {
            if (Log.IsDebugEnabled) Log.Debug(_name + " queuing message: " + message);
            if (_running.Value)
            {
                _messageQueue.Add(message);
            }
        }

        private void Close(string reason)
        {
            if (Log.IsDebugEnabled) Log.Debug(_name + " socket close (" + reason + ")");
            if (RequiresDisposal())
            {
                Log.Info(_name + " disconnected (" + reason + ")");
                Dispose();
            }
        }

        private Boolean RequiresDisposal()
        {
            return _running.CompareAndSet(true, false);
        }

        private void Dispose()
        {
            _stream.Close();
            _messageQueue.Dispose();
        }

        private void HandleHeartbeat(long interval, long transmitTime, long receiveTime, bool syncClock)
        {
            if (syncClock)
            {
                SendSyncClock(transmitTime, receiveTime);
            }
            else
            {
                SendHeartbeat();
            }
        }

        public void Subscribe(string subject)
        {
            QueueMessage("<Subscribe Subject=\"" + subject + "\"/>");
        }

        public void Unsubscribe(string subject)
        {
            QueueMessage("<Unsubscribe Subject=\"" + subject + "\"/>");
        }

        private void SendHeartbeat()
        {
            var now = JavaTime.CurrentTimeMillis();
            QueueMessage("<Heartbeat TransmitTime=\"" + now + "\"/>");
            Log.Info(this);
        }

        private void SendSyncClock(long originateTime, long receiveTime)
        {
            QueueMessage("<SyncClock OriginateTime=\"" + originateTime + "\" ReceiveTime=\"" + receiveTime +
                         "\" TransmitTime=\"" + JavaTime.CurrentTimeMillis() + "\"/>");
        }

        public void CloseSession()
        {
            Close("session close requested");
        }

        public void CheckSessionIsActive()
        {
            throw new System.NotImplementedException();
        }
    }
}