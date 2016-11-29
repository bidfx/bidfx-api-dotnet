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
            new PisaExample().Run();
        }

        private void Run()
        {
            PrepareSession();
            SendSubscriptions();
        }

        private static void PrepareSession()
        {
            var session = DefaultSession.GetSession();
            session.PriceUpdate += OnPriceUpdate;
            session.PriceStatus += OnPriceStatus;
            session.AddProviderPlugin(new PuffinProviderPlugin
            {
                Host = "ny-tunnel.qadev.tradingscreen.com",
                Port = 443,
                Tunnel = true,
                Username = "axaapitest",
                Password = "B3CarefulWithThatAXAEug3n3!"
            });
            Log.Info("starting the Pisa session");
            session.Start();
        }

        private static void OnPriceUpdate(object source, PriceUpdateEventArgs args)
        {
            Log.Info("price update: " + args);
        }

        private static void OnPriceStatus(object source, SubscriptionStatusEventArgs args)
        {
            Log.Info("price status: " + args);
        }

        private static void SendSubscriptions()
        {
            var subscriber = DefaultSession.GetSubscriber();
            subscriber.Subscribe("AssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=DE000A1R04X6");
            subscriber.Subscribe("AssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=XS1344742892");
            subscriber.Subscribe("AssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=XS0906394043");
            subscriber.Subscribe("AssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=XS1288894691");
            subscriber.Subscribe("AssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=FR0010096941");
        }
    }
}