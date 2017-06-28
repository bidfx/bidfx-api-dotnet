using System.Collections.Generic;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields
{
    public class DataDictionaryUtilsTest
    {
        private static readonly FieldDef Bid = new FieldDef
        {
            Fid = 0,
            Type = FieldType.Double,
            Name = "Bid"
        };

        private static readonly FieldDef Ask = new FieldDef
        {
            Fid = 1,
            Type = FieldType.Double,
            Name = "Ask"
        };

        private static readonly FieldDef BidSize = new FieldDef
        {
            Fid = 2,
            Type = FieldType.Long,
            Name = "BidSize"
        };

        private static readonly FieldDef AskSize = new FieldDef
        {
            Fid = 3,
            Type = FieldType.Long,
            Name = "AskSize"
        };

        [Test]
        public void TestAddAllFields()
        {
            IDataDictionary dataDictionary = new ExtendableDataDictionary();
            dataDictionary.AddFieldDef(Bid);
            dataDictionary.AddFieldDef(Ask);
            DataDictionaryUtils.AddAllFields(dataDictionary, new List<FieldDef> {BidSize, AskSize});
            Assert.AreEqual(new List<FieldDef> {Bid, Ask, BidSize, AskSize}, dataDictionary.AllFieldDefs());
        }

        [Test]
        public void TestNullIsNotAValidFieldDef()
        {
            Assert.IsFalse(DataDictionaryUtils.IsValid(null));
        }

        [Test]
        public void TestAFieldDefIsValidIfAllComponentsAreSet()
        {
            var fieldDef = new FieldDef()
            {
                Fid = 1,
                Name = "Bid",
                Type = FieldType.Integer,
                Encoding = FieldEncoding.Fixed4,
                Scale = 4
            };
            Assert.IsTrue(DataDictionaryUtils.IsValid(fieldDef));
        }

        [Test]
        public void TestAFieldDefIsNotValidIfFidIsNotSet()
        {
            var fieldDef = new FieldDef()
            {
                Name = "Bid",
                Type = FieldType.Integer,
            };
            Assert.IsFalse(DataDictionaryUtils.IsValid(fieldDef));
        }

        [Test]
        public void TestAFieldDefIsNotValidIfNameIsNotSet()
        {
            var fieldDef = new FieldDef()
            {
                Fid = 1,
                Type = FieldType.Integer
            };
            Assert.IsFalse(DataDictionaryUtils.IsValid(fieldDef));
        }

        [Test]
        public void TestAFieldDefIsNotValidIfTypeIsNotSet()
        {
            var fieldDef = new FieldDef()
            {
                Fid = 1,
                Name = "Bid"
            };
            Assert.IsFalse(DataDictionaryUtils.IsValid(fieldDef));
        }
    }
}