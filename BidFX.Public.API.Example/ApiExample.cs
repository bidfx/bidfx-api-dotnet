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
        private const string Username = "";
        private const string Password = "";
        
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void Main(string[] args)
        {
            try
            {
                Log.Info("testing with " + PublicApi.Name + " version " + PublicApi.Version);
                new ApiExample().RunTest();
            }
            catch (Exception e)
            {
                Log.Error(e);
                Environment.Exit(1);
            }
        }

        private ApiExample()
        {
            DefaultClient.Client.Host = "ny-tunnel.uatprod.tradingscreen.com";
            DefaultClient.Client.Username = Username;
            DefaultClient.Client.Password = Password;
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
                foreach (IProviderProperties providerProperties in DefaultClient.Client.PriceSession.ProviderProperties())
                {
                    Log.Info(providerProperties.ToString());
                }
                DefaultClient.Client.Stop();
                return;
            }
            
            //Send a trade
            DefaultClient.Client.TradeSession.OrderSubmitEventHandler += OnOrderSubmitResponse;
            SendSpotEURGBPTrade();
        }

        private void SendLevelOneStreamingSubscriptions(params string[] sources)
        {
            foreach (string source in sources)
            {
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneSpotStreamingSubject(Username,
                        "FX_ACCT", "EURUSD", source, "EUR", "1000000.11").CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneSpotStreamingSubject(Username,
                        "FX_ACCT", "EURGBP", source, "EUR", "1000000.00").CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneForwardStreamingSubject(Username,
                        "FX_ACCT", "EURUSD", source, "USD", "1000000.00", "", "20180530").CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneNdfStreamingSubject(Username,
                        "FX_ACCT", "USDKRW", source, "USD", "1000000.00", "1W", "").CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneSwapStreamingSubject(Username,
                            "FX_ACCT", "EURUSD", source, "EUR", "1000000.00", "1M", "", "1000000.00", "2M", "")
                        .CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneNdsStreamingSubject(Username,
                        "FX_ACCT", "CHFJPY", source, "CHF", "1000000.00", "BD", "20180815", "1000000.00", "BD",
                        "20180918").CreateSubject());
            }
        }

        private void SendLevelOneQuoteSubscriptions(params string[] sources)
        {
            foreach (string source in sources)
            {
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneSpotQuoteSubject(Username,
                        "FX_ACCT", "EURUSD", source, "EUR", "1000000.00").CreateSubject(), true);
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneSpotQuoteSubject(Username,
                        "FX_ACCT", "EURGBP", source, "EUR", "1000000.00").CreateSubject(), true);
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneForwardQuoteSubject(Username,
                        "FX_ACCT", "EURUSD", source, "USD", "1000000.00", "1Y", "").CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneNdfQuoteSubject(Username,
                        "FX_ACCT", "USDKRW", source, "USD", "1000000.00", "1Y", "").CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneSwapQuoteSubject(Username,
                            "FX_ACCT", "EURUSD", source, "EUR", "1000000.00", "1Y", "", "1000000.00", "2Y", "")
                        .CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneNdsQuoteSubject(Username,
                            "FX_ACCT", "CHFJPY", source, "CHF", "1000000.00", "6M", "", "1000000.00", "1Y", "")
                        .CreateSubject());
            }
        }

        private void SendLevelTwoStreamingSubscriptions()
        {
            DefaultClient.Client.PriceSubscriber.Subscribe(
                CommonSubjects.CreateLevelTwoSpotStreamingSubject(Username, "FX_ACCT", "EURUSD", "EUR", "1000000.00")
                    .CreateSubject());
            DefaultClient.Client.PriceSubscriber.Subscribe(
                CommonSubjects.CreateLevelTwoForwardStreamingSubject(Username, "FX_ACCT", "EURGBP", "EUR", "1000000.00",
                    "BD", "20181220").CreateSubject());
            DefaultClient.Client.PriceSubscriber.Subscribe(
                CommonSubjects.CreateLevelTwoNdfStreamingSubject(Username, "FX_ACCT", "USDJPY", "USD", "1000000.00", "4M", "")
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
                CommonSubjects.CreatePremiumFxSubject("EURUSD", false, false).SetComponent("User", Username).CreateSubject());
            DefaultClient.Client.PriceSubscriber.Subscribe(
                CommonSubjects.CreatePremiumFxSubject("EURGBP", true, false).SetComponent("User", Username).CreateSubject());
            DefaultClient.Client.PriceSubscriber.Subscribe(
                CommonSubjects.CreatePremiumFxSubject("GBPJPY", false, true).SetComponent("User", Username).CreateSubject());
            DefaultClient.Client.PriceSubscriber.Subscribe(
                CommonSubjects.CreatePremiumFxSubject("EURCAD", true, true).SetComponent("User", Username).CreateSubject());
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
                .SetDealType("Swap")
                .SetHandlingType("stream")
                .SetOrderType("Limit")
                .SetPrice(1.180000m)
                .SetQuantity(2000000)
                .SetSide("Sell")
                .SetTenor("2W")
                .SetFarCurrency("GBP")
                .SetFarQuantity(2000000)
                .SetFarSide("Buy")
                .SetFarTenor("4M")
                .SetReferenceOne(".NET API Example")
                .Build();
            FutureOrder futureOrder = new FutureOrderBuilder()
                .SetInstrumentCode("HGU8 Comdty")
                .SetInstrumentCodeType("BLOOMBERG")
                .SetOrderType("MARKET")
                .SetQuantity(20)
                .SetReferenceOne(".NET API Example")
                .SetSide("SELL")
                .Build();
            
            long messageId = DefaultClient.Client.TradeSession.SubmitOrder(fxOrder);
            Log.InfoFormat("FXOrder Submitted. MessageId {0}", messageId);
            long futuremessageId = DefaultClient.Client.TradeSession.SubmitOrder(futureOrder);
            Log.InfoFormat("FutureOrder Submitted. MessageId {0}", futuremessageId);
            
        }

        private static void OnOrderSubmitResponse(object sender, Order order)
        {
            Log.InfoFormat("Order Response: MessageId => {0}, OrderID => {1}, State => {2}, FullOrder => {3}", order.GetMessageId(),
                order.GetOrderTsId(), order.GetState(), order);
        }

        private static void OnDisconnect(object sender, DisconnectEventArgs eventArgs)
        {
            Log.InfoFormat("Forced to disconnect. Closing program. {0}", eventArgs.Reason);
            Environment.Exit(1);
        }
    }
}