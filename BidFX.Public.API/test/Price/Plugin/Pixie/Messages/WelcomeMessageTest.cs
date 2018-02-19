using System.IO;
using BidFX.Public.API.Price.Tools;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public class WelcomeMessageTest
    {
        private const string EncodedMessage = "570001000010e1000026ae";

        [Test]
        public void decodes_a_buffer_into_a_message()
        {
            MemoryStream memoryStream = VarintTest.HexStream(EncodedMessage);
            Assert.AreEqual(PixieMessageType.Welcome, memoryStream.ReadByte());
            WelcomeMessage message = new WelcomeMessage(memoryStream);
            Assert.AreEqual(0, message.Options);
            Assert.AreEqual(1, message.Version);
            Assert.AreEqual(4321, message.ClientId);
            Assert.AreEqual(9902, message.ServerId);
        }
    }
}