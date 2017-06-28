using System.Collections.Generic;
using System.IO;
using BidFX.Public.API.Price.Plugin.Pixie.Fields;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public class DataDictionaryMessageTest
    {
        private static readonly byte[] Message =
        {
            (byte) 0, (byte) 9, (byte) 0, (byte) 68, (byte) 86, (byte) 6, (byte) 4, (byte) 66, (byte) 105,
            (byte) 100, (byte) 1, (byte) 68, (byte) 86, (byte) 6, (byte) 4, (byte) 65, (byte) 115, (byte) 107, (byte) 2,
            (byte) 76, (byte) 86, (byte) 0, (byte) 8, (byte) 66, (byte) 105, (byte) 100, (byte) 83, (byte) 105,
            (byte) 122, (byte) 101, (byte) 3, (byte) 76, (byte) 86, (byte) 0, (byte) 8, (byte) 65, (byte) 115,
            (byte) 107, (byte) 83, (byte) 105, (byte) 122, (byte) 101, (byte) 4, (byte) 76, (byte) 86, (byte) 0,
            (byte) 8, (byte) 66, (byte) 105, (byte) 100, (byte) 84, (byte) 105, (byte) 109, (byte) 101, (byte) 5,
            (byte) 76, (byte) 86, (byte) 0, (byte) 8, (byte) 65, (byte) 115, (byte) 107, (byte) 84, (byte) 105,
            (byte) 109, (byte) 101, (byte) 6, (byte) 68, (byte) 86, (byte) 2, (byte) 14, (byte) 79, (byte) 114,
            (byte) 100, (byte) 101, (byte) 114, (byte) 81, (byte) 117, (byte) 97, (byte) 110, (byte) 116, (byte) 105,
            (byte) 116, (byte) 121, (byte) 9, (byte) 68, (byte) 90, (byte) 4, (byte) 10, (byte) 78, (byte) 101,
            (byte) 116, (byte) 67, (byte) 104, (byte) 97, (byte) 110, (byte) 103, (byte) 101, (byte) 10, (byte) 68,
            (byte) 90, (byte) 2, (byte) 14, (byte) 80, (byte) 101, (byte) 114, (byte) 99, (byte) 101, (byte) 110,
            (byte) 116, (byte) 67, (byte) 104, (byte) 97, (byte) 110, (byte) 103, (byte) 101
        };

        private readonly List<FieldDef> _fieldDefs = new List<FieldDef>
        {
            new FieldDef() {Fid = 0, Type = FieldType.Double, Name = "Bid", Encoding = FieldEncoding.Varint, Scale = 6},
            new FieldDef() {Fid = 1, Type = FieldType.Double, Name = "Ask", Encoding = FieldEncoding.Varint, Scale = 6},
            new FieldDef()
            {
                Fid = 2,
                Type = FieldType.Long,
                Name = "BidSize",
                Encoding = FieldEncoding.Varint,
                Scale = 0
            },
            new FieldDef()
            {
                Fid = 3,
                Type = FieldType.Long,
                Name = "AskSize",
                Encoding = FieldEncoding.Varint,
                Scale = 0
            },
            new FieldDef()
            {
                Fid = 4,
                Type = FieldType.Long,
                Name = "BidTime",
                Encoding = FieldEncoding.Varint,
                Scale = 0
            },
            new FieldDef()
            {
                Fid = 5,
                Type = FieldType.Long,
                Name = "AskTime",
                Encoding = FieldEncoding.Varint,
                Scale = 0
            },
            new FieldDef()
            {
                Fid = 6,
                Type = FieldType.Double,
                Name = "OrderQuantity",
                Encoding = FieldEncoding.Varint,
                Scale = 2
            },
            new FieldDef()
            {
                Fid = 9,
                Type = FieldType.Double,
                Name = "NetChange",
                Encoding = FieldEncoding.ZigZag,
                Scale = 4
            },
            new FieldDef()
            {
                Fid = 10,
                Type = FieldType.Double,
                Name = "PercentChange",
                Encoding = FieldEncoding.ZigZag,
                Scale = 2
            }
        };

        [Test]
        public void TestDecodeFullDataDictionaryMessage()
        {
            var memoryStream = new MemoryStream(Message);
            var dataDictionaryMessage = new DataDictionaryMessage(memoryStream);
            Assert.AreEqual(_fieldDefs, dataDictionaryMessage.FieldDefs);
        }

        [Test]
        public void TestToString()
        {
            var actual = new DataDictionaryMessage(new MemoryStream(Message)).ToString();
            Assert.AreEqual("DataDictionaryMessage(update=False, fields=[\n" +
                            "  fieldDef(FID=0, name=\"Bid\", type=Double, encoding=Varint, scale=6)\n" +
                            "  fieldDef(FID=1, name=\"Ask\", type=Double, encoding=Varint, scale=6)\n" +
                            "  fieldDef(FID=2, name=\"BidSize\", type=Long, encoding=Varint, scale=0)\n" +
                            "  fieldDef(FID=3, name=\"AskSize\", type=Long, encoding=Varint, scale=0)\n" +
                            "  fieldDef(FID=4, name=\"BidTime\", type=Long, encoding=Varint, scale=0)\n" +
                            "  fieldDef(FID=5, name=\"AskTime\", type=Long, encoding=Varint, scale=0)\n" +
                            "  fieldDef(FID=6, name=\"OrderQuantity\", type=Double, encoding=Varint, scale=2)\n" +
                            "  fieldDef(FID=9, name=\"NetChange\", type=Double, encoding=ZigZag, scale=4)\n" +
                            "  fieldDef(FID=10, name=\"PercentChange\", type=Double, encoding=ZigZag, scale=2)\n" +
                            "])", actual);
        }
    }
}