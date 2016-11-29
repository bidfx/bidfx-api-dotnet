using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TS.Pisa.FI.Example
{
    public class SnapshotTimingExample
    {
        private readonly FixedIncomeSession _session;
        private readonly SubscriptionSet _pendingSubjects = new SubscriptionSet();
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
                    Console.WriteLine("complete " + _subscriptions + " price snapshots in "
                                      + _stopwatch.ElapsedMilliseconds + " milliseconds");
                    _session.Stop();
                }
            }
        }

        public void OnStatus(object source, StatusEventArgs statusEvent)
        {
            Console.WriteLine(statusEvent.Subject.Isin + " status " + statusEvent.Status + " - " + statusEvent.Reason);
        }

        private void SendSubscriptions()
        {
            Console.WriteLine("making price subscriptions");
            var stopwatch = Stopwatch.StartNew();
            foreach (var isin in System.IO.File.ReadLines("ISIN_list_5000.txt"))
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

    internal class SubscriptionSet
    {
        private readonly ISet<FixedIncomeSubject> _set = new HashSet<FixedIncomeSubject>();

        public int Count
        {
            get
            {
                lock (_set)
                {
                    return _set.Count;
                }
            }
        }

        public bool Remove(FixedIncomeSubject subject)
        {
            lock (_set)
            {
                return _set.Remove(subject);
            }
        }

        public void Add(FixedIncomeSubject subject)
        {
            lock (_set)
            {
                _set.Add(subject);
            }
        }
    }
}