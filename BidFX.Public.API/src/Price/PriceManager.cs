﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using BidFX.Public.API.Price.Plugin.Pixie;
using BidFX.Public.API.Price.Plugin.Puffin;
using BidFX.Public.API.Price.Tools;
using log4net;

namespace BidFX.Public.API.Price
{
    /// <summary>
    /// A pricing manager that provides access to market data from many provider plugin compoenets.
    /// </summary>
    internal class PriceManager : ISession, IBulkSubscriber
    {
#if DEBUG
private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
#else
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
#endif

        private readonly AtomicBoolean _running = new AtomicBoolean(false);
        private readonly List<IProviderPlugin> _providerPlugins = new List<IProviderPlugin>();
        private readonly SubscriptionSet _subscriptions = new SubscriptionSet();
        private readonly IApiEventHandler _inapiEventHandler;
        private readonly Thread _subscriptionRefreshThread;
        private readonly object _readyLock = new object();
        private const bool Tunnel = true; //Set to false if connecting to a local instance of a provider for testing.

        public static string Username { get; internal set; } //Delete when SubjectMutator is removed

        public TimeSpan SubscriptionRefreshInterval { get; set; }

//        public string Username { get; set; } //Uncomment when SubjectMutator is removed
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool DisableHostnameSslChecks { get; set; }
        public TimeSpan ReconnectInterval { get; set; }

        public event EventHandler<PriceUpdateEvent> PriceUpdateEventHandler;
        public event EventHandler<SubscriptionStatusEvent> SubscriptionStatusEventHandler;
        public event EventHandler<ProviderStatusEvent> ProviderStatusEventHandler;

        public PriceManager(string username) // remove this param when SubjectMutator is removed
        {
            Username = username; // remove this when SubjectMutator is removed
            ProviderStatusEventHandler += OnProviderStatus;
            _inapiEventHandler = new ApiEventDispatcher(this);
            _subscriptionRefreshThread = new Thread(RefreshStaleSubscriptionsLoop)
            {
                Name = "subscription-refresh",
                IsBackground = true
            };
        }

        public void Start()
        {
            AddPublicPuffinProvider();
            AddHighwayProvider();
            if (_running.CompareAndSet(false, true))
            {
                Log.Info("started");
                foreach (IProviderPlugin providerPlugin in _providerPlugins)
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
                Service = "static://puffin",
                DisableHostnameSslChecks = DisableHostnameSslChecks,
                ReconnectInterval = ReconnectInterval
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
                Service = "static://highway",
                DisableHostnameSslChecks = DisableHostnameSslChecks,
                ReconnectInterval = ReconnectInterval
            });
        }

        private void RefreshStaleSubscriptionsLoop()
        {
            while (_running.Value)
            {
                foreach (Subscription subscription in _subscriptions.StaleSubscriptions())
                {
                    RefreshSubscription(subscription, true);
                }

                Thread.Sleep(SubscriptionRefreshInterval);
            }
        }

        public void Stop()
        {
            if (_running.CompareAndSet(true, false))
            {
                Log.Info("stopping");
                _subscriptions.Clear();
                foreach (IProviderPlugin providerPlugin in _providerPlugins)
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
            get { return Running && _providerPlugins.All(pp => ProviderStatus.Ready == pp.ProviderStatus || ProviderStatus.Unauthorized == pp.ProviderStatus); }
        }

        public bool LoggedIn
        {
            get { return _providerPlugins.Any(pp => ProviderStatus.Ready == pp.ProviderStatus || ProviderStatus.TemporarilyDown == pp.ProviderStatus); }
        }

        private bool WaitUntilReady(TimeSpan timeout)
        {
            if (Ready)
            {
                return true;
            }

            lock (_readyLock)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                do
                {
                    TimeSpan remaining = timeout.Subtract(stopwatch.Elapsed);
                    if (remaining.CompareTo(TimeSpan.Zero) > 0 && Monitor.Wait(_readyLock, remaining))
                    {
                        continue;
                    }

                    return false;
                } while (!Ready);

                return true;
            }
        }

        public bool WaitUntilLoggedIn(TimeSpan timeout)
        {
            return WaitUntilReady(timeout) && LoggedIn;
        }

