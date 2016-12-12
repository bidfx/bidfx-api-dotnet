using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using TS.Pisa.Tools;

namespace TS.Pisa
{
    /// <summary>
    /// A master Pisa session that provides access to market data from many provider plugin compoenets.
    /// </summary>
    internal class MasterSession : ISession, IBulkSubscriber
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly AtomicBoolean _running = new AtomicBoolean(false);
        private readonly List<IProviderPlugin> _providerPlugins = new List<IProviderPlugin>();
        private readonly SubscriptionSet _subscriptions = new SubscriptionSet();
        private readonly IPisaEventHandler _pisaEventHandler;
        private readonly Thread _subscriptionRefreshThread;
        private readonly object _readyLock = new object();

        public TimeSpan SubscriptionRefreshInterval { get; set; }

        public event EventHandler<PriceUpdateEventArgs> PriceUpdate;
        public event EventHandler<SubscriptionStatusEventArgs> PriceStatus;
        public event EventHandler<ProviderPluginEventArgs> ProviderPlugin;

        public MasterSession()
        {
            SubscriptionRefreshInterval = TimeSpan.FromMinutes(5);
            ProviderPlugin += OnProviderStatusEvent;
            _pisaEventHandler = new PisaEventDispatcher(this);
            _subscriptionRefreshThread = new Thread(RefreshStaleSubscriptionsLoop)
            {
                Name = "subscription-refresh",
                IsBackground = true
            };
        }

        public void Start()
        {
            if (_running.CompareAndSet(false, true))
            {
                Log.Info("started");
                foreach (var providerPlugin in _providerPlugins)
                {
                    providerPlugin.Start();
                }
                _subscriptionRefreshThread.Start();
                Thread.Sleep(500); // give the plugins a chance to connect
            }
        }

        private void RefreshStaleSubscriptionsLoop()
        {
            while (_running.Value)
            {
                Thread.Sleep(SubscriptionRefreshInterval);
                foreach (var subject in _subscriptions.StaleSubjects())
                {
                    RefreshSubscription(subject);
                }
            }
        }

        public void Stop()
        {
            if (_running.CompareAndSet(true, false))
            {
                Log.Info("stopping");
                _subscriptions.Clear();
                foreach (var providerPlugin in _providerPlugins)
                {
                    providerPlugin.Stop();
                }
            }
        }

        public bool Running
        {
            get { return _running.Value; }
        }

        public bool Ready
        {
            get { return Running && _providerPlugins.All(pp => ProviderStatus.Ready == pp.ProviderStatus); }
        }

