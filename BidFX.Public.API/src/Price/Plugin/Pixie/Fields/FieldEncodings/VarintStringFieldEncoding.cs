using System.IO;
using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields.FieldEncodings
{
    public class VarintStringFieldEncoding : FieldEncoding
    {
        public override void SkipFieldValue(Stream stream)
        {
            int size = Varint.ReadU32(stream);
            SkipFieldValue(stream, size - 1);
        }
    }
}