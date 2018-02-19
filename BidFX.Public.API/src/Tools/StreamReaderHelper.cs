using System;
using System.IO;

namespace BidFX.Public.API.Price.Tools
{
    internal static class StreamReaderHelper
    {
        public static int ReadInt4(Stream stream)
        {
            byte[] bytes = ReadBytes(stream, 4);
            return BitConverter.ToInt32(bytes, 0);
        }

        public static float ReadFloat4(Stream stream)
        {
            byte[] bytes = ReadBytes(stream, 4);
            return BitConverter.ToSingle(bytes, 0);
        }

        public static double ReadDouble8(Stream stream)
        {
            byte[] bytes = ReadBytes(stream, 8);
            return BitConverter.ToDouble(bytes, 0);
        }

        public static short ReadShort(Stream stream)
        {
            byte[] bytes = ReadBytes(stream, 2);
            return BitConverter.ToInt16(bytes, 0);
        }

        public static int ReadMedium(Stream stream)
        {
            byte[] bytes = ReadBytes(stream, 3);
            return bytes[0] + (bytes[1] << 8) + (bytes[2] << 16);
        }

        public static long ReadLong(Stream stream)
        {
            byte[] bytes = ReadBytes(stream, 8);
            return BitConverter.ToInt64(bytes, 0);
        }

        private static byte[] ReadBytes(Stream stream, int size)
        {
            byte[] bytes = new byte[size];
            for (int i = 0; i < size; i++)
            {
                bytes[i] = (byte) stream.ReadByte();
            }

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return bytes;
        }
    }
}