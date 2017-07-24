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
                SendLevelOneStreamingSubscriptions("RBCFX", "SSFX", "MSFX", "CSFX", "JPMCFX", "HSBCFX", "RBSFX",
                    "UBSFX", "NOMURAFX", "CITIFX", "COBAFX");
                SendLevelOneQuoteSubscriptions("RBCFX", "SSFX", "MSFX", "CSFX", "JPMCFX", "HSBCFX", "RBSFX",
                    "UBSFX", "NOMURAFX", "CITIFX", "COBAFX");
                SendLevelTwoStreamingSubscriptions();
                SendIndicativeSubscriptions("EURUSD", "GBPUSD");
                SendPremiumFxSubscriptions();
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

        private void SendLevelOneStreamingSubscriptions(params string[] sources)
        {
            foreach (var source in sources)
            {
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    SubjectBuilder.CreateLevelOneSpotStreamingSubject(source, "EURUSD", "EUR", "1000000.00",
                        "FX_ACCT"));
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    SubjectBuilder.CreateLevelOneSpotStreamingSubject(source, "EURGBP", "EUR", "1000000.00",
                        "FX_ACCT"));
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    SubjectBuilder.CreateLevelOneForwardStreamingSubject(source, "EURUSD", "USD", "1000000.00",
                        "FX_ACCT", "20170820"));
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    SubjectBuilder.CreateLevelOneNdfStreamingSubject(source, "USDKRW", "USD", "1000000.00",
                        "FX_ACCT", "20170820", "20170818"));
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    SubjectBuilder.CreateLevelOneSwapStreamingSubject(source, "EURNOK", "EUR", "1000000.00",
                        "FX_ACCT", "1000000.00", "20170820", "20170920"));
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    SubjectBuilder.CreateLevelOneNdsStreamingSubject(source, "CHFJPY", "CHF", "1000000.00",
                        "FX_ACCT", "20170818", "20170918", "1000000.00", "20170820", "20170920"));
            }
        }

        private void SendLevelOneQuoteSubscriptions(params string[] sources)
        {
            foreach (var source in sources)
            {
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    SubjectBuilder.CreateLevelOneSpotQuoteSubject(source, "EURUSD", "EUR", "1000000.00",
                        "FX_ACCT"));
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    SubjectBuilder.CreateLevelOneSpotQuoteSubject(source, "EURGBP", "EUR", "1000000.00",
                        "FX_ACCT"));
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    SubjectBuilder.CreateLevelOneForwardQuoteSubject(source, "EURUSD", "USD", "1000000.00",
                        "FX_ACCT", "20170820"));
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    SubjectBuilder.CreateLevelOneNdfQuoteSubject(source, "USDKRW", "USD", "1000000.00",
                        "FX_ACCT", "20170820", "20170818"));
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    SubjectBuilder.CreateLevelOneSwapQuoteSubject(source, "EURNOK", "EUR", "1000000.00",
                        "FX_ACCT", "1000000.00", "20170820", "20170920"));
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    SubjectBuilder.CreateLevelOneNdsQuoteSubject(source, "CHFJPY", "CHF", "1000000.00",
                        "FX_ACCT", "20170818", "20170918", "1000000.00", "20170820", "20170920"));
            }
        }

        private void SendLevelTwoStreamingSubscriptions()
        {
            DefaultClient.Client.PriceSubscriber.Subscribe(
                SubjectBuilder.CreateLevelTwoSpotStreamingSubject("EURUSD", "EUR", "1000000.00", "FX_ACCT"));
            DefaultClient.Client.PriceSubscriber.Subscribe(
                SubjectBuilder.CreateLevelTwoForwardStreamingSubject("EURGBP", "EUR", "1000000.00", "FX_ACCT",
                    "20170820"));
            DefaultClient.Client.PriceSubscriber.Subscribe(
                SubjectBuilder.CreateLevelTwoNdfStreamingSubject("USDJPY", "USD", "1000000.00", "FX_ACCT", "20170820",
                    "20170818"));
        }

        private void SendIndicativeSubscriptions(params string[] ccyPairs)
        {
            foreach (var ccyPair in ccyPairs)
            {
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    SubjectBuilder.CreateIndicativePriceSubject(ccyPair));
            }
        }

        private void SendPremiumFxSubscriptions()
        {
            DefaultClient.Client.PriceSubscriber.Subscribe(
                SubjectBuilder.CreatePremiumFxSubject("EURUSD", false, false));
            DefaultClient.Client.PriceSubscriber.Subscribe(
                SubjectBuilder.CreatePremiumFxSubject("EURGBP", true, false));
            DefaultClient.Client.PriceSubscriber.Subscribe(
                SubjectBuilder.CreatePremiumFxSubject("GBPJPY", false, true));
            DefaultClient.Client.PriceSubscriber.Subscribe(
                SubjectBuilder.CreatePremiumFxSubject("EURCAD", true, true));
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