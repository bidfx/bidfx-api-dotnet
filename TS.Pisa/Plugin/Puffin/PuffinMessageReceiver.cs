using System;
using System.Reflection;
using TS.Pisa.Tools;

namespace TS.Pisa.Plugin.Puffin
{
    public class PuffinMessageReceiver
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public long TimeOfLastMessage { get; set; }
        public PuffinClient.OnHeartbeatListener OnHeartbeatListener { get; set; }
        public EventHandler<PriceUpdateEventArgs> PriceUpdate { get; set; }

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
                case PuffinTagName.SyncClock:
                    OnClockSyncMessage(element);
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
            var subject = element.AttributeValue(PuffinFieldName.Subject).Text;
            if (PriceUpdate != null) PriceUpdate(this, new PriceUpdateEventArgs {Subject = subject});
        }

        private void OnSetMessage(PuffinElement element)
        {
            var subject = element.AttributeValue(PuffinFieldName.Subject).Text;
            if (PriceUpdate != null) PriceUpdate(this, new PriceUpdateEventArgs {Subject = subject});
        }

        private void OnStatusMessage(PuffinElement element)
        {
        }

        private void OnHeartbeatMessage(PuffinElement element, long receiveTime)
        {
            var interval = TokenToLong(element.AttributeValue(PuffinFieldName.Interval), 600000L);
            var transmitTime = TokenToLong(element.AttributeValue(PuffinFieldName.TransmitTime), 0L);
            var syncClock = TokenToBoolean(element.AttributeValue(PuffinFieldName.SyncClock), false);
            OnHeartbeatListener(interval, transmitTime, receiveTime, syncClock);
        }

        private void OnClockSyncMessage(PuffinElement element)
        {
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
    }
}