using System.IO;
using BidFX.Public.NAPI.Price.Tools;

namespace BidFX.Public.NAPI.Price.Plugin.Pixie.Messages
{
    public class LoginMessage : IOutgoingPixieMessage
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

        protected bool Equals(LoginMessage other)
        {
            return string.Equals(Username, other.Username) && string.Equals(Password, other.Password) && string.Equals(Alias, other.Alias) && string.Equals(Application, other.Application) && string.Equals(ApplicationVersion, other.ApplicationVersion);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LoginMessage) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Username != null ? Username.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Password != null ? Password.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Alias != null ? Alias.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Application != null ? Application.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ApplicationVersion != null ? ApplicationVersion.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}