using System.IO;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields.FieldEncodings
{
    public class Fixed16FieldEncoding : IFieldEncoding
    {
        public void SkipFieldValue(Stream stream)
        {
            stream.Seek(16, SeekOrigin.Current);
        }
    }
}