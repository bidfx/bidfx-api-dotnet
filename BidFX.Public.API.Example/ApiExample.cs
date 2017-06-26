using System;
using System.Reflection;
using System.Threading;
using BidFX.Public.API.Price;
using BidFX.Public.API.Price.Subject;
using log4net;

namespace BidFX.Public.API.Example
{
    internal class ApiExample
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly PriceManager _priceManager;

        public static void Main(string[] args)
        {
            Log.Info("testing with " + PublicApi.Name + " version " + PublicApi.Version);
            new ApiExample().RunTest();
        }

        private ApiExample()
        {
            var client = new Client
            {
                Host = "localhost",
                Port = 9902,
                Tunnel = false,
                Username = "pisagui",
                Password = "pisagui123"
            };
            _priceManager = client.GetPriceManager();
            _priceManager.PriceUpdateEventHandler += OnPriceUpdate;
            _priceManager.SubscriptionStatusEventHandler += OnSubscriptionStatus;
            _priceManager.ProviderStatusEventHandler += OnProviderStatus;
        }

        private void RunTest()
        {
            if (_priceManager.WaitUntilReady(TimeSpan.FromSeconds(15)))
            {
                Log.Info("pricing session is ready");
                SendSubscriptions();
            }
            else
            {
                Log.Warn("timed out waiting on session to be ready");
                foreach (var providerProperties in _priceManager.ProviderProperties())
                {
                    Log.Info(providerProperties.ToString());
                }
                _priceManager.Stop();
            }
        }

        private void SendSubscriptions()
        {
            Thread.Sleep(5000);
            _priceManager.Subscribe(new Subject(
                "AssetClass=Fx,Currency=EUR,Customer=0001,Dealer=101100,Exchange=OTC,Level=1,Quantity=1000000.00,QuoteStyle=RFS,Source=UBSFX,SubClass=Spot,Symbol=EURUSD,Tenor=Spot,User=pisagui,ValueDate=20170623"));
//            _priceManager.Subscribe(new Subject("AssetClass=Fx,Currency=EUR,Customer=0001,Dealer=101100,Exchange=OTC,Level=1,Quantity=1000000.00,QuoteStyle=RFS,Source=DBFX,SubClass=Spot,Symbol=EURGBP,Tenor=Spot,User=pmacdona,ValueDate=20170623"));
//            _priceManager.Subscribe(new Subject("AssetClass=Fx,Currency=EUR,Customer=0001,Dealer=101100,Exchange=OTC,Level=1,Quantity=1000000.00,QuoteStyle=RFS,Source=DBFX,SubClass=Spot,Symbol=USDJPY,Tenor=Spot,User=pmacdona,ValueDate=20170623"));
//            _priceManager.Subscribe(new Subject("AssetClass=Fx,Currency=EUR,Customer=0001,Dealer=101100,Exchange=OTC,Level=1,Quantity=1000000.00,QuoteStyle=RFS,Source=DBFX,SubClass=Spot,Symbol=GBPUSD,Tenor=Spot,User=pmacdona,ValueDate=20170623"));
//            _priceManager.Subscribe(new Subject("AssetClass=Fx,Currency=EUR,Customer=0001,Dealer=101100,Exchange=OTC,Level=1,Quantity=1000000.00,QuoteStyle=RFS,Source=SSFX,SubClass=Spot,Symbol=EURUSD,Tenor=Spot,User=pmacdona,ValueDate=20170623"));
//            _priceManager.Subscribe(new Subject("AssetClass=Fx,Currency=EUR,Customer=0001,Dealer=101100,Exchange=OTC,Level=1,Quantity=1000000.00,QuoteStyle=RFS,Source=SSFX,SubClass=Spot,Symbol=EURGBP,Tenor=Spot,User=pmacdona,ValueDate=20170623"));
//            _priceManager.Subscribe(new Subject("AssetClass=Fx,Currency=EUR,Customer=0001,Dealer=101100,Exchange=OTC,Level=1,Quantity=1000000.00,QuoteStyle=RFS,Source=SSFX,SubClass=Spot,Symbol=USDJPY,Tenor=Spot,User=pmacdona,ValueDate=20170623"));
//            _priceManager.Subscribe(new Subject("AssetClass=Fx,Currency=EUR,Customer=0001,Dealer=101100,Exchange=OTC,Level=1,Quantity=1000000.00,QuoteStyle=RFS,Source=SSFX,SubClass=Spot,Symbol=GBPUSD,Tenor=Spot,User=pmacdona,ValueDate=20170623"));
//            _priceManager.Subscribe(new Subject("AssetClass=Fx,Currency=EUR,Customer=0001,Dealer=101100,Exchange=OTC,Level=1,Quantity=1000000.00,QuoteStyle=RFS,Source=CSFX,SubClass=Spot,Symbol=EURUSD,Tenor=Spot,User=pmacdona,ValueDate=20170623"));
//            _priceManager.Subscribe(new Subject("AssetClass=Fx,Currency=EUR,Customer=0001,Dealer=101100,Exchange=OTC,Level=1,Quantity=1000000.00,QuoteStyle=RFS,Source=CSFX,SubClass=Spot,Symbol=EURGBP,Tenor=Spot,User=pmacdona,ValueDate=20170623"));
//            _priceManager.Subscribe(new Subject("AssetClass=Fx,Currency=EUR,Customer=0001,Dealer=101100,Exchange=OTC,Level=1,Quantity=1000000.00,QuoteStyle=RFS,Source=CSFX,SubClass=Spot,Symbol=USDJPY,Tenor=Spot,User=pmacdona,ValueDate=20170623"));
//            _priceManager.Subscribe(new Subject("AssetClass=Fx,Currency=EUR,Customer=0001,Dealer=101100,Exchange=OTC,Level=1,Quantity=1000000.00,QuoteStyle=RFS,Source=CSFX,SubClass=Spot,Symbol=GBPUSD,Tenor=Spot,User=pmacdona,ValueDate=20170623"));
        }

        private static void OnPriceUpdate(object source, PriceUpdateEvent priceUpdateEvent)
        {
            Log.Info("price update: " + priceUpdateEvent.Subject + " -> " + priceUpdateEvent.ChangedPriceFields);
        }

        private static void OnSubscriptionStatus(object source, SubscriptionStatusEvent subscriptionStatusEvent)
        {
            Log.Info("price status: " + subscriptionStatusEvent);
        }

        private static void OnProviderStatus(object sender, ProviderStatusEvent providerStatusEvent)
        {
            Log.Info("provider status: " +
                     providerStatusEvent.Name + " changed status from " +
                     providerStatusEvent.PreviousProviderStatus
                     + " to " + providerStatusEvent.ProviderStatus
                     + " because: " + providerStatusEvent.StatusReason);
        }
    }
}