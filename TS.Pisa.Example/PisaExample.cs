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
            new PisaExample().RunText();
        }

        private void RunText()
        {
            var session = PrepareSession();
            Log.Info("starting the Pisa session");
            session.Start();
            SendSubscriptions(session);
        }

        private static ISession PrepareSession()
        {
            var session = DefaultSession.GetDefault();
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
            return session;
        }

        private static void OnPriceUpdate(object source, PriceUpdateEventArgs args)
        {
            Log.Info("price update: " + args);
        }

        private static void OnPriceStatus(object source, PriceStatusEventArgs args)
        {
            Log.Info("price status: " + args);
        }

        private void SendSubscriptions(ISubscriber session)
        {
            session.Subscribe("AssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=DE000A1R04X6");
            session.Subscribe("AssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=XS1344742892");
            session.Subscribe("AssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=XS0906394043");
            session.Subscribe("AssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=XS1288894691");
            session.Subscribe("AssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=FR0010096941");
        }
    }
}