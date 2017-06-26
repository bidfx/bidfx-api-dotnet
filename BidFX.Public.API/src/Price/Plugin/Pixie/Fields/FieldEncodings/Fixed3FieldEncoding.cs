using System.IO;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields.FieldEncodings
{
    public class Fixed3FieldEncoding : IFieldEncoding
    {
        public void SkipFieldValue(Stream stream)
        {
            stream.Seek(3, SeekOrigin.Current);
        }
    }
}