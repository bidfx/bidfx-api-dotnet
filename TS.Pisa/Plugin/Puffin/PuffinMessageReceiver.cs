using System;
using System.Reflection;
using TS.Pisa.Tools;

namespace TS.Pisa.Plugin.Puffin
{
    public class PuffinMessageReceiver
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private long _timeOfLastMessage = JavaTime.CurrentTimeMillis();
        public PuffinClient.OnHeartbeatListener OnHeartbeatListener { get; set; }
        public EventHandler<PriceUpdateEventArgs> PriceUpdate { get; set; }

        public void OnMessage(PuffinElement element)
        {
            _timeOfLastMessage = JavaTime.CurrentTimeMillis();
            if (PuffinTagName.Update.Equals(element.Tag)) OnUpdateMessage(element);
            else if (PuffinTagName.Set.Equals(element.Tag)) OnSetMessage(element);
            else if (PuffinTagName.Status.Equals(element.Tag)) OnStatusMessage(element);
            else if (PuffinTagName.Heartbeat.Equals(element.Tag)) OnHeartbeatMessage(element, _timeOfLastMessage);
            else if (PuffinTagName.ClockSync.Equals(element.Tag)) OnClockSyncMessage(element);
            else if (PuffinTagName.Close.Equals(element.Tag)) OnCloseMessage(element);
            else if (PuffinTagName.Subscribe.Equals(element.Tag)) OnSubscribeMessage(element);
            else if (PuffinTagName.Unsubscribe.Equals(element.Tag)) OnUnsubscribeMessage(element);
            else if (PuffinTagName.WeightedMatchers.Equals(element.Tag)) OnWeightedMatchersMessage(element);
            else OnUnknownMessage(element);
        }

        private void OnUpdateMessage(PuffinElement element)
        {
            var subject = element.AttributeValue(PuffinFieldName.Subject).Text;
            if(PriceUpdate!=null) PriceUpdate(this, new PriceUpdateEventArgs() {Subject = subject});
        }

        private void OnSetMessage(PuffinElement element)
        {
            var subject = element.AttributeValue(PuffinFieldName.Subject).Text;
            if (PriceUpdate != null) PriceUpdate(this, new PriceUpdateEventArgs() {Subject = subject});
        }

        private void OnStatusMessage(PuffinElement element)
        {
            Log.Debug(element);
        }

        private void OnHeartbeatMessage(PuffinElement element, long receiveTime)
        {
            var interval = TokenToLong(element.AttributeValue(PuffinFieldName.Interval),600000L);
            var transmitTime = TokenToLong(element.AttributeValue(PuffinFieldName.TransmitTime),0L);
            var syncClock = TokenToBoolean(element.AttributeValue(PuffinFieldName.SyncClock),false);
            OnHeartbeatListener(interval, transmitTime, receiveTime, syncClock);
        }

        private void OnClockSyncMessage(PuffinElement element)
        {
            Log.Debug(element);
        }

        private void OnCloseMessage(PuffinElement element)
        {
            Log.Debug(element);
        }

        private void OnSubscribeMessage(PuffinElement element)
        {
            Log.Debug(element);
        }

        private void OnUnsubscribeMessage(PuffinElement element)
        {
            Log.Debug(element);
        }

        private void OnWeightedMatchersMessage(PuffinElement element)
        {
            Log.Debug(element);
        }

        private void OnUnknownMessage(PuffinElement element)
        {
            if (IsLikelyMessageType(element.Tag))
            {
                Log.Warn("ignoring unknown message:" + element);
            }
            throw new PuffinSyntaxException("received bad message: " + element);
        }

        private bool IsLikelyMessageType(string type)
        {
            char[] c = type.ToCharArray();
            if (c.Length < 2) return false;
            if (!char.IsUpper(c[0])) return false;
            for (var i = 1; i < c.Length; i++)
            {
                if (!char.IsLetterOrDigit(c[i])) return false;
            }
            return true;
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