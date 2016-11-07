using TS.Pisa.Plugin.Puffin;

namespace TS.Pisa.Example
{
    internal class Program
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Main(string[] args)
        {
            var program = new Program();
            program.Test();
        }

        private void Test()
        {
            var session = PrepareSession();
            var subscriber = session.CreateSubscriber();
            subscriber.Subscribe("AssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=DE000A14KK32");
            subscriber.Subscribe("AssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=XS1083955911");
            subscriber.Subscribe("AssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=XS1130066175");
            log.Info("starting the Pisa session");
            session.Start();
        }

        private static ISession PrepareSession()
        {
            var session = DefaultSession.GetDefault();
            session.AddProviderPlugin(new PuffinProviderPlugin
            {
                Host = "ny-tunnel.uatdev.tradingscreen.com",
                Username = "axaapitest",
                Password = "B3CarefulWithThatAXAEug3n3!"
            });
            return session;
        }
    }
}