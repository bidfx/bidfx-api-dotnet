using System;
using System.Linq;
using System.Reflection;
using BidFX.Public.API.Price;
using BidFX.Public.API.Price.Subject;
using BidFX.Public.API.Trade;
using BidFX.Public.API.Trade.Order;
using log4net;

namespace BidFX.Public.API.Example
{
    internal class ApiExample
    {
        private const string username = "";
        private const string password = "";
        
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
            DefaultClient.Client.Username = username;
            DefaultClient.Client.Password = password;
            if (!DefaultClient.Client.SetLevelTwoSubscriptionLimit("4-18684088267991776204692688552290739494"))
            {
                throw new Exception("Couldn't restrict number of depth subscriptions");
            }
            if (!DefaultClient.Client.SetLevelOneSubscriptionLimit("50-22676531820744303226904389626603522073"))
            {
                throw new Exception("Couldn't restrict number of level one subscriptions");
            }
            
            var session = DefaultClient.Client.PriceSession;
            session.PriceUpdateEventHandler += OnPriceUpdate;
            session.SubscriptionStatusEventHandler += OnSubscriptionStatus;
            session.ProviderStatusEventHandler += OnProviderStatus;
            
        }

        private void RunTest()
        {
            
            //Pricing
            if (DefaultClient.Client.PriceSession.WaitUntilReady(TimeSpan.FromSeconds(15)))
            {
                Log.Info("pricing session is ready");
/*                SendLevelOneStreamingSubscriptions("RBCFX", "SSFX", "MSFX", "CSFX", "JPMCFX", "HSBCFX", "RBSFX",
                    "UBSFX", "NOMURAFX", "CITIFX", "COBAFX");
                SendLevelOneQuoteSubscriptions("RBCFX", "SSFX", "MSFX", "CSFX", "JPMCFX", "HSBCFX", "RBSFX",
                    "UBSFX", "NOMURAFX", "CITIFX", "COBAFX");
                SendIndicativeSubscriptions("EURUSD", "GBPUSD");
                SendPremiumFxSubscriptions();*/
                SendLevelTwoStreamingSubscriptions();
            }
            else
            {
                if (DefaultClient.Client.PriceSession.ProviderProperties().Any(pp => ProviderStatus.Unauthorized == pp.ProviderStatus))
                {
                    Log.Warn("Invalid Credentials");
                }
                else
                {
                    Log.Warn("timed out waiting on session to be ready");
                }
                foreach (IProviderProperties providerProperties in DefaultClient.Client.PriceSession.ProviderProperties())
                {
                    Log.Info(providerProperties.ToString());
                }
                DefaultClient.Client.PriceSession.Stop();
                return;
            }
            
            //Send a trade
            /*
            DefaultClient.Client.TradeSession.OrderSubmitEventHandler += OnOrderSubmitResponse;
            SendSpotEURGBPTrade();
            */
        }

