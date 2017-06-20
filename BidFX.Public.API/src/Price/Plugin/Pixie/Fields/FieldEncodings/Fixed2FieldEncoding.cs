using System.IO;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields.FieldEncodings
{
    public class Fixed2FieldEncoding : FieldEncoding
    {
        public override void SkipFieldValue(Stream stream)
        {
            SkipFieldValue(stream, 2);
        }
    }
}