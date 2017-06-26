using System.IO;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields.FieldEncodings
{
    public class Fixed2FieldEncoding : IFieldEncoding
    {
        public void SkipFieldValue(Stream stream)
        {
            stream.Seek(2, SeekOrigin.Current);
        }
    }
}