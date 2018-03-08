using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using BidFX.Public.API.Price;
using BidFX.Public.API.Price.Subject;
using log4net;

namespace BidFX.Public.API.Example
{
    public class BestOfBookStressTest
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const string USERNAME = "";
        private const string PASSWORD = "";
        private const string ACCOUNT = "FX_ACCT";
        private readonly Client _client;

        private int updateCount = 0;
        private Stopwatch stopwatch = new Stopwatch();
        
        private readonly string[] ccyPairs =
        {
            "EURGBP",
            "EURAUD",
            "GBPAUD",
            "EURNZD",
            "GBPNZD",
            "AUDNZD",
            "EURUSD",
            "GBPUSD",
            "AUDUSD",
            "NZDUSD",
            "EURCAD",
            "GBPCAD",
            "AUDCAD",
            "NZDCAD",
            "USDCAD",
            "EURCHF",
            "GBPCHF",
            "AUDCHF",
            "NZDCHF",
            "USDCHF",
            "CADCHF",
            "EURNOK",
            "GBPNOK",
            "AUDNOK",
            "NZDNOK",
            "USDNOK",
            "CADNOK",
            "CHFNOK",
            "EURSEK",
            "GBPSEK",
            "AUDSEK",
            "NZDSEK",
            "USDSEK",
            "CADSEK",
            "CHFSEK",
            "NOKSEK",
            "DKKSEK",
            "EURJPY",
            "GBPJPY",
            "AUDJPY",
            "NZDJPY",
            "USDJPY",
            "CADJPY",
            "CHFJPY",
            "NOKJPY",
            "DKKJPY",
            "SEKJPY"
        };

        private readonly string[] settlementDates =
        {
            "20180309",
            "20180912",
            "20180316",
            "20180323",
            "20180403",
            "20180409",
            "20180509",
            "20180611",
            "20180709",
            "20180809",
            "20180910",
            "20181009",
            "20191109",
            "20181210",
            "20190109",
            "20190211",
            "20190311",
            "20200309",
            "20210309"
        };
        
        public static void Main3(string[] args)
        {
            new BestOfBookStressTest().Run();
        }

        private void Run()
        {
            Client _client = DefaultClient.Client;
            _client.Host = "ny-tunnel.uatprod.tradingscreen.com";
            _client.Username = USERNAME;
            _client.Password = PASSWORD;
            _client.PriceSession.PriceUpdateEventHandler += OnPriceUpdate;

            if (_client.PriceSession.WaitUntilReady(TimeSpan.FromSeconds(15)))
            {
                SendSubscriptions();
                stopwatch.Start();
                Thread.Sleep(TimeSpan.FromSeconds(120));
                stopwatch.Stop();
                _client.PriceSession.Stop();
                OutputData();
            }
            else
            {
                if (_client.PriceSession.ProviderProperties().Any(pp => ProviderStatus.Unauthorized == pp.ProviderStatus))
                {
                    Log.Warn("Invalid credentials.");
                }
                else
                {
                    Log.Warn("Timed out waiting for session to be ready.");
                }
                foreach (IProviderProperties providerProperties in DefaultClient.Client.PriceSession.ProviderProperties())
                {
                    Log.Info(providerProperties.ToString());
                }

                DefaultClient.Client.PriceSession.Stop();
            }
        }

        private void SendSubscriptions()
        {
            SubjectBuilder sb =
                CommonSubjects.CreateLevelTwoForwardStreamingSubject("FX_ACCT", "XXXYYY", "XXX", "1000000", "BD",
                    "20180101");
            sb.SetComponent(SubjectComponentName.Rows, "1");
            foreach (string ccyPair in ccyPairs)
            {
                string ccy = ccyPair.Substring(0, 3);
                foreach (string settlementDate in settlementDates)
                {
                    sb.SetComponent(SubjectComponentName.Symbol, ccyPair);
                    sb.SetComponent(SubjectComponentName.Currency, ccy);
                    sb.SetComponent(SubjectComponentName.SettlementDate, settlementDate);
                    DefaultClient.Client.PriceSubscriber.Subscribe(sb.CreateSubject());
                }
            }
        }
        
        private void OnPriceUpdate(object source, PriceUpdateEvent priceEvent)
        {
            if (stopwatch.IsRunning)
            {
                updateCount++;
            }
        }

        private void OutputData()
        {
            Log.InfoFormat("{0} price updates received in {1} ms from {3} subscriptions - {2} updates/s", updateCount, stopwatch.ElapsedMilliseconds, 1000* updateCount/stopwatch.ElapsedMilliseconds,ccyPairs.Length * settlementDates.Length);
        }
    }
}