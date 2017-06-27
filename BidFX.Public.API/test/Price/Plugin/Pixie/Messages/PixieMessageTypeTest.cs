using NUnit.Framework;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public class PixieMessageTypeTest
    {
        [Test]
        public void TestConstantValues()
        {
            Assert.AreEqual('A', PixieMessageType.Ack);
            Assert.AreEqual('P', PixieMessageType.PriceSync);
            Assert.AreEqual('S', PixieMessageType.SubscriptionSync);
            Assert.AreEqual('D', PixieMessageType.DataDictionary);
            Assert.AreEqual('G', PixieMessageType.Grant);
            Assert.AreEqual('H', PixieMessageType.Heartbeat);
            Assert.AreEqual('W', PixieMessageType.Welcome);
        }
    }
}