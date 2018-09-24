using System.IO;
using BidFX.Public.API.Price.Tools;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public class GrantMessageTest
    {
        private const string EncodedGrant = "477401";
        private const string EncodedDeny = "476614696e76616c69642063726564656e7469616c73";

        [Test]
        public void TestDecodeGrant()
        {
            MemoryStream memoryStream = VarintTest.HexStream(EncodedGrant);
            memoryStream.ReadByte();
            GrantMessage grantMessage = new GrantMessage(memoryStream);
            Assert.AreEqual(true, grantMessage.Granted);
        }

        [Test]
        public void TestDecodeDeny()
        {
            MemoryStream memoryStream = VarintTest.HexStream(EncodedDeny);
            memoryStream.ReadByte();
            GrantMessage grantMessage = new GrantMessage(memoryStream);
            Assert.AreEqual(false, grantMessage.Granted);
            Assert.AreEqual("invalid credentials", grantMessage.Reason);
        }

        [Test]
        public void TestToString()
        {
            MemoryStream memoryStream = VarintTest.HexStream(EncodedGrant);
            memoryStream.ReadByte();
            Assert.AreEqual(@"Grant(granted=True, reason="""")", new GrantMessage(memoryStream).ToString());
        }
    }
}