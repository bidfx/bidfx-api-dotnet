/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

using System;
using BidFX.Public.API.Price.Plugin.Pixie.Messages;
using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Price.Plugin.Pixie
{
    internal class AckData
    {
        public ulong Revision { get; set; }
        public ulong RevisionTime { get; set; }
        public long PriceReceivedTime { get; set; }
        public long HandlingStartNanoTime { get; set; }


        public AckMessage ToAckMessage()
        {
            long ackTime = JavaTime.CurrentTimeMillis();
            long handlingEndNanoTime = JavaTime.NanoTime();
            return ToAckMessage(ackTime, handlingEndNanoTime);
        }

        internal AckMessage ToAckMessage(long ackTime, long handlingEndNanoTime)
        {
            long handlingDuration = Math.Max(0, handlingEndNanoTime - HandlingStartNanoTime);
            long handlingDurationMicros = (handlingDuration + 500L) / 1000L;
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