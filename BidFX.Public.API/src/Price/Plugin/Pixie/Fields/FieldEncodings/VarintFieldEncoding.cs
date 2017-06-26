using System.IO;
using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields.FieldEncodings
{
    public class VarintFieldEncoding : IFieldEncoding
    {
        public void SkipFieldValue(Stream stream)
        {
            while (!Varint.IsFinalByte(stream.ReadByte()))
            {
            }
        }
    }
}