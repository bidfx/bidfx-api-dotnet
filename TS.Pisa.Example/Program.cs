using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;
using TS.Pisa.Plugin.Puffin;
namespace TS.Pisa.Example
{
    internal class Program
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static void Main(string[] args)
        {
            var program = new Program();
            program.Test();
        }
        private void Test()
        {
            var reference = PisaVersion.Reference;
            Log.Info("pisa version:    " + PisaVersion.Version);
            Log.Info("program version: " + Assembly.GetExecutingAssembly().GetName().Version);
            Log.Info("user:            " + System.Security.Principal.WindowsIdentity.GetCurrent().Name);
            Log.Info("host:            " + IPGlobalProperties.GetIPGlobalProperties().HostName);
            var session = PrepareSession();
            Log.Info("starting the Pisa session");
            session.Start();
//            Thread.Sleep(5000);
//            session.Subscribe("AssetClass=Equity,Exchange=LSE,Level=1,Source=ComStock,Symbol=E:IAG");
//            Thread.Sleep(5000);
//            session.Unsubscribe("AssetClass=Equity,Exchange=LSE,Level=1,Source=ComStock,Symbol=E:IAG");
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
