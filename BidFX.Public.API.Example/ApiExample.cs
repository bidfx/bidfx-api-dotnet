using System;
using System.Reflection;
using System.Threading;
using BidFX.Public.API.Price;
using BidFX.Public.API.Price.Subject;

namespace BidFX.Public.API.Example
{
    internal class ApiExample
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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
                Username = "pmacdona",
                Password = "Secret99"
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
            _priceManager.Subscribe(new Subject("AssetClass=Fx,Currency=EUR,Customer=0001,Dealer=101100,Exchange=OTC,Level=1,Quantity=1.00,QuoteStyle=RFS,Source=MSFX,SubClass=Spot,Symbol=EURUSD,Tenor=Spot,User=pmacdona,ValueDate=20080702"));
            Thread.Sleep(5000);
            _priceManager.Subscribe(new Subject("AssetClass=Fx,Currency=EUR,Customer=0001,Dealer=101100,Exchange=OTC,Level=1,Quantity=1.00,QuoteStyle=RFS,Source=MSFX,SubClass=Spot,Symbol=EURGBP,Tenor=Spot,User=pmacdona,ValueDate=20080702"));
            Thread.Sleep(5000);
            _priceManager.Unsubscribe(new Subject("AssetClass=Fx,Currency=EUR,Customer=0001,Dealer=101100,Exchange=OTC,Level=1,Quantity=1.00,QuoteStyle=RFS,Source=MSFX,SubClass=Spot,Symbol=EURUSD,Tenor=Spot,User=pmacdona,ValueDate=20080702"));
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