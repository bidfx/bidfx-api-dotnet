/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

using System;
using System.Numerics;
using BidFX.Public.API.Price;
using BidFX.Public.API.Trade;
using log4net;

namespace BidFX.Public.API
{
    public class Client
    {
        private static readonly ILog Log =
            LogManager.GetLogger("Client");

        private readonly UserInfo _userInfo = new UserInfo();

        /// <summary>
        /// The username to authenticate with.
        /// </summary>
        public string Username
        {
            get { return _userInfo.Username; }
            set
            {
                _userInfo.Username = value;
            }
        }

        /// <summary>
        /// The password associated for the given username.
        /// </summary>
        public string Password
        {
            get { return _userInfo.Password; }
            set { _userInfo.Password = value;  }
        }

        /// <summary>
        /// The host name of the BidFX point of presence you wish to connect to.
        /// </summary>
        public string Host
        {
            get { return _userInfo.Host; }
            set { _userInfo.Host = value; }
        }

        /// <summary>
        /// The port number on which to connect.
        /// The default is set to 443, which is used when tunneling to BidFX.
        /// </summary>
        public int Port
        {
            get { return _userInfo.Port; }
            set { _userInfo.Port = value; }
        }

        /// <summary>
        /// The Product Serial used to authorize API connections.
        /// </summary>
        public string ProductSerial
        {
            get { return _userInfo.ProductSerial;  }
            set { _userInfo.ProductSerial = value;  }
        }

        internal string Product
        {
            get { return _userInfo.Product; }
            set { _userInfo.Product = value; }
        }

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

        public void Stop()
        {
            if (_priceManager != null)
            {
                _priceManager.Stop();
            }

            if (_tradeSession != null)
            {
                _tradeSession.Stop();
            }
        }

        private void CreatePriceManager()
        {
            if (_priceManager != null)
            {
                return;
            }

            _priceManager = new PriceManager
            {
                SubscriptionRefreshInterval = SubscriptionRefreshInterval,
                DisableHostnameSslChecks = DisableHostnameSslChecks,
                ReconnectInterval = ReconnectInterval,
                UserInfo = _userInfo
            };
            
            _priceManager.Start();
        }

        private void CreateTradeManager()
        {
            if (_tradeSession != null)
            {
                return;
            }

            _tradeSession = new TradeSession(_userInfo);
            _tradeSession.Start();
        }
    }
}