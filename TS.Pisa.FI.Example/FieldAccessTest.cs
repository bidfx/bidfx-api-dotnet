using System;
using TS.Pisa.Plugin.Puffin;

namespace TS.Pisa.FI.Example
{
    public class FieldAccessExample
    {
        private readonly FixedIncomeSession _session;

        public FieldAccessExample(FixedIncomeSession session)
        {
            _session = session;
            session.OnPrice += OnPrice;
            session.OnStatus += OnStatus;
        }

        private static void OnPrice(object source, PriceEventArgs priceEvent)
        {
            var price = priceEvent.AllPriceFields;
            var bid = price.DoubleField(FieldName.Bid) ?? 0.0;
            var ask = price.DoubleField(FieldName.Ask) ?? 0.0;
            var spread = ask - bid;
            Console.WriteLine(priceEvent.Subject.Isin + " bid=" + bid + " ask=" + ask + " (spread=" + spread + ")");
        }

        public void OnStatus(object source, StatusEventArgs statusEvent)
        {
            Console.WriteLine(statusEvent.Subject.Isin + " " + statusEvent.Status + " - " + statusEvent.Reason);
        }

        private void SendSubscriptions()
        {
            foreach (var isin in System.IO.File.ReadLines("ISIN_list_all.txt"))
            {
                _session.Subscribe(new FixedIncomeSubject("SGC", isin.Trim()));
            }
        }

        public void Run()
        {
            _session.Start();
            SendSubscriptions();
        }
    }
}