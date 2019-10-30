using System.IO;
using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    internal class LoginMessage : IOutgoingPixieMessage
    {
        public string Username { get; internal set; }
        public string Password { get; internal set; }
        public string Alias { get; internal set; }
        public string Application { get; internal set; }
        public string ApplicationVersion { get; internal set; }
        public string Api { get; internal set; }
        public string ApiVersion { get; internal set; }
        public string Product { get; internal set; }
        public string ProductSerial { get; internal set; }

        public LoginMessage(string username, string password, string alias, string application,
            string applicationVersion, string api, string apiVersion, string product, string productSerial)
        {
            Username = Params.NotNull(username);
            Password = Params.NotNull(password);
            Alias = Params.NotNull(alias);
            Application = Params.NotNull(application);
            ApplicationVersion = Params.NotNull(applicationVersion);
            Api = Params.NotNull(api);
            ApiVersion = Params.NotNull(apiVersion);
            Product = Params.NotNull(product);
            ProductSerial = Params.NotBlank(productSerial, "Product Serial may not be blank");
        }

        public MemoryStream Encode(int version)
        {
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.WriteByte(PixieMessageType.Login);
            Varint.WriteString(memoryStream, Username);
            Varint.WriteString(memoryStream, Password);
            Varint.WriteString(memoryStream, Alias);
            if (version >= PixieVersion.Version2)
            {
                Varint.WriteString(memoryStream, Application);
                Varint.WriteString(memoryStream, ApplicationVersion);
            }

            if (version >= PixieVersion.Version4)
            {
                Varint.WriteString(memoryStream, Api);
                Varint.WriteString(memoryStream, ApiVersion);
                Varint.WriteString(memoryStream, Product);
                Varint.WriteString(memoryStream, ProductSerial);
            }
            return memoryStream;
        }

        private bool Equals(LoginMessage other)
        {
            return string.Equals(Username, other.Username) && string.Equals(Password, other.Password) &&
                   string.Equals(Alias, other.Alias) && string.Equals(Application, other.Application) &&
                   string.Equals(ApplicationVersion, other.ApplicationVersion) &&
                   string.Equals(Api, other.Api) &&
                   string.Equals(ApiVersion, other.ApiVersion) &&
                   string.Equals(Product, other.Product) &&
                   string.Equals(ProductSerial, other.ProductSerial);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((LoginMessage) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Username != null ? Username.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (Password != null ? Password.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Alias != null ? Alias.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Application != null ? Application.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ApplicationVersion != null ? ApplicationVersion.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Api != null ? Api.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ApiVersion != null ? ApiVersion.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Product != null ? Product.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ProductSerial != null ? ProductSerial.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}