        private void SendLevelOneStreamingSubscriptions(params string[] sources)
        {
            foreach (string source in sources)
            {
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneSpotStreamingSubject(username,
                        "FX_ACCT", "EURUSD", source, "EUR", "1000000.11").CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneSpotStreamingSubject(username,
                        "FX_ACCT", "EURGBP", source, "EUR", "1000000.00").CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneForwardStreamingSubject(username,
                        "FX_ACCT", "EURUSD", source, "USD", "1000000.00", "", "20180530").CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneNdfStreamingSubject(username,
                        "FX_ACCT", "USDKRW", source, "USD", "1000000.00", "1W", "").CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneSwapStreamingSubject(username,
                            "FX_ACCT", "EURNOK", source, "EUR", "1000000.00", "1M", "", "1000000.00", "2M", "")
                        .CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneNdsStreamingSubject(username,
                        "FX_ACCT", "CHFJPY", source, "CHF", "1000000.00", "BD", "20180815", "1000000.00", "BD",
                        "20180918").CreateSubject());
            }
        }

        private void SendLevelOneQuoteSubscriptions(params string[] sources)
        {
            foreach (string source in sources)
            {
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneSpotQuoteSubject(username,
                        "FX_ACCT", "EURUSD", source, "EUR", "1000000.00").CreateSubject(), true);
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneSpotQuoteSubject(username,
                        "FX_ACCT", "EURGBP", source, "EUR", "1000000.00").CreateSubject(), true);
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneForwardQuoteSubject(username,
                        "FX_ACCT", "EURUSD", source, "USD", "1000000.00", "1Y", "").CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneNdfQuoteSubject(username,
                        "FX_ACCT", "USDKRW", source, "USD", "1000000.00", "1Y", "").CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneSwapQuoteSubject(username,
                            "FX_ACCT", "EURNOK", source, "EUR", "1000000.00", "1Y", "", "1000000.00", "2Y", "")
                        .CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneNdsQuoteSubject(username,
                            "FX_ACCT", "CHFJPY", source, "CHF", "1000000.00", "6M", "", "1000000.00", "1Y", "")
                        .CreateSubject());
            }
        }

        private void SendLevelTwoStreamingSubscriptions()
        {
            DefaultClient.Client.PriceSubscriber.Subscribe(
                CommonSubjects.CreateLevelTwoSpotStreamingSubject(username, "FX_ACCT", "EURUSD", "EUR", "1000000.00")
                    .CreateSubject());
            DefaultClient.Client.PriceSubscriber.Subscribe(
                CommonSubjects.CreateLevelTwoForwardStreamingSubject(username, "FX_ACCT", "EURGBP", "EUR", "1000000.00",
                    "BD", "20181220").CreateSubject());
            DefaultClient.Client.PriceSubscriber.Subscribe(
                CommonSubjects.CreateLevelTwoNdfStreamingSubject(username, "FX_ACCT", "USDJPY", "USD", "1000000.00", "4M", "")
                    .CreateSubject());
        }

        private void SendIndicativeSubscriptions(params string[] ccyPairs)
        {
            foreach (string ccyPair in ccyPairs)
            {
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateIndicativePriceSubject(ccyPair).CreateSubject());
            }
        }

        private void SendPremiumFxSubscriptions()
        {
            DefaultClient.Client.PriceSubscriber.Subscribe(
                CommonSubjects.CreatePremiumFxSubject("EURUSD", false, false).SetComponent("User", "lasman").CreateSubject());
            DefaultClient.Client.PriceSubscriber.Subscribe(
                CommonSubjects.CreatePremiumFxSubject("EURGBP", true, false).SetComponent("User", "lasman").CreateSubject());
            DefaultClient.Client.PriceSubscriber.Subscribe(
                CommonSubjects.CreatePremiumFxSubject("GBPJPY", false, true).SetComponent("User", "lasman").CreateSubject());
            DefaultClient.Client.PriceSubscriber.Subscribe(
                CommonSubjects.CreatePremiumFxSubject("EURCAD", true, true).SetComponent("User", "lasman").CreateSubject());
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
            FxOrder fxOrder = new FxOrderBuilder()
                .SetAccount("FX_ACCT")
                .SetCurrencyPair("EURGBP")
                .SetCurrency("GBP")
                .SetDealType("Spot")
                .SetHandlingType("stream")
                .SetOrderType("Limit")
                .SetPrice("1.180000")
                .SetQuantity("2000000")
                .SetSide("Sell")
                .SetTenor("Spot")
                .SetReferenceOne(".NET API Example")
                .SetAllocationTemplate("TEST2")
                .Build();
            long messageId = DefaultClient.Client.TradeSession.SubmitOrder(fxOrder);
            Log.InfoFormat("Order Submitted. MessageId {0}", messageId);
        }

        private static void OnOrderSubmitResponse(object sender, OrderResponse orderResponse)
        {
            Log.InfoFormat("Order Response: MessageId => {0}, OrderID => {1}, State => {2}", orderResponse.GetMessageId(),
                orderResponse.GetOrderId(), orderResponse.GetState());
            Log.InfoFormat("Errors: {0}", string.Join(", ", orderResponse.GetErrors()));
        }
    }
}