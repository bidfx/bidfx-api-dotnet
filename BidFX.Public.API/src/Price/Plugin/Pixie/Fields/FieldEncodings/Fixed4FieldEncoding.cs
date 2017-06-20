using System.IO;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields.FieldEncodings
{
    public class Fixed4FieldEncoding : FieldEncoding
    {
        public override void SkipFieldValue(Stream stream)
        {
            SkipFieldValue(stream, 4);
        }
    }
}