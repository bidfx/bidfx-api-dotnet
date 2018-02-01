using System;
using System.Reflection;
using System.Threading;
using BidFX.Public.API.Price;
using BidFX.Public.API.Price.Subject;
using BidFX.Public.API.Trade;
using BidFX.Public.API.Trade.Order;
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
            DefaultClient.Client.Host = "ny-tunnel.uatdev.tradingscreen.com";
            DefaultClient.Client.Username = "lasman";
            DefaultClient.Client.Password = "HelloWorld123";
            /*
            var session = DefaultClient.Client.PriceSession;
            session.PriceUpdateEventHandler += OnPriceUpdate;
            session.SubscriptionStatusEventHandler += OnSubscriptionStatus;
            session.ProviderStatusEventHandler += OnProviderStatus;
            */
        }

        private void RunTest()
        {
            /*
            //Pricing
            if (DefaultClient.Client.PriceSession.WaitUntilReady(TimeSpan.FromSeconds(15)))
            {
                Log.Info("pricing session is ready");
                SendLevelOneStreamingSubscriptions("RBCFX", "SSFX", "MSFX", "CSFX", "JPMCFX", "HSBCFX", "RBSFX",
                    "UBSFX", "NOMURAFX", "CITIFX", "COBAFX");
                SendLevelOneQuoteSubscriptions("RBCFX", "SSFX", "MSFX", "CSFX", "JPMCFX", "HSBCFX", "RBSFX",
                    "UBSFX", "NOMURAFX", "CITIFX", "COBAFX");
                SendIndicativeSubscriptions("EURUSD", "GBPUSD");
                SendPremiumFxSubscriptions();
                SendLevelTwoStreamingSubscriptions();
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
            */
            //Send a trade
            DefaultClient.Client.TradeManager.OrderSubmitEventHandler += OnOrderSubmitResponse;
/*            SendSpotEURGBPTrade();
            Thread.Sleep(TimeSpan.FromMilliseconds(200));
            SendSpotEURGBPTrade();
            Thread.Sleep(TimeSpan.FromMilliseconds(400));*/
            SendSpotEURGBPTrade();
            Thread.Sleep(TimeSpan.FromMilliseconds(2000));
        }

        private void SendLevelOneStreamingSubscriptions(params string[] sources)
        {
            foreach (var source in sources)
            {
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneSpotStreamingSubject(
                        "FX_ACCT", "EURUSD", source, "EUR", "1000000.00").CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneSpotStreamingSubject(
                        "FX_ACCT", "EURGBP", source, "EUR", "1000000.00").CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneForwardStreamingSubject(
                        "FX_ACCT", "EURUSD", source, "USD", "1000000.00", "1W", "").CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneNdfStreamingSubject(
                        "FX_ACCT", "USDKRW", source, "USD", "1000000.00", "1W", "").CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneSwapStreamingSubject(
                        "FX_ACCT", "EURNOK", source, "EUR", "1000000.00", "1M", "", "1000000.00", "2M", "").CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneNdsStreamingSubject(
                        "FX_ACCT", "CHFJPY", source, "CHF", "1000000.00", "BD", "20180818", "1000000.00", "BD", "20180918").CreateSubject());
            }
        }

        private void SendLevelOneQuoteSubscriptions(params string[] sources)
        {
            foreach (var source in sources)
            {
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneSpotQuoteSubject(
                        "FX_ACCT", "EURUSD", source, "EUR", "1000000.00").CreateSubject(), true);
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneSpotQuoteSubject(
                        "FX_ACCT", "EURGBP", source, "EUR", "1000000.00").CreateSubject(), true);
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneForwardQuoteSubject(
                        "FX_ACCT", "EURUSD", source, "USD", "1000000.00", "1Y", "").CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneNdfQuoteSubject(
                        "FX_ACCT", "USDKRW", source, "USD", "1000000.00", "1Y", "").CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneSwapQuoteSubject(
                        "FX_ACCT", "EURNOK", source, "EUR", "1000000.00", "1Y", "", "1000000.00", "2Y", "").CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneNdsQuoteSubject(
                        "FX_ACCT", "CHFJPY", source, "CHF", "1000000.00", "6M", "", "1000000.00", "1Y", "").CreateSubject());
            }
        }

        private void SendLevelTwoStreamingSubscriptions()
        {
            DefaultClient.Client.PriceSubscriber.Subscribe(
                CommonSubjects.CreateLevelTwoSpotStreamingSubject("FX_ACCT", "EURUSD", "EUR", "1000000.00").CreateSubject());
            DefaultClient.Client.PriceSubscriber.Subscribe(
                CommonSubjects.CreateLevelTwoForwardStreamingSubject("FX_ACCT", "EURGBP", "EUR", "1000000.00",
                    "BD", "20181220").CreateSubject());
            DefaultClient.Client.PriceSubscriber.Subscribe(
                CommonSubjects.CreateLevelTwoNdfStreamingSubject("FX_ACCT", "USDJPY", "USD", "1000000.00", "4M", "").CreateSubject());
        }

        private void SendIndicativeSubscriptions(params string[] ccyPairs)
        {
            foreach (var ccyPair in ccyPairs)
            {
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateIndicativePriceSubject(ccyPair).CreateSubject());
            }
        }

        private void SendPremiumFxSubscriptions()
        {
            DefaultClient.Client.PriceSubscriber.Subscribe(
                CommonSubjects.CreatePremiumFxSubject("EURUSD", false, false).CreateSubject());
            DefaultClient.Client.PriceSubscriber.Subscribe(
                CommonSubjects.CreatePremiumFxSubject("EURGBP", true, false).CreateSubject());
            DefaultClient.Client.PriceSubscriber.Subscribe(
                CommonSubjects.CreatePremiumFxSubject("GBPJPY", false, true).CreateSubject());
            DefaultClient.Client.PriceSubscriber.Subscribe(
                CommonSubjects.CreatePremiumFxSubject("EURCAD", true, true).CreateSubject());
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

        private void SendSpotEURGBPTrade()
        {
            var fxOrder = new FxOrderBuilder()
                .SetAccount("FX_ACCT")
                .SetCurrencyPair("EURGBP")
                .SetCurrency("GBP")
                .SetDealType("Spot")
                .SetHandlingType("stream")
                .SetPriceType("Limit")
                .SetPrice("1.180000")
                .SetQuantity("2000000")
                .SetSide("Sell")
                .SetSettlementDate("2018-02-07")
                .SetReferenceOne(".NET API Example")
                .Build();
            var messageId = DefaultClient.Client.TradeManager.SubmitOrder(fxOrder);
            Log.InfoFormat("Order Submitted. MessageId {0}", messageId);
        }

        private static void OnOrderSubmitResponse(object sender, OrderResponse orderResponse)
        {
            Log.InfoFormat("Order Response: MessageId => {0}, OrderID => {1}, State => {2}", orderResponse.MessageId,
                orderResponse.GetOrderId(), orderResponse.GetState());
        }
    }
}