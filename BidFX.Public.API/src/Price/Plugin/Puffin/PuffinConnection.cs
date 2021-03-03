/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using BidFX.Public.API.Price.Tools;
using Serilog;
using Serilog.Core;

namespace BidFX.Public.API.Price.Plugin.Puffin
{
    internal class PuffinConnection : ISubscriber
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(Constants.SourceContextPropertyName, "PuffinConnection");
        private const string Subject = "Subject";

        private readonly AtomicBoolean _running = new AtomicBoolean(true);
        private readonly Thread _publisherThread;
        private readonly Stream _stream;
        private readonly IProviderPlugin _provider;
        private readonly BlockingCollection<string> _messageQueue = new BlockingCollection<string>();
        private readonly PuffinMessageReader _puffinMessageReader;
        private readonly TimeSpan _heartbeatInterval;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private DateTime _timeOfLastMessageReceived = DateTime.Now;
        private DateTime _timeOfLastMessageSent = DateTime.Now;

        protected internal PuffinConnection(Stream stream, IProviderPlugin provider, TimeSpan heartbeatInterval)
        {
            _stream = stream;
            _provider = provider;
            _heartbeatInterval = heartbeatInterval;
            _puffinMessageReader = new PuffinMessageReader(stream);
            _publisherThread = new Thread(SendOutgoingMessages)
            {
                Name = provider.Name + "-write",
                IsBackground = true
            };
            _publisherThread.Start();
        }

        public void Subscribe(Subject.Subject subject, bool autoRefresh = false, bool refresh = false)
        {
            QueueMessage(
                new PuffinElement(PuffinTagName.Subscribe).AddAttribute(Subject, subject.ToString()).ToString());
        }

        public void Unsubscribe(Subject.Subject subject)
        {
            QueueMessage(new PuffinElement(PuffinTagName.Unsubscribe).AddAttribute(Subject, subject.ToString())
                .ToString());
        }

        public void Close(string reason)
        {
            if (_running.CompareAndSet(true, false))
            {
                Log.Information("closing connection to Puffin ({reason})", reason);
                _cancellationTokenSource.Cancel();
                _stream.Close();
                _messageQueue.Dispose();
            }
        }

        public void ProcessIncommingMessages()
        {
            try
            {
                while (_running.Value)
                {
                    PuffinElement message = _puffinMessageReader.ReadMessage();
                    if (message == null)
                    {
                        Log.Information("the Puffin server closed the connection");
                        break;
                    }

                    Log.Debug("received message: {message}", message);

                    OnMessage(message);
                }
            }
            catch (ThreadInterruptedException)
            {
                Log.Warning("thread interrupted while consuming");
            }
            catch (Exception e)
            {
                if (_running.Value)
                {
                    Log.Error("unexpected error reading from Puffin: " + e.Message);
                }
                else
                {
                    Log.Information("Exception reading from Pixie, but stopped running: " + e.Message);
                }
            }
            finally
            {
                Close("the input stream from Puffin terminated");
            }
        }

        private void SendOutgoingMessages()
        {
            try
            {
                while (_running.Value)
                {
                    string message;
                    if (_messageQueue.TryTake(out message, 100, _cancellationTokenSource.Token))
                    {
                        SendMessageToPuffin(message);
                    }
                    else
                    {
                        HeartbeatCheck();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Log.Information("finished writing to Puffin"); // closed already called
            }
            catch (Exception e)
            {
                Log.Error("unexpected error occured in the publisher thread.", e);
                Close(e.Message);
            }
        }

        private void HeartbeatCheck()
        {
            if (IsTimeOlderThan(_timeOfLastMessageReceived, _heartbeatInterval.Add(_heartbeatInterval)))
            {
                Log.Warning("no message received from Puffin since {timeOfLastMessageReceived}; assume connection failure",  _timeOfLastMessageReceived);
                Close("inactive connection");
            }
            else
            {
                if (IsTimeOlderThan(_timeOfLastMessageSent, _heartbeatInterval))
                {
                    SendMessageToPuffin(new PuffinElement(PuffinTagName.Heartbeat)
                        .AddAttribute("TransmitTime", JavaTime.CurrentTimeMillis()).ToString());
                }
            }
        }

        private static bool IsTimeOlderThan(DateTime time, TimeSpan interval)
        {
            return DateTime.Now.Subtract(time).CompareTo(interval) > 0;
        }

        private void SendMessageToPuffin(string message)
        {
            Log.Debug("sending message: {message}", message);

            _timeOfLastMessageSent = DateTime.Now;
            _stream.Write(Encoding.ASCII.GetBytes(message), 0, message.Length);
            _stream.Flush();
        }

        private void QueueMessage(string message)
        {
            if (_running.Value)
            {
                Log.Debug("queuing message: {message}", message);

                _messageQueue.Add(message);
            }
        }

        private void OnMessage(PuffinElement element)
        {
            _timeOfLastMessageReceived = DateTime.Now;
            switch (element.Tag)
            {
                case PuffinTagName.Update:
                    OnPriceUpdateMessage(element);
                    break;
                case PuffinTagName.Set:
                    OnPriceSetMessage(element);
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
                    Log.Warning("ignoring unexpected message: {element}", element);
                    break;
            }
        }

        private void OnPriceUpdateMessage(PuffinElement element)
        {
            string subject = element.AttributeValue(Subject).Text;
            IPriceMap priceMap = PriceAdaptor.ToPriceMap(element.Content.FirstOrDefault());
            _provider.InapiEventHandler.OnPriceUpdate(new Subject.Subject(subject), priceMap, false);
        }

        private void OnPriceSetMessage(PuffinElement element)
        {
            string subject = element.AttributeValue(Subject).Text;
            IPriceMap priceMap = PriceAdaptor.ToPriceMap(element.Content.FirstOrDefault());
            _provider.InapiEventHandler.OnPriceUpdate(new Subject.Subject(subject), priceMap, true);
        }

        private void OnStatusMessage(PuffinElement element)
        {
            string subject = element.AttributeValue(Subject).Text;
            SubscriptionStatus status = PriceAdaptor.ToStatus((int) element.AttributeValue("Id").Value);
            _provider.InapiEventHandler.OnSubscriptionStatus(new Subject.Subject(subject), status,
                element.AttributeValue("Text").Text);
        }

        private void OnHeartbeatMessage(PuffinElement element)
        {
            if ("true".Equals(element.AttributeValue("SyncClock").Text))
            {
                long transmitTime = (long) (element.AttributeValue("TransmitTime").Value ?? 0L);
                QueueMessage(new PuffinElement(PuffinTagName.ClockSync)
                    .AddAttribute("OriginateTime", transmitTime)
                    .AddAttribute("ReceiveTime", JavaTime.ToJavaTime(_timeOfLastMessageReceived))
                    .AddAttribute("TransmitTime", JavaTime.CurrentTimeMillis())
                    .ToString());
            }
        }

        private void OnCloseMessage(PuffinElement element)
        {
            Close("received close message from server: " + element);
        }
    }
}