using System;
using System.IO;
using System.Text;
using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public class WelcomeMessage
    {
        public int Options { get; set; }
        public int Version { get; set; }
        public int ClientId { get; set; }
        public int ServerId { get; set; }

        public WelcomeMessage(Stream stream)
        {
            Options = Varint.ReadU32(stream);
            Version = Varint.ReadU32(stream);
            ClientId = ReadInt4(stream);
            ServerId = ReadInt4(stream);
        }

        private static int ReadInt4(Stream stream)
        {
            var bytes = new byte[4];
            for (var i = 0; i < 4; i++)
            {
                bytes[i] = (byte) stream.ReadByte();
            }
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public override string ToString()
        {
            return "Welcome(version=" + Version +
                   ", clientID=" + HexId(ClientId) +
                   ", serverID=" + HexId(ServerId) + ')';
        }

        public static string HexId(int id)
        {
            var hex = id.ToString("X");
            return new StringBuilder()
                .Append("0x")
                .Append(hex).ToString();
        }

        protected bool Equals(WelcomeMessage other)
        {
            return Options == other.Options && Version == other.Version && ClientId == other.ClientId && ServerId == other.ServerId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WelcomeMessage) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Options;
                hashCode = (hashCode * 397) ^ Version;
                hashCode = (hashCode * 397) ^ ClientId;
                hashCode = (hashCode * 397) ^ ServerId;
                return hashCode;
            }
        }
    }
}