using System;
using System.Collections.Generic;
using TS.Pisa.Plugin.Puffin;

namespace TS.Pisa.FI
{
    /// <summary>
    /// This class creates a Pisa Master Session and adds a single Puffin Provider plugin to it.
    /// It can be used to subscribe and unsubscribe from price updates.
    /// </summary>
    public class FixedIncomeSession
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool Tunnel { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        private readonly ISession _session;
        private readonly Dictionary<string, FixedIncomeSubject> _subjects;

        /// <summary>
        /// The event fired upon a price update being received
        /// </summary>
        public event EventHandler<FIPriceUpdateEventArgs> OnPriceUpdate;

        /// <summary>
        /// The event fired upon a price status update being received
        /// </summary>
        public event EventHandler<FIPriceStatusEventArgs> OnPriceStatus;

        /// <summary>
        /// Create the fixed income session
        /// </summary>
        public FixedIncomeSession()
        {
            _subjects = new Dictionary<string, FixedIncomeSubject>();
            _session = DefaultSession.GetDefault();
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

        private void OnPriceUpdateHandler(object source, PriceUpdateEventArgs args)
        {
            var subject = _subjects[args.Subject] ?? GetSubject(args.Subject);
            if (subject == null) return;
            var fiPriceUpdateEventArgs = new FIPriceUpdateEventArgs
            {
                Bank = subject.Bank,
                Isin = subject.Isin,
                PriceImage = args.PriceImage,
                PriceUpdate = args.PriceUpdate
            };
            if (OnPriceUpdate != null)
            {
                OnPriceUpdate(this, fiPriceUpdateEventArgs);
            }
        }

        private void OnPriceStatusHandler(object source, PriceStatusEventArgs args)
        {
            var subject = _subjects[args.Subject] ?? GetSubject(args.Subject);
            if (subject == null) return;
            var fiPriceStatusEventArgs = new FIPriceStatusEventArgs
            {
                Bank = subject.Bank,
                Isin = subject.Isin,
                Status = args.Status,
                StatusText = args.StatusText
            };
            if (OnPriceStatus != null)
            {
                OnPriceStatus(this, fiPriceStatusEventArgs);
            }
        }

        /// <summary>
        /// Subscribes to price updates.
        /// </summary>
        /// <param name="bank">The 3 letter code for the bank to subscribe to</param>
        /// <param name="isin">The 12 alphanumeric isin number to subscribe to</param>
        public void Subscribe(string bank, string isin)
        {
            var subject = new FixedIncomeSubject
            {
                Bank = bank,
                Isin = isin
            };
            _subjects.Add(subject.ToString(), subject);
            _session.Subscribe(subject.ToString());
        }

        /// <summary>
        /// Unsubscribes from prices updates.
        /// </summary>
        /// <param name="bank">The 3 letter code for the bank to unsubscribe from</param>
        /// <param name="isin">The 12 alphanumeric isin number to unsubscribe from</param>
        public void Unsubscribe(string bank, string isin)
        {
            var subject = new FixedIncomeSubject
            {
                Bank = bank,
                Isin = isin
            };
            _subjects.Add(subject.ToString(), subject);
            _session.Unsubscribe(subject.ToString());
        }

        private FixedIncomeSubject GetSubject(string subject)
        {
            if (!subject.Contains("Exchange") || !subject.Contains("Symbol")) return null;
            var bankStartIndex = subject.IndexOf("Exchange", StringComparison.Ordinal);
            var bank = subject.Substring(bankStartIndex + 9, 3);
            var isinStartIndex = subject.IndexOf("Symbol", StringComparison.Ordinal);
            var isin = subject.Substring(isinStartIndex + 7, 12);
            return new FixedIncomeSubject
            {
                Bank = bank,
                Isin = isin
            };
        }

        private class FixedIncomeSubject
        {
            public string Bank { get; set; }
            public string Isin { get; set; }

            public override string ToString()
            {
                return "AssetClass=FixedIncome,Exchange=" + Bank + ",Level=1,Source=Lynx,Symbol=" + Isin;
            }
        }
    }
}