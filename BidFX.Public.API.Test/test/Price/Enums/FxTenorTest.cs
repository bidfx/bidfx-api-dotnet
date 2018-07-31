using NUnit.Framework;

namespace BidFX.Public.API.Enums
{
    public class FxTenorTest
    {
        [Test]
        public void FxTenorsTest()
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
    }
}