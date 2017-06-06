using BidFX.Public.NAPI.Plugin.Puffin;
using NUnit.Framework;

namespace BidFX.Public.NAPI.test.Plugin.Puffin
{
    [TestFixture]
    public class FieldExtractorTest
    {
        private const string GrantMsg = "<Grant Text=\"\" Access=\"true\"/>";

        private const string WelcomeMsg =
            "<WelcomeMessage Name=\"PublicPuffin\" Version=\"8\" ZipRequests=\"false\" ZipPrices=\"true\" Encrypt=\"false\" SessionKey=\"162asosvck5wy\" Interval=\"60000\" Host=\"lndwc-27-230.dev.tradingscreen.com\" Port=\"9901\" Time=\"1480424571256\" ServerId=\"027f06ada41786c54e9ead0f2142fd4d\" SendWeightedMatchers=\"false\" PublicKey=\"MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBAI1cki5X8ON3arU1UToapmjjXlQOM4k7TCGrSLL4uYcdhpTFpWcsS+UWmX9SaWHytLEwEW8onnzIx+GUvG9wyXMCAwEAAQ==\"/>";

        [Test]
        public void BlankFieldExtraction()
        {
            Assert.AreEqual("", FieldExtractor.Extract(GrantMsg, "Text"));
        }

        [Test]
        public void BooleanFieldExtraction()
        {
            Assert.AreEqual("true", FieldExtractor.Extract(GrantMsg, "Access"));
        }

        [Test]
        public void StringFieldExtraction()
        {
            Assert.AreEqual("PublicPuffin", FieldExtractor.Extract(WelcomeMsg, "Name"));
        }

        [Test]
        public void DigitFieldExtraction()
        {
            Assert.AreEqual("8", FieldExtractor.Extract(WelcomeMsg, "Version"));
        }

        [Test]
        public void IntegerFieldExtraction()
        {
            Assert.AreEqual("60000", FieldExtractor.Extract(WelcomeMsg, "Interval"));
        }

        [Test]
        public void LongFieldExtraction()
        {
            Assert.AreEqual(
                "MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBAI1cki5X8ON3arU1UToapmjjXlQOM4k7TCGrSLL4uYcdhpTFpWcsS+UWmX9SaWHytLEwEW8onnzIx+GUvG9wyXMCAwEAAQ==",
                FieldExtractor.Extract(WelcomeMsg, "PublicKey"));
        }

        [Test]
        public void IsNotFooledIntoMatchingFieldNameFragments()
        {
            Assert.AreEqual(null, FieldExtractor.Extract(WelcomeMsg, "Requests"));
            Assert.AreEqual(null, FieldExtractor.Extract(WelcomeMsg, "Prices"));
            Assert.AreEqual(null, FieldExtractor.Extract(WelcomeMsg, "Key"));
        }

        [Test]
        public void NeverMatchesABlankFieldName()
        {
            Assert.AreEqual(null, FieldExtractor.Extract(WelcomeMsg, ""));
        }

        [Test]
        public void TestFieldExtraction()
        {
            Assert.AreEqual("", FieldExtractor.Extract(GrantMsg, "Text"));
            Assert.AreEqual("true", FieldExtractor.Extract(GrantMsg, "Access"));
            Assert.AreEqual("PublicPuffin", FieldExtractor.Extract(WelcomeMsg, "Name"));
            Assert.AreEqual("8", FieldExtractor.Extract(WelcomeMsg, "Version"));
            Assert.AreEqual("60000", FieldExtractor.Extract(WelcomeMsg, "Interval"));
            Assert.AreEqual(
                "MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBAI1cki5X8ON3arU1UToapmjjXlQOM4k7TCGrSLL4uYcdhpTFpWcsS+UWmX9SaWHytLEwEW8onnzIx+GUvG9wyXMCAwEAAQ==",
                FieldExtractor.Extract(WelcomeMsg, "PublicKey"));
        }

        [Test]
        public void MissingFieldsAreReturnedAsNull()
        {
            Assert.AreEqual(null, FieldExtractor.Extract(WelcomeMsg, "Missing"));
        }

        [Test]
        public void WrongCaseFieldsAreReturnedAsNull()
        {
            Assert.AreEqual(null, FieldExtractor.Extract(WelcomeMsg, "name"));
        }

        [Test]
        public void MatchingMoreThanFieldsVauesAreReturnedAsNull()
        {
            Assert.AreEqual(null, FieldExtractor.Extract(WelcomeMsg, "Version=\"8\""));
        }
    }
}