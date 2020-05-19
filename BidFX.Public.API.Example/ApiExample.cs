using System;
using System.Reflection;
using BidFX.Public.API.Price;
using BidFX.Public.API.Price.Subject;
using BidFX.Public.API.Trade.Order;
using Serilog;

namespace BidFX.Public.API.Example
{
    internal class ApiExample
    {
        private const string Username = "";
        private const string Password = "";
        private const string ProductSerial = "";
        private const string Account = "";

        public static void Main(string[] args)
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console(outputTemplate:
                        "[{Timestamp:HH:mm:ss} {Level:u4}][{SourceContext}] {Message:lj}{NewLine}{Exception}")
                    .CreateLogger();
                Log.Information("testing with " + PublicApi.Name + " version " + PublicApi.Version);
                new ApiExample().RunTest();
            }
            catch (Exception e)
            {
                Log.Error(e, "unexpected error");
                Environment.Exit(1);
            }
        }

        private ApiExample()
        {
            DefaultClient.Client.Host = "ny-tunnel.uatprod.tradingscreen.com";
            DefaultClient.Client.Username = Username;
            DefaultClient.Client.Password = Password;
            DefaultClient.Client.ProductSerial = ProductSerial;
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
                Log.Information("pricing session is ready");
                SendLevelOneStreamingSubscriptions("RBCFX", "SSFX", "MSFX", "CSFX", "JPMCFX", "HSBCFX", "RBSFX",
                    "UBSFX", "NOMURAFX", "CITIFX", "COBAFX");
                SendLevelOneQuoteSubscriptions("RBCFX", "SSFX", "MSFX", "CSFX", "JPMCFX", "HSBCFX", "RBSFX",
                    "UBSFX", "NOMURAFX", "CITIFX", "COBAFX");
                SendIndicativeSubscriptions("EURUSD", "GBPUSD");
                SendLevelTwoStreamingSubscriptions();
            }
            else
            {
                Log.Warning("timed out waiting on session to be ready");
                foreach (IProviderProperties providerProperties in DefaultClient.Client.PriceSession.ProviderProperties())
                {
                    Log.Information(providerProperties.ToString());
                }
                DefaultClient.Client.Stop();
                return;
            }
            
            //Trading
            DefaultClient.Client.TradeSession.OrderSubmitEventHandler += OnOrderSubmitResponse;
            DefaultClient.Client.TradeSession.OrderQueryEventHandler += OnOrderSubmitResponse;
            SendTrades();
        }

        private void SendLevelOneStreamingSubscriptions(params string[] sources)
        {
            foreach (string source in sources)
            {
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneSpotStreamingSubject(Account, "EURUSD", source, "EUR", "1000000.11").CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneSpotStreamingSubject(Account, "EURGBP", source, "EUR", "1000000.00").CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneForwardStreamingSubject(Account, "EURUSD", source, "USD", "1000000.00", "", "20180530").CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneNdfStreamingSubject(Account, "USDKRW", source, "USD", "1000000.00", "1W", "").CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneSwapStreamingSubject(Account, "EURUSD", source, "EUR", "1000000.00", "1M", "", "1000000.00", "2M", "")
                        .CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneNdsStreamingSubject(Account, "CHFJPY", source, "CHF", "1000000.00", "BD", "20180815", "1000000.00", "BD",
                        "20180918").CreateSubject());
            }
        }

        private void SendLevelOneQuoteSubscriptions(params string[] sources)
        {
            foreach (string source in sources)
            {
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneSpotQuoteSubject(Account, "EURUSD", source, "EUR", "1000000.00").CreateSubject(), true);
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneSpotQuoteSubject(Account, "EURGBP", source, "EUR", "1000000.00").CreateSubject(), true);
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneForwardQuoteSubject(Account, "EURUSD", source, "USD", "1000000.00", "1Y", "").CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneNdfQuoteSubject(Account, "USDKRW", source, "USD", "1000000.00", "1Y", "").CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneSwapQuoteSubject(Account, "EURUSD", source, "EUR", "1000000.00", "1Y", "", "1000000.00", "2Y", "")
                        .CreateSubject());
                DefaultClient.Client.PriceSubscriber.Subscribe(
                    CommonSubjects.CreateLevelOneNdsQuoteSubject(Account, "CHFJPY", source, "CHF", "1000000.00", "6M", "", "1000000.00", "1Y", "")
                        .CreateSubject());
            }
        }

        private void SendLevelTwoStreamingSubscriptions()
        {
            DefaultClient.Client.PriceSubscriber.Subscribe(
                CommonSubjects.CreateLevelTwoSpotStreamingSubject(Account, "EURUSD", "EUR", "1000000.00")
                    .CreateSubject());
            DefaultClient.Client.PriceSubscriber.Subscribe(
                CommonSubjects.CreateLevelTwoForwardStreamingSubject(Account, "EURGBP", "EUR", "1000000.00",
                    "BD", "20181220").CreateSubject());
            DefaultClient.Client.PriceSubscriber.Subscribe(
                CommonSubjects.CreateLevelTwoNdfStreamingSubject(Account, "USDJPY", "USD", "1000000.00", "4M", "")
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

        private static void OnPriceUpdate(object source, PriceUpdateEvent priceUpdateEvent)
        {
            Log.Information("price update: " + priceUpdateEvent.Subject + " -> " + priceUpdateEvent.ChangedPriceFields);
        }

        private static void OnSubscriptionStatus(object source, SubscriptionStatusEvent subscriptionStatusEvent)
        {
            Log.Information("price status: " + subscriptionStatusEvent);
        }

        private static void OnProviderStatus(object sender, ProviderStatusEvent providerStatusEvent)
        {
            Log.Information("provider status: " +
                     providerStatusEvent.Name + " changed status from " +
                     providerStatusEvent.PreviousProviderStatus
                     + " to " + providerStatusEvent.ProviderStatus
                     + " because: " + providerStatusEvent.StatusReason);
        }

        private void SendTrades()
        {
            DefaultClient.Client.TradeSession.SubmitQuery("20190320-123811771_37291");
        }

        private static void OnOrderSubmitResponse(object sender, Order order)
        {
            Log.Information("Order Response: MessageId => {0}, OrderID => {1}, State => {2}, FullOrder => {3}", order.GetMessageId(),
                order.GetOrderTsId(), order.GetState(), order);
        }
    }
}