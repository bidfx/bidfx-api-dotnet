using System;
using System.Reflection;
using System.Threading;
using BidFX.Public.API.Price;
using BidFX.Public.API.Price.Subject;
using log4net;

namespace BidFX.Public.API.Example
{
    internal class ApiExample
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly PriceManager _priceManager;
        
        public static void Main(string[] args)
        {
            Log.Info("testing with " + PublicApi.Name + " version " + PublicApi.Version);
            new ApiExample().RunTest();
        }

        private ApiExample()
        {
            var client = new Client
            {
                Host = "ny-tunnel.uatprod.tradingscreen.com",
                Port = 443,
                Tunnel = true,
                Username = "pmacdona",
                Password = "Secret99"
            };
            _priceManager = client.GetPriceManager();
            _priceManager.PriceUpdateEventHandler += OnPriceUpdate;
            _priceManager.SubscriptionStatusEventHandler += OnSubscriptionStatus;
            _priceManager.ProviderStatusEventHandler += OnProviderStatus;
        }

        private void RunTest()
        {
            if (_priceManager.WaitUntilReady(TimeSpan.FromSeconds(15)))
            {
                Log.Info("pricing session is ready");
                SendLevel1RfsSubscriptions("RBCFX");//, "SSFX", "MSFX", "CSFX", "JPMCFX", "HSBCFX", "RBSFX", "UBSFX", "NOMURAFX", "CITIFX", "COBAFX");
                SendLevel2RfsSubscriptions();
                SendLevel1RfqSubscriptions("SSFX");
            }
            else
            {
                Log.Warn("timed out waiting on session to be ready");
                foreach (var providerProperties in _priceManager.ProviderProperties())
                {
                    Log.Info(providerProperties.ToString());
                }
                _priceManager.Stop();
            }
        }

        private void SendLevel1RfsSubscriptions(params string[] sources)
        {
            foreach (var source in sources)
            {
                var levelOneSpotRfsSubject = SubjectBuilder.CreateLevelOneSpotRfsSubject(source, "EURUSD", "EUR", "1000000.00",
                    "FX_ACCT");
                _priceManager.Subscribe(
                    levelOneSpotRfsSubject);
//                _priceManager.Subscribe(
//                    SubjectBuilder.CreateLevelOneSpotRfsSubject(source, "EURGBP", "EUR", "1000000.00",
//                        "FX_ACCT"));
//                _priceManager.Subscribe(
//                    SubjectBuilder.CreateLevelOneSpotRfsSubject(source, "USDJPY", "JPY", "1000000.00",
//                        "FX_ACCT"));
//                _priceManager.Subscribe(
//                    SubjectBuilder.CreateLevelOneSpotRfsSubject(source, "GBPUSD", "GBP", "1000000.00",
//                        "FX_ACCT"));
//                _priceManager.Subscribe(
//                    SubjectBuilder.CreateLevelOneSpotRfsSubject(source, "EURNOK", "EUR", "1000000.00",
//                        "FX_ACCT"));
//                _priceManager.Subscribe(
//                    SubjectBuilder.CreateLevelOneSpotRfsSubject(source, "CHFJPY", "CHF", "1000000.00",
//                        "FX_ACCT"));
//                _priceManager.Subscribe(
//                    SubjectBuilder.CreateLevelOneSpotRfsSubject(source, "NZDUSD", "USD", "1000000.00",
//                        "FX_ACCT"));
//                _priceManager.Subscribe(
//                    SubjectBuilder.CreateLevelOneSpotRfsSubject(source, "USDSEK", "USD", "1000000.00",
//                        "FX_ACCT"));
            }
        }

        private void SendLevel2RfsSubscriptions()
        {
            _priceManager.Subscribe(SubjectBuilder.CreateLevelTwoSpotRfsSubject("EURUSD", "EUR", "1000000.00", "FX_ACCT"));
            _priceManager.Subscribe(SubjectBuilder.CreateLevelTwoSpotRfsSubject("EURGBP", "EUR", "1000000.00", "FX_ACCT"));
            _priceManager.Subscribe(SubjectBuilder.CreateLevelTwoSpotRfsSubject("USDJPY", "USD", "1000000.00", "FX_ACCT"));
            _priceManager.Subscribe(SubjectBuilder.CreateLevelTwoSpotRfsSubject("EURNOK", "NOK", "1000000.00", "FX_ACCT"));
        }

        private void SendLevel1RfqSubscriptions(params string[] sources)
        {
            foreach (var source in sources)
            {
                _priceManager.Subscribe(SubjectBuilder.CreateLevelOneSpotRfqSubject(source, "EURUSD", "EUR",
                    "1000000.00", "FX_ACCT", true));
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
    }
}