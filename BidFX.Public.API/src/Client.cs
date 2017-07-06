using BidFX.Public.API.Price;
using BidFX.Public.API.Price.Tools;

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

        /// <summary>
        /// Creates and initiates a pricing session, in which you can subscribe to and recieve updates from subscriptions
        /// </summary>
        /// <returns>The pricing manager</returns>
        public PriceManager GetPriceManager()
        {
            if (_priceManager == null)
            {
                _priceManager = new PriceManager(Username)
                {
                    Host = Host,
                    Port = Port,
                    Tunnel = Tunnel,
                    Password = Password
                };
                _priceManager.Start();
            }
            return _priceManager;
        }
    }
}