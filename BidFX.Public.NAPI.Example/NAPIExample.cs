using System;
using System.Reflection;
using BidFX.Public.NAPI.Plugin.Puffin;

namespace BidFX.Public.NAPI.Example
{
    internal class NAPIExample
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void Main(string[] args)
        {
            Log.Info("testing with " + NAPI.Name + " version " + NAPI.Version);
            new NAPIExample().RunTest();
        }

        private NAPIExample()
        {
            var session = DefaultSession.Session;
            session.AddProviderPlugin(new PuffinProviderPlugin
            {
                Host = "ny-tunnel.qadev.tradingscreen.com",
                Port = 443,
                Tunnel = true,
                Username = "axaapi",
                Password = "HelloWorld123"
            });
            session.PriceUpdateEventHandler += OnPriceUpdate;
            session.SubscriptionStatusEventHandler += OnSubscriptionStatus;
            session.ProviderStatusEventHandler += OnProviderStatus;
            session.Start();
        }

        private void RunTest()
        {
            var session = DefaultSession.Session;
            if (session.WaitUntilReady(TimeSpan.FromSeconds(15)))
            {
                Log.Info("pricing session is ready");
                SendSubscriptions();
            }
            else
            {
                Log.Warn("timed out waiting on session to be ready");
                foreach (var providerProperties in session.ProviderProperties())
                {
                    Log.Info(providerProperties.ToString());
                }
                session.Stop();
            }
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

        private static void SendSubscriptions()
        {
            var subscriber = DefaultSession.Subscriber;
            subscriber.Subscribe("AssetClass=Fx,Exchange=OTC,Level=1,Source=Indi,Symbol=EURUSD");
        }
    }
}