using BidFX.Public.API.Price.Plugin.Pixie.Messages;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Plugin.Pixie
{
    public class AckDataTest
    {
        private const long Revision = 1234567L;
        private const long RevisionTime = 1422808975000L;
        private const long PriceReceivedTime = Revision + 61;
        private const long AckTime = PriceReceivedTime + 1;
        private const long StartNanoTime = 12300000000L;


        [Test]
        public void ToAckMessageComputesTheHandlingDurationInMicroseconds()
        {
            long endNanoTime = StartNanoTime + 321000;
            AckData ackData = new AckData
            {
                Revision = Revision,
                RevisionTime = RevisionTime,
                PriceReceivedTime = PriceReceivedTime,
                HandlingStartNanoTime = StartNanoTime
            };
            Assert.AreEqual(
                new AckMessage
                {
                    Revision = Revision,
                    RevisionTime = RevisionTime,
                    PriceReceivedTime = PriceReceivedTime,
                    AckTime = AckTime,
                    HandlingDuration = 321
                },
                ackData.ToAckMessage(AckTime, endNanoTime));
        }

        [Test]
        public void ToAckMessageComputesTheHandlingDurationInMicrosecondsAlwaysAsAPositiveValue()
        {
            long endNanoTime = StartNanoTime - 99000;
            AckData ackData = new AckData
            {
                Revision = Revision,
                RevisionTime = RevisionTime,
                PriceReceivedTime = PriceReceivedTime,
                HandlingStartNanoTime = StartNanoTime
            };
            Assert.AreEqual(
                new AckMessage
                {
                    Revision = Revision,
                    RevisionTime = RevisionTime,
                    PriceReceivedTime = PriceReceivedTime,
                    AckTime = AckTime,
                    HandlingDuration = 0
                },
                ackData.ToAckMessage(AckTime, endNanoTime));
        }

        [Test]
        public void ToAckMessageComputesTheHandlingDurationInMicrosecondsByRoundingToNearest()
        {
            long endNanoTime = StartNanoTime + 321000;
            AckData ackData = new AckData
            {
                Revision = Revision,
                RevisionTime = RevisionTime,
                PriceReceivedTime = PriceReceivedTime,
                HandlingStartNanoTime = StartNanoTime
            };
            Assert.AreEqual(
                new AckMessage
                {
                    Revision = Revision,
                    RevisionTime = RevisionTime,
                    PriceReceivedTime = PriceReceivedTime,
                    AckTime = AckTime,
                    HandlingDuration = 321
                },
                ackData.ToAckMessage(AckTime, endNanoTime + 499));
            Assert.AreEqual(
                new AckMessage
                {
                    Revision = Revision,
                    RevisionTime = RevisionTime,
                    PriceReceivedTime = PriceReceivedTime,
                    AckTime = AckTime,
                    HandlingDuration = 322
                },
                ackData.ToAckMessage(AckTime, endNanoTime + 999));
            Assert.AreEqual(
                new AckMessage
                {
                    Revision = Revision,
                    RevisionTime = RevisionTime,
                    PriceReceivedTime = PriceReceivedTime,
                    AckTime = AckTime,
                    HandlingDuration = 322
                },
                ackData.ToAckMessage(AckTime, endNanoTime + 500));
        }
    }
}