using System;
using System.Collections.Generic;

namespace TS.Pisa
{
    internal class MasterSession : ISession, ISubscriberSession
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly List<IProviderPlugin> _providerPlugins = new List<IProviderPlugin>();

        private readonly Dictionary<string, ISet<ISubscriber>> _subscriptions =
            new Dictionary<string, ISet<ISubscriber>>();

        public event EventHandler<PriceUpdateEventArgs> PriceUpdate;

        public void Start()
        {
            log.Info("MasterSession started");
            foreach (var providerPlugin in _providerPlugins)
            {
                providerPlugin.Start();
            }
        }

        public void Stop()
        {
            log.Info("MasterSession stopped");
            foreach (var providerPlugin in _providerPlugins)
            {
                providerPlugin.Stop();
            }
        }

        public void AddProviderPlugin(IProviderPlugin providerPlugin)
        {
            providerPlugin.PriceUpdate = PriceUpdate;
            _providerPlugins.Add(providerPlugin);
        }

        public void Subscribe(string subject)
        {
            log.Info("subscribe to " + subject);
            ISet<ISubscriber> subscription;
            if (!_subscriptions.TryGetValue(subject, out subscription))
            {
                subscription = new HashSet<ISubscriber>();
                _subscriptions[subject] = subscription;
            }
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
            log.Info("unsubscribe from " + subject);
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
        }

        public void ResubscribeAll()
        {
        }

        public void Close()
        {
        }
    }
}