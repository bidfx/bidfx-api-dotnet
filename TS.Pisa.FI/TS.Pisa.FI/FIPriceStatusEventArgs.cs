using System;

namespace TS.Pisa.FI
{
    public class FIPriceStatusEventArgs : EventArgs
    {
        public string Bank { get; internal set; }
        public string Isin { get; internal set; }
        public PriceStatus Status { get; internal set; }
        public string StatusText { get; internal set; }

        public override string ToString()
        {
            return "Bank: " + Bank + "; isin: " + Isin + " => " + Status + " (" + StatusText + ")";
        }
    }
}