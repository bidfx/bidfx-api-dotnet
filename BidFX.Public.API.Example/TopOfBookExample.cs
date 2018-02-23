using System;
using System.Linq;
using System.Reflection;
using System.Text;
using BidFX.Public.API.Price;
using BidFX.Public.API.Price.Subject;
using log4net;

namespace BidFX.Public.API.Example
{
    public class TopOfBookExample
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const string USERNAME = "";
        private const string PASSWORD = "";
        private const string ACCOUNT = "FX_ACCT";
        private readonly Client _client;

        public static void Main2(string[] args)
        {
            new TopOfBookExample().Run();
        }

        private void Run()
        {
            Client _client = DefaultClient.Client;
            _client.Host = "ny-tunnel.uatprod.tradingscreen.com";
            _client.Username = USERNAME;
            _client.Password = PASSWORD;
            _client.PriceSession.PriceUpdateEventHandler += OnPriceUpdate;

            if (_client.PriceSession.WaitUntilLoggedIn(TimeSpan.FromSeconds(15)))
            {
                Subject depthSubject = CommonSubjects
                    .CreateLevelTwoSpotStreamingSubject(ACCOUNT, "GBPUSD", "GBP", "1000000.00").CreateSubject();
                _client.PriceSubscriber.Subscribe(depthSubject);
            }
            else
            {
                if (!_client.PriceSession.LoggedIn)
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

        private void OnPriceUpdate(object source, PriceUpdateEvent priceEvent)
        {
            if (priceEvent.AllPriceFields.FieldNames.Contains("BidLevels") &&
                priceEvent.AllPriceFields.FieldNames.Contains("AskLevels"))
            {
                StringBuilder stringBuilder = new StringBuilder();
                int bids = priceEvent.AllPriceFields.IntField("BidLevels") ?? 0;
                int asks = priceEvent.AllPriceFields.IntField("AskLevels") ?? 0;
                for (int i = 1; i <= Math.Min(Math.Max(bids, asks), 3); i++)
                {
                    string bidfirm = priceEvent.AllPriceFields.StringField("BidFirm" + i) ?? "";
                    decimal bid = priceEvent.AllPriceFields.DecimalField("Bid" + i) ?? 0;
                    decimal ask = priceEvent.AllPriceFields.DecimalField("Ask" + i) ?? 0;
                    string askfirm = priceEvent.AllPriceFields.StringField("AskFirm" + i) ?? "";
                    stringBuilder.AppendLine();
                    stringBuilder.Append(bidfirm.PadLeft(6));
                    stringBuilder.Append(" ");
                    stringBuilder.Append(bid.ToString("#.00000").PadLeft(8));
                    stringBuilder.Append("|");
                    stringBuilder.Append(ask.ToString("#.00000").PadRight(8));
                    stringBuilder.Append(" ");
                    stringBuilder.Append(askfirm.PadRight(8));
                }

                Log.Info(stringBuilder.ToString());
            }
        }
    }
}