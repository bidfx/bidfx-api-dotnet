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

        private readonly ISession _session = DefaultSession.GetDefault();

        private readonly Dictionary<string, FixedIncomeSubject> _subscriptions =
            new Dictionary<string, FixedIncomeSubject>();

        /// <summary>
        /// The event fired upon a price update being received
        /// </summary>
        public event EventHandler<PriceEventArgs> OnPrice;

        /// <summary>
        /// The event fired upon a price status update being received
        /// </summary>
        public event EventHandler<StatusEventArgs> OnStatus;

        /// <summary>
        /// Create the fixed income session
        /// </summary>
        public FixedIncomeSession()
        {
            Tunnel = true;
            Port = 443;
            _session.PriceUpdate += OnPriceUpdateHandler;
            _session.PriceStatus += OnPriceStatusHandler;
        }

        /// <summary>
        /// Adds a puffin provider plugin to the session and starts the connection
        /// </summary>
        public void Start()
        {
            _session.AddProviderPlugin(new PuffinProviderPlugin
            {
                Host = Host,
                Port = Port,
                Tunnel = Tunnel,
                Username = Username,
                Password = Password
            });
            _session.Start();
        }

        /// <summary>
        /// Stops the puffin provider plugin
        /// </summary>
        public void Stop()
        {
            _session.Stop();
            _subscriptions.Clear();
        }

        private void OnPriceUpdateHandler(object source, PriceUpdateEventArgs pisaPriceEvent)
        {
            var publishEvent = OnPrice;
            if (publishEvent == null)
            {
                if (Log.IsDebugEnabled) Log.Debug("ignore prive event as there are no subscribers");
            }
            else
            {
                var subject = LookUpSubject(pisaPriceEvent.Subject);
                if (subject != null)
                {
                    publishEvent(this, new PriceEventArgs
                    {
                        Subject = subject,
                        AllPriceFields = pisaPriceEvent.PriceImage,
                        ChangedPriceFields = pisaPriceEvent.PriceUpdate
                    });
                }
            }
        }

        private void OnPriceStatusHandler(object source, PriceStatusEventArgs pisaStatusEvent)
        {
            var publishEvent = OnStatus;
            if (publishEvent == null)
            {
                if (Log.IsDebugEnabled) Log.Debug("ignore status event as there are no subscribers");
            }
            else
            {
                var subject = LookUpSubject(pisaStatusEvent.Subject);
                if (subject != null)
                {
                    publishEvent(this, new StatusEventArgs
                    {
                        Subject = subject,
                        Status = pisaStatusEvent.Status,
                        Reason = pisaStatusEvent.Reason
                    });
                }
            }
        }

        private FixedIncomeSubject LookUpSubject(string pisaSubject)
        {
            FixedIncomeSubject subject;
            _subscriptions.TryGetValue(pisaSubject, out subject);
            return subject;
        }

        /// <summary>
        /// Subscribes to price updates.
        /// </summary>
        /// <param name="subject">The subject to subscribe to</param>
        public void Subscribe(FixedIncomeSubject subject)
        {
            var pisaSubject = subject.PisaSubject();
            _subscriptions[pisaSubject] = subject;
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
}