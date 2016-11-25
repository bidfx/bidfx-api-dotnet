using System;
using TS.Pisa.Plugin.Puffin;

namespace TS.Pisa.FI.Example
{
    internal class PisaFIExample
    {
        private const string SocGenVenue = "SGC";

        public static void Main(string[] args)
        {
            var session = new FixedIncomeSession
            {
                Host = "ny-tunnel.uatdev.tradingscreen.com",
                Username = "axaapitest",
                Password = "B3CarefulWithThatAXAEug3n3!"
            };
            session.OnPrice += OnPrice;
            session.OnStatus += OnStatus;
            session.Start();
            SendSubscriptions(session);
        }

        private static void OnPrice(object source, PriceEventArgs priceEvent)
        {
            var price = priceEvent.AllPriceFields;
            var bid = price.DoubleField(FieldName.Bid) ?? 0.0;
            var ask = price.DoubleField(FieldName.Ask) ?? 0.0;
            var spread = ask - bid;
            Console.WriteLine(priceEvent.Subject.Isin + " bid=" + bid + " ask=" + ask + " (spread=" + spread + ")");
        }

        private static void OnStatus(object source, StatusEventArgs statusEvent)
        {
            Console.WriteLine(statusEvent.Subject.Isin + " " + statusEvent.Status + " - " + statusEvent.Reason);
        }

        private static void SendSubscriptions(FixedIncomeSession session)
        {
            foreach (var isin in System.IO.File.ReadLines("ISIN_list_10.txt"))
            {
                session.Subscribe(new FixedIncomeSubject(SocGenVenue, isin));
            }
        }
    }
}