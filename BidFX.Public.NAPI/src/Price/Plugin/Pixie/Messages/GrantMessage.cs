using System;
using System.IO;
using BidFX.Public.NAPI.Price.Tools;

namespace BidFX.Public.NAPI.Price.Plugin.Pixie.Messages
{
    public class GrantMessage
    {
        public bool Granted { get; set; }
        public string Reason { get; set; }

        public GrantMessage(Stream stream)
        {
            Granted = 't' == stream.ReadByte();
            Reason = Varint.ReadString(stream);
        }

        public override string ToString()
        {
            return "Grant(granted=" + Granted + ", reason=\"" + Reason + "\")";
        }

        protected bool Equals(GrantMessage other)
        {
            return Granted == other.Granted && string.Equals(Reason, other.Reason);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GrantMessage) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Granted.GetHashCode() * 397) ^ (Reason != null ? Reason.GetHashCode() : 0);
            }
        }
    }
}