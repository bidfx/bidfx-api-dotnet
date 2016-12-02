using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

        private void OnPrice(object source, PriceUpdateEventArgs priceEvent)
        {
            _pendingSubjects.HasPrice(priceEvent.Subject);
            DisplayResults();
        }

        public void OnStatus(object source, SubscriptionStatusEventArgs status)
        {
            if (!"Puffin connection is down".Equals(status.Reason))
            {
                _pendingSubjects.HasStatus(status.Subject);
                DisplayResults();
            }
        }

        private void DisplayResults()
        {
            var pendingUpdates = _pendingSubjects.CountPending;
            Console.Write("\rpending price snapshots: " + pendingUpdates);
            if (pendingUpdates == 0)
            {
                Console.WriteLine();
                Console.WriteLine("complete " + _subscriptions + " price snapshots in "
                                  + _stopwatch.ElapsedMilliseconds + " milliseconds: "
                                  + _pendingSubjects.CountPrices + " OK, "
                                  + _pendingSubjects.CountStatuses + " with status."
                );
                _session.Stop();
            }
        }


        private void SendSubscriptions()
        {
            Console.WriteLine("making price subscriptions");
            foreach (var isin in System.IO.File.ReadLines("../../ISIN_list_5000.txt"))
            {
                try
                {
                    var subject = new FixedIncomeSubject("SGC", isin.Trim());
                    _pendingSubjects.Add(subject);
                    _session.Subscribe(subject);
                    _subscriptions++;
                }
                catch (IllegalSubjectException e)
                {
                    Console.WriteLine("cannot subscribe to isin=" + isin, e);
                }
            }
            Console.WriteLine("subscribed to " + _subscriptions + " instruments");
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
        private readonly Dictionary<FixedIncomeSubject, int> _map = new Dictionary<FixedIncomeSubject, int>();

        public int CountPending
        {
            get { return CountWithType(0); }
        }

        public int CountPrices
        {
            get { return CountWithType(1); }
        }

        public int CountStatuses
        {
            get { return CountWithType(2); }
        }

        public void HasPrice(FixedIncomeSubject subject)
        {
            SetType(subject, 1);
        }

        public void HasStatus(FixedIncomeSubject subject)
        {
            SetType(subject, 2);
        }

        public void Add(FixedIncomeSubject subject)
        {
            SetType(subject, 0);
        }

        private int CountWithType(int t)
        {
            lock (_map)
            {
                return _map.Values.Count(state => state == t);
            }
        }

        private void SetType(FixedIncomeSubject subject, int t)
        {
            lock (_map)
            {
                _map[subject] = t;
            }
        }
    }
}