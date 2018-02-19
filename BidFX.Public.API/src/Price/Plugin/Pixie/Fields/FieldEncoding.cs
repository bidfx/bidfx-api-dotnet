using System;
using System.IO;
using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields
{
    /// <summary>
    /// This enumeration defines the different types of field encoding that are used by the Pixie protocol. When a
    /// client encountered a field type that it can not recognise due to protocol upgrades then it can use the field
    /// encoding to attempt to at least skip over the field on the incoming message. Although we may add new field
    /// types in the future we attempt to cover all possible field encodings in version 1 of the protocol.
    /// </summary>
    internal enum FieldEncoding
    {
        /// <summary>
        /// Zero sized field encoding so no operation required as no field is encoded.
        /// </summary>
        Noop = '0',

        /// <summary>
        /// Fixed size one byte field encoding.
        /// </summary>
        Fixed1 = '1',

        /// <summary>
        /// Fixed size two bytes field encoding.
        /// </summary>
        Fixed2 = '2',

        /// <summary>
        /// Fixed size three bytes field encoding.
        /// </summary>
        Fixed3 = '3',

        /// <summary>
        /// Fixed size four bytes field encoding.
        /// </summary>
        Fixed4 = '4',

        /// <summary>
        /// Fixed size eight bytes field encoding.
        /// </summary>
        Fixed8 = '8',

        /// <summary>
        /// Fixed size sixteen bytes field encoding.
        /// </summary>
        Fixed16 = '@',

        /// <summary>
        /// Varint-sized byte stream encoding type.
        /// </summary>
        ByteArray = 'B',

        /// <summary>
        /// Varint number encoding. Good for unsigned int or long values.
        /// </summary>
        Varint = 'V',

        /// <summary>
        /// Varint-string encoding type.
        /// </summary>
        VarintString = 'S',

        /// <summary>
        /// Zigzag number encdoing. Good for signed int or long values.
        /// </summary>
        ZigZag = 'Z'
    }

    internal static class FieldEncodingMethods
    {
        /// <summary>
        /// Attempts to skip over the value part of a field of known encoding type. The different implementations of this method "skip" a specific amount of bytes.
        /// </summary>
        /// <param name="fieldEncoding">the field encoding which defines how many bytes to skip</param>
        /// <param name="stream">the stream from which to skip bytes</param>
        /// <exception cref="ArgumentException">When field encoding is not recognised</exception>
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
                    uint byteArraySize = Varint.ReadU32(stream);
                    stream.Seek(byteArraySize, SeekOrigin.Current);
                    break;
                case FieldEncoding.VarintString:
                    uint varintStringSize = Varint.ReadU32(stream);
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

        /// <summary>
        /// Gets the enum based on it's code used in the Pixie wire format.
        /// </summary>
        /// <param name="code">the code letter of the type</param>
        /// <returns>the enum</returns>
        /// <exception cref="ArgumentException"></exception>
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