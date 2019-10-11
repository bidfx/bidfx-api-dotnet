/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

using System.Collections.Generic;
using System.Linq;
using BidFX.Public.API.Price.Subject;

namespace BidFX.Public.API.Price
{
    internal class SubscriptionSet
    {
        private readonly Dictionary<Subject.Subject, Subscription> _map =
            new Dictionary<Subject.Subject, Subscription>();

        private int _levelTwoSubjects = 0;
        private int _levelOneSubjects = 0;

        public Subscription Add(Subject.Subject subject, bool autoRefresh)
        {
            lock (_map)
            {
                if (!_map.ContainsKey(subject))
                {
                    _map[subject] = new Subscription(subject, autoRefresh);
                    string level = subject.GetComponent(SubjectComponentName.Level);
                    if (level != null && level.Equals("2"))
                    {
                        _levelTwoSubjects++;
                    }
                    if (level != null && level.Equals("1"))
                    {
                        _levelOneSubjects++;
                    }
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
                    string level = subject.GetComponent(SubjectComponentName.Level);
                    if (level != null && level.Equals("2"))
                    {
                        _levelTwoSubjects--;
                    }
                    if (level != null && level.Equals("1"))
                    {
                        _levelOneSubjects--;
                    }
                }
            }
        }

        public IEnumerable<Subscription> Clear()
        {
            lock (_map)
            {
                List<Subscription> copy = new List<Subscription>(_map.Values);
                _levelOneSubjects = 0;
                _levelTwoSubjects = 0;
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

        public int LevelTwoSubjects()
        {
            return _levelTwoSubjects;
        }

        public int LevelOneSubjects()
        {
            return _levelOneSubjects;
        }

        public IEnumerable<Subscription> StaleSubscriptions()
        {
            lock (_map)
            {
                return (from subscription in _map.Values
                    where SubscriptionStatus.OK != subscription.SubscriptionStatus &&
                          !CommonComponents.Quote.Equals(subscription.Subject.GetComponent(SubjectComponentName.RequestFor))
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