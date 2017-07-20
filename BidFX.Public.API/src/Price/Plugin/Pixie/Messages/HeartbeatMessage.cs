using System.IO;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    /// <summary>
    /// A heartbeat is used to keep alive connections with little or no traffic.
    /// </summary>
    internal class HeartbeatMessage : IOutgoingPixieMessage
    {
        private static readonly byte[] RawBytes = {PixieMessageType.Heartbeat};
        private static readonly MemoryStream Heartbeat = new MemoryStream(RawBytes);

        public MemoryStream Encode(int version)
        {
            return Heartbeat;
        }

        protected bool Equals(HeartbeatMessage other)
        {
            return true;
        }

        public override string ToString()
        {
            return "Heartbeat()";
        }
    }
}