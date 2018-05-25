/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

using System;
using System.IO;
using System.Numerics;
using System.Reflection;
using BidFX.Public.API.Price;
using BidFX.Public.API.Trade;
using log4net;

namespace BidFX.Public.API
{
    public class Client
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static BigInteger SubscriptionLimitPublicKey = BigInteger.Parse("125134336105432108045835366424157859929");

        private int _levelTwoSubscriptionLimit = 20;
        private int _levelOneSubscriptionLimit = 500;

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
        private TradeSession _tradeSession;

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
        public TradeSession TradeSession
        {
            get
            {
                CreateTradeManager();
                return _tradeSession;
            }
        }

        public bool SetDepthSubscriptionLimit(string subscriptionLimitString)
        {
            try
            {
                Tuple<int, BigInteger> subscriptionLimit = splitSubscriptionLimitString(subscriptionLimitString);
                if (!IsValidLimitSignature(subscriptionLimit.Item1, subscriptionLimit.Item2))
                {
                    throw new ArgumentException("signature of subscription limit string does not match expected value");
                }

                _levelOneSubscriptionLimit = subscriptionLimit.Item1;
                if (_priceManager != null)
                {
                    _priceManager.LevelTwoSubscriptionLimit = _levelTwoSubscriptionLimit;
                }
                return true;
            }
            catch (Exception e)
            {
                Log.Error("Could not update depth subscription limit", e);
                return false;
            }
        }

        public bool SetLevelOneSubscriptionLimit(string subscriptionLimitString)
        {
            try
            {
                Tuple<int, BigInteger> subscriptionLimit = splitSubscriptionLimitString(subscriptionLimitString);
                if (!IsValidLimitSignature(subscriptionLimit.Item1, subscriptionLimit.Item2))
                {
                    throw new ArgumentException("signature of subscription limit string does not match expected value");
                }

                _levelOneSubscriptionLimit = subscriptionLimit.Item1;
                if (_priceManager != null)
                {
                    _priceManager.LevelOneSubscriptionLimit = _levelOneSubscriptionLimit;
                }
                return true;
            }
            catch (Exception e)
            {
                Log.Error("Could not update depth subscription limit", e);
                return false;
            }
        }

        private Tuple<int, BigInteger> splitSubscriptionLimitString(string subscriptionLimitString)
        {
            String[] subscriptionLimit = subscriptionLimitString.Split('-');
            return Tuple.Create(int.Parse(subscriptionLimit[0]), BigInteger.Parse(subscriptionLimit[1]));
        }
        
        private bool IsValidLimitSignature(int limit, BigInteger signature)
        {
            return BigInteger.ModPow(signature, 65537, SubscriptionLimitPublicKey).Equals(limit);
        }

        private void CreatePriceManager()
        {
            if (_priceManager != null)
            {
                return;
            }

            _priceManager = new PriceManager()
            {
                Host = Host,
                Port = Port == 0 ? 443 : Port,
                Password = Password,
                SubscriptionRefreshInterval = SubscriptionRefreshInterval,
                DisableHostnameSslChecks = DisableHostnameSslChecks,
                ReconnectInterval = ReconnectInterval,
                Username = Username,
                LevelTwoSubscriptionLimit = _levelTwoSubscriptionLimit,
                LevelOneSubscriptionLimit = _levelOneSubscriptionLimit
              
            };
            _priceManager.Start();
        }

        private void CreateTradeManager()
        {
            if (_tradeSession != null)
            {
                return;
            }

            _tradeSession = new TradeSession
            {
                Host = Host,
                Port = Port == 0 ? 443 : Port,
                Username = Username,
                Password = Password
            };
            _tradeSession.Start();
        }

        private void LoadExternalAssembly(string assemblyName)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string resourceName = new AssemblyName(assemblyName).Name + ".dll";
                string resource = Array.Find(GetType().Assembly.GetManifestResourceNames(),
                    element => element.EndsWith(resourceName));

                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
                {
                    byte[] assemblyData = new byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };
        }
    }
}