using System.IO;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields.FieldEncodings
{
    public class NoopFieldEncoding : FieldEncoding
    {
        public override void SkipFieldValue(Stream stream)
        {
            //Do nothing
        }
    }
}