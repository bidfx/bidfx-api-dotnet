using System;

namespace TS.Pisa.FI
{
    public class FIPriceUpdateEventArgs : EventArgs
    {
        public string Bank { get; internal set; }
        public string Isin { get; internal set; }
        public IPriceMap PriceImage { get; internal set; }
        public IPriceMap PriceUpdate { get; internal set; }

        public override string ToString()
        {
            return "Bank: " + Bank + "; isin: " + Isin + " => " + PriceImage;
        }
    }
}