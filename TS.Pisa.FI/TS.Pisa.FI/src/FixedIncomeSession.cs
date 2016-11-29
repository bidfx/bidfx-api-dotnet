using System;
using System.Collections.Generic;
using System.Reflection;
using TS.Pisa.Plugin.Puffin;

namespace TS.Pisa.FI
{
    /// <summary>
    /// This class creates a Pisa Session suitable for subscribing to fixed income products via TradingScreen's
    /// Puffin price service. It can be used to subscribe and unsubscribe from fixed income instruments.
    /// </summary>
    public class FixedIncomeSession
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public string Host { get; set; }
        public int Port { get; set; }
        public bool Tunnel { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        private readonly ISubscriber _session = DefaultSession.GetSubscriber();
        private readonly SubjectMap _subscriptions = new SubjectMap();

        /// <summary>
        /// The event fired upon a price update being received.
        /// </summary>
        public event EventHandler<PriceUpdateEventArgs> OnPrice;

        /// <summary>
        /// The event fired upon a price status update being received.
        /// </summary>
        public event EventHandler<SubscriptionStatusEventArgs> OnStatus;

        /// <summary>
        /// Create the fixed income session
        /// </summary>
        public FixedIncomeSession()
        {
            Tunnel = true;
            Port = 443;
        }

        /// <summary>
        /// Adds a puffin provider plugin to the session and starts the connection
        /// </summary>
        public void Start()
        {
            var session = DefaultSession.GetSession();
            session.AddProviderPlugin(new PuffinProviderPlugin
            {
                Host = Host,
                Port = Port,
                Tunnel = Tunnel,
                Username = Username,
                Password = Password
            });
            session.PriceUpdate += OnPriceUpdate;
            session.PriceStatus += OnPriceStatus;
            session.Start();
        }

        /// <summary>
        /// Stops the puffin provider plugin
        /// </summary>
        public void Stop()
        {
            DefaultSession.GetSession().Stop();
            _subscriptions.Clear();
        }

        private void OnPriceUpdate(object source, TS.Pisa.PriceUpdateEventArgs pisaPriceEvent)
        {
            var publishEvent = OnPrice;
            if (publishEvent == null)
            {
                if (Log.IsDebugEnabled) Log.Debug("ignore prive event as there are no subscribers");
            }
            else
            {
                var subject = _subscriptions.Get(pisaPriceEvent.Subject);
                if (subject != null)
                {
                    publishEvent(this, new PriceUpdateEventArgs
                    {
                        Subject = subject,
                        AllPriceFields = pisaPriceEvent.AllPriceFields,
                        ChangedPriceFields = pisaPriceEvent.ChangedPriceFields
                    });
                }
            }
        }

        private void OnPriceStatus(object source, TS.Pisa.SubscriptionStatusEventArgs pisaStatusEvent)
        {
            var publishEvent = OnStatus;
            if (publishEvent == null)
            {
                if (Log.IsDebugEnabled) Log.Debug("ignore status event as there are no subscribers");
            }
            else
            {
                var subject = _subscriptions.Get(pisaStatusEvent.Subject);
                if (subject != null)
                {
                    publishEvent(this, new SubscriptionStatusEventArgs
                    {
                        Subject = subject,
                        Status = pisaStatusEvent.Status,
                        Reason = pisaStatusEvent.Reason
                    });
                }
            }
        }

        /// <summary>
        /// Subscribes to price updates.
        /// </summary>
        /// <param name="subject">The subject to subscribe to</param>
        public void Subscribe(FixedIncomeSubject subject)
        {
            var pisaSubject = subject.PisaSubject();
            _subscriptions.Put(pisaSubject, subject);
            _session.Subscribe(pisaSubject);
        }

        /// <summary>
        /// Unsubscribes from prices updates.
        /// </summary>
        /// <param name="subject">The subject to unsubscribe from</param>
        public void Unsubscribe(FixedIncomeSubject subject)
        {
            var pisaSubject = subject.PisaSubject();
            _subscriptions.Remove(pisaSubject);
            _session.Unsubscribe(pisaSubject);
        }
    }

    internal class SubjectMap
    {
        private readonly Dictionary<string, FixedIncomeSubject> _map =
            new Dictionary<string, FixedIncomeSubject>();


        public void Put(string pisaSubject, FixedIncomeSubject fiSubject)
        {
            lock (_map)
            {
                _map[pisaSubject] = fiSubject;
            }
        }

        public FixedIncomeSubject Get(string pisaSubject)
        {
            lock (_map)
            {
                FixedIncomeSubject fiSubject;
                _map.TryGetValue(pisaSubject, out fiSubject);
                return fiSubject;
            }
        }

        public void Remove(string pisaSubject)
        {
            lock (_map)
            {
                _map.Remove(pisaSubject);
            }
        }
        public void Clear()
        {
            lock (_map)
            {
                _map.Clear();
            }
        }
    }
}