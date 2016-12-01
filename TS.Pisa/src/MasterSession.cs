using System;
using System.Collections.Generic;
using System.Linq;
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

        public event EventHandler<PriceUpdateEventArgs> PriceUpdate;
        public event EventHandler<SubscriptionStatusEventArgs> PriceStatus;
        public event EventHandler<ProviderPluginEventArgs> ProviderPlugin;
        private readonly IPisaEventHandler _pisaEventHandler;

        public MasterSession()
        {
            ProviderPlugin += OnProviderStatusEvent;
            _pisaEventHandler = new PisaEventDispatcher(this);
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
            }
        }

        public void Stop()
        {
            if (_running.CompareAndSet(true, false))
            {
                Log.Info("stopping");
                foreach (var providerPlugin in _providerPlugins)
                {
                    providerPlugin.Stop();
                }
            }
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
                foreach (var subject in _subscriptions.CopyAndClear())
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
            foreach (var providerPlugin in _providerPlugins)
            {
                ResubscribeToAllOn(providerPlugin, _subscriptions.Copy());
            }
        }

        private static void ResubscribeToAllOn(IProviderPlugin providerPlugin, IEnumerable<string> subscriptions)
        {
            var subset = subscriptions.Where(providerPlugin.IsSubjectCompatible).ToList();
            Log.Info("resubscribing to " + subset.Count + " instruments on " + providerPlugin.Name);
            foreach (var subject in subset)
            {
                providerPlugin.Subscribe(subject);
            }
        }

        private void OnProviderStatusEvent(object sender, ProviderPluginEventArgs evt)
        {
            Log.Info("received " + evt);
            if (ProviderStatus.Ready.Equals(evt.ProviderStatus))
            {
                ResubscribeToAllOn(evt.Provider, _subscriptions.Copy());
            }
        }

        private class PisaEventDispatcher : IPisaEventHandler
        {
            private readonly MasterSession _session;

            public PisaEventDispatcher(MasterSession session)
            {
                _session = session;
            }

            public void OnPriceEvent(PriceUpdateEventArgs updateEvent)
            {
                if (_session.PriceUpdate != null)
                {
                    _session.PriceUpdate(this, updateEvent);
                }
            }

            public void OnStatusEvent(SubscriptionStatusEventArgs statusEvent)
            {
                if (_session.PriceStatus != null)
                {
                    _session.PriceStatus(this, statusEvent);
                }
            }

            public void OnProviderEvent(ProviderPluginEventArgs providerEvent)
            {
                if (_session.ProviderPlugin != null)
                {
                    _session.ProviderPlugin(this, providerEvent);
                }
            }
        }

        private class SubscriptionSet
        {
            private readonly HashSet<string> _set = new HashSet<string>();

            public void Add(string subject)
            {
                lock (_set)
                {
                    _set.Add(subject);
                }
            }

            public void Remove(string subject)
            {
                lock (_set)
                {
                    _set.Remove(subject);
                }
            }

            public IEnumerable<string> CopyAndClear()
            {
                lock (_set)
                {
                    var copy = new List<string>(_set);
                    _set.Clear();
                    return copy;
                }
            }

            public List<string> Copy()
            {
                lock (_set)
                {
                    return new List<string>(_set);
                }
            }
        }
    }
}