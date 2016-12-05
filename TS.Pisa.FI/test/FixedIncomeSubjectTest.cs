using NUnit.Framework;

namespace TS.Pisa.FI.test
{
    [TestFixture]
    public class FixedIncomeSubjectTest
    {
        [Test]
        public void TestToString()
        {
            Assert.AreEqual("Venue=SGC,ISIN=US0378331005", new FixedIncomeSubject("SGC", "US0378331005").ToString());
            Assert.AreEqual("Venue=SGC,ISIN=AT0000A0GLY4", new FixedIncomeSubject("SGC", "AT0000A0GLY4").ToString());
            Assert.AreEqual("Venue=BNP,ISIN=AT0000A0GLY4", new FixedIncomeSubject("BNP", "AT0000A0GLY4").ToString());
        }

        [Test]
        public void TestToPisaSubject()
        {
            Assert.AreEqual("AssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=US0378331005",
                new FixedIncomeSubject("SGC", "US0378331005").PisaSubject());
            Assert.AreEqual("AssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=AT0000A0GLY4",
                new FixedIncomeSubject("SGC", "AT0000A0GLY4").PisaSubject());
            Assert.AreEqual("AssetClass=FixedIncome,Exchange=BNP,Level=1,Source=Lynx,Symbol=AT0000A0GLY4",
                new FixedIncomeSubject("BNP", "AT0000A0GLY4").PisaSubject());
        }

        [Test]
        public void TestVenueMayNotBeNull()
        {
            Assert.AreEqual("null is not a valid venue code", IllegalSubject(null, "US0378331005"));
        }

        [Test]
        public void TestVenueMayNotBeBlank()
        {
            Assert.AreEqual("given code \"\" is not a valid venue code", IllegalSubject("", "US0378331005"));
        }

        [Test]
        public void TestVenueMayNotHaveSpaces()
        {
            Assert.AreEqual("given code \"SGX \" is not a valid venue code", IllegalSubject("SGX ", "US0378331005"));
        }

        [Test]
        public void TestIsinMayNotBeNull()
        {
            Assert.AreEqual("null is not a valid ISIN code",
                IllegalSubject("SGC", null));
        }

        [Test]
        public void TestISINMayNotHaveSpaces()
        {
            Assert.AreEqual("given code \"US0378331005 \" is not a valid ISIN code",
                IllegalSubject("SGX", "US0378331005 "));
        }

        [Test]
        public void TestISINShouldBe12CharsLong()
        {
            Assert.AreEqual("given code \"US0378331\" is not a valid ISIN code",
                IllegalSubject("SGX", "US0378331"));
            Assert.AreEqual("given code \"US037833100523\" is not a valid ISIN code",
                IllegalSubject("SGX", "US037833100523"));
        }

        private static string IllegalSubject(string venue, string isin)
        {
            try
            {
                new FixedIncomeSubject(venue, isin);
                return "IllegalSubjectException exception expected";
            }
            catch (IllegalSubjectException e)
            {
                return e.Message;
            }
        }

    }
}