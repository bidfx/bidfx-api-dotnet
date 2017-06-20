using System.IO;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields.FieldEncodings
{
    public class Fixed1FieldEncoding : FieldEncoding
    {
        public override void SkipFieldValue(Stream stream)
        {
            SkipFieldValue(stream, 1);
        }
    }
}