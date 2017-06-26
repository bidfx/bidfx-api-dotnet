using System;
using System.IO;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields.FieldEncodings
{
    public interface IFieldEncoding
    {
        void SkipFieldValue(Stream stream);
    }
}