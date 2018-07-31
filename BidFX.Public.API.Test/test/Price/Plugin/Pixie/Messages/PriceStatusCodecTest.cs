using NUnit.Framework;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public class PriceStatusCodecTest
    {
        [Test]
        public void TestDecodeStatus()
        {
            Assert.AreEqual(SubscriptionStatus.OK, PriceStatusCodec.DecodeStatus('O'));
            Assert.AreEqual(SubscriptionStatus.PENDING, PriceStatusCodec.DecodeStatus('P'));
            Assert.AreEqual(SubscriptionStatus.STALE, PriceStatusCodec.DecodeStatus('S'));
            Assert.AreEqual(SubscriptionStatus.CANCELLED, PriceStatusCodec.DecodeStatus('C'));
            Assert.AreEqual(SubscriptionStatus.DISCONTINUED, PriceStatusCodec.DecodeStatus('D'));
            Assert.AreEqual(SubscriptionStatus.PROHIBITED, PriceStatusCodec.DecodeStatus('H'));
            Assert.AreEqual(SubscriptionStatus.UNAVAILABLE, PriceStatusCodec.DecodeStatus('U'));
            Assert.AreEqual(SubscriptionStatus.REJECTED, PriceStatusCodec.DecodeStatus('R'));
            Assert.AreEqual(SubscriptionStatus.TIMEOUT, PriceStatusCodec.DecodeStatus('T'));
            Assert.AreEqual(SubscriptionStatus.INACTIVE, PriceStatusCodec.DecodeStatus('I'));
            Assert.AreEqual(SubscriptionStatus.EXHAUSTED, PriceStatusCodec.DecodeStatus('E'));
            Assert.AreEqual(SubscriptionStatus.CLOSED, PriceStatusCodec.DecodeStatus('L'));
        }
    }
}