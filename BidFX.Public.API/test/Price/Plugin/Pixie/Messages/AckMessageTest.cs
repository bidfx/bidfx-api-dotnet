using System;
using BidFX.Public.API.Price.Tools;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public class AckMessageTest
    {
        private const string EncodedMessageV1 = "4187ad4be89997de9729b09b97de9729b19b97de9729";
        private const string EncodedMessageV2 = EncodedMessageV1 + "9a05";

        private static readonly AckMessage _message = new AckMessage
        {
            Revision = 1234567L,
            RevisionTime = 1415120801000L,
            PriceReceivedTime = 1415120801200L,
            AckTime = 1415120801201L,
            HandlingDuration = 666L
        };
        
        [Test]
        public void TestCannotConstructWithNegativeHandlingDuration()
        {
            try
            {
                new AckMessage()
                {
                    HandlingDuration = -1234
                };
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                
            }
        }

        [Test]
        public void TestEncode()
        {
            Assert.AreEqual(EncodedMessageV2, VarintTest.StreamAsHex(_message.Encode(3)));
        }

        [Test]
        public void TestEncodeV1()
        {
            Assert.AreEqual(EncodedMessageV1, VarintTest.StreamAsHex(_message.Encode(1)));
        }

        [Test]
        public void TestToString()
        {
            Assert.AreEqual("Ack(revision=1234567, " +
                            "revisionTime=20141104170641000, " +
                            "priceReceivedTime=20141104170641200, " +
                            "ackTime=20141104170641201, " +
                            "handlingDuration=666us)",
                _message.ToString());
        }

        [Test]
        public void TestEqualsSelf()
        {
            Assert.IsTrue(_message.Equals(_message));
        }

        [Test]
        public void TestEqualsSimilar()
        {
            var ackMessage = new AckMessage{
                Revision = 1234567L,
                RevisionTime = 1415120801000L,
                PriceReceivedTime = 1415120801200L,
                AckTime = 1415120801201L,
                HandlingDuration = 666L
            };
            Assert.IsTrue(_message.Equals(ackMessage));
            Assert.AreEqual(_message.GetHashCode(), ackMessage.GetHashCode());
        }
    }
}