using System;

namespace TS.Pisa.FI
{
    public class FIPriceUpdateEventArgs : EventArgs
    {
        /// <summary>
        /// The name of the bank which the price update refers to
        /// </summary>
        public string Bank { get; internal set; }

        /// <summary>
        /// The isin number which with the price update refers to
        /// </summary>
        public string Isin { get; internal set; }


        public IPriceMap PriceImage { get; internal set; }
        public IPriceMap PriceUpdate { get; internal set; }

        public override string ToString()
        {
            return "Bank: " + Bank + "; isin: " + Isin + " => " + PriceImage;
        }
    }
}