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
        private readonly FieldDef _double = new FieldDef{Fid = 1, Type = FieldType.Double, Encoding = FieldEncoding.Fixed8, Name = "Field"};
        private static FieldDef _long = new FieldDef{Fid = 1, Type = FieldType.Long, Encoding = FieldEncoding.Fixed8, Name = "Field"};
        private static FieldDef _int = new FieldDef{Fid = 1, Type = FieldType.Integer, Encoding = FieldEncoding.Fixed4, Name = "Field"};
        private static FieldDef _string = new FieldDef{Fid = 1, Type = FieldType.String, Encoding = FieldEncoding.VarintString, Name = "Field"};

        [Test]
        public void test_decode_double_field()
        {
            var value = 34892343.2132;
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            var memoryStream = new MemoryStream(bytes);
            var decodeField = PriceUpdateDecoder.DecodeField(memoryStream,
                _double);
            Assert.AreEqual(value,
                decodeField);
        }

        [Test]
        public void test_decode_long_field()
        {
            var value = 483948038L;
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            var memoryStream = new MemoryStream(bytes);
            var decodeField = PriceUpdateDecoder.DecodeField(memoryStream,
                _long);
            Assert.AreEqual(value,
                decodeField);
        }

        [Test]
        public void test_decode_int_field()
        {
            var value = 483948038;
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            var memoryStream = new MemoryStream(bytes);
            var decodeField = PriceUpdateDecoder.DecodeField(memoryStream,
                _int);
            Assert.AreEqual(value,
                decodeField);
        }

        [Test]
        public void test_decode_string_field()
        {
            var s = "this is certainly a string";
            var memoryStream = new MemoryStream();
            Varint.WriteString(memoryStream, s);
            memoryStream.Position = 0;
            var decodeField = PriceUpdateDecoder.DecodeField(memoryStream,
                _string);
            Assert.AreEqual(s,
                decodeField);
        }

        [Test]
        public void skip_bytes_in_case_type_is_unknown()
        {
            var s = "ciao";
            byte[] unknown = new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9};
            double price = 9876.543;

            var memoryStream = new MemoryStream();

            // STRING
            Varint.WriteString(memoryStream, s);
            // UNDECODABLE
            Varint.WriteU32(memoryStream, unknown.Length);
            memoryStream.Write(unknown, 0, unknown.Length);
            // PRICE
            Varint.WriteU64(memoryStream, (long) (price * 1000000));

            memoryStream.Position = 0;

            Assert.AreEqual(s, PriceUpdateDecoder.DecodeField(memoryStream, _string));
            Assert.AreEqual(null, PriceUpdateDecoder.DecodeField(memoryStream,  new FieldDef{Fid = 1, Type = FieldType.Unrecognised,Encoding = FieldEncoding.ByteArray, Name = "field", Scale = 0}));
            Assert.AreEqual(price, PriceUpdateDecoder.DecodeField(memoryStream, new FieldDef{Fid = 1, Type = FieldType.Double, Encoding = FieldEncoding.Varint, Name = "field", Scale = 6}));
        }

        [Test]
        public void test_decode_price_field()
        {
            var memoryStream = new MemoryStream();
            var price = 43.45664;
            var priceTimesMillion = 43456640L;
            Varint.WriteU64(memoryStream, priceTimesMillion);

            memoryStream.Position = 0;
            Assert.AreEqual(price, PriceUpdateDecoder.DecodeField(memoryStream, new FieldDef{Fid = 1, Type = FieldType.Double, Encoding = FieldEncoding.Varint, Name = "field", Scale = 6}));
        }

        [Test]
        public void test_decode_size_field()
        {
            var memoryStream = new MemoryStream();
            var size = 3823908209333543L;
            Varint.WriteU64(memoryStream, size);

            memoryStream.Position = 0;
            Assert.AreEqual(size, PriceUpdateDecoder.DecodeField(memoryStream, new FieldDef(){Fid = 1, Type = FieldType.Long, Encoding = FieldEncoding.Varint, Name = "field"}));
        }

        [Test]
        public void test_decode_size_quantity()
        {
            var memoryStream = new MemoryStream();
            var quantity = 15000000.55;
            var quantityTimesHundred = 1500000055L;
            Varint.WriteU64(memoryStream, quantityTimesHundred);

            memoryStream.Position = 0;
            Assert.AreEqual(quantity, PriceUpdateDecoder.DecodeField(memoryStream, new FieldDef{Fid = 1, Type = FieldType.Double, Encoding = FieldEncoding.Varint, Name = "field", Scale = 2}));
        }

        [Test]
        public void test_decode_of_unknown_status()
        {
            var memoryStream = VarintTest.HexStream("73010b01");


            var mockSyncable = new Mock<ISyncable>();
            PriceUpdateDecoder.Visit(memoryStream, 1, null, null, mockSyncable.Object);
            mockSyncable.Verify(x => x.PriceStatus(1, SubscriptionStatus.PENDING, ""));
        }
    }
}