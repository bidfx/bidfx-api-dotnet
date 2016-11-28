using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TS.Pisa.FI.Example
{
    public class SnapshotTimingExample
    {
        private readonly FixedIncomeSession _session;
        private readonly HashSet<FixedIncomeSubject> _pendingSubjects = new HashSet<FixedIncomeSubject>();
        private int _subscriptions;
        private Stopwatch _stopwatch;


        public SnapshotTimingExample(FixedIncomeSession session)
        {
            _session = session;
            session.OnPrice += OnPrice;
            session.OnStatus += OnStatus;
        }

        private void OnPrice(object source, PriceEventArgs priceEvent)
        {
            if (_pendingSubjects.Remove(priceEvent.Subject))
            {
                Console.Write("\rprice snapshots: " + (_subscriptions - _pendingSubjects.Count));
                if (_pendingSubjects.Count == 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("complete price snapshots in "
                                      + _stopwatch.ElapsedMilliseconds + " milliseconds");
                    _session.Stop();
                }
            }
        }

        public void OnStatus(object source, StatusEventArgs statusEvent)
        {
            Console.WriteLine("status received for " + statusEvent.Status + " - " + statusEvent.Subject);
        }

        private void SendSubscriptions()
        {
            Console.WriteLine("making price subscriptions");
            var stopwatch = Stopwatch.StartNew();
            foreach (var isin in System.IO.File.ReadLines("ISIN_list_all.txt"))
            {
                var subject = new FixedIncomeSubject("SGC", isin.Trim());
                _pendingSubjects.Add(subject);
                _session.Subscribe(subject);
                _subscriptions++;
            }
            Console.WriteLine("subscribed to " + _subscriptions + " instruments in "
                              + stopwatch.ElapsedMilliseconds + " milliseconds");
        }

        public void Run()
        {
            _session.Start();
            SendSubscriptions();
            _stopwatch = Stopwatch.StartNew();
        }
    }
}