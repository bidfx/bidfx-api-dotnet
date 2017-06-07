using System;
using System.IO;
using System.Text;
using BidFX.Public.NAPI.Price.Tools;

namespace BidFX.Public.NAPI.Price.Plugin.Pixie.Messages
{
    public class WelcomeMessage : IPixieMessage
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

        public MemoryStream Encode(int version)
        {
            throw new NotImplementedException();
        }

        public static string HexId(int id)
        {
            var hex = id.ToString("X");
            return new StringBuilder()
                .Append("0x")
                .Append(hex).ToString();
        }
    }
}