        public ICollection<IProviderProperties> ProviderProperties()
        {
            List<IProviderProperties> list = new List<IProviderProperties>();
            foreach (IProviderPlugin providerPlugin in _providerPlugins)
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
            if (_running.Value)
            {
                throw new IllegalStateException("add providers before starting the NAPIClient session");
            }

            providerPlugin.InapiEventHandler = _inapiEventHandler;
            _providerPlugins.Add(providerPlugin);
        }

        public void Subscribe(Subject.Subject subject, bool autoRefresh = false, bool refresh = false)
        {
            Log.Info("subscribe to " + subject);
            Subscription subscription = _subscriptions.Add(subject, autoRefresh);
            RefreshSubscription(subscription, refresh);
        }

        private void RefreshSubscription(Subscription subscription, bool refresh = true)
        {
            foreach (IProviderPlugin providerPlugin in _providerPlugins)
            {
                if (providerPlugin.IsSubjectCompatible(subscription.Subject))
                {
                    providerPlugin.Subscribe(subscription.Subject, subscription.AutoRefresh, refresh);
                }
            }
        }

        public void Unsubscribe(Subject.Subject subject)
        {
            Log.Info("unsubscribe from " + subject);
            _subscriptions.Remove(subject);
            foreach (IProviderPlugin providerPlugin in _providerPlugins)
            {
                if (providerPlugin.IsSubjectCompatible(subject))
                {
                    providerPlugin.Unsubscribe(subject);
                }
            }
        }

        public void UnsubscribeAll()
        {
            foreach (IProviderPlugin providerPlugin in _providerPlugins)
            foreach (Subscription subscription in _subscriptions.Clear())
            {
                if (providerPlugin.IsSubjectCompatible(subscription.Subject))
                {
                    providerPlugin.Unsubscribe(subscription.Subject);
                }
            }
        }

        public void ResubscribeAll()
        {
            List<Subject.Subject> subjects = _subscriptions.Subjects();
            Log.Info("resubscribing to all " + subjects.Count + " instruments");
            foreach (IProviderPlugin providerPlugin in _providerPlugins)
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

        private static void ResubscribeToAllOn(IProviderPlugin providerPlugin,
            IEnumerable<Subject.Subject> subscriptions)
        {
            List<Subject.Subject> subjects = subscriptions.Where(providerPlugin.IsSubjectCompatible).ToList();
            Log.Info("resubscribing to " + subjects.Count + " instruments on " + providerPlugin.Name);
            foreach (Subject.Subject subject in subjects)
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
                IProviderPlugin providerPlugin = ProviderPluginByName(providerStatusEvent.Name);
                if (providerPlugin == null)
                {
                    return;
                }

                if (ProviderStatus.Ready == providerStatusEvent.ProviderStatus)
                {
                    ResubscribeToAllOn(providerPlugin, _subscriptions.Subjects());
                }
                else
                {
                    string reason = ProviderStatusToSubscriptionReason(providerStatusEvent);
                    List<Subject.Subject> subjects = _subscriptions.Subjects().Where(providerPlugin.IsSubjectCompatible).ToList();
                    foreach (Subject.Subject subject in subjects)
                    {
                        _inapiEventHandler.OnSubscriptionStatus(subject, SubscriptionStatus.STALE, reason);
                    }
                }
            }
        }

        private IProviderPlugin ProviderPluginByName(string name)
        {
            foreach (IProviderPlugin providerPlugin in _providerPlugins)
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

        private class ApiEventDispatcher : IApiEventHandler
        {
            private readonly PriceManager _session;

            public ApiEventDispatcher(PriceManager session)
            {
                _session = session;
            }

            public void OnPriceUpdate(Subject.Subject subject, IPriceMap priceUpdate, bool replaceAllFields)
            {
                if (!_session._running.Value)
                {
                    return;
                }

                Subscription subscription = _session._subscriptions.GetSubscription(subject);
                if (subscription == null)
                {
                    return;
                }

                if (SubscriptionStatus.OK != subscription.SubscriptionStatus)
                {
                    OnSubscriptionStatus(subject, SubscriptionStatus.OK, "Received price update.");
                }
                
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
                if (!_session._running.Value)
                {
                    return;
                }

                Subscription subscription = _session._subscriptions.GetSubscription(subject);
                if (subscription == null)
                {
                    return;
                }

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
                if (!_session._running.Value)
                {
                    return;
                }

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
    }
}