using System;
using System.IO;
using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public class AckMessage : IOutgoingPixieMessage
    {
        public ulong Revision { get; set; }
        public ulong RevisionTime { get; set; }
        public long PriceReceivedTime { get; set; }
        public long AckTime { get; set; }
        private long _handlingDuration;

        public long HandlingDuration
        {
            get { return _handlingDuration; }
            set { _handlingDuration = Params.NotNegative(value); }
        }

        public MemoryStream Encode(int version)
        {
            var memoryStream = new MemoryStream();
            memoryStream.WriteByte(PixieMessageType.Ack);
            Varint.WriteU64(memoryStream, Revision);
            Varint.WriteU64(memoryStream, RevisionTime);
            Varint.WriteU64(memoryStream, PriceReceivedTime);
            Varint.WriteU64(memoryStream, AckTime);
            if (version >= 2)
            {
                Varint.WriteU64(memoryStream, HandlingDuration);
            }
            return memoryStream;
        }

        public override string ToString()
        {
            return "Ack(revision=" + Revision
                   + ", revisionTime=" + JavaTime.ToIsoTime((long) RevisionTime)
                   + ", priceReceivedTime=" + JavaTime.ToIsoTime(PriceReceivedTime)
                   + ", ackTime=" + JavaTime.ToIsoTime(AckTime)
                   + ", handlingDuration=" + HandlingDuration
                   + "us)";
        }

        protected bool Equals(AckMessage other)
        {
            return Revision == other.Revision && RevisionTime == other.RevisionTime &&
                   PriceReceivedTime == other.PriceReceivedTime && AckTime == other.AckTime &&
                   HandlingDuration == other.HandlingDuration;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AckMessage) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Revision.GetHashCode();
                hashCode = (hashCode * 397) ^ RevisionTime.GetHashCode();
                hashCode = (hashCode * 397) ^ PriceReceivedTime.GetHashCode();
                hashCode = (hashCode * 397) ^ AckTime.GetHashCode();
                hashCode = (hashCode * 397) ^ HandlingDuration.GetHashCode();
                return hashCode;
            }
        }
    }
}