using System.IO;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields.FieldEncodings
{
    public class NoopFieldEncoding : IFieldEncoding
    {
        public void SkipFieldValue(Stream stream)
        {
            //Do nothing
        }
    }
}