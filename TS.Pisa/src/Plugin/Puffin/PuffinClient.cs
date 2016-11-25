using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly Thread _consumerThread,_heartbeatThread,_resubscribeThread;
        private readonly Stream _stream;
        private readonly IProviderPlugin _provider;
        private readonly BlockingCollection<string> _messageQueue = new BlockingCollection<string>();
        private readonly PuffinMessageReader _puffinMessageReader;
        public int Interval { get; set; }
        private long TimeOfLastMessage { get; set; }
        private readonly HashSet<string> _subjectsToResubscribe = new HashSet<string>();

        protected internal PuffinClient(Stream stream, IProviderPlugin provider)
        {
            _stream = stream;
            Interval = 60000;
            _provider = provider;
            _consumerThread = new Thread(Consume) {Name = provider.Name + "-read"};
            _heartbeatThread = new Thread(ScheduleHeartbeat) {Name = provider.Name + "-heartbeat"};
            _resubscribeThread = new Thread(ResubscribeToBadStatus) {Name = provider.Name + "-resubscribe"};
            _puffinMessageReader = new PuffinMessageReader(stream);
            TimeOfLastMessage = JavaTime.CurrentTimeMillis();
        }

        protected internal void Start()
        {
            if (_running.CompareAndSet(false, true))
            {
                _consumerThread.Start();
                _heartbeatThread.Start();
                _resubscribeThread.Start();
            }
            Publish();
        }

        private void ScheduleHeartbeat()
        {
            try
            {
                while (_running.Value)
                {
                    if (IsMessageStreamActive())
                    {
                        var now = JavaTime.CurrentTimeMillis();
                        QueueMessage(new PuffinElement(PuffinTagName.Heartbeat)
                            .AddAttribute("TransmitTime", now)
                            .ToString());
                        if (Log.IsDebugEnabled) Log.Debug("sleeping for the interval: " + Interval);
                        Thread.Sleep(Interval);
                    }
                    else
                    {
                        var timeSinceLastMessage =
                            GetReadableTimeOfMillis(JavaTime.CurrentTimeMillis() - TimeOfLastMessage);
                        Log.Warn("no message have been received for " + timeSinceLastMessage +
                                 "; assume connection failure");
                        Close("inactive connection");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("An unexpected error occured in the heartbeat thread.", e);
                Close("Unexpected error in the heatbeat thread.");
            }
        }

        private void ResubscribeToBadStatus()
        {
            try
            {
                while (_running.Value)
                {
                    Thread.Sleep(300000);
                    foreach (var subject in _subjectsToResubscribe)
                    {
                        Subscribe(subject);
                    }
                    _subjectsToResubscribe.Clear();
                }
            }
            catch (ThreadInterruptedException)
            {
                Log.Warn("thread interrupted");
            }
            catch (Exception e)
            {
                Log.Error("An unexpected error occured in the resubscribe thread.", e);
                Close("Unexpected error in the resubscribe thread.");
            }
        }

        private void Consume()
        {
            try
            {
                while (_running.Value)
                {
                    var message = _puffinMessageReader.ReadMessage();
                    if (Log.IsDebugEnabled) Log.Debug("received message: " + message);
                    OnMessage(message);
                }
            }
            catch (ThreadInterruptedException)
            {
                Log.Warn("thread interrupted while consuming");
            }
            catch (Exception e)
            {
                Log.Error("An unexpected error occured in the comsumer thread.", e);
                Close("Unexpected error in the comsumer thread.");
            }
        }

        private void Publish()
        {
            try
            {
                while (_running.Value)
                {
                    string message;
                    if (_messageQueue.TryTake(out message))
                    {
                        if (Log.IsDebugEnabled) Log.Debug("sending message: " + message);
                        _stream.Write(Encoding.ASCII.GetBytes(message), 0, message.Length);
                        _stream.Flush();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("An unexpected error occured in the publisher thread.", e);
                Close("Unexpected error in the publisher thread.");
            }
        }

        private void QueueMessage(string message)
        {
            if (Log.IsDebugEnabled) Log.Debug("queuing message: " + message);
            _messageQueue.Add(message);
        }

        private void Close(string reason)
        {
            if (Log.IsDebugEnabled) Log.Debug("stream close (" + reason + ")");
            if (_running.CompareAndSet(true, false))
            {
                _consumerThread.Interrupt();
                _resubscribeThread.Interrupt();
                _stream.Close();
                _messageQueue.Dispose();
                Log.Info("disconnected (" + reason + ")");
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

        public void CloseSession()
        {
            Close("session close requested");
        }

        public void CheckSessionIsActive()
        {
            if (IsMessageStreamActive())
            {
                var timeSinceLastMessage = GetReadableTimeOfMillis(JavaTime.CurrentTimeMillis() - TimeOfLastMessage);
                Log.Warn("no message have been received for " + timeSinceLastMessage +
                         "; assume connection failure");
                Close("inactive connection");
            }
        }

        private void OnMessage(PuffinElement element)
        {
            TimeOfLastMessage = JavaTime.CurrentTimeMillis();
            switch (element.Tag)
            {
                case PuffinTagName.Update:
                    OnUpdateMessage(element);
                    break;
                case PuffinTagName.Set:
                    OnSetMessage(element);
                    break;
                case PuffinTagName.Status:
                    OnStatusMessage(element);
                    break;
                case PuffinTagName.Heartbeat:
                    OnHeartbeatMessage(element, TimeOfLastMessage);
                    break;
                case PuffinTagName.Close:
                    OnCloseMessage(element);
                    break;
                default:
                    Log.Warn("ignoring unexpected message:" + element);
                    break;
            }
        }

        private void OnUpdateMessage(PuffinElement element)
        {
            var eventHandler = _provider.PriceUpdate;
            if (eventHandler != null)
            {
                var subject = element.AttributeValue(PuffinFieldName.Subject).Text;
                var priceMap = PriceAdaptor.ToPriceMap(element.Content.FirstOrDefault());
                eventHandler(this, new PriceUpdateEventArgs
                {
                    Subject = subject,
                    PriceImage = priceMap,
                    PriceUpdate = priceMap
                });
            }
        }

        private void OnSetMessage(PuffinElement element)
        {
            var eventHandler = _provider.PriceUpdate;
            if (eventHandler != null)
            {
                var subject = element.AttributeValue(PuffinFieldName.Subject).Text;
                var priceMap = PriceAdaptor.ToPriceMap(element.Content.FirstOrDefault());
                eventHandler(this, new PriceUpdateEventArgs
                {
                    Subject = subject,
                    PriceImage = priceMap,
                    PriceUpdate = priceMap
                });
            }
        }

        private void OnStatusMessage(PuffinElement element)
        {
            var eventHandler = _provider.PriceStatus;
            var subject = element.AttributeValue(PuffinFieldName.Subject).Text;
            var statusCode = element.AttributeValue("Id").ToInteger();
            var status = PriceAdaptor.ToStatus(statusCode);
            if (IsBadStatus(status))
            {
                _subjectsToResubscribe.Add(subject);
            }
            if (eventHandler != null)
            {
                eventHandler(this, new PriceStatusEventArgs
                {
                    Subject = subject,
                    Status = status,
                    Reason = element.AttributeValue("Text").Text
                });
            }
        }

        private bool IsBadStatus(PriceStatus status)
        {
            return status.Equals(PriceStatus.REJECTED)
                || status.Equals(PriceStatus.CANCELLED)
                   || status.Equals(PriceStatus.STALE)
                   || status.Equals(PriceStatus.DISCONTINUED)
                   || status.Equals(PriceStatus.PROHIBITED)
                   || status.Equals(PriceStatus.UNAVAILABLE)
                   || status.Equals(PriceStatus.TIMEOUT)
                   || status.Equals(PriceStatus.INACTIVE)
                   || status.Equals(PriceStatus.EXHAUSTED)
                   || status.Equals(PriceStatus.CLOSED);
        }

        private void OnHeartbeatMessage(PuffinElement element, long receiveTime)
        {
            Interval = TokenToInt(element.AttributeValue(PuffinFieldName.Interval), 600000);
            var syncClock = TokenToBoolean(element.AttributeValue(PuffinFieldName.SyncClock), false);
            if (syncClock)
            {
                var transmitTime = TokenToLong(element.AttributeValue(PuffinFieldName.TransmitTime), 0L);
                QueueMessage(new PuffinElement(PuffinTagName.ClockSync)
                    .AddAttribute("OriginateTime", transmitTime)
                    .AddAttribute("ReceiveTime", receiveTime)
                    .AddAttribute("TransmitTime", JavaTime.CurrentTimeMillis())
                    .ToString());
            }
        }

        private void OnCloseMessage(PuffinElement element)
        {
            Log.Warn("received close message from server: " + element);
        }

        private bool IsMessageStreamActive()
        {
            var time = JavaTime.CurrentTimeMillis() - Interval * 2;
            return TimeOfLastMessage > time;
        }

        private bool TokenToBoolean(PuffinToken token, bool defaultValue)
        {
            return token == null ? defaultValue : token.ToBoolean();
        }

        private long TokenToLong(PuffinToken token, long defaultValue)
        {
            return token == null ? defaultValue : token.ToLong();
        }

        private int TokenToInt(PuffinToken token, int defaultValue)
        {
            return token == null ? defaultValue : token.ToInteger();
        }

        private string GetReadableTimeOfMillis(long milliseconds)
        {
            var t = TimeSpan.FromMilliseconds(milliseconds);
            return string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                t.Hours,
                t.Minutes,
                t.Seconds,
                t.Milliseconds);
        }
    }
}