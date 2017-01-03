using System;
using System.Reflection;
using TS.Pisa.Plugin.Puffin;

namespace TS.Pisa.Example
{
    internal class PisaExample
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void Main(string[] args)
        {
            Log.Info("testing with " + Pisa.Name + " version " + Pisa.Version);
            new PisaExample().RunTest();
        }

        private PisaExample()
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
            subscriber.Subscribe("AssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=DE000A1R04X6");
            subscriber.Subscribe("AssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=XS1344742892");
            subscriber.Subscribe("AssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=XS0906394043");
            subscriber.Subscribe("AssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=XS1288894691");
            subscriber.Subscribe("AssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=FR0010096941");
        }
    }
}