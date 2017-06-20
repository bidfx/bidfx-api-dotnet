using System;
using System.IO;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields.FieldEncodings
{
    public abstract class FieldEncoding
    {
        public abstract void SkipFieldValue(Stream stream);

        public static void SkipFieldValue(Stream stream, int length)
        {
            stream.Seek(length, SeekOrigin.Current);
        }

        public static FieldEncoding ValueOf(int code)
        {
            switch (code)
            {
                case '0':
                    return new NoopFieldEncoding();
                case '1':
                    return new Fixed1FieldEncoding();
                case '2':
                    return new Fixed2FieldEncoding();
                case '3':
                    return new Fixed3FieldEncoding();
                case '4':
                    return new Fixed4FieldEncoding();
                case '8':
                    return new Fixed8FieldEncoding();
                case '@':
                    return new Fixed16FieldEncoding();
                case 'B':
                    return new ByteArrayFieldEncoding();
                case 'S':
                    return new VarintStringFieldEncoding();
                case 'V':
                    return new VarintFieldEncoding();
                case 'Z':
                    return new ZigZagFieldEncoding();
                default:
                    throw new ArgumentException("unrecognised field encoding code: " + code + " ('" + (char) code +
                                                "').");
            }
        }
    }
}