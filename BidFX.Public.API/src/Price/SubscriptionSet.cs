using System.Collections.Generic;
using System.Linq;

namespace BidFX.Public.API.Price
{
    internal class SubscriptionSet
    {
        private readonly Dictionary<Subject.Subject, Subscription> _map =
            new Dictionary<Subject.Subject, Subscription>();

        public Subscription Add(Subject.Subject subject, bool autoRefresh)
        {
            lock (_map)
            {
                if (!_map.ContainsKey(subject))
                {
                    _map[subject] = new Subscription(subject, autoRefresh);
                }

                return _map[subject];
            }
        }

        public void Remove(Subject.Subject subject)
        {
            lock (_map)
            {
                if (_map.ContainsKey(subject))
                {
                    _map.Remove(subject);
                }
            }
        }

        public IEnumerable<Subscription> Clear()
        {
            lock (_map)
            {
                var copy = new List<Subscription>(_map.Values);
                _map.Clear();
                return copy;
            }
        }

        public List<Subject.Subject> Subjects()
        {
            lock (_map)
            {
                return new List<Subject.Subject>(_map.Keys);
            }
        }

        public IEnumerable<Subscription> StaleSubscriptions()
        {
            lock (_map)
            {
                return (from subscription in _map.Values
                    where SubscriptionStatus.OK != subscription.SubscriptionStatus
                    select subscription).ToList();
            }
        }

        public Subscription GetSubscription(Subject.Subject subject)
        {
            lock (_map)
            {
                Subscription subscription;
                _map.TryGetValue(subject, out subscription);
                return subscription;
            }
        }
    }
}