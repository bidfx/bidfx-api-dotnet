namespace BidFX.Public.NAPI.PriceManager.Plugin.Puffin
{
    internal interface IPuffinSessionListener
    {
        void OnHeartbeat(long interval, long transmitTime, long receiveTime, bool clockSync);
        void OnClockSync(long originateTime, long receivedTime, long transmitTime);
        void OnClose(string reason);
    }
}