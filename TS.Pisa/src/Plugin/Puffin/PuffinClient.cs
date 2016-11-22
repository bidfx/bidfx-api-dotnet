using System;
using System.Collections.Concurrent;
using System.IO;
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
        private readonly Thread _heartbeatThread;
        private readonly Stream _stream;
        private readonly string _name;
        private readonly BlockingCollection<string> _messageQueue = new BlockingCollection<string>();
        private readonly PuffinMessageReader _puffinMessageReader;
        private readonly PuffinMessageReceiver _messageReceiver;
        public int Interval { get; set; }

        public delegate void OnHeartbeatListener(int interval, long transmitTime, long receiveTime, bool clockSync);

        public PuffinClient(Stream stream, IProviderPlugin provider)
        {
            _stream = stream;
            _name = provider.Name;
            Interval = 60000;
            _consumerThread = new Thread(Consume) {Name = _name + "-read"};
            _heartbeatThread = new Thread(ScheduleHeartbeat) {Name = _name + "-heartbeat"};
            _puffinMessageReader = new PuffinMessageReader(stream);
            _messageReceiver = new PuffinMessageReceiver(provider) {OnHeartbeatListener = HandleHeartbeat};
        }

        /// <summary>
        /// Starts the consumer thread, interval thread and starts publishing
        /// </summary>
        public void Start()
        {
            if (_running.CompareAndSet(false, true))
            {
                _consumerThread.Start();
                _heartbeatThread.Start();
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
                    if (Log.IsDebugEnabled) Log.Debug("sleeping for the interval: " + Interval);
                    Thread.Sleep(Interval);
                }
                else
                {
                    var timeSinceLastMessage =
                        GetReadableTimeOfMillis(JavaTime.CurrentTimeMillis() - GetTimeOfLastMessage());
                    Log.Warn(_name + " no message have been received for " + timeSinceLastMessage +
                             "; assume connection failure");
                    Close("inactive connection");
                }
            }
        }

        /// <summary>
        /// Consumes input from the stream and routes it to the message receiver
        /// </summary>
        private void Consume()
        {
            try
            {
                while (_running.Value)
                {
                    var message = _puffinMessageReader.ReadMessage();
                    if (Log.IsDebugEnabled) Log.Debug(_name + " received message: " + message);
                    _messageReceiver.OnMessage(message);
                }
            }
            catch (ThreadInterruptedException)
            {
                Log.Warn(_name + " thread interrupted while consuming");
            }
        }

        /// <summary>
        /// Gets the time that we received the last message
        /// </summary>
        /// <returns>The time in milliseconds that we received the last message</returns>
        private long GetTimeOfLastMessage()
        {
            return _messageReceiver.TimeOfLastMessage;
        }

        /// <summary>
        /// Gets a human readable time for a given number of milliseconds
        /// </summary>
        /// <param name="milliseconds">The time in milliseconds</param>
        /// <returns>The time in the format of xxh:xxm:xxs:xxxms</returns>
        private string GetReadableTimeOfMillis(long milliseconds)
        {
            var t = TimeSpan.FromMilliseconds(milliseconds);
            return string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                t.Hours,
                t.Minutes,
                t.Seconds,
                t.Milliseconds);
        }

        /// <summary>
        /// Checks if we have received any messages from the server in the last two intervals
        /// </summary>
        /// <returns>True if we have received a message within the last two intervals, false otherwise.</returns>
        private bool IsMessageStreamActive()
        {
            var time = JavaTime.CurrentTimeMillis() - Interval * 2;
            return GetTimeOfLastMessage() > time;
        }

        /// <summary>
        /// Publish queued messages to the socket.
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
        /// Place a message on the streams outgoing message queue.
        /// This method will block if the message queue is full.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public void QueueMessage(string message)
        {
            if (Log.IsDebugEnabled) Log.Debug(_name + " queuing message: " + message);
            _messageQueue.Add(message);
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
                _consumerThread.Interrupt();
                _stream.Close();
                _messageQueue.Dispose();
                Log.Info(_name + " disconnected (" + reason + ")");
            }
        }

        /// <summary>
        /// Sets the interval and queues a clock sync message if required.
        /// </summary>
        /// <param name="interval">The new interval</param>
        /// <param name="transmitTime">The transmit time of the heartbeat</param>
        /// <param name="receiveTime">The time we received the heartbeat</param>
        /// <param name="clockSync">True if we should reply with a clock sync message</param>
        private void HandleHeartbeat(int interval, long transmitTime, long receiveTime, bool clockSync)
        {
            Interval = interval;
            if (clockSync)
            {
                SendClockSync(transmitTime, receiveTime);
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

        /// <summary>
        /// Queues a heartbeat message
        /// </summary>
        private void SendHeartbeat()
        {
            var now = JavaTime.CurrentTimeMillis();
            QueueMessage(new PuffinElement(PuffinTagName.Heartbeat)
                .AddAttribute("TransmitTime", now)
                .ToString());
        }

        /// <summary>
        /// Queues a sync clock message
        /// </summary>
        /// <param name="originateTime">The time they sent the heartbeat</param>
        /// <param name="receiveTime">The time we received the heartbeat</param>
        private void SendClockSync(long originateTime, long receiveTime)
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
                var timeSinceLastMessage = GetReadableTimeOfMillis(JavaTime.CurrentTimeMillis() - GetTimeOfLastMessage());
                Log.Warn(_name + " no message have been received for " + timeSinceLastMessage +
                         "; assume connection failure");
                Close("inactive connection");
            }
        }
    }
}