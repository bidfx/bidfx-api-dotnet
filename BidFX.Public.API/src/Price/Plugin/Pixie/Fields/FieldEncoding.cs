using System;
using System.IO;
using BidFX.Public.API.Price.Tools;

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
        public static void SkipFieldValue(this FieldEncoding fieldEncoding, Stream stream)
        {
            switch (fieldEncoding)
            {
                case FieldEncoding.Noop:
                    break;
                case FieldEncoding.Fixed1:
                    stream.Seek(1, SeekOrigin.Current);
                    break;
                case FieldEncoding.Fixed2:
                    stream.Seek(2, SeekOrigin.Current);
                    break;
                case FieldEncoding.Fixed3:
                    stream.Seek(3, SeekOrigin.Current);
                    break;
                case FieldEncoding.Fixed4:
                    stream.Seek(4, SeekOrigin.Current);
                    break;
                case FieldEncoding.Fixed8:
                    stream.Seek(8, SeekOrigin.Current);
                    break;
                case FieldEncoding.Fixed16:
                    stream.Seek(16, SeekOrigin.Current);
                    break;
                case FieldEncoding.ByteArray:
                    var byteArraySize = Varint.ReadU32(stream);
                    stream.Seek(byteArraySize, SeekOrigin.Current);
                    break;
                case FieldEncoding.VarintString:
                    var varintStringSize = Varint.ReadU32(stream);
                    stream.Seek(varintStringSize - 1, SeekOrigin.Current);
                    break;
                case FieldEncoding.Varint:
                    while (!Varint.IsFinalByte(stream.ReadByte()))
                    {
                    }
                    break;
                case FieldEncoding.ZigZag:
                    while (!Varint.IsFinalByte(stream.ReadByte()))
                    {
                    }
                    break;
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