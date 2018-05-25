using BidFX.Public.API.Price.Plugin.Puffin;
using NUnit.Framework;

namespace BidFX.Public.API.Price
{
    public class PriceMapTest
    {
        [Test]
        public void MergingPriceMapWithFewerBidLevelsClearsExcessLevels()
        {
            PriceMap uut = new PriceMap();
            uut.SetField("BidLevels", new PriceField("5", 5));
            uut.SetField("AskLevels", new PriceField("5", 5));
            uut.SetField("BidFirm1", new PriceField("Firm1", "Firm1"));
            uut.SetField("BidFirm2", new PriceField("Firm2", "Firm2"));
            uut.SetField("BidFirm3", new PriceField("Firm3", "Firm3"));
            uut.SetField("BidFirm4", new PriceField("Firm4", "Firm4"));
            uut.SetField("BidFirm5", new PriceField("Firm5", "Firm5"));
            uut.SetField("AskFirm1", new PriceField("Firm1", "Firm1"));
            uut.SetField("AskFirm2", new PriceField("Firm2", "Firm2"));
            uut.SetField("AskFirm3", new PriceField("Firm3", "Firm3"));
            uut.SetField("AskFirm4", new PriceField("Firm4", "Firm4"));
            uut.SetField("AskFirm5", new PriceField("Firm5", "Firm5"));
            uut.SetField("BidSize1", new PriceField("1000000", 1000000));
            uut.SetField("BidSize2", new PriceField("1000000", 1000000));
            uut.SetField("BidSize3", new PriceField("1000000", 1000000));
            uut.SetField("BidSize4", new PriceField("1000000", 1000000));
            uut.SetField("BidSize5", new PriceField("1000000", 1000000));
            uut.SetField("AskSize1", new PriceField("1000000", 1000000));
            uut.SetField("AskSize2", new PriceField("1000000", 1000000));
            uut.SetField("AskSize3", new PriceField("1000000", 1000000));
            uut.SetField("AskSize4", new PriceField("1000000", 1000000));
            uut.SetField("AskSize5", new PriceField("1000000", 1000000));
            uut.SetField("Bid1", new PriceField("1.2304", 1.2304));
            uut.SetField("Bid2", new PriceField("1.2303", 1.2303));
            uut.SetField("Bid3", new PriceField("1.2302", 1.2302));
            uut.SetField("Bid4", new PriceField("1.2301", 1.2301));
            uut.SetField("Bid5", new PriceField("1.2300", 1.2300));
            uut.SetField("Ask1", new PriceField("1.2305", 1.2305));
            uut.SetField("Ask2", new PriceField("1.2306", 1.2306));
            uut.SetField("Ask3", new PriceField("1.2307", 1.2307));
            uut.SetField("Ask4", new PriceField("1.2308", 1.2308));
            uut.SetField("Ask5", new PriceField("1.2309", 1.2309));
            uut.SetField("BidTime1", new PriceField("2500", 2500));
            uut.SetField("BidTime2", new PriceField("2501", 2501));
            uut.SetField("BidTime3", new PriceField("2502", 2502));
            uut.SetField("BidTime4", new PriceField("2503", 2503));
            uut.SetField("BidTime5", new PriceField("2504", 2504));
            uut.SetField("AskTime1", new PriceField("1500", 1500));
            uut.SetField("AskTime2", new PriceField("1501", 1501));
            uut.SetField("AskTime3", new PriceField("1502", 1502));
            uut.SetField("AskTime4", new PriceField("1503", 1503));
            uut.SetField("AskTime5", new PriceField("1504", 1504));

            PriceMap newPriceMap = new PriceMap();
            newPriceMap.SetField("BidLevels", new PriceField("2", 2));
            newPriceMap.SetField("AskLevels", new PriceField("3", 3));
            newPriceMap.SetField("BidFirm1", new PriceField("Firm1", "Firm1"));
            newPriceMap.SetField("BidFirm2", new PriceField("Firm2", "Firm2"));
            newPriceMap.SetField("AskFirm1", new PriceField("Firm1", "Firm1"));
            newPriceMap.SetField("AskFirm2", new PriceField("Firm2", "Firm2"));
            newPriceMap.SetField("AskFirm3", new PriceField("Firm3", "Firm3"));
            newPriceMap.SetField("BidSize1", new PriceField("1000000", 1000000));
            newPriceMap.SetField("BidSize2", new PriceField("1000000", 1000000));
            newPriceMap.SetField("AskSize1", new PriceField("1000000", 1000000));
            newPriceMap.SetField("AskSize2", new PriceField("1000000", 1000000));
            newPriceMap.SetField("AskSize3", new PriceField("1000000", 1000000));
            newPriceMap.SetField("Bid1", new PriceField("1.2304", 1.2304));
            newPriceMap.SetField("Bid2", new PriceField("1.2303", 1.2303));
            newPriceMap.SetField("Ask1", new PriceField("1.2305", 1.2305));
            newPriceMap.SetField("Ask2", new PriceField("1.2306", 1.2306));
            newPriceMap.SetField("Ask3", new PriceField("1.2307", 1.2307));
            uut.SetField("BidTime1", new PriceField("5500", 5500));
            uut.SetField("BidTime2", new PriceField("5501", 5501));
            uut.SetField("AskTime1", new PriceField("6500", 6500));
            uut.SetField("AskTime2", new PriceField("6501", 6501));
            uut.SetField("AskTime3", new PriceField("6502", 6502));

            uut.MergedPriceMap(newPriceMap, false);
            
            Assert.AreEqual(3, uut.IntField("AskLevels"));
            Assert.AreEqual(2, uut.IntField("BidLevels"));
            Assert.AreEqual("Firm1", uut.StringField("BidFirm1"));
            Assert.AreEqual("Firm2", uut.StringField("BidFirm2"));
            Assert.IsNull(uut.StringField("BidFirm3"));
            Assert.IsNull(uut.StringField("BidFirm4"));
            Assert.IsNull(uut.StringField("BidFirm5"));
            Assert.AreEqual("Firm1", uut.StringField("AskFirm1"));
            Assert.AreEqual("Firm2", uut.StringField("AskFirm2"));
            Assert.AreEqual("Firm3", uut.StringField("AskFirm3"));
            Assert.IsNull(uut.StringField("AskFirm4"));
            Assert.IsNull(uut.StringField("AskFirm5"));
            Assert.AreEqual(1000000, uut.IntField("BidSize1"));
            Assert.AreEqual(1000000, uut.IntField("BidSize2"));
            Assert.IsNull(uut.IntField("BidSize3"));
            Assert.IsNull(uut.IntField("BidSize4"));
            Assert.IsNull(uut.IntField("BidSize5"));
            Assert.AreEqual(1000000, uut.IntField("AskSize1"));
            Assert.AreEqual(1000000, uut.IntField("AskSize2"));
            Assert.AreEqual(1000000, uut.IntField("AskSize3"));
            Assert.IsNull(uut.IntField("AskSize4"));
            Assert.IsNull(uut.IntField("AskSize5"));
            Assert.AreEqual(1.2304, uut.DecimalField("Bid1"));
            Assert.AreEqual(1.2303, uut.DecimalField("Bid2"));
            Assert.IsNull(uut.DecimalField("Bid3"));
            Assert.IsNull(uut.DecimalField("Bid4"));
            Assert.IsNull(uut.DecimalField("Bid5"));
            Assert.AreEqual(1.2305, uut.DecimalField("Ask1"));
            Assert.AreEqual(1.2306, uut.DecimalField("Ask2"));
            Assert.AreEqual(1.2307, uut.DecimalField("Ask3"));
            Assert.IsNull(uut.DecimalField("Ask4"));
            Assert.IsNull(uut.DecimalField("Ask5"));
            Assert.AreEqual(5500, uut.IntField("BidTime1"));
            Assert.AreEqual(5501, uut.IntField("BidTime2"));
            Assert.IsNull(uut.IntField("BidTime3"));
            Assert.IsNull(uut.IntField("BidTime4"));
            Assert.IsNull(uut.IntField("BidTime5"));
            Assert.AreEqual(6500, uut.IntField("AskTime1"));
            Assert.AreEqual(6501, uut.IntField("AskTime2"));
            Assert.AreEqual(6502, uut.IntField("AskTime3"));
            Assert.IsNull(uut.IntField("AskTime4"));
            Assert.IsNull(uut.IntField("AskTime5"));
        }
    }
}