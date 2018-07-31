using NUnit.Framework;

namespace BidFX.Public.API.Price.Plugin.Puffin
{
    [TestFixture]
    public class ValueParserTest
    {
        [Test]
        public void ConvertDecimal()
        {
            Assert.AreEqual(0m, ValueParser.ParseDecimal("0", null));
            Assert.AreEqual(0.25m, ValueParser.ParseDecimal("0.25", null));
            Assert.AreEqual(123.45m, ValueParser.ParseDecimal("123.45", null));
            Assert.AreEqual(-123.45m, ValueParser.ParseDecimal("-123.45", null));
            Assert.AreEqual(1E7m, ValueParser.ParseDecimal("1E7", null));
            Assert.AreEqual(-1E7m, ValueParser.ParseDecimal("-1E7", null));
            Assert.AreEqual(-1E-7m, ValueParser.ParseDecimal("-1E-7", null));
            Assert.AreEqual(null, ValueParser.ParseDecimal("-number", null));
        }


        [Test]
        public void ConvertLong()
        {
            Assert.AreEqual(0L, ValueParser.ParseLong("0", null));
            Assert.AreEqual(12345L, ValueParser.ParseLong("12345", null));
            Assert.AreEqual(-12345L, ValueParser.ParseLong("-12345", null));
            Assert.AreEqual(1234592873972322L, ValueParser.ParseLong("1234592873972322", null));
            Assert.AreEqual(-1234592873972322L, ValueParser.ParseLong("-1234592873972322", null));
            Assert.AreEqual(null, ValueParser.ParseLong("-1.5", null));
            Assert.AreEqual(null, ValueParser.ParseLong("-number", null));
        }


        [Test]
        public void ConvertInt()
        {
            Assert.AreEqual(0, ValueParser.ParseInt("0", null));
            Assert.AreEqual(12345, ValueParser.ParseInt("12345", null));
            Assert.AreEqual(-12345, ValueParser.ParseInt("-12345", null));
            Assert.AreEqual(null, ValueParser.ParseInt("1234592873972322", null));
            Assert.AreEqual(null, ValueParser.ParseInt("-1234592873972322", null));
            Assert.AreEqual(null, ValueParser.ParseInt("-1.5", null));
            Assert.AreEqual(null, ValueParser.ParseInt("-number", null));
        }


        [Test]
        public void ConvertFraction()
        {
            Assert.AreEqual(0m, ValueParser.ParseFraction("0"));
            Assert.AreEqual(0.25m, ValueParser.ParseFraction("0.25"));
            Assert.AreEqual(0.5m, ValueParser.ParseFraction("1/2"));
            Assert.AreEqual(0.25m, ValueParser.ParseFraction("1/4"));
            Assert.AreEqual(23.25m, ValueParser.ParseFraction("23 1/4"));
            Assert.AreEqual(0.375m, ValueParser.ParseFraction("3/8"));
            Assert.AreEqual(-0.375m, ValueParser.ParseFraction("-3/8"));
            Assert.AreEqual(12345.625m, ValueParser.ParseFraction("12345 15/24"));
            Assert.AreEqual(12345.625m, ValueParser.ParseFraction("+12345 15/24"));
            Assert.AreEqual(-12345.625m, ValueParser.ParseFraction("-12345 15/24"));
            Assert.AreEqual(-123.45m, ValueParser.ParseFraction("-123.45"));
            Assert.AreEqual(0m, ValueParser.ParseFraction("oops"));
            Assert.AreEqual(0m, ValueParser.ParseFraction("-number"));
        }
    }
}