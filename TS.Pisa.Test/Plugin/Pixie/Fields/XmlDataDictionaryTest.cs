using System;
using System.IO;
using System.Xml;
using TS.Pisa.Tools.Config;
namespace TS.Pisa.Plugin.Pixie.Fields
{
    [TestFixture]
    public class XmlDataDictionaryTest
    {
        private readonly string _Xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" + "<dataDictionary>\n" + "    <fieldDefs>\n" + "        <fieldDef FID=\"0\" type=\"Long\"     name=\"SystemTime\" />\n" + "        <fieldDef FID=\"1\" type=\"String\"   name=\"PriceID\" />\n"
             + "        <fieldDef FID=\"2\" type=\"Double\"   name=\"Bid\" />\n" + "        <fieldDef FID=\"3\" type=\"Double\"   name=\"Ask\" encoding=\"varint\" scale=\"6\" />\n" + "        <fieldDef FID=\"4\" type=\"Long\"     name=\"BidSize\" />\n"
             + "        <fieldDef FID=\"5\" type=\"Long\"     name=\"AskSize\" encoding=\"zigzag\" />\n" + "        <fieldDef FID=\"10\" type=\"Discard\" name=\"Status\" />\n" + "    </fieldDefs>\n" + "</dataDictionary>\n";
        private static readonly IFieldDef SystemTime = BeanFieldDef.CreateFieldDef(0, FieldType.Long, "SystemTime");

        private static readonly IFieldDef PriceId = BeanFieldDef.CreateFieldDef(1, FieldType.String, "PriceID");
        private static readonly IFieldDef Bid = BeanFieldDef.CreateFieldDef(2, FieldType.Double, "Bid");

        private static readonly IFieldDef Ask = BeanFieldDef.CreateFieldDef(3, FieldType.Double, "Ask", FieldEncoding.Varint, 6);
        private static readonly IFieldDef BidSize = BeanFieldDef.CreateFieldDef(4, FieldType.Long, "BidSize");

        private static readonly IFieldDef AskSize = BeanFieldDef.CreateFieldDef(5, FieldType.Long, "AskSize", FieldEncoding.Zigzag, 0);
        private static readonly IFieldDef Status = BeanFieldDef.CreateFieldDef(10, FieldType.Discard, "Status");

