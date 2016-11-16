using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;

namespace TS.Pisa.Plugin.Puffin
{
    public class PuffinClient : IPuffinRequestor
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly AtomicBoolean _running = new AtomicBoolean(false);
        private Thread _consumerThread;
        private Stream _stream;
        private BlockingCollection<string> _messageQueue = new BlockingCollection<string>();

        public PuffinClient(Stream stream)
        {
            _stream = stream;
            _consumerThread = new Thread(RunningLoop) {Name = _consumerThread + "-read"};
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
            //Todo add implementation
        }

        /// <summary>
        /// Publish queued messaged to the socket.
        /// </summary>
        private void Publish()
        {
            while (_running.Value)
            {
                var message = _messageQueue.Take();
                if(Log.IsDebugEnabled) Log.Debug("sending message: "+message);
                _stream.Write(Encoding.ASCII.GetBytes(message), 0, message.Length);
            }
        }

        public void QueueMessage(string message)
        {
            if(Log.IsDebugEnabled) Log.Debug("queuing message: "+message);
            _messageQueue.Add(message);
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
            QueueMessage("<Heartbeat/>");
        }

        public void CloseSession()
        {
            throw new System.NotImplementedException();
        }

        public void CheckSessionIsActive()
        {
            throw new System.NotImplementedException();
        }
    }
}