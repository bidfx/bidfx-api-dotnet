using System.IO;
using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public class PriceSync
    {
        public int Options { get; internal set; }
        public uint Edition { get; internal set; }
        public ulong Revision { get; internal set; }
        public ulong RevisionTime { get; internal set; }
        public ulong ConflationLatency { get; internal set; }
        public uint Size { get; internal set; }
        private readonly Stream _priceUpdateStream;

        public PriceSync(Stream stream)
        {
            Options = stream.ReadByte();
            Revision = Varint.ReadU64(stream);
            RevisionTime = Varint.ReadU64(stream);
            ConflationLatency = Varint.ReadU64(stream);
            Edition = Varint.ReadU32(stream);
            Size = Varint.ReadU32(stream);
            _priceUpdateStream = stream;
        }
    }
}