using System;
using System.Reflection;
using TS.Pisa.Plugin.Puffin.Xml;
using TS.Pisa.Tools;
using XmlSyntaxException = System.Security.XmlSyntaxException;

namespace TS.Pisa.Plugin.Puffin
{
    public class PuffinMessageReceiver
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private long _timeOfLastMessage = JavaTime.CurrentTimeMillis();
        private PuffinClient.OnHeartbeatListener _onHeartbeatListener;
        public void OnMessage(XmlElement message)
        {
            if(Log.IsDebugEnabled) Log.Debug("Message received: "+message);
            _timeOfLastMessage = JavaTime.CurrentTimeMillis();
            if (message.IsA(PuffinTagName.Update)) OnUpdateMessage(message);
            else if (message.IsA(PuffinTagName.Set)) OnSetMessage(message);
            else if (message.IsA(PuffinTagName.Status)) OnStatusMessage(message);
            else if (message.IsA(PuffinTagName.Heartbeat)) OnHeartbeatMessage(message, _timeOfLastMessage);
            else if (message.IsA(PuffinTagName.ClockSync)) OnClockSyncMessage(message);
            else if (message.IsA(PuffinTagName.Close)) OnCloseMessage(message);
            else if (message.IsA(PuffinTagName.Subscribe)) OnSubscribeMessage(message);
            else if (message.IsA(PuffinTagName.Unsubscribe)) OnUnsubscribeMessage(message);
            else if (message.IsA(PuffinTagName.WeightedMatchers)) OnWeightedMatchersMessage(message);
            else OnUnknownMessage(message);
        }

        public void SetHeartbeatListener(PuffinClient.OnHeartbeatListener l)
        {
            _onHeartbeatListener = l;
        }

        private void OnUpdateMessage(XmlElement message)
        {

        }

        private void OnSetMessage(XmlElement message)
        {

        }

        private void OnStatusMessage(XmlElement message)
        {

        }

        private void OnHeartbeatMessage(XmlElement message, long receiveTime)
        {
            var interval = Convert.ToInt64(message.GetAttributeValueAsText(PuffinFieldName.Interval));
            var transmitTime = Convert.ToInt64(message.GetAttributeValueAsText(PuffinFieldName.TransmitTime));
            var syncClock = Convert.ToBoolean(message.GetAttributeValueAsText(PuffinFieldName.SyncClock));
            _onHeartbeatListener(interval,transmitTime, receiveTime, syncClock);
        }

        private void OnClockSyncMessage(XmlElement message)
        {

        }

        private void OnCloseMessage(XmlElement message)
        {

        }

        private void OnSubscribeMessage(XmlElement message)
        {

        }

        private void OnUnsubscribeMessage(XmlElement message)
        {

        }

        private void OnWeightedMatchersMessage(XmlElement message)
        {

        }

        private void OnUnknownMessage(XmlElement message)
        {
            if (IsLikelyMessageType(message.GetTag()))
            {
                Log.Warn("ignoring unknown message:" + message);
            }
            throw new XmlSyntaxException("received bad message: " + message);
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
    }
}