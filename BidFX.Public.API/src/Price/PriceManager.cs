using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using BidFX.Public.API.Price.Plugin.Pixie;
using BidFX.Public.API.Price.Plugin.Puffin;
using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Price
{
    /// <summary>
    /// A master NAPIClient session that provides access to market data from many provider plugin compoenets.
    /// </summary>
    public class PriceManager : ISession, IBulkSubscriber
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly AtomicBoolean _running = new AtomicBoolean(false);
        private readonly List<IProviderPlugin> _providerPlugins = new List<IProviderPlugin>();
        private readonly SubscriptionSet _subscriptions = new SubscriptionSet();
        private readonly IApiEventHandler _inapiEventHandler;
        private readonly Thread _subscriptionRefreshThread;
        private readonly object _readyLock = new object();

        public TimeSpan SubscriptionRefreshInterval { get; set; }
        public string Username { get; set; }
        public string Password { get; set;  }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool Tunnel { get; set; }

        public event EventHandler<PriceUpdateEvent> PriceUpdateEventHandler;
        public event EventHandler<SubscriptionStatusEvent> SubscriptionStatusEventHandler;
        public event EventHandler<ProviderStatusEvent> ProviderStatusEventHandler;

        public PriceManager()
        {
            SubscriptionRefreshInterval = TimeSpan.FromMinutes(5);
            ProviderStatusEventHandler += OnProviderStatus;
            _inapiEventHandler = new INAPIEventDispatcher(this);
            _subscriptionRefreshThread = new Thread(RefreshStaleSubscriptionsLoop)
            {
                Name = "subscription-refresh",
                IsBackground = true
            };
        }

        public void Start()
        {
            AddPublicPuffinProvider();
//            AddHighwayProvider();
            if (_running.CompareAndSet(false, true))
            {
                Log.Info("started");
                foreach (var providerPlugin in _providerPlugins)
                {
                    providerPlugin.Start();
                }
                _subscriptionRefreshThread.Start();
            }
        }

        private void AddPublicPuffinProvider()
        {
            AddProviderPlugin(new PuffinProviderPlugin
            {
                Host = Host,
                Password = Password,
                Username = Username,
                Port = Port,
                Tunnel = Tunnel,
                Service = "static://puffin"
            });
        }

        private void AddHighwayProvider()
        {
            AddProviderPlugin(new PixieProviderPlugin
            {
                Host = Host,
                Password = Password,
                Username = Username,
                Port = Port,
                Tunnel = Tunnel,
                Service = "static://highway"
            });
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
                    StatusReason = providerPlugin.StatusReason
                });
            }
            return list;
        }

        private void AddProviderPlugin(IProviderPlugin providerPlugin)
        {
            if (_running.Value) throw new IllegalStateException("add providers before starting the NAPIClient session");
            providerPlugin.InapiEventHandler = _inapiEventHandler;
            _providerPlugins.Add(providerPlugin);
        }

        public void Subscribe(Subject.Subject subject, bool refresh = false)
        {
            Log.Info("subscribe to " + subject);
            _subscriptions.Add(subject);
            RefreshSubscription(subject, refresh);
        }

        private void RefreshSubscription(Subject.Subject subject, bool refresh = true)
        {
            foreach (var providerPlugin in _providerPlugins)
            {
                if (providerPlugin.IsSubjectCompatible(subject))
                {
                    providerPlugin.Subscribe(subject, refresh);
                }
            }
        }

        public void Unsubscribe(Subject.Subject subject)
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

        private static void ResubscribeToAllOn(IProviderPlugin providerPlugin, IEnumerable<Subject.Subject> subscriptions)
        {
            var subjects = subscriptions.Where(providerPlugin.IsSubjectCompatible).ToList();
            Log.Info("resubscribing to " + subjects.Count + " instruments on " + providerPlugin.Name);
            foreach (var subject in subjects)
            {
                providerPlugin.Subscribe(subject);
            }
        }

        private void OnProviderStatus(object sender, ProviderStatusEvent providerStatusEvent)
        {
            Log.Info("received " + providerStatusEvent);
            NotifyProviderStatusChange();
            if (providerStatusEvent.ProviderStatus != providerStatusEvent.PreviousProviderStatus)
            {
                var providerPlugin = ProviderPluginByName(providerStatusEvent.Name);
                if (providerPlugin == null) return;
                if (ProviderStatus.Ready == providerStatusEvent.ProviderStatus)
                {
                    ResubscribeToAllOn(providerPlugin, _subscriptions.Subjects());
                }
                else
                {
                    var reason = ProviderStatusToSubscriptionReason(providerStatusEvent);
                    var subjects = _subscriptions.Subjects().Where(providerPlugin.IsSubjectCompatible).ToList();
                    foreach (var subject in subjects)
                    {
                        _inapiEventHandler.OnSubscriptionStatus(subject, SubscriptionStatus.STALE, reason);
                    }
                }
            }
        }

        private IProviderPlugin ProviderPluginByName(string name)
        {
            foreach (var providerPlugin in _providerPlugins)
            {
                if (name.Equals(providerPlugin.Name))
                {
                    return providerPlugin;
                }
            }
            Log.Error("cannot find provider plugin with name \"" + name + '"');
            return null;
        }

        private static string ProviderStatusToSubscriptionReason(IProviderProperties properties)
        {
            switch (properties.ProviderStatus)
            {
                case ProviderStatus.TemporarilyDown:
                    return "Puffin connection is down";
                case ProviderStatus.ScheduledDowntime:
                    return "Puffin feed has scheduled downtime";
                case ProviderStatus.Unavailable:
                    return "Puffin feed is unavailable";
                case ProviderStatus.Invalid:
                    return "Puffin provider is invalid";
                case ProviderStatus.Closed:
                    return "Puffin provider is closed";
                default:
                    return properties.StatusReason;
            }
        }

        private void NotifyProviderStatusChange()
        {
            lock (_readyLock)
            {
                Monitor.PulseAll(_readyLock);
            }
        }

        private class INAPIEventDispatcher : IApiEventHandler
        {
            private readonly PriceManager _session;

            public INAPIEventDispatcher(PriceManager session)
            {
                _session = session;
            }

            public void OnPriceUpdate(Subject.Subject subject, IPriceMap priceUpdate, bool replaceAllFields)
            {
                if (!_session._running.Value) return;
                var subscription = _session._subscriptions.GetSubscription(subject);
                if (subscription == null) return;
                subscription.AllPriceFields.MergedPriceMap(priceUpdate, replaceAllFields);
                if (_session.PriceUpdateEventHandler != null)
                {
                    _session.PriceUpdateEventHandler(this, new PriceUpdateEvent
                    {
                        Subject = subject,
                        AllPriceFields = subscription.AllPriceFields,
                        ChangedPriceFields = priceUpdate
                    });
                }
            }

            public void OnSubscriptionStatus(Subject.Subject subject, SubscriptionStatus status, string reason)
            {
                if (!_session._running.Value) return;
                var subscription = _session._subscriptions.GetSubscription(subject);
                if (subscription == null) return;
                subscription.SubscriptionStatus = status;
                subscription.AllPriceFields.Clear();
                if (_session.SubscriptionStatusEventHandler != null)
                {
                    _session.SubscriptionStatusEventHandler(this, new SubscriptionStatusEvent
                    {
                        Subject = subject,
                        SubscriptionStatus = status,
                        StatusReason = reason
                    });
                }
            }

            public void OnProviderStatus(IProviderPlugin providerPlugin, ProviderStatus previousStatus)
            {
                if (!_session._running.Value) return;
                if (_session.ProviderStatusEventHandler != null)
                {
                    _session.ProviderStatusEventHandler(this, new ProviderStatusEvent
                    {
                        Name = providerPlugin.Name,
                        PreviousProviderStatus = previousStatus,
                        ProviderStatus = providerPlugin.ProviderStatus,
                        StatusReason = providerPlugin.StatusReason
                    });
                }
            }
        }

        private class SubscriptionSet
        {
            private readonly Dictionary<Subject.Subject, Subscription> _map = new Dictionary<Subject.Subject, Subscription>();

            public void Add(Subject.Subject subject)
            {
                lock (_map)
                {
                    if (!_map.ContainsKey(subject))
                    {
                        _map[subject] = new Subscription();
                    }
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

            public IEnumerable<Subject.Subject> Clear()
            {
                lock (_map)
                {
                    var copy = new List<Subject.Subject>(_map.Keys);
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

            public IEnumerable<Subject.Subject> StaleSubjects()
            {
                lock (_map)
                {
                    return (from pair in _map
                        where SubscriptionStatus.OK != pair.Value.SubscriptionStatus
                        select pair.Key).ToList();
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