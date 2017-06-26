using System.IO;
using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields.FieldEncodings
{
    public class VarintStringFieldEncoding : IFieldEncoding
    {
        public void SkipFieldValue(Stream stream)
        {
            var size = Varint.ReadU32(stream);
            stream.Seek(size - 1, SeekOrigin.Current);
        }
    }
}