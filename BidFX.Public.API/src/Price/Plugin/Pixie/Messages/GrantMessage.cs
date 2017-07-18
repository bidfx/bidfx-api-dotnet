using System.IO;
using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    /// <summary>
    /// The grant message is used once by a server to respond to a login message.
    /// </summary>
    public class GrantMessage
    {
        public bool Granted { get; set; }
        public string Reason { get; set; }

        /// <summary>
        /// Creates an instance by decoding the data from a buffer.
        /// </summary>
        /// <param name="stream"></param>
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