        public bool WaitUntilReady(TimeSpan timeout)
        {
            if (Ready) return true;
            lock (_readyLock)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                do
                {
                    var remaining = timeout.Subtract(stopwatch.Elapsed);
                    if (remaining.CompareTo(TimeSpan.Zero) > 0 && Monitor.Wait(_readyLock, remaining)) continue;
                    return false;
                } while (!Ready);
                return true;
            }
        }

        public ICollection<IProviderProperties> ProviderProperties()
        {
            var list = new List<IProviderProperties>();
            foreach (var providerPlugin in _providerPlugins)
            {
                list.Add(new ProviderProperties
                {
                    Name = providerPlugin.Name,
                    ProviderStatus = providerPlugin.ProviderStatus,
                    ProviderStatusText = providerPlugin.ProviderStatusText
                });
            }
            return list;
        }

        public void AddProviderPlugin(IProviderPlugin providerPlugin)
        {
            if (_running.Value) throw new IllegalStateException("add providers before starting the Pisa session");
            providerPlugin.PisaEventHandler = _pisaEventHandler;
            _providerPlugins.Add(providerPlugin);
        }

        public void Subscribe(string subject)
        {
            Log.Info("subscribe to " + subject);
            _subscriptions.Add(subject);
            RefreshSubscription(subject);
        }

        private void RefreshSubscription(string subject)
        {
            foreach (var providerPlugin in _providerPlugins)
            {
                if (providerPlugin.IsSubjectCompatible(subject))
                {
                    providerPlugin.Subscribe(subject);
                }
            }
        }

        public void Unsubscribe(string subject)
        {
            Log.Info("unsubscribe from " + subject);
            _subscriptions.Remove(subject);
            foreach (var providerPlugin in _providerPlugins)
            {
                if (providerPlugin.IsSubjectCompatible(subject))
                {
                    providerPlugin.Unsubscribe(subject);
                }
            }
        }

        public void UnsubscribeAll()
        {
            foreach (var providerPlugin in _providerPlugins)
            {
                foreach (var subject in _subscriptions.Clear())
                {
                    if (providerPlugin.IsSubjectCompatible(subject))
                    {
                        providerPlugin.Unsubscribe(subject);
                    }
                }
            }
        }

        public void ResubscribeAll()
        {
            var subjects = _subscriptions.Subjects();
            Log.Info("resubscribing to all " + subjects.Count + " instruments");
            foreach (var providerPlugin in _providerPlugins)
            {
                if (ProviderStatus.Ready.Equals(providerPlugin.ProviderStatus))
                {
                    ResubscribeToAllOn(providerPlugin, subjects);
                }
                else
                {
                    Log.Info("skip resubscriptions on " + providerPlugin.ProviderStatus + " " + providerPlugin.Name);
                }
            }
        }

        private static void ResubscribeToAllOn(IProviderPlugin providerPlugin, IEnumerable<string> subscriptions)
        {
            var subjects = subscriptions.Where(providerPlugin.IsSubjectCompatible).ToList();
            Log.Info("resubscribing to " + subjects.Count + " instruments on " + providerPlugin.Name);
            foreach (var subject in subjects)
            {
                providerPlugin.Subscribe(subject);
            }
        }

        private void OnProviderStatusEvent(object sender, ProviderPluginEventArgs evt)
        {
            Log.Info("received " + evt);
            NotifyProviderStatusChange();
            if (ProviderStatus.Ready.Equals(evt.ProviderStatus))
            {
                ResubscribeToAllOn(evt.Provider, _subscriptions.Subjects());
            }
            else if (ProviderStatus.TemporarilyDown.Equals(evt.ProviderStatus))
            {
                var subjects = _subscriptions.Subjects().Where(evt.Provider.IsSubjectCompatible).ToList();
                foreach (var subject in subjects)
                {
                    _pisaEventHandler.OnStatusEvent(subject, SubscriptionStatus.STALE, "Puffin connection is down");
                }
            }
        }

        private void NotifyProviderStatusChange()
        {
            lock (_readyLock)
            {
                Monitor.PulseAll(_readyLock);
            }
        }

        private class PisaEventDispatcher : IPisaEventHandler
        {
            private readonly MasterSession _session;

            public PisaEventDispatcher(MasterSession session)
            {
                _session = session;
            }

            public void OnPriceEvent(string subject, IPriceMap priceUpdate, bool replaceAllFields)
            {
                if (!_session._running.Value) return;
                var subscription = _session._subscriptions.GetSubscription(subject);
                if (subscription == null) return;
                subscription.AllPriceFields.MergedPriceMap(priceUpdate, replaceAllFields);
                if (_session.PriceUpdate != null)
                {
                    _session.PriceUpdate(this, new PriceUpdateEventArgs
                    {
                        Subject = subject,
                        AllPriceFields = subscription.AllPriceFields,
                        ChangedPriceFields = priceUpdate
                    });
                }
            }

            public void OnStatusEvent(string subject, SubscriptionStatus status, string reason)
            {
                if (!_session._running.Value) return;
                var subscription = _session._subscriptions.GetSubscription(subject);
                if (subscription == null) return;
                subscription.SubscriptionStatus = status;
                subscription.AllPriceFields.Clear();
                if (_session.PriceStatus != null)
                {
                    _session.PriceStatus(this, new SubscriptionStatusEventArgs
                    {
                        Subject = subject,
                        SubscriptionStatus = status,
                        Reason = reason
                    });
                }
            }

            public void OnProviderEvent(IProviderPlugin providerPlugin)
            {
                if (!_session._running.Value) return;
                if (_session.ProviderPlugin != null)
                {
                    _session.ProviderPlugin(this, new ProviderPluginEventArgs
                    {
                        Provider = providerPlugin,
                        ProviderStatus = providerPlugin.ProviderStatus,
                        Reason = providerPlugin.ProviderStatusText
                    });
                }
            }
        }

        private class SubscriptionSet
        {
            private readonly Dictionary<string, Subscription> _map = new Dictionary<string, Subscription>();

            public void Add(string subject)
            {
                lock (_map)
                {
                    if (!_map.ContainsKey(subject))
                    {
                        _map[subject] = new Subscription();
                    }
                }
            }

            public void Remove(string subject)
            {
                lock (_map)
                {
                    if (_map.ContainsKey(subject))
                    {
                        _map.Remove(subject);
                    }
                }
            }

            public IEnumerable<string> Clear()
            {
                lock (_map)
                {
                    var copy = new List<string>(_map.Keys);
                    _map.Clear();
                    return copy;
                }
            }

            public List<string> Subjects()
            {
                lock (_map)
                {
                    return new List<string>(_map.Keys);
                }
            }

            public IEnumerable<string> StaleSubjects()
            {
                lock (_map)
                {
                    return (from pair in _map
                        where !SubscriptionStatus.OK.Equals(pair.Value.SubscriptionStatus)
                        select pair.Key).ToList();
                }
            }

            public Subscription GetSubscription(string subject)
            {
                lock (_map)
                {
                    Subscription subscription;
                    _map.TryGetValue(subject, out subscription);
                    return subscription;
                }
            }

            internal class Subscription
            {
                public SubscriptionStatus SubscriptionStatus { get; set; }
                public PriceMap AllPriceFields { get; private set; }

                public Subscription()
                {
                    SubscriptionStatus = SubscriptionStatus.PENDING;
                    AllPriceFields = new PriceMap();
                }
            }
        }
    }
}