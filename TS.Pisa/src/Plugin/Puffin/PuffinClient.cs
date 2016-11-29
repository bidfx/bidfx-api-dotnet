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
    internal class PuffinClient : IPuffinRequestor
    {
        private const string Subject = "Subject";

        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly AtomicBoolean _running = new AtomicBoolean(false);
        private readonly Thread _publisherThread;
        private readonly Thread _heartbeatThread;
        private readonly Stream _stream;
        private readonly IProviderPlugin _provider;
        private readonly BlockingCollection<string> _messageQueue = new BlockingCollection<string>();
        private readonly PuffinMessageReader _puffinMessageReader;
        private readonly TimeSpan _heartbeatInterval;
        private DateTime _timeOfLastMessage = DateTime.Now;
        private readonly HashSet<string> _subjectsToResubscribe = new HashSet<string>();

        protected internal PuffinClient(Stream stream, IProviderPlugin provider, TimeSpan heartbeatInterval)
        {
            _stream = stream;
            _provider = provider;
            _heartbeatInterval = heartbeatInterval;
            _publisherThread = new Thread(SendOutgoingMessages) {Name = provider.Name + "-write"};
            _heartbeatThread = new Thread(ScheduleHeartbeat) {Name = provider.Name + "-heartbeat"};
            _puffinMessageReader = new PuffinMessageReader(stream);
        }

        public void Start()
        {
            if (_running.CompareAndSet(false, true))
            {
                _publisherThread.Start();
                _heartbeatThread.Start();
                ProcessIncommingMessages();
            }
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
                        if (Log.IsDebugEnabled) Log.Debug("sleeping for the interval: " + _heartbeatInterval);
                        Thread.Sleep(_heartbeatInterval);
                    }
                    else
                    {
                        Log.Warn("no message have been received since " + _timeOfLastMessage +
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

        private void ProcessIncommingMessages()
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

        private void SendOutgoingMessages()
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
            if (_running.Value)
            {
                if (Log.IsDebugEnabled) Log.Debug("queuing message: " + message);
                _messageQueue.Add(message);
            }
        }

        private void Close(string reason)
        {
            if (_running.CompareAndSet(true, false))
            {
                Log.Info("closing connection to Puffin (" + reason + ")");
                _stream.Close();
                _publisherThread.Interrupt();
                _messageQueue.Dispose();
            }
        }

        public void Subscribe(string subject)
        {
            QueueMessage(new PuffinElement(PuffinTagName.Subscribe)
                .AddAttribute(Subject, subject)
                .ToString());
        }

        public void Unsubscribe(string subject)
        {
            QueueMessage(new PuffinElement(PuffinTagName.Unsubscribe)
                .AddAttribute(Subject, subject)
                .ToString());
        }

        public void Stop()
        {
            Close("session close requested");
        }

        public void CheckSessionIsActive()
        {
            if (IsMessageStreamActive())
            {
                Log.Warn("no message have been received since " + _timeOfLastMessage + "; assume connection failure");
                Close("inactive connection");
            }
        }

        private void OnMessage(PuffinElement element)
        {
            _timeOfLastMessage = DateTime.Now;
            switch (element.Tag)
            {
                case PuffinTagName.Update:
                    OnUpdateMessage(element);
                    break;
                case PuffinTagName.Set:
                    OnUpdateMessage(element);
                    break;
                case PuffinTagName.Status:
                    OnStatusMessage(element);
                    break;
                case PuffinTagName.Heartbeat:
                    OnHeartbeatMessage(element);
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
            var subject = element.AttributeValue(Subject).Text;
            _subjectsToResubscribe.Remove(subject);
            var priceMap = PriceAdaptor.ToPriceMap(element.Content.FirstOrDefault());
            _provider.PisaEventHandler.OnPriceEvent(new PriceUpdateEventArgs
            {
                Subject = subject,
                AllPriceFields = priceMap,
                ChangedPriceFields = priceMap
            });
        }

        private void OnStatusMessage(PuffinElement element)
        {
            var subject = element.AttributeValue(Subject).Text;
            var status = PriceAdaptor.ToStatus((int) element.AttributeValue("Id").Value);
            if (status != SubscriptionStatus.OK)
            {
                _subjectsToResubscribe.Add(subject);
            }
            else
            {
                _subjectsToResubscribe.Remove(subject);
            }
            _provider.PisaEventHandler.OnStatusEvent(new SubscriptionStatusEventArgs
            {
                Subject = subject,
                Status = status,
                Reason = element.AttributeValue("Text").Text
            });
        }

        private void OnHeartbeatMessage(PuffinElement element)
        {
            if ("true".Equals(element.AttributeValue("SyncClock").Text))
            {
                var transmitTime = (long) (element.AttributeValue("TransmitTime").Value ?? 0L);
                QueueMessage(new PuffinElement(PuffinTagName.ClockSync)
                    .AddAttribute("OriginateTime", transmitTime)
                    .AddAttribute("ReceiveTime", JavaTime.ToJavaTime(_timeOfLastMessage))
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
            var idleTime = DateTime.Now.Subtract(_timeOfLastMessage);
            var twoBeats = _heartbeatInterval.Add(_heartbeatInterval);
            var compareTo = idleTime.CompareTo(twoBeats);
            return compareTo < 0;
        }

    }
}