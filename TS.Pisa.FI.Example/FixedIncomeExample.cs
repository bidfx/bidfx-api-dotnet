using System;

namespace TS.Pisa.FI.Example
{
    public class FixedIncomeExample
    {
        private readonly FixedIncomeSession _fixedIncomeSession;

        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Main(string[] args)
        {
            Log.Info("testing with " + Pisa.Name + " version " + Pisa.Version);
            new FixedIncomeExample().RunTest();
        }

        private FixedIncomeExample()
        {
            _fixedIncomeSession = new FixedIncomeSession
            {
                Host = "ny-tunnel.qadev.tradingscreen.com",
//                Host = "ny-tunnel.uatdev.tradingscreen.com",
//                Host = "localhost", Port = 9901, Tunnel = false,
                Username = "axaapi",
                Password = "HelloWorld123"
            };
            _fixedIncomeSession.PriceUpdateEventHandler += OnPriceUpdate;
            _fixedIncomeSession.SubscriptionStatusEventHandler += OnSubscriptionStatus;
            _fixedIncomeSession.Start();
        }

        private void RunTest()
        {
            if (_fixedIncomeSession.Session.WaitUntilReady(TimeSpan.FromSeconds(15)))
            {
                Log.Info("fixed income pricing session is ready");
                SendSubscriptions();
            }
            else
            {
                Log.Warn("timed out waiting on session to be ready");
                foreach (var providerProperties in _fixedIncomeSession.Session.ProviderProperties())
                {
                    Log.Info(providerProperties.ToString());
                }
                _fixedIncomeSession.Stop();
            }
        }

        private static void OnPriceUpdate(object source, FiPriceUpdateEvent priceUpdateEvent)
        {
            var price = priceUpdateEvent.AllPriceFields;
            var bid = price.DecimalField(FieldName.Bid) ?? 0.0m;
            var ask = price.DecimalField(FieldName.Ask) ?? 0.0m;
            var bidSize = price.LongField(FieldName.BidSize) ?? 0L;
            var askSize = price.LongField(FieldName.AskSize) ?? 0L;
            Log.Info(priceUpdateEvent.Subject + " {" + " bidSize=" + bidSize + " bid=" + bid
                     + " ask=" + ask + " askSize=" + askSize + " }");
        }

        private static void OnSubscriptionStatus(object source, FiSubscriptionStatusEvent subscriptionStatusEvent)
        {
            Log.Warn(subscriptionStatusEvent.Subject + " {"
                     + " status=" + subscriptionStatusEvent.SubscriptionStatus
                     + " reason=\"" + subscriptionStatusEvent.StatusReason + '"' + " }");
        }

        private void SendSubscriptions()
        {
            foreach (var isin in System.IO.File.ReadLines("../../TS.Pisa.FI.Example/ISIN_list_5000.txt"))
            {
                try
                {
                    _fixedIncomeSession.Subscribe(new FixedIncomeSubject("SGC", isin.Trim()));
                }
                catch (IllegalSubjectException e)
                {
                    Log.Warn("cannot subscribe to ISIN = \"" + isin + '"', e);
                }
            }
        }
    }
}