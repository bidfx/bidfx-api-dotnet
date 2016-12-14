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
    public class FixedIncomeSession : IBackground
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public string Host { get; set; }
        public int Port { get; set; }
        public bool Tunnel { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        private readonly SubjectMap _subscriptions = new SubjectMap();

        /// <summary>
        /// The event fired upon a price update being received.
        /// </summary>
        public event EventHandler<FiPriceUpdateEvent> PriceUpdateEventHandler;

        /// <summary>
        /// The event fired upon a price status update being received.
        /// </summary>
        public event EventHandler<FiSubscriptionStatusEvent> SubscriptionStatusEventHandler;

        /// <summary>
        /// Creates a new fixed income pricing session.
        /// </summary>
        public FixedIncomeSession()
        {
            Tunnel = true;
            Port = 443;
        }

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
            session.PriceUpdateEventHandler += OnPriceUpdate;
            session.SubscriptionStatusEventHandler += OnSubscriptionStatusEventHandler;
            session.Start();
        }

        public void Stop()
        {
            DefaultSession.GetSession().Stop();
            _subscriptions.Clear();
        }

        private void OnPriceUpdate(object source, PriceUpdateEvent pisaPriceEvent)
        {
            var publishEvent = PriceUpdateEventHandler;
            if (publishEvent == null)
            {
                if (Log.IsDebugEnabled) Log.Debug("ignore prive event as there are no subscribers");
            }
            else
            {
                var subject = _subscriptions.Get(pisaPriceEvent.Subject);
                if (subject != null)
                {
                    publishEvent(this, new FiPriceUpdateEvent
                    {
                        Subject = subject,
                        AllPriceFields = pisaPriceEvent.AllPriceFields,
                        ChangedPriceFields = pisaPriceEvent.ChangedPriceFields
                    });
                }
            }
        }

        private void OnSubscriptionStatusEventHandler(object source, SubscriptionStatusEvent pisaStatusEvent)
        {
            var eventHandler = SubscriptionStatusEventHandler;
            if (eventHandler == null)
            {
                if (Log.IsDebugEnabled) Log.Debug("ignore subscription status event as there are no subscribers");
            }
            else
            {
                var subject = _subscriptions.Get(pisaStatusEvent.Subject);
                if (subject != null)
                {
                    eventHandler(this, new FiSubscriptionStatusEvent
                    {
                        Subject = subject,
                        SubscriptionStatus = pisaStatusEvent.SubscriptionStatus,
                        StatusReason = pisaStatusEvent.StatusReason
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
            DefaultSession.GetSubscriber().Subscribe(pisaSubject);
        }

        /// <summary>
        /// Unsubscribes from prices updates.
        /// </summary>
        /// <param name="subject">The subject to unsubscribe from</param>
        public void Unsubscribe(FixedIncomeSubject subject)
        {
            var pisaSubject = subject.PisaSubject();
            _subscriptions.Remove(pisaSubject);
            DefaultSession.GetSubscriber().Unsubscribe(pisaSubject);
        }

        /// <summary>
        /// Gets the underlying Pisa session used by this session.
        /// </summary>
        public ISession Session {
            get { return DefaultSession.GetSession(); }
        }

        public bool Running
        {
            get { return Session.Running; }
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