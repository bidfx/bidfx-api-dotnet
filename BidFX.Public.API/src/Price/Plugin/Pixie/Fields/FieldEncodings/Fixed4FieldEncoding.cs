using System.IO;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields.FieldEncodings
{
    public class Fixed4FieldEncoding : IFieldEncoding
    {
        public void SkipFieldValue(Stream stream)
        {
            stream.Seek(4, SeekOrigin.Current);
        }
    }
}