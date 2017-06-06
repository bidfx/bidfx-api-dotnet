using System.IO;
using BidFX.Public.NAPI.PriceManager.Tools;

namespace BidFX.Public.NAPI.PriceManager.Plugin.Pixie.Messages
{
    public class LoginMessage : IPixieMessage
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Alias { get; set; }
        public string Application { get; set; }
        public string ApplicationVersion { get; set; }

        public MemoryStream Encode(int version)
        {
            var memoryStream = new MemoryStream();
            memoryStream.WriteByte(PixieMessageType.Login);
            Varint.WriteString(memoryStream, Username);
            Varint.WriteString(memoryStream, Password);
            Varint.WriteString(memoryStream, Alias);
            if (version < 2) return memoryStream;
            Varint.WriteString(memoryStream, Application);
            Varint.WriteString(memoryStream, ApplicationVersion);
            return memoryStream;
        }
    }
}