using System;
using BidFX.Public.API.Price.Plugin.Pixie.Messages;
using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Price.Plugin.Pixie
{
    public class AckData
    {
        public long Revision { get; set; }
        public long RevisionTime { get; set; }
        public long PriceReceivedTime { get; set; }
        public long HandlingStartNanoTime { get; set; }
        
        
        public AckMessage ToAckMessage()
        {
            var ackTime = JavaTime.CurrentTimeMillis();
            var handlingEndNanoTime = JavaTime.NanoTime();
            return ToAckMessage(ackTime, handlingEndNanoTime);
        }

        private AckMessage ToAckMessage(long ackTime, long handlingEndNanoTime)
        {
            var handlingDuration = Math.Max(0, handlingEndNanoTime - HandlingStartNanoTime);
            var handlingDurationMicros = (handlingDuration + 500L) / 1000L;
            return new AckMessage
            {
                AckTime = ackTime,
                HandlingDuration = handlingDurationMicros,
                PriceReceivedTime = PriceReceivedTime,
                Revision = Revision,
                RevisionTime = RevisionTime
            };
        }
    }
}