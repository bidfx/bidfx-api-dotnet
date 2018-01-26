using System.IO;
using System.Text;

namespace BidFX.Public.API.Price.Tools
{
    internal class Varint
    {
        private const int BitsPerByte = 7;
        private const int ContinuationBit = 1 << BitsPerByte;
        private const uint MaskLow7Bits = ContinuationBit - 1;

        public static uint ReadU32(Stream stream)
        {
            uint result = 0;
            for (var offset = 0; offset < 32; offset += BitsPerByte)
            {
                var nextByte = stream.ReadByte();
                if (nextByte == -1)
                {
                    throw new EndOfStreamException("stream ended while reading varint");
                }
                var b = (byte) nextByte;
                result |= (uint) (b & 0x7f) << offset;
                if (IsFinalByte(b)) break;
            }
            return result;
        }

        public static ulong ReadU64(Stream stream)
        {
            ulong result = 0L;
            for (var offset = 0; offset < 64; offset += BitsPerByte)
            {
                var nextByte = stream.ReadByte();
                if (nextByte == -1)
                {
                    throw new EndOfStreamException("stream ended while reading varint");
                }
                var b = (byte) nextByte;
                result |= (ulong) (b & MaskLow7Bits) << offset;
                if (IsFinalByte(b)) break;
            }
            return result;
        }

        public static void WriteU32(Stream stream, int value)
        {
            WriteU32(stream, (uint) value);
        }

        public static void WriteU32(Stream stream, uint value)
        {
            while ((value & ~MaskLow7Bits) != 0)
            {
                stream.WriteByte((byte) (value & MaskLow7Bits | ContinuationBit));
                value = value >> BitsPerByte;
            }
            stream.WriteByte((byte) value);
        }

        public static void WriteU64(Stream stream, long value)
        {
            WriteU64(stream, (ulong) value);
        }

        public static void WriteU64(Stream stream, ulong value)
        {
            while ((value & ~MaskLow7Bits) != 0)
            {
                stream.WriteByte((byte) (value & MaskLow7Bits | ContinuationBit));
                value = value >> BitsPerByte;
            }
            stream.WriteByte((byte) value);
        }

        public static bool IsFinalByte(int b)
        {
            return (b & (1 << 7)) == 0;
        }

        public static string ReadString(Stream stream)
        {
            var s = "";
            var length = ReadU32(stream);
            if (length == 0) return null;
            for (var x = 0; x < length - 1; x++)
            {
                var nextByte = stream.ReadByte();
                if (nextByte == -1)
                {
                    throw new EndOfStreamException("stream ended while reading string");
                }
                s += (char) nextByte;
            }
            return s;
        }

        public static void WriteString(Stream stream, string s)
        {
            if (s == null)
            {
                WriteU32(stream, 0);
            }
            else
            {
                var bytes = Encoding.ASCII.GetBytes(s);
                WriteU32(stream, bytes.Length + 1);
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        public static void WriteStringArray(Stream stream, string[] array)
        {
            WriteU32(stream, array.Length);
            foreach (var s in array)
            {
                WriteString(stream, s);
            }
        }

        public static long ZigzagToLong(ulong zigzag)
        {
            return (long) (zigzag >> 1) ^ -(long) (zigzag & 1L);
        }

        public static int ZigzagToInt(uint zigzag)
        {
            return (int) (zigzag >> 1) ^ -(int) (zigzag & 1);
        }
    }
}