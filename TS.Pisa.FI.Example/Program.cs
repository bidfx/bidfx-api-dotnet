using System;

namespace TS.Pisa.FI.Example
{
    public class Program
    {
        private readonly FixedIncomeSession _session;

        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Main(string[] args)
        {
            Log.Info("testing with " + Pisa.Name + " version " + Pisa.Version);
            new Program().RunTest();
        }

        private void RunTest()
        {
            if (_session.WaitUntilReady(TimeSpan.FromSeconds(11)))
            {
                Log.Info("session is ready");
                SendSubscriptions();
            }
            else
            {
                Log.Warn("timed out waiting on session to be ready");
                foreach (var providerPropertiese in _session.ProviderProperties())
                {
                    Log.Info(providerPropertiese.ToString());
                }
                _session.Stop();
            }
        }

        private Program()
        {
            _session = new FixedIncomeSession
            {
                Host = "ny-tunnel.qadev.tradingscreen.com",
//                Host = "ny-tunnel.uatdev.tradingscreen.com",
//                Host = "localhost", Port = 9901, Tunnel = false,
                Username = "axaapi",
                Password = "HelloWorld123"
            };
            _session.OnPrice += OnPrice;
            _session.OnStatus += OnStatus;
            _session.Start();
        }

        private static void OnPrice(object source, PriceUpdateEventArgs evt)
        {
            var price = evt.AllPriceFields;
            var bid = price.DecimalField(FieldName.Bid) ?? 0.0m;
            var ask = price.DecimalField(FieldName.Ask) ?? 0.0m;
            Log.Info(evt.Subject + " {"
                     + " bid=" + bid
                     + " ask=" + ask
                     + " spread=" + (ask - bid)
                     + " }");
        }

        private static void OnStatus(object source, SubscriptionStatusEventArgs evt)
        {
            Log.Warn(evt.Subject + " {"
                     + " status=" + evt.Status
                     + " reason=\"" + evt.Reason + '"'
                     + " }");
        }

        private void SendSubscriptions()
        {
            foreach (var isin in System.IO.File.ReadLines("../../TS.Pisa.FI.Example/ISIN_list_5000.txt"))
            {
                try
                {
                    _session.Subscribe(new FixedIncomeSubject("SGC", isin.Trim()));
                }
                catch (IllegalSubjectException e)
                {
                    Log.Warn("cannot subscribe to ISIN = \"" + isin + '"', e);
                }
            }
        }
    }
}