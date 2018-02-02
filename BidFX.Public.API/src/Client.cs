using System;
using System.Reflection;
using BidFX.Public.API.Price;
using BidFX.Public.API.Trade;

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
        /// The time interval to wait between attempts to recover bad subscriptions.
        /// The default is set to five minutes.
        /// </summary>
        public TimeSpan SubscriptionRefreshInterval { get; set; }

        /// <summary>
        /// The time interval to wait between attempts to reconnect to a provider after a connection drop.
        /// The default is set to 10 seconds.
        /// </summary>
        public TimeSpan ReconnectInterval { get; set; }

        private PriceManager _priceManager;
        private TradeManager _tradeManager;

        /// <summary>
        /// Creates a new Client that has yet to be configured.
        /// </summary>
        public Client()
        {
            LoadExternalAssembly("Newtonsoft.Json");
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

        /// <summary>
        /// The trading manager used to submit prices
        /// </summary>
        public TradeManager TradeManager
        {
            get
            {
                CreateTradeManager();
                return _tradeManager;
            }
        }

        private void CreatePriceManager()
        {
            if (_priceManager != null) return;
            _priceManager = new PriceManager(Username) // Remove this param when SubjectMutator is removed
            {
                Host = Host,
                Port = Port == 0 ? 443 : Port,
                Password = Password,
                SubscriptionRefreshInterval = SubscriptionRefreshInterval,
                DisableHostnameSslChecks = DisableHostnameSslChecks,
                ReconnectInterval = ReconnectInterval,
//                    Username = Username // uncomment when SubjectMutator is removed
            };
            _priceManager.Start();
        }

        private void CreateTradeManager()
        {
            if (_tradeManager != null) return;
            _tradeManager = new TradeManager
            {
                Host = Host,
                Port = Port == 0 ? 443 : Port,
                Username = Username,
                Password = Password
            };
            _tradeManager.Start();
        }

        private void LoadExternalAssembly(string assemblyName)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var resourceName = new AssemblyName(assemblyName).Name + ".dll";
                var resource = Array.Find(GetType().Assembly.GetManifestResourceNames(),
                    element => element.EndsWith(resourceName));

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
                {
                    var assemblyData = new byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };
        }
    }
}