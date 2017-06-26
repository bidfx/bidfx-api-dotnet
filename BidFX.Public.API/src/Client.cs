using BidFX.Public.API.Price;

namespace BidFX.Public.API
{
    public class Client
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool Tunnel { get; set; }

        private PriceManager _priceManager;

        public PriceManager GetPriceManager()
        {
            if (_priceManager == null)
            {
                _priceManager = new PriceManager
                {
                    Host = Host,
                    Port = Port,
                    Tunnel = Tunnel,
                    Username = Username,
                    Password = Password
                };
                _priceManager.Start();
            }
            return _priceManager;
        }
    }
}