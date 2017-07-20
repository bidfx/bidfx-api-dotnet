using System;
using BidFX.Public.API.Price;

namespace BidFX.Public.API
{
    public class Client
    {
        /// <summary>
        /// The username to authenticate with.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The password associated for the given username.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The host name of the BidFX point of presence you wish to connect to.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// The port number on which to connect.
        /// The default is set to 443, which is used when tunneling to BidFX.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// A flag indicating wether the hostname should be checked on the SSL certificates.
        /// </summary>
        public bool DisableHostnameSslChecks { get; set; }

        /// <summary>
        /// How often the API should attempt to resubscribe to failed subscriptions.
        /// The default is set to five minutes.
        /// </summary>
        public TimeSpan SubscriptionRefreshInterval { get; set; }

        /// <summary>
        /// The time interval to wait between attempts to reconnect to a provider after a connection drop.
        /// </summary>
        public TimeSpan ReconnectInterval { get; set; }

        private PriceManager _priceManager;

        /// <summary>
        /// Creates a new Client that has yet to be configured.
        /// </summary>
        public Client()
        {
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
                Port = 443,
                Tunnel = true,
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