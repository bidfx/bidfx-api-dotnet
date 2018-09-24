using NUnit.Framework;

namespace BidFX.Public.API.Enums
{
    public class FxTenorTest
    {
        [Test]
        public void FxTenorsBizStringTest()
        {
            Assert.AreEqual("O/N", FxTenor.GetTenor("TOD").GetBizString());
            Assert.AreEqual("T/N", FxTenor.GetTenor("TOM").GetBizString());
            Assert.AreEqual("SPOT", FxTenor.GetTenor("SPOT").GetBizString());
            Assert.AreEqual("S/N", FxTenor.GetTenor("SPOT_NEXT").GetBizString());
            Assert.AreEqual("1W", FxTenor.GetTenor("1W").GetBizString());
            Assert.AreEqual("2W", FxTenor.GetTenor("2W").GetBizString());
            Assert.AreEqual("3W", FxTenor.GetTenor("3W").GetBizString());
            Assert.AreEqual("1M", FxTenor.GetTenor("1M").GetBizString());
            Assert.AreEqual("2M", FxTenor.GetTenor("2M").GetBizString());
            Assert.AreEqual("3M", FxTenor.GetTenor("3M").GetBizString());
            Assert.AreEqual("4M", FxTenor.GetTenor("4M").GetBizString());
            Assert.AreEqual("5M", FxTenor.GetTenor("5M").GetBizString());
            Assert.AreEqual("6M", FxTenor.GetTenor("6M").GetBizString());
            Assert.AreEqual("7M", FxTenor.GetTenor("7M").GetBizString());
            Assert.AreEqual("8M", FxTenor.GetTenor("8M").GetBizString());
            Assert.AreEqual("9M", FxTenor.GetTenor("9M").GetBizString());
            Assert.AreEqual("10M", FxTenor.GetTenor("10M").GetBizString());
            Assert.AreEqual("11M", FxTenor.GetTenor("11M").GetBizString());
            Assert.AreEqual("1Y", FxTenor.GetTenor("1Y").GetBizString());
            Assert.AreEqual("2Y", FxTenor.GetTenor("2Y").GetBizString());
            Assert.AreEqual("3Y", FxTenor.GetTenor("3Y").GetBizString());
            Assert.AreEqual("IMMH", FxTenor.GetTenor("IMMH").GetBizString());
            Assert.AreEqual("IMMM", FxTenor.GetTenor("IMMM").GetBizString());
            Assert.AreEqual("IMMU", FxTenor.GetTenor("IMMU").GetBizString());
            Assert.AreEqual("IMMZ", FxTenor.GetTenor("IMMZ").GetBizString());
            Assert.AreEqual("BD", FxTenor.GetTenor("BD").GetBizString());
        }
        
        [Test]
        public void FxTenorsRESTStringTest()
        {
            Assert.AreEqual("TOD", FxTenor.GetTenor("TOD").GetRestString());
            Assert.AreEqual("TOM", FxTenor.GetTenor("TOM").GetRestString());
            Assert.AreEqual("SPOT", FxTenor.GetTenor("SPOT").GetRestString());
            Assert.AreEqual("SPOT_NEXT", FxTenor.GetTenor("SPOT_NEXT").GetRestString());
            Assert.AreEqual("1W", FxTenor.GetTenor("1W").GetRestString());
            Assert.AreEqual("2W", FxTenor.GetTenor("2W").GetRestString());
            Assert.AreEqual("3W", FxTenor.GetTenor("3W").GetRestString());
            Assert.AreEqual("1M", FxTenor.GetTenor("1M").GetRestString());
            Assert.AreEqual("2M", FxTenor.GetTenor("2M").GetRestString());
            Assert.AreEqual("3M", FxTenor.GetTenor("3M").GetRestString());
            Assert.AreEqual("4M", FxTenor.GetTenor("4M").GetRestString());
            Assert.AreEqual("5M", FxTenor.GetTenor("5M").GetRestString());
            Assert.AreEqual("6M", FxTenor.GetTenor("6M").GetRestString());
            Assert.AreEqual("7M", FxTenor.GetTenor("7M").GetRestString());
            Assert.AreEqual("8M", FxTenor.GetTenor("8M").GetRestString());
            Assert.AreEqual("9M", FxTenor.GetTenor("9M").GetRestString());
            Assert.AreEqual("10M", FxTenor.GetTenor("10M").GetRestString());
            Assert.AreEqual("11M", FxTenor.GetTenor("11M").GetRestString());
            Assert.AreEqual("1Y", FxTenor.GetTenor("1Y").GetRestString());
            Assert.AreEqual("2Y", FxTenor.GetTenor("2Y").GetRestString());
            Assert.AreEqual("3Y", FxTenor.GetTenor("3Y").GetRestString());
            Assert.AreEqual("IMMH", FxTenor.GetTenor("IMMH").GetRestString());
            Assert.AreEqual("IMMM", FxTenor.GetTenor("IMMM").GetRestString());
            Assert.AreEqual("IMMU", FxTenor.GetTenor("IMMU").GetRestString());
            Assert.AreEqual("IMMZ", FxTenor.GetTenor("IMMZ").GetRestString());
            Assert.AreEqual("BD", FxTenor.GetTenor("BD").GetRestString());
        }
    }
}