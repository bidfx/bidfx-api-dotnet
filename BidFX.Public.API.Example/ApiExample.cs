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
        
        public static void Main(string[] args)
        {
            Log.Info("testing with " + PublicApi.Name + " version " + PublicApi.Version);
            new ApiExample().RunTest();
        }

        private ApiExample()
        {
            DefaultClient.Client.Host = "ny-tunnel.uatprod.tradingscreen.com";
            DefaultClient.Client.Username = "pmacdona";
            DefaultClient.Client.Password = "Secret99";
            var session = DefaultClient.Client.PriceSession;
            session.PriceUpdateEventHandler += OnPriceUpdate;
            session.SubscriptionStatusEventHandler += OnSubscriptionStatus;
            session.ProviderStatusEventHandler += OnProviderStatus;
        }

        private void RunTest()
        {
            if (DefaultClient.Client.PriceSession.WaitUntilReady(TimeSpan.FromSeconds(15)))
            {
                Log.Info("pricing session is ready");
                SendLevel1RfsSubscriptions("RBCFX", "SSFX", "MSFX", "CSFX", "JPMCFX", "HSBCFX", "RBSFX", "UBSFX", "NOMURAFX", "CITIFX", "COBAFX");
                SendLevel2RfsSubscriptions();
                SendLevel1RfqSubscriptions("SSFX");
            }
            else
            {
                Log.Warn("timed out waiting on session to be ready");
                foreach (var providerProperties in DefaultClient.Client.PriceSession.ProviderProperties())
                {
                    Log.Info(providerProperties.ToString());
                }
                DefaultClient.Client.PriceSession.Stop();
            }
        }

        private void SendLevel1RfsSubscriptions(params string[] sources)
        {
            foreach (var source in sources)
            {
                DefaultClient.Client.PriceSubscriber.Subscribe(SubjectBuilder.CreateLevelOneSpotStreamingSubject(source, "EURUSD", "EUR", "1000000.00",
                    "FX_ACCT"));
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    SubjectBuilder.CreateLevelOneSpotStreamingSubject(source, "EURGBP", "EUR", "1000000.00",
                        "FX_ACCT"));
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    SubjectBuilder.CreateLevelOneSpotStreamingSubject(source, "USDJPY", "JPY", "1000000.00",
                        "FX_ACCT"));
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    SubjectBuilder.CreateLevelOneSpotStreamingSubject(source, "GBPUSD", "GBP", "1000000.00",
                        "FX_ACCT"));
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    SubjectBuilder.CreateLevelOneSpotStreamingSubject(source, "EURNOK", "EUR", "1000000.00",
                        "FX_ACCT"));
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    SubjectBuilder.CreateLevelOneSpotStreamingSubject(source, "CHFJPY", "CHF", "1000000.00",
                        "FX_ACCT"));
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    SubjectBuilder.CreateLevelOneSpotStreamingSubject(source, "NZDUSD", "USD", "1000000.00",
                        "FX_ACCT"));
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    SubjectBuilder.CreateLevelOneSpotStreamingSubject(source, "USDSEK", "USD", "1000000.00",
                        "FX_ACCT"));
            }
        }

        private void SendLevel2RfsSubscriptions()
        {
            DefaultClient.Client.PriceSubscriber.Subscribe(SubjectBuilder.CreateLevelTwoSpotStreamingSubject("EURUSD", "EUR", "1000000.00", "FX_ACCT"));
            DefaultClient.Client.PriceSubscriber.Subscribe(SubjectBuilder.CreateLevelTwoSpotStreamingSubject("EURGBP", "EUR", "1000000.00", "FX_ACCT"));
            DefaultClient.Client.PriceSubscriber.Subscribe(SubjectBuilder.CreateLevelTwoSpotStreamingSubject("USDJPY", "USD", "1000000.00", "FX_ACCT"));
            DefaultClient.Client.PriceSubscriber.Subscribe(SubjectBuilder.CreateLevelTwoSpotStreamingSubject("EURNOK", "NOK", "1000000.00", "FX_ACCT"));
        }

        private void SendLevel1RfqSubscriptions(params string[] sources)
        {
            foreach (var source in sources)
            {
                DefaultClient.Client.PriceSubscriber.Subscribe(SubjectBuilder.CreateLevelOneSpotQuoteSubject(source, "EURUSD", "EUR",
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