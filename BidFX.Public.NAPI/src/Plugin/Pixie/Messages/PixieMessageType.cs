namespace BidFX.Public.NAPI.Plugin.Pixie.Messages
{
    public class PixieMessageType
    {
        public static readonly byte Ack               = (byte) 'A';
        public static readonly byte DataDictionary   = (byte) 'D';
        public static readonly byte Grant             = (byte) 'G';
        public static readonly byte Heartbeat         = (byte) 'H';
        public static readonly byte Login             = (byte) 'L';
        public static readonly byte PriceSync        = (byte) 'P';
        public static readonly byte SubscriptionSync = (byte) 'S';
        public static readonly byte Welcome           = (byte) 'W';
    }
}