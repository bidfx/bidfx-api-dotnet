using BidFX.Public.API.Price.Tools;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public class HeartbeatMessageTest
    {
        private const string EncodedMessage = "48";
        
        [Test]
        public void TestEncode()
        {
            Assert.AreEqual(EncodedMessage, VarintTest.StreamAsHex(new HeartbeatMessage().Encode(3)));
        }

        [Test]
        public void TestToString()
        {
            Assert.AreEqual("Heartbeat()", new HeartbeatMessage().ToString());
        }
    }
}