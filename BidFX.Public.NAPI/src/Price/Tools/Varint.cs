using System.IO;
using System.Text;

namespace BidFX.Public.NAPI.Price.Tools
{
    public class Varint
    {
        private const int BitsPerByte = 7;
        private const int ContinuationBit = 1 << BitsPerByte;
        private const int MaskLow7Bits = ContinuationBit - 1;
        
        public static int ReadU32(Stream stream)
        {
            var result = 0;
            for (var offset = 0; offset < 32; offset += 7)
            {
                var nextByte = stream.ReadByte();
                if (nextByte == -1)
                {
                    throw new EndOfStreamException("stream ended while reading varint");
                }
                var b = (byte) nextByte;
                result |= (b & 0x7f) << offset;
                if (IsFinalByte(b)) break;
            }
            return result;
        }

        public static void WriteU32(Stream stream, int value)
        {
            while ((value & ~MaskLow7Bits) != 0)
            {
                stream.WriteByte((byte) (value & MaskLow7Bits | ContinuationBit));
                value = (int) ((uint)value >> BitsPerByte);
            }
            stream.WriteByte((byte)value);
        }

        public static void WriteU64(Stream stream, long value)
        {
            while ((value & ~MaskLow7Bits) != 0)
            {
                stream.WriteByte((byte) (value & MaskLow7Bits | ContinuationBit));
                value = (int) ((uint)value >> BitsPerByte);
            }
            stream.WriteByte((byte)value);
        }

        private static bool IsFinalByte(int b)
        {
            return b >= 0;
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
    }
}