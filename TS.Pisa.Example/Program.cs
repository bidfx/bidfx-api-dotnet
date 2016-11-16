using System.Threading;
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
            log.Info("starting the Pisa session");
            session.Start();
            session.Subscribe("AssetClass=Equity,Exchange=LSE,Level=1,Source=ComStock,Symbol=E:IAG");
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
