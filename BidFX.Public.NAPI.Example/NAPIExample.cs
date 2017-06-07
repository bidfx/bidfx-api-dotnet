using System;
using System.Reflection;
using BidFX.Public.NAPI.Price;

namespace BidFX.Public.NAPI.Example
{
    internal class NAPIExample
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly PriceManager _priceManager;

        public static void Main(string[] args)
        {
            Log.Info("testing with " + Price.NAPI.Name + " version " + Price.NAPI.Version);
            new NAPIExample().RunTest();
        }

        private NAPIExample()
        {
            var client = new Client
            {
                Host = "ny-tunnel.uatdev.tradingscreen.com",
                Port = 443,
                Tunnel = true,
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
            _priceManager.Subscribe("AssetClass=Fx,Exchange=OTC,Level=1,Source=Indi,Symbol=EURGBP");
            _priceManager.Subscribe("AssetClass=Fx,Exchange=OTC,Level=1,Source=Indi,Symbol=EURUSD");
            _priceManager.Subscribe("AssetClass=Fx,Exchange=OTC,Level=1,Source=Indi,Symbol=GBPAUD");
            _priceManager.Subscribe("AssetClass=Fx,Exchange=OTC,Level=1,Source=Indi,Symbol=USDCAD");
            _priceManager.Subscribe("AssetClass=Fx,Exchange=OTC,Level=1,Source=Indi,Symbol=GBPCAD");
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