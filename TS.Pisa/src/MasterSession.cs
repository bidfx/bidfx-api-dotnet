using System;
using System.Collections.Generic;

namespace TS.Pisa
{
    internal class MasterSession : ISession, ISubscriberSession
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly List<IProviderPlugin> _providerPlugins = new List<IProviderPlugin>();
        private readonly HashSet<string> _subscriptions = new HashSet<string>();

        public event EventHandler<PriceUpdateEventArgs> PriceUpdate;
        public event EventHandler<PriceStatusEventArgs> PriceStatus;

        public void Start()
        {
            Log.Info("MasterSession started");
            foreach (var providerPlugin in _providerPlugins)
            {
                providerPlugin.Start();
            }
        }

        public void Stop()
        {
            Log.Info("MasterSession stopped");
            foreach (var providerPlugin in _providerPlugins)
            {
                providerPlugin.Stop();
            }
        }

        public void AddProviderPlugin(IProviderPlugin providerPlugin)
        {
            providerPlugin.PriceUpdate = PriceUpdate;
            providerPlugin.PriceStatus = PriceStatus;
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
            foreach (var subject in _subscriptions)
            {
                Unsubscribe(subject);
            }
        }

        public void ResubscribeAll()
        {
            foreach (var subject in _subscriptions)
            {
                Subscribe(subject);
            }
        }

        public void Close()
        {
            foreach (var providerPlugin in _providerPlugins)
            {
                providerPlugin.Stop();
            }
        }
    }
}