using System;

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

        private static void OnPrice(object source, PriceUpdateEventArgs priceEvent)
        {
            var price = priceEvent.AllPriceFields;
            var bid = price.DecimalField(FieldName.Bid) ?? 0.0m;
            var ask = price.DecimalField(FieldName.Ask) ?? 0.0m;
            var spread = ask - bid;
            Console.WriteLine(priceEvent.Subject.Isin + " bid=" + bid + " ask=" + ask + " (spread=" + spread + ")");
        }

        public void OnStatus(object source, SubscriptionStatusEventArgs statusEvent)
        {
            Console.WriteLine(statusEvent.Subject.Isin + " " + statusEvent.Status + " - " + statusEvent.Reason);
        }

        private void SendSubscriptions()
        {
            foreach (var isin in System.IO.File.ReadLines("../../ISIN_list_5000.txt"))
            {
                try
                {
                    _session.Subscribe(new FixedIncomeSubject("SGC", isin.Trim()));
                }
                catch (IllegalSubjectException e)
                {
                    Console.WriteLine("cannot subscribe to isin="+isin,e);
                }
            }
        }

        public void Run()
        {
            _session.Start();
            SendSubscriptions();
        }
    }
}