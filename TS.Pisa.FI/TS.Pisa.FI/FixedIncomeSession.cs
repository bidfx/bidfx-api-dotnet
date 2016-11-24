using System;
using System.Collections.Generic;
using TS.Pisa.Plugin.Puffin;

namespace TS.Pisa.FI
{
    public class FixedIncomeSession
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool Tunnel { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        private readonly ISession _session;
        private readonly Dictionary<string, FixedIncomeSubject> _subjects;
        public event EventHandler<FIPriceUpdateEventArgs> OnPriceUpdate;
        public event EventHandler<FIPriceStatusEventArgs> OnPriceStatus;

        public FixedIncomeSession()
        {
            _subjects = new Dictionary<string, FixedIncomeSubject>();
            _session = DefaultSession.GetDefault();
            _session.PriceUpdate += OnPriceUpdateHandler;
            _session.PriceStatus += OnPriceStatusHandler;
        }

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