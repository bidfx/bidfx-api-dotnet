using System.IO;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields.FieldEncodings
{
    public class Fixed8FieldEncoding : IFieldEncoding
    {
        public void SkipFieldValue(Stream stream)
        {
            stream.Seek(8, SeekOrigin.Current);
        }
    }
}