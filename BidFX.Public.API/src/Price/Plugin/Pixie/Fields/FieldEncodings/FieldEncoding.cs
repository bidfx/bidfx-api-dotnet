using System;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields.FieldEncodings
{
    public enum FieldEncoding
    {
        Noop = '0',
        Fixed1 = '1',
        Fixed2 = '2',
        Fixed3 = '3',
        Fixed4 = '4',
        Fixed8 = '8',
        Fixed16 = '@',
        ByteArray = 'B',
        Varint = 'V',
        VarintString = 'S',
        ZigZag = 'Z'
    }

    static class FieldEncodingMethods
    {
        public static IFieldEncoding GetFieldEncoding(this FieldEncoding fieldEncoding)
        {
            switch (fieldEncoding)
            {
                case FieldEncoding.Noop:
                    return new NoopFieldEncoding();
                case FieldEncoding.Fixed1:
                    return new Fixed1FieldEncoding();
                case FieldEncoding.Fixed2:
                    return new Fixed2FieldEncoding();
                case FieldEncoding.Fixed3:
                    return new Fixed3FieldEncoding();
                case FieldEncoding.Fixed4:
                    return new Fixed4FieldEncoding();
                case FieldEncoding.Fixed8:
                    return new Fixed8FieldEncoding();
                case FieldEncoding.Fixed16:
                    return new Fixed16FieldEncoding();
                case FieldEncoding.ByteArray:
                    return new ByteArrayFieldEncoding();
                case FieldEncoding.VarintString:
                    return new VarintStringFieldEncoding();
                case FieldEncoding.Varint:
                    return new VarintFieldEncoding();
                case FieldEncoding.ZigZag:
                    return new ZigZagFieldEncoding();
                default:
                    throw new ArgumentException("unrecognised field encoding code: " + fieldEncoding + " ('" +
                                                (char) fieldEncoding +
                                                "').");
            }
        }

        public static FieldEncoding ValueOf(int code)
        {
            switch (code)
            {
                case (char) FieldEncoding.Noop:
                    return FieldEncoding.Noop;
                case (char) FieldEncoding.Fixed1:
                    return FieldEncoding.Fixed1;
                case (char) FieldEncoding.Fixed2:
                    return FieldEncoding.Fixed2;
                case (char) FieldEncoding.Fixed3:
                    return FieldEncoding.Fixed3;
                case (char) FieldEncoding.Fixed4:
                    return FieldEncoding.Fixed4;
                case (char) FieldEncoding.Fixed8:
                    return FieldEncoding.Fixed8;
                case (char) FieldEncoding.Fixed16:
                    return FieldEncoding.Fixed16;
                case (char) FieldEncoding.ByteArray:
                    return FieldEncoding.ByteArray;
                case (char) FieldEncoding.VarintString:
                    return FieldEncoding.VarintString;
                case (char) FieldEncoding.Varint:
                    return FieldEncoding.Varint;
                case (char) FieldEncoding.ZigZag:
                    return FieldEncoding.ZigZag;
                default:
                    throw new ArgumentException("unrecognised field encoding code: " + code + " ('" + (char) code +
                                                "').");
            }
        }
    }
}