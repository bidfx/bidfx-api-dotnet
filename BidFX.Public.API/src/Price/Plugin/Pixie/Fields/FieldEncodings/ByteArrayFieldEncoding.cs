using System.IO;
using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields.FieldEncodings
{
    public class ByteArrayFieldEncoding : FieldEncoding
    {
        public override void SkipFieldValue(Stream stream)
        {
            var size = Varint.ReadU32(stream);
            SkipFieldValue(stream, size);
        }
    }
}