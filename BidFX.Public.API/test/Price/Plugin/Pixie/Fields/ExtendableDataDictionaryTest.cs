using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields
{
    public class ExtendableDataDictionaryTest
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

        private static readonly FieldDef Status = new FieldDef
        {
            Fid = 9,
            Type = FieldType.String,
            Name = "Status"
        };

        private IDataDictionary _dataDictionary;

        [SetUp]
        public void SetUp()
        {
            _dataDictionary = new ExtendableDataDictionary();
            _dataDictionary.AddFieldDef(Bid);
            _dataDictionary.AddFieldDef(Ask);
            _dataDictionary.AddFieldDef(BidSize);
            _dataDictionary.AddFieldDef(AskSize);
            _dataDictionary.AddFieldDef(Status);
        }

        [Test]
        public void TestNewDictionaryIsInitiallyEmpty()
        {
            Assert.AreEqual(0, new ExtendableDataDictionary().Size());
        }

        [Test]
        public void TestLookupByFid()
        {
            Assert.AreEqual(Bid, _dataDictionary.FieldDefByFid(Bid.Fid));
            Assert.AreEqual(Ask, _dataDictionary.FieldDefByFid(Ask.Fid));
            Assert.AreEqual(BidSize, _dataDictionary.FieldDefByFid(BidSize.Fid));
            Assert.AreEqual(AskSize, _dataDictionary.FieldDefByFid(AskSize.Fid));
            Assert.AreEqual(Status, _dataDictionary.FieldDefByFid(Status.Fid));
        }

        [Test]
        public void TestLookupByFidReturnsNullForUnknownFids()
        {
            Assert.IsNull(_dataDictionary.FieldDefByFid(6));
            Assert.IsNull(_dataDictionary.FieldDefByFid(15));
            Assert.IsNull(_dataDictionary.FieldDefByFid(100));
        }

        [Test]
        public void TestLookupByName()
        {
            Assert.AreEqual(Bid, _dataDictionary.FieldDefByName(Bid.Name));
            Assert.AreEqual(Ask, _dataDictionary.FieldDefByName(Ask.Name));
            Assert.AreEqual(BidSize, _dataDictionary.FieldDefByName(BidSize.Name));
            Assert.AreEqual(AskSize, _dataDictionary.FieldDefByName(AskSize.Name));
            Assert.AreEqual(Status, _dataDictionary.FieldDefByName(Status.Name));
        }

        [Test]
        public void TestLookupByNameReturnsNullForUnknownFieldNames()
        {
            Assert.IsNull(_dataDictionary.FieldDefByName(""));
            Assert.IsNull(_dataDictionary.FieldDefByName("Unknown"));
            Assert.IsNull(_dataDictionary.FieldDefByName("SillyFieldName"));
        }

        [Test]
        public void TestAllFieldsAreReturnInFidSequenceOrder()
        {
            Assert.AreEqual(new List<FieldDef> {Bid, Ask, BidSize, AskSize, Status}, _dataDictionary.AllFieldDefs());
        }

        [Test]
        public void TestAllFieldsAreReturnInFidSequenceOrderEvenWhenAddedInADifferentOrder()
        {
            IDataDictionary dataDictionary = new ExtendableDataDictionary();
            dataDictionary.AddFieldDef(Ask);
            dataDictionary.AddFieldDef(BidSize);
            dataDictionary.AddFieldDef(Status);
            dataDictionary.AddFieldDef(Bid);
            dataDictionary.AddFieldDef(AskSize);
            Assert.AreEqual(new List<FieldDef> {Bid, Ask, BidSize, AskSize, Status}, dataDictionary.AllFieldDefs());
        }

        [Test]
        public void TestNewFieldsDefsCanBeAddedToTheEndOfTheDictionary()
        {
            FieldDef endSize = new FieldDef()
            {
                Fid = 10,
                Type = FieldType.Long,
                Name = "EndSize"
            };
            Assert.IsNull(_dataDictionary.FieldDefByFid(endSize.Fid));
            Assert.IsNull(_dataDictionary.FieldDefByName(endSize.Name));
            _dataDictionary.AddFieldDef(endSize);
            Assert.AreEqual(endSize, _dataDictionary.FieldDefByFid(endSize.Fid));
            Assert.AreEqual(endSize, _dataDictionary.FieldDefByName(endSize.Name));
        }

        [Test]
        public void TestNewFieldsDefsCanBeAddedWithVaryLargeFiDsAndTheDictionaryWillGrow()
        {
            FieldDef endSize = new FieldDef()
            {
                Fid = 5000,
                Type = FieldType.Long,
                Name = "EndSize"
            };
            Assert.IsNull(_dataDictionary.FieldDefByFid(endSize.Fid));
            Assert.IsNull(_dataDictionary.FieldDefByName(endSize.Name));
            _dataDictionary.AddFieldDef(endSize);
            Assert.AreEqual(endSize, _dataDictionary.FieldDefByFid(endSize.Fid));
            Assert.AreEqual(endSize, _dataDictionary.FieldDefByName(endSize.Name));
        }

        [Test]
        public void TestNewFieldsDefsCanBeAddedToTheMiddleOfTheDictionary()
        {
            FieldDef mid = new FieldDef()
            {
                Fid = 6,
                Type = FieldType.Double,
                Name = "Mid"
            };
            Assert.IsNull(_dataDictionary.FieldDefByFid(mid.Fid));
            Assert.IsNull(_dataDictionary.FieldDefByName(mid.Name));
            _dataDictionary.AddFieldDef(mid);
            Assert.AreEqual(mid, _dataDictionary.FieldDefByFid(mid.Fid));
            Assert.AreEqual(mid, _dataDictionary.FieldDefByName(mid.Name));
        }

        [Test]
        public void TestFieldDefinitionsCanBeHaveTheirNameRedefined()
        {
            FieldDef redefined = new FieldDef()
            {
                Fid = Bid.Fid,
                Type = FieldType.Double,
                Name = "BidPrice"
            };
            _dataDictionary.AddFieldDef(redefined);
            Assert.AreEqual(redefined, _dataDictionary.FieldDefByFid(redefined.Fid));
            Assert.AreEqual(redefined, _dataDictionary.FieldDefByName(redefined.Name));
        }

        [Test]
        public void WhenAFieldNameIsRedefinedItsOldDefinitionIsRemoved()
        {
            FieldDef redefined = new FieldDef()
            {
                Fid = Bid.Fid,
                Type = Bid.Type,
                Name = "BidPrice"
            };
            _dataDictionary.AddFieldDef(redefined);
            Assert.IsNull(_dataDictionary.FieldDefByName(Bid.Name));
            Assert.AreEqual(new List<FieldDef> {redefined, Ask, BidSize, AskSize, Status},
                _dataDictionary.AllFieldDefs());
        }

        [Test]
        public void TestFieldDefinitionsCanBeHaveTheirFidRedefined()
        {
            FieldDef redefined = new FieldDef()
            {
                Fid = 4,
                Type = Bid.Type,
                Name = Bid.Name
            };
            _dataDictionary.AddFieldDef(redefined);
            Assert.AreEqual(redefined, _dataDictionary.FieldDefByFid(redefined.Fid));
            Assert.AreEqual(redefined, _dataDictionary.FieldDefByName(redefined.Name));
        }

        [Test]
        public void WhenAFieldFidIsRedefinedItsOldDefinitionIsRemoved()
        {
            FieldDef redefined = new FieldDef()
            {
                Fid = 4,
                Type = Bid.Type,
                Name = Bid.Name
            };
            _dataDictionary.AddFieldDef(redefined);
            Assert.IsNull(_dataDictionary.FieldDefByFid(Bid.Fid));
            Assert.AreEqual(new List<FieldDef> {Ask, BidSize, AskSize, redefined, Status},
                _dataDictionary.AllFieldDefs());
        }

        [Test]
        public void TestCannotAddNullFieldDef()
        {
            try
            {
                _dataDictionary.AddFieldDef(null);
                Assert.Fail("error should be thrown");
            }
            catch (ArgumentException e)
            {
            }
        }

        [Test]
        public void TestCannotAddNullFieldDefWithUndefinedFid()
        {
            try
            {
                FieldDef fieldDef = new FieldDef()
                {
                    Type = FieldType.String,
                    Name = "Account"
                };
                _dataDictionary.AddFieldDef(fieldDef);
                Assert.Fail("error should be thrown");
            }
            catch (ArgumentException e)
            {
            }
        }

        [Test]
        public void TestCannotAddNullFieldDefWithUndefinedType()
        {
            try
            {
                FieldDef fieldDef = new FieldDef()
                {
                    Fid = 13,
                    Name = "Account"
                };
                _dataDictionary.AddFieldDef(fieldDef);
                Assert.Fail("error should be thrown");
            }
            catch (ArgumentException e)
            {
            }
        }

        [Test]
        public void TestCannotAddNullFieldDefWithUndefinedName()
        {
            try
            {
                FieldDef fieldDef = new FieldDef()
                {
                    Fid = 13,
                    Type = FieldType.String
                };
                _dataDictionary.AddFieldDef(fieldDef);
            }
            catch (ArgumentException e)
            {
            }
        }

        [Test]
        public void TestNextFreeFidForAnEmptyDataDictionaryIsZero()
        {
            Assert.AreEqual(0, new ExtendableDataDictionary().NextFreeFid());
        }

        [Test]
        public void TestNextFreeFidForAOneEntryDataDictionaryIsOneMoreThanTheLargestExistingFid()
        {
            IDataDictionary dataDictionary = new ExtendableDataDictionary();
            dataDictionary.AddFieldDef(Ask);
            Assert.AreEqual(Ask.Fid + 1, dataDictionary.NextFreeFid());
        }

        [Test]
        public void TestNextFreeFidForATwoEntryDataDictionaryIsOneMoreThanTheLargestExistingFid()
        {
            IDataDictionary dataDictionary = new ExtendableDataDictionary();
            dataDictionary.AddFieldDef(Ask);
            dataDictionary.AddFieldDef(AskSize);
            Assert.AreEqual(AskSize.Fid + 1, dataDictionary.NextFreeFid());
        }

        [Test]
        public void TestNextFreeFidForAPopulatedDataDictionaryIsOneMoreThanTheLargestExistingFid()
        {
            Assert.AreEqual(Status.Fid + 1, _dataDictionary.NextFreeFid());
            _dataDictionary.AddFieldDef(new FieldDef() {Fid = 19, Type = FieldType.String, Name = "SettlementType"});
            Assert.AreEqual(20, _dataDictionary.NextFreeFid());
        }

        [Test]
        public void TestTheAbilityOfTheCollectionToGrowItsCapacity()
        {
            IDataDictionary dataDictionary = new ExtendableDataDictionary();
            Assert.AreEqual(0, dataDictionary.Size());
            Assert.AreEqual(0, dataDictionary.NextFreeFid());

            for (int i = 0; i < 1000; ++i)
            {
                dataDictionary.AddFieldDef(new FieldDef() {Fid = i, Type = FieldType.String, Name = "Bidder" + i});
                Assert.AreEqual(i + 1, dataDictionary.Size());
                Assert.AreEqual(i + 1, dataDictionary.NextFreeFid());
            }
        }
    }
}