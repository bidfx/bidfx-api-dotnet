using System.IO;
using BidFX.Public.API.Price.Plugin.Pixie.Fields;
using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public class PriceSync
    {
        public uint Options { get; internal set; }
        public uint Edition { get; internal set; }
        public ulong Revision { get; internal set; }
        public ulong RevisionTime { get; internal set; }
        public ulong ConflationLatency { get; internal set; }
        public uint Size { get; internal set; }
        private readonly MemoryStream _priceUpdateStream;
        private readonly IStreamInflator _inflator;

        public PriceSync(MemoryStream stream, IStreamInflator inflator)
        {
            Options = (uint) stream.ReadByte();
            Revision = Varint.ReadU64(stream);
            RevisionTime = Varint.ReadU64(stream);
            ConflationLatency = Varint.ReadU64(stream);
            Edition = Varint.ReadU32(stream);
            Size = Varint.ReadU32(stream);
            _priceUpdateStream = stream;
            _inflator = inflator;
        }
        
        public bool IsCompressed()
        {
            return BitSetter.IsSet(Options, 0);
        }

        public void Visit(IDataDictionary dataDictionary, IGridHeaderRegistry gridHeaderRegistry)
        {
            if (Size <= 0) return;

            var stream = IsCompressed() ? _inflator.Inflate(_priceUpdateStream) : _priceUpdateStream;
            PriceUpdateDecoder.Visit(stream, (int) Size, dataDictionary, gridHeaderRegistry);
        }
    }
}