using System;
using System.IO;
using BidFX.Public.API.Price.Plugin.Pixie.Fields;
using BidFX.Public.API.Price.Tools;
using Moq;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public class PriceUpdateDecoderTest
    {
        private readonly FieldDef _double = new FieldDef
        {
            Fid = 1,
            Type = FieldType.Double,
            Encoding = FieldEncoding.Fixed8,
            Name = "Field"
        };

        private static FieldDef _long = new FieldDef
        {
            Fid = 1,
            Type = FieldType.Long,
            Encoding = FieldEncoding.Fixed8,
            Name = "Field"
        };

        private static FieldDef _int = new FieldDef
        {
            Fid = 1,
            Type = FieldType.Integer,
            Encoding = FieldEncoding.Fixed4,
            Name = "Field"
        };

        private static FieldDef _string = new FieldDef
        {
            Fid = 1,
            Type = FieldType.String,
            Encoding = FieldEncoding.VarintString,
            Name = "Field"
        };

        [Test]
        public void test_decode_double_field()
        {
            double value = 34892343.2132;
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            MemoryStream memoryStream = new MemoryStream(bytes);
            object decodeField = PriceUpdateDecoder.DecodeField(memoryStream,
                _double);
            Assert.AreEqual(value,
                decodeField);
        }

        [Test]
        public void test_decode_long_field()
        {
            long value = 483948038L;
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            MemoryStream memoryStream = new MemoryStream(bytes);
            object decodeField = PriceUpdateDecoder.DecodeField(memoryStream,
                _long);
            Assert.AreEqual(value,
                decodeField);
        }

        [Test]
        public void test_decode_int_field()
        {
            int value = 483948038;
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            MemoryStream memoryStream = new MemoryStream(bytes);
            object decodeField = PriceUpdateDecoder.DecodeField(memoryStream,
                _int);
            Assert.AreEqual(value,
                decodeField);
        }

        [Test]
        public void test_decode_string_field()
        {
            string s = "this is certainly a string";
            MemoryStream memoryStream = new MemoryStream();
            Varint.WriteString(memoryStream, s);
            memoryStream.Position = 0;
            object decodeField = PriceUpdateDecoder.DecodeField(memoryStream,
                _string);
            Assert.AreEqual(s,
                decodeField);
        }

        [Test]
        public void skip_bytes_in_case_type_is_unknown()
        {
            string s = "ciao";
            byte[] unknown = new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9};
            double price = 9876.543;

            MemoryStream memoryStream = new MemoryStream();

            // STRING
            Varint.WriteString(memoryStream, s);
            // UNDECODABLE
            Varint.WriteU32(memoryStream, unknown.Length);
            memoryStream.Write(unknown, 0, unknown.Length);
            // PRICE
            Varint.WriteU64(memoryStream, (long) (price * 1000000));

            memoryStream.Position = 0;

            Assert.AreEqual(s, PriceUpdateDecoder.DecodeField(memoryStream, _string));
            Assert.AreEqual(null,
                PriceUpdateDecoder.DecodeField(memoryStream,
                    new FieldDef
                    {
                        Fid = 1,
                        Type = FieldType.Unrecognised,
                        Encoding = FieldEncoding.ByteArray,
                        Name = "field",
                        Scale = 0
                    }));
            Assert.AreEqual(price,
                PriceUpdateDecoder.DecodeField(memoryStream,
                    new FieldDef
                    {
                        Fid = 1,
                        Type = FieldType.Double,
                        Encoding = FieldEncoding.Varint,
                        Name = "field",
                        Scale = 6
                    }));
        }

        [Test]
        public void test_decode_price_field()
        {
            MemoryStream memoryStream = new MemoryStream();
            double price = 43.45664;
            long priceTimesMillion = 43456640L;
            Varint.WriteU64(memoryStream, priceTimesMillion);

            memoryStream.Position = 0;
            Assert.AreEqual(price,
                PriceUpdateDecoder.DecodeField(memoryStream,
                    new FieldDef
                    {
                        Fid = 1,
                        Type = FieldType.Double,
                        Encoding = FieldEncoding.Varint,
                        Name = "field",
                        Scale = 6
                    }));
        }

        [Test]
        public void test_decode_size_field()
        {
            MemoryStream memoryStream = new MemoryStream();
            long size = 3823908209333543L;
            Varint.WriteU64(memoryStream, size);

            memoryStream.Position = 0;
            Assert.AreEqual(size,
                PriceUpdateDecoder.DecodeField(memoryStream,
                    new FieldDef() {Fid = 1, Type = FieldType.Long, Encoding = FieldEncoding.Varint, Name = "field"}));
        }

        [Test]
        public void test_decode_size_quantity()
        {
            MemoryStream memoryStream = new MemoryStream();
            double quantity = 15000000.55;
            long quantityTimesHundred = 1500000055L;
            Varint.WriteU64(memoryStream, quantityTimesHundred);

            memoryStream.Position = 0;
            Assert.AreEqual(quantity,
                PriceUpdateDecoder.DecodeField(memoryStream,
                    new FieldDef
                    {
                        Fid = 1,
                        Type = FieldType.Double,
                        Encoding = FieldEncoding.Varint,
                        Name = "field",
                        Scale = 2
                    }));
        }

        [Test]
        public void test_decode_of_unknown_status()
        {
            MemoryStream memoryStream = VarintTest.HexStream("73010b01");


            Mock<ISyncable> mockSyncable = new Mock<ISyncable>();
            PriceUpdateDecoder.Visit(memoryStream, 1, null, null, mockSyncable.Object);
            mockSyncable.Verify(x => x.PriceStatus(1, SubscriptionStatus.PENDING, ""));
        }
    }
}