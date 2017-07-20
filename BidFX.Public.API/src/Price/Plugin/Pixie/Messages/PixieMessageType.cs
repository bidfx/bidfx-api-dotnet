namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    internal static class PixieMessageType
    {
        public const byte Ack = (byte) 'A';
        public const byte DataDictionary = (byte) 'D';
        public const byte Grant = (byte) 'G';
        public const byte Heartbeat = (byte) 'H';
        public const byte Login = (byte) 'L';
        public const byte PriceSync = (byte) 'P';
        public const byte SubscriptionSync = (byte) 'S';
        public const byte Welcome = (byte) 'W';
    }
}