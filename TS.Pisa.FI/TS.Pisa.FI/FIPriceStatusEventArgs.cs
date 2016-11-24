using System;

namespace TS.Pisa.FI
{
    public class FIPriceStatusEventArgs : EventArgs
    {
        /// <summary>
        /// The name of the bank which the price status update refers to
        /// </summary>
        public string Bank { get; internal set; }

        /// <summary>
        /// The isin number which with the price status update refers to
        /// </summary>
        public string Isin { get; internal set; }

        /// <summary>
        /// The status of the subscription
        /// </summary>
        public PriceStatus Status { get; internal set; }

        /// <summary>
        /// The status text of the subscription
        /// </summary>
        public string StatusText { get; internal set; }

        public override string ToString()
        {
            return "Bank: " + Bank + "; isin: " + Isin + " => " + Status + " (" + StatusText + ")";
        }
    }
}