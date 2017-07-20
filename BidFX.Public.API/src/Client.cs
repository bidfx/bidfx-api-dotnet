using System;
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
        public bool DisableHostnameSslChecks { get; set; }
        public TimeSpan SubscriptionRefreshInterval { get; set; }
        public TimeSpan ReconnectInterval { get; set; }

        private PriceManager _priceManager;

        /// <summary>
        /// Creates a new Client that has yet to be configured.
        /// </summary>
        public Client()
        {
            Port = 443;
            Tunnel = true;
            DisableHostnameSslChecks = false;
            ReconnectInterval = TimeSpan.FromSeconds(10);
            SubscriptionRefreshInterval = TimeSpan.FromMinutes(5);
        }

        /// <summary>
        /// The pricing session interface used for configuring the pricing API.
        /// </summary>
        /// <returns>The pricing manager</returns>
        public ISession PriceSession
        {
            get
            {
                CreatePriceManager();
                return _priceManager;
            }
        }

        /// <summary>
        /// The pricing subscriber interface used for subscribing to prices.
        /// </summary>
        public IBulkSubscriber PriceSubscriber
        {
            get
            {
                CreatePriceManager();
                return _priceManager;
            }
        }

        private void CreatePriceManager()
        {
            if (_priceManager != null) return;
            _priceManager = new PriceManager(Username) // Remove this param when SubjectMutator is removed
            {
                Host = Host,
                Port = Port,
                Tunnel = Tunnel,
                Password = Password,
                SubscriptionRefreshInterval = SubscriptionRefreshInterval,
                DisableHostnameSslChecks = DisableHostnameSslChecks,
                ReconnectInterval = ReconnectInterval,
//                    Username = Username // uncomment when SubjectMutator is removed
            };
            _priceManager.Start();
        }
    }
}