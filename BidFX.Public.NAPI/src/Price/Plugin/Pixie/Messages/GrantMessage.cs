using System;
using System.IO;
using BidFX.Public.NAPI.Price.Tools;

namespace BidFX.Public.NAPI.Price.Plugin.Pixie.Messages
{
    public class GrantMessage : IPixieMessage
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

        public MemoryStream Encode(int version)
        {
            throw new NotImplementedException();
        }
    }
}