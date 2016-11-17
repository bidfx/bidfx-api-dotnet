using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using TS.Pisa.Tools;
using Timer = System.Timers.Timer;

namespace TS.Pisa.Plugin.Puffin
{
    public class PuffinClient : IPuffinRequestor
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly AtomicBoolean _running = new AtomicBoolean(false);
        private readonly Thread _consumerThread;
        private readonly Thread _intervalThread;
        private readonly Stream _stream;
        private readonly string _name;
        private readonly BlockingCollection<string> _messageQueue = new BlockingCollection<string>();
        private readonly PuffinMessageReader _puffinMessageReader;
        private readonly PuffinMessageReceiver _messageReceiver;
        private readonly PuffinMessageReceiver _messageReceiver = new PuffinMessageReceiver();
        public int Interval { get; set; }
        private bool _failed = false;

        public delegate void OnHeartbeatListener(int interval, long transmitTime, long receiveTime, bool clockSync);

        public PuffinClient(Stream stream, IProviderPlugin provider)
        {
            _stream = stream;
            _name = provider.Name;
            _consumerThread = new Thread(RunningLoop) {Name = _name + "-read"};
            _name = name;
            Interval = 60000;
            _consumerThread = new Thread(Consume) {Name = _name + "-read"};
            _intervalThread = new Thread(ScheduleHeartbeat) {Name = _name + "-interval"};
            _puffinMessageReader = new PuffinMessageReader(stream);
            _messageReceiver = new PuffinMessageReceiver(provider);
            _messageReceiver.OnHeartbeatListener = HandleHeartbeat;
            _messageReceiver.PriceUpdate = priceUpdate;

        }

        public void Start()
        {
            if (_running.CompareAndSet(false, true))
            {
                _consumerThread.Start();
                _intervalThread.Start();
            }
            Publish();
        }

        /// <summary>
        /// Schedules the heartbeat to be sent every interval and checks if the message stream is still active
        /// </summary>
        private void ScheduleHeartbeat()
        {
            while (_running.Value)
            {
                if (IsMessageStreamActive())
                {
                    SendHeartbeat();
                    if(Log.IsDebugEnabled) Log.Debug("Sleeping for "+Interval);
                    Thread.Sleep(Interval);
                }
                else
                {
                    Log.Warn("No message have been received for double the interval time; assume connection failure");
                    Close("inactive connection");
                }
            }
        }

        /// <summary>
        /// Consumes input from the stream and routes it to the message receiver
        /// </summary>
        private void Consume()
        {
            var message = _puffinMessageReader.ReadMessage();
            if (Log.IsDebugEnabled) Log.Debug("received: "+message);
            _messageReceiver.OnMessage(message);
            try
            {
                while (_running.Value)
                {
                    var message = _puffinMessageReader.ReadMessage();
                    if (Log.IsDebugEnabled) Log.Debug(_name+" received message: "+message);
                    _messageReceiver.OnMessage(message);
                }
            }
            catch (ThreadInterruptedException e)
            {
                Log.Warn("thread interrupted while consuming");
            }
        }

        private long GetTimeOfLastMessage()
        {
            return _messageReceiver.TimeOfLastMessage;
        }

        private bool IsMessageStreamActive()
        {
            var time = JavaTime.CurrentTimeMillis() - Interval * 2;
            return GetTimeOfLastMessage() > time;
        }

        /// <summary>
        /// Publish queued messaged to the socket.
        /// Does not block.
        /// </summary>
        private void Publish()
        {
            while (_running.Value)
            {
                string message;
                if (_messageQueue.TryTake(out message))
                {
                    if (Log.IsDebugEnabled) Log.Debug(_name + " sending message: " + message);
                    _stream.Write(Encoding.ASCII.GetBytes(message), 0, message.Length);
                    _stream.Flush();
                }
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

        /// <summary>
        /// Closes the stream, disposes the message queue and interrupts the consumer thread.
        /// </summary>
        /// <param name="reason">The reason to close the stream.</param>
        private void Close(string reason)
        {
            if (Log.IsDebugEnabled) Log.Debug(_name + " stream close (" + reason + ")");
            if (_running.CompareAndSet(true, false))
            {
                Log.Info(_name + " disconnected (" + reason + ")");
                _consumerThread.Interrupt();
                _stream.Close();
                _messageQueue.Dispose();
            }
        }

        private void HandleHeartbeat(int interval, long transmitTime, long receiveTime, bool syncClock)
        {
            if (syncClock)
            {
                SendSyncClock(transmitTime, receiveTime);
            }
        }

        public void Subscribe(string subject)
        {
            QueueMessage(new PuffinElement(PuffinTagName.Subscribe)
                .AddAttribute("Subject", subject)
                .ToString());
        }

        public void Unsubscribe(string subject)
        {
            QueueMessage(new PuffinElement(PuffinTagName.Unsubscribe)
                .AddAttribute("Subject", subject)
                .ToString());
        }

        private void SendHeartbeat()
        {
            var now = JavaTime.CurrentTimeMillis();
            QueueMessage(new PuffinElement(PuffinTagName.Heartbeat)
                .AddAttribute("TransmitTime", now)
                .ToString());
        }

        private void SendSyncClock(long originateTime, long receiveTime)
        {
            QueueMessage(new PuffinElement(PuffinTagName.ClockSync)
                .AddAttribute("OriginateTime", originateTime)
                .AddAttribute("ReceiveTime", receiveTime)
                .AddAttribute("TransmitTime", JavaTime.CurrentTimeMillis())
                .ToString());
        }

        public void CloseSession()
        {
            Close("session close requested");
        }

        public void CheckSessionIsActive()
        {
            if (IsMessageStreamActive())
            {
                Log.Warn("No message have been received for double the interval time; assume connection failure");
                Close("inactive connection");
            }
        }
    }
}