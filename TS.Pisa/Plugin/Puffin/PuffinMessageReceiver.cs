using System;
using System.Linq;
using System.Reflection;
using TS.Pisa.Tools;

namespace TS.Pisa.Plugin.Puffin
{
    public class PuffinMessageReceiver
    {
        private readonly IProviderPlugin _provider;

        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private long _timeOfLastMessage = JavaTime.CurrentTimeMillis();

        public PuffinMessageReceiver(IProviderPlugin provider)
        {
            _provider = provider;
        }

        public PuffinClient.OnHeartbeatListener OnHeartbeatListener { get; set; }

        public PuffinMessageReceiver()
        {
            TimeOfLastMessage = JavaTime.CurrentTimeMillis();
        }

        public void OnMessage(PuffinElement element)
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
            if (eventHandler != null)
            {
                var subject = element.AttributeValue(PuffinFieldName.Subject).Text;
                var statusCode = element.AttributeValue("Id").ToInteger();
                eventHandler(this, new PriceStatusEventArgs
                {
                    Subject = subject,
                    Status = PriceAdaptor.ToStatus(statusCode),
                    StatusText = element.AttributeValue("Text").Text
                });
            }
        }

        private void OnHeartbeatMessage(PuffinElement element, long receiveTime)
        {
            var interval = TokenToInt(element.AttributeValue(PuffinFieldName.Interval), 600000);
            var transmitTime = TokenToLong(element.AttributeValue(PuffinFieldName.TransmitTime), 0L);
            var syncClock = TokenToBoolean(element.AttributeValue(PuffinFieldName.SyncClock), false);
            OnHeartbeatListener(interval, transmitTime, receiveTime, syncClock);
        }

        private void OnCloseMessage(PuffinElement element)
        {
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
    }
}