        private readonly XmlDataDictionary _dataDictionary;
        /// <exception cref="System.Exception"/>
        public XmlDataDictionaryTest()
        {
            _dataDictionary = new XmlDataDictionary(CreateDocument(_Xml));
        }
        /// <exception cref="System.Exception"/>
        public virtual void TestCannotBeConstructedWithNullDocument()
        {
            new XmlDataDictionary(null);
        }
        /// <exception cref="System.Exception"/>
        public virtual void TestCannotBeConstructedWithDuplicateFieldNames()
        {
            try
            {
                new XmlDataDictionary(CreateDocument(_Xml.ReplaceFirst("BidSize", "AskSize")));
            }
            catch (ConfigException e)
            {
                Assert.AreEqual("duplicate name \"AskSize\" in data dictionary FIDs 5 and 4", e.Message);
                throw;
            }
        }
        /// <exception cref="System.Exception"/>
        public virtual void TestCannotBeConstructedWithDuplicateFIDs()
        {
            try
            {
                new XmlDataDictionary(CreateDocument(_Xml.ReplaceFirst("FID=\"2\"", "FID=\"3\"")));
            }
            catch (ConfigException e)
            {
                Assert.AreEqual("duplicate FID 3 in data dictionary for field names \"Ask\" and \"Bid\"", e.Message);
                throw;
            }
        }
        /// <exception cref="System.Exception"/>
        public virtual void TestCannotBeConstructedWithInvalidFieldType()
        {
            try
            {
                new XmlDataDictionary(CreateDocument(_Xml.ReplaceFirst("String", "prostitute")));
            }
            catch (ConfigException e)
            {
                // TODO review this test.  Error message different in JDK 1.7
                //            assertEquals("cannot parse data dictionary due to: No enum const class com.tradingscreen.pisa.plugin.pixie.fields.FieldType.PROSTITUTE",
                //                         e.getMessage());
                throw;
            }
        }
        /// <exception cref="System.Exception"/>
        public virtual void TestCannotBeConstructedWithTheUnrecognisedFieldType()
        {
            try
            {
                new XmlDataDictionary(CreateDocument(_Xml.ReplaceFirst("String", FieldType.Unrecognised.ToString())));
            }
            catch (ConfigException e)
            {
                Assert.AreEqual("cannot configure a data dictionary with an UNRECOGNISED field type: fieldDef(FID=1, name=\"PriceID\", type=UNRECOGNISED, encoding=null, scale=0)", e.Message);
                throw;
            }
        }
        /// <exception cref="System.Exception"/>
        public virtual void TestCannotBeConstructedWithUnsupportedFieldEncoding()
        {
            try
            {
                new XmlDataDictionary(CreateDocument(_Xml.ReplaceFirst("varint", FieldEncoding.Fixed1.ToString())));
            }
            catch (ConfigException e)
            {
                Assert.AreEqual("cannot configure FIXED1 encoding for a field type DOUBLE.", e.Message);
                throw;
            }
        }
        /// <exception cref="System.Exception"/>
        public virtual void TestCannotBeConstructedWithAScaleAppliedToAnUnscalableType()
        {
            try
            {
                new XmlDataDictionary(CreateDocument(_Xml.ReplaceFirst("Long\"", "Long\" scale=\"2\"")));
            }
            catch (ConfigException e)
            {
                Assert.AreEqual("cannot configure a decimal scale for field type LONG.", e.Message);
                throw;
            }
        }
        /// <exception cref="System.Exception"/>
        public virtual void TestCannotBeConstructedWithAScaleAppliedToAnUnscalableEncoding()
        {
            try
            {
                new XmlDataDictionary(CreateDocument(_Xml.ReplaceFirst("varint\"", "FIXED8\"")));
            }
            catch (ConfigException e)
            {
                Assert.AreEqual("cannot configure a decimal scale for FIXED8 encoding.", e.Message);
                throw;
            }
        }
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestParsedXmlDictionaryHas_7_FieldDefs()
        {
            Assert.AreEqual(7, _dataDictionary.AllFieldDefs().Count);
        }
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestLookupByFID()
        {
            Assert.AreEqual(Bid, _dataDictionary.FieldDefByFID(Bid.GetFID()));
            Assert.AreEqual(Ask, _dataDictionary.FieldDefByFID(Ask.GetFID()));
            Assert.AreEqual(BidSize, _dataDictionary.FieldDefByFID(BidSize.GetFID()));
            Assert.AreEqual(AskSize, _dataDictionary.FieldDefByFID(AskSize.GetFID()));
            Assert.AreEqual(Status, _dataDictionary.FieldDefByFID(Status.GetFID()));
        }
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestLookupByFIDReturnsNullForUnknownFIDS()
        {
            Assert.IsNull(_dataDictionary.FieldDefByFID(6));
            Assert.IsNull(_dataDictionary.FieldDefByFID(15));
            Assert.IsNull(_dataDictionary.FieldDefByFID(100));
        }
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestLookupByName()
        {
            Assert.AreEqual(Bid, _dataDictionary.FieldDefByName(Bid.GetName()));
            Assert.AreEqual(Ask, _dataDictionary.FieldDefByName(Ask.GetName()));
            Assert.AreEqual(BidSize, _dataDictionary.FieldDefByName(BidSize.GetName()));
            Assert.AreEqual(AskSize, _dataDictionary.FieldDefByName(AskSize.GetName()));
            Assert.AreEqual(Status, _dataDictionary.FieldDefByName(Status.GetName()));
        }
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestLookupByNameReturnsNullForUnknownFieldNames()
        {
            Assert.IsNull(_dataDictionary.FieldDefByName(string.Empty));
            Assert.IsNull(_dataDictionary.FieldDefByName("Unknown"));
            Assert.IsNull(_dataDictionary.FieldDefByName("SillyFieldName"));
        }
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestAllFieldsAreReturnInFIDSequenceOrder()
        {
            Assert.AreEqual(Arrays.AsList(SystemTime, PriceId, Bid, Ask, BidSize, AskSize, Status), _dataDictionary.AllFieldDefs());
        }
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestNewFieldsDefsCanBeAddedToTheEndOfTheDictionary()
        {
            IFieldDef endSize = BeanFieldDef.CreateFieldDef(11, FieldType.Long, "EndSize");
            Assert.IsNull(_dataDictionary.FieldDefByFID(endSize.GetFID()));
            Assert.IsNull(_dataDictionary.FieldDefByName(endSize.GetName()));
            _dataDictionary.AddFieldDef(endSize);
            Assert.AreEqual(endSize, _dataDictionary.FieldDefByFID(endSize.GetFID()));
            Assert.AreEqual(endSize, _dataDictionary.FieldDefByName(endSize.GetName()));
        }
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestNewFieldsDefsCanBeAddedToTheMiddleOfTheDictionary()
        {
            IFieldDef mid = BeanFieldDef.CreateFieldDef(6, FieldType.Double, "Mid");
            Assert.IsNull(_dataDictionary.FieldDefByFID(mid.GetFID()));
            Assert.IsNull(_dataDictionary.FieldDefByName(mid.GetName()));
            _dataDictionary.AddFieldDef(mid);
            Assert.AreEqual(mid, _dataDictionary.FieldDefByFID(mid.GetFID()));
            Assert.AreEqual(mid, _dataDictionary.FieldDefByName(mid.GetName()));
        }
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestFieldDefinitionsCanBeHaveTheirNameRedefined()
        {
            IFieldDef redefined = BeanFieldDef.CreateFieldDef(Bid.GetFID(), FieldType.Double, "BidPrice");
            _dataDictionary.AddFieldDef(redefined);
            Assert.AreEqual(redefined, _dataDictionary.FieldDefByFID(redefined.GetFID()));
            Assert.AreEqual(redefined, _dataDictionary.FieldDefByName(redefined.GetName()));
        }
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void WhenAFieldNameIsRedefinedItsOldDefinitionIsRemoved()
        {
            IFieldDef redefined = BeanFieldDef.CreateFieldDef(Bid.GetFID(), Bid.GetType(), "BidPrice");
            _dataDictionary.AddFieldDef(redefined);
            Assert.IsNull(_dataDictionary.FieldDefByName(Bid.GetName()), "old name removed");
            Assert.AreEqual(Arrays.AsList(SystemTime, PriceId, redefined, Ask, BidSize, AskSize, Status), _dataDictionary.AllFieldDefs());
        }
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestFieldDefinitionsCanBeHaveTheirFIDRedefined()
        {
            IFieldDef redefined = BeanFieldDef.CreateFieldDef(4, Bid.GetType(), Bid.GetName());
            _dataDictionary.AddFieldDef(redefined);
            Assert.AreEqual(redefined, _dataDictionary.FieldDefByFID(redefined.GetFID()));
            Assert.AreEqual(redefined, _dataDictionary.FieldDefByName(redefined.GetName()));
        }
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void WhenAFieldFIDIsRedefinedItsOldDefinitionIsRemoved()
        {
            IFieldDef redefined = BeanFieldDef.CreateFieldDef(Ask.GetFID(), Bid.GetType(), Bid.GetName());
            _dataDictionary.AddFieldDef(redefined);
            Assert.IsNull(_dataDictionary.FieldDefByFID(Bid.GetFID()), "old FID removed");
            Assert.AreEqual(Arrays.AsList(SystemTime, PriceId, redefined, BidSize, AskSize, Status), _dataDictionary.AllFieldDefs());
        }
        /// <exception cref="System.Exception"/>
        public virtual void TestCannotAddNullFieldDef()
        {
            _dataDictionary.AddFieldDef(null);
        }
        /// <exception cref="System.Exception"/>
        public virtual void TestCannotAddNullFieldDefWithUndefinedFID()
        {
            BeanFieldDef fieldDef = new BeanFieldDef();
            fieldDef.SetType(FieldType.String);
            fieldDef.SetName("Account");
            _dataDictionary.AddFieldDef(fieldDef);
        }
        /// <exception cref="System.Exception"/>
        public virtual void TestCannotAddNullFieldDefWithUndefinedType()
        {
            BeanFieldDef fieldDef = new BeanFieldDef();
            fieldDef.SetFID(13);
            fieldDef.SetName("Account");
            _dataDictionary.AddFieldDef(fieldDef);
        }
        /// <exception cref="System.Exception"/>
        public virtual void TestCannotAddNullFieldDefWithUndefinedName()
        {
            BeanFieldDef fieldDef = new BeanFieldDef();
            fieldDef.SetFID(13);
            fieldDef.SetType(FieldType.String);
            _dataDictionary.AddFieldDef(fieldDef);
        }
        private IXmlDocument CreateDocument(string xml)
        {
            Stream inputStream = new MemoryStream(Sharpen.Runtime.GetBytesForString(xml));
            DocumentBuilderFactory builderFactory = DocumentBuilderFactory.NewInstance();
            builderFactory.SetNamespaceAware(true);
            try
            {
                return builderFactory.NewDocumentBuilder().Parse(inputStream);
            }
            catch (Exception e)
            {
                throw new InternalError("could not parse Xml test document due to " + e);
            }
        }
    }
}
