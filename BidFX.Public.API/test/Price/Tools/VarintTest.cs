using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Tools
{
    public class VarintTest
    {
        private static readonly char[] HexChars =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'
        };

        [Test]
        public void TestWriteUnsigned32()
        {
            Assert.AreEqual("00", WriteU32ToHexString(0));
            Assert.AreEqual("01", WriteU32ToHexString(1));
            Assert.AreEqual("02", WriteU32ToHexString(2));
            Assert.AreEqual("11", WriteU32ToHexString(17));
            Assert.AreEqual("7f", WriteU32ToHexString(127));

            Assert.AreEqual("8001", WriteU32ToHexString(128));
            Assert.AreEqual("8002", WriteU32ToHexString(256));
            Assert.AreEqual("a274", WriteU32ToHexString(14882));
            Assert.AreEqual("a08d06", WriteU32ToHexString(100000));
            Assert.AreEqual("ffffffff07", WriteU32ToHexString(int.MaxValue));
        }

        [Test]
        public void TestWriteU32WithNegativeUsesFiveBytes()
        {
            var stream = new MemoryStream(16);
            Varint.WriteU32(stream, -1);
            Assert.AreEqual(5, stream.ToArray().Length);
        }

        [Test]
        public void TestWriteU32CanBeReadWithReadU32()
        {
            Assert.AreEqual(255, Varint.ReadU32(WriteU32ToNewStream(255)));
            Assert.AreEqual(1, Varint.ReadU32(WriteU32ToNewStream(1)));
            Assert.AreEqual(2, Varint.ReadU32(WriteU32ToNewStream(2)));
            Assert.AreEqual(3, Varint.ReadU32(WriteU32ToNewStream(3)));
            Assert.AreEqual(17, Varint.ReadU32(WriteU32ToNewStream(17)));
            Assert.AreEqual(127, Varint.ReadU32(WriteU32ToNewStream(127)));
            Assert.AreEqual(128, Varint.ReadU32(WriteU32ToNewStream(128)));
            Assert.AreEqual(1000, Varint.ReadU32(WriteU32ToNewStream(1000)));
            Assert.AreEqual(10000, Varint.ReadU32(WriteU32ToNewStream(10000)));
            Assert.AreEqual(872682762, Varint.ReadU32(WriteU32ToNewStream(872682762)));
            Assert.AreEqual(int.MaxValue, Varint.ReadU32(WriteU32ToNewStream(int.MaxValue)));
        }

        [Test]
        public void TestReadUnsigned32()
        {
            Assert.AreEqual(0, Varint.ReadU32(HexStream("00")));
            Assert.AreEqual(1, Varint.ReadU32(HexStream("01")));
            Assert.AreEqual(2, Varint.ReadU32(HexStream("02")));
            Assert.AreEqual(16, Varint.ReadU32(HexStream("10")));
            Assert.AreEqual(127, Varint.ReadU32(HexStream("7f")));

            Assert.AreEqual(128, Varint.ReadU32(HexStream("8001")));
            Assert.AreEqual(129, Varint.ReadU32(HexStream("8101")));
            Assert.AreEqual(130, Varint.ReadU32(HexStream("8201")));
            Assert.AreEqual(65535, Varint.ReadU32(HexStream("ffff03")));
        }

        [Test]
        public void TestWriteUnsigned64()
        {
            Assert.AreEqual("00", WriteU64ToHexString(0));
            Assert.AreEqual("01", WriteU64ToHexString(1));
            Assert.AreEqual("02", WriteU64ToHexString(2));
            Assert.AreEqual("11", WriteU64ToHexString(17));
            Assert.AreEqual("7f", WriteU64ToHexString(127));

            Assert.AreEqual("8001", WriteU64ToHexString(128));
            Assert.AreEqual("a274", WriteU64ToHexString(14882));
            Assert.AreEqual("a08d06", WriteU64ToHexString(100000));
            Assert.AreEqual("ffffffffffffffff7f", WriteU64ToHexString(9223372036854775807L));
        }

        [Test]
        public void TestWriteU64WithNegativeUsesUpTenBytes()
        {
            MemoryStream stream = new MemoryStream(16);
            Varint.WriteU64(stream, -23L);
            Assert.AreEqual(10, stream.ToArray().Length);
        }

        [Test]
        public void TestWriteU64CanBeReadWithReadU64()
        {
            Assert.AreEqual(0, Varint.ReadU64(WriteU64ToNewStream(0)));
            Assert.AreEqual(1, Varint.ReadU64(WriteU64ToNewStream(1)));
            Assert.AreEqual(2, Varint.ReadU64(WriteU64ToNewStream(2)));
            Assert.AreEqual(3, Varint.ReadU64(WriteU64ToNewStream(3)));
            Assert.AreEqual(17, Varint.ReadU64(WriteU64ToNewStream(17)));
            Assert.AreEqual(127, Varint.ReadU64(WriteU64ToNewStream(127)));
            Assert.AreEqual(128, Varint.ReadU64(WriteU64ToNewStream(128)));
            Assert.AreEqual(1000, Varint.ReadU64(WriteU64ToNewStream(1000)));
            Assert.AreEqual(10000, Varint.ReadU64(WriteU64ToNewStream(10000)));
            Assert.AreEqual(872682762, Varint.ReadU64(WriteU64ToNewStream(872682762)));
            Assert.AreEqual(int.MaxValue, Varint.ReadU64(WriteU64ToNewStream(int.MaxValue)));
            Assert.AreEqual(10000000000000L, Varint.ReadU64(WriteU64ToNewStream(10000000000000L)));
            Assert.AreEqual(836286376726767676L, Varint.ReadU64(WriteU64ToNewStream(836286376726767676L)));
            Assert.AreEqual(long.MaxValue, Varint.ReadU64(WriteU64ToNewStream(long.MaxValue)));
        }

        [Test]
        public void TestReadUnsigned64()
        {
            Assert.AreEqual(0, Varint.ReadU64(HexStream("00")));
            Assert.AreEqual(1, Varint.ReadU64(HexStream("01")));
            Assert.AreEqual(2, Varint.ReadU64(HexStream("02")));
            Assert.AreEqual(16, Varint.ReadU64(HexStream("10")));
            Assert.AreEqual(127, Varint.ReadU64(HexStream("7f")));

            Assert.AreEqual(128, Varint.ReadU64(HexStream("8001")));
            Assert.AreEqual(129, Varint.ReadU64(HexStream("8101")));
            Assert.AreEqual(130, Varint.ReadU64(HexStream("8201")));
            Assert.AreEqual(65535, Varint.ReadU64(HexStream("ffff03")));
        }

        [Test]
        public void TestReadString()
        {
            Assert.AreEqual("X", Varint.ReadString(WriteStringToStream("X")));
            Assert.AreEqual("test", Varint.ReadString(WriteStringToStream("test")));
            Assert.AreEqual("hello world", Varint.ReadString(WriteStringToStream("hello world")));
        }

        [Test]
        public void TestWriteString()
        {
            var stream = new MemoryStream(10);
            Varint.WriteString(stream, "test");
            Assert.AreEqual("0574657374", StreamAsHex(stream));
        }

        [Test]
        public void TestZigzagToInt()
        {
            Assert.AreEqual(0, Varint.ZigzagToInt(0));
            Assert.AreEqual(-1, Varint.ZigzagToInt(1));
            Assert.AreEqual(1, Varint.ZigzagToInt(2));
            Assert.AreEqual(-2, Varint.ZigzagToInt(3));
            Assert.AreEqual(2, Varint.ZigzagToInt(4));
            Assert.AreEqual(-3, Varint.ZigzagToInt(5));
            Assert.AreEqual(3, Varint.ZigzagToInt(6));
            Assert.AreEqual(-4, Varint.ZigzagToInt(7));
            Assert.AreEqual(4, Varint.ZigzagToInt(8));

            Assert.AreEqual(1000, Varint.ZigzagToInt(2000));
            Assert.AreEqual(-1000, Varint.ZigzagToInt(1999));
        }

        [Test]
        public void TestZigzagToLong()
        {
            Assert.AreEqual(0, Varint.ZigzagToLong(0));
            Assert.AreEqual(-1, Varint.ZigzagToLong(1));
            Assert.AreEqual(1, Varint.ZigzagToLong(2));
            Assert.AreEqual(-2, Varint.ZigzagToLong(3));
            Assert.AreEqual(2, Varint.ZigzagToLong(4));
            Assert.AreEqual(-3, Varint.ZigzagToLong(5));
            Assert.AreEqual(3, Varint.ZigzagToLong(6));
            Assert.AreEqual(-4, Varint.ZigzagToLong(7));
            Assert.AreEqual(4, Varint.ZigzagToLong(8));

            Assert.AreEqual(10000000000L, Varint.ZigzagToLong(20000000000L));
            Assert.AreEqual(-10000000000L, Varint.ZigzagToLong(19999999999L));
        }

        private static MemoryStream WriteStringToStream(string s)
        {
            var bytes = new byte[s.Length + 1];
            bytes[0] = (byte) (s.Length + 1);
            Encoding.UTF8.GetBytes(s, 0, s.Length, bytes, 1);
            return new MemoryStream(bytes);
        }

        private static string WriteU32ToHexString(int value)
        {
            return StreamAsHex(WriteU32ToNewStream(value));
        }

        private static string WriteU64ToHexString(long value)
        {
            return StreamAsHex(WriteU64ToNewStream(value));
        }

        private static MemoryStream WriteU32ToNewStream(int value)
        {
            var memoryStream = new MemoryStream();
            Varint.WriteU32(memoryStream, value);
            return new MemoryStream(memoryStream.ToArray());
        }

        private static MemoryStream WriteU64ToNewStream(long value)
        {
            var memoryStream = new MemoryStream();
            Varint.WriteU64(memoryStream, value);
            return new MemoryStream(memoryStream.ToArray());
        }

        private static string StreamAsHex(MemoryStream stream)
        {
            var array = stream.ToArray();
            return EncodeAsHex(array);
        }

        private static MemoryStream HexStream(string hexString)
        {
            var bytes = DecodeFromHex(hexString);
            return new MemoryStream(bytes);
        }

        private static string EncodeAsHex(byte[] bytes)
        {
            var stringBuilder = new StringBuilder(bytes.Length << 1);
            foreach (var b in bytes)
            {
                stringBuilder.Append(HexChars[(b >> 4 & 0xf)]);
                stringBuilder.Append(HexChars[(b & 0xf)]);
            }
            return stringBuilder.ToString();
        }

        private static byte[] DecodeFromHex(string hex)
        {
            var bytes = new byte[hex.Length + 1 >> 1];
            var byteIndex = 0;
            var charIndex = 0;
            var charArray = hex.ToCharArray();
            if ((hex.Length & 1) == 1)
            {
                bytes[byteIndex++] = (byte) DecodeNibble(charArray[charIndex++]);
            }
            while (byteIndex < bytes.Length)
            {
                bytes[byteIndex++] = DecodeByte(charArray[charIndex++], charArray[charIndex++]);
            }
            return bytes;
        }

        private static int DecodeNibble(char c)
        {
            if (c >= '0' && c <= '9') return c - '0';
            if (c >= 'a' && c <= 'f') return c - 'a' + 10;
            if (c >= 'A' && c <= 'F') return c - 'A' + 10;
            throw new FormatException();
        }

        private static byte DecodeByte(char c1, char c2)
        {
            var high = DecodeNibble(c1);
            var low = DecodeNibble(c2);
            return (byte) ((high & 0x0f) << 4 | low);
        }
    }
}