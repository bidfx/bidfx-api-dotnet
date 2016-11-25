using System;

namespace TS.Pisa.FI
{
    public class PriceUpdateEventArgs : EventArgs
    {
        /// <summary>
        /// The venue code of the publisher of the price update.
        /// </summary>
        /// <remarks>
        /// The venue code of the publisher of the price update.
        /// This will normally be a bank, broker, exchange or other liquidity provider.
        /// </remarks>
        public string Venue { get; internal set; }

        /// <summary>
        /// The ISIN code of the fixed income product containing in the price update.
        /// </summary>
        public string Isin { get; internal set; }


        /// <summary>
        /// The complete map of price fields containing all current prices published by the venue.
        /// </summary>
        /// <remarks>
        /// The complete map of price fields containing all current prices published by the venue.
        /// This map will contain all of the recently changed fields plus any other that remain current
        /// but have not changed in this price update.
        /// </remarks>
        public IPriceMap AllPriceFields { get; internal set; }

        /// <summary>
        /// The map of price fields that changed in the current price update event.
        /// </summary>
        /// <remarks>
        /// The map of price fields that changed in the current price update event.
        /// This will be a subset of the full set of published price fields.
        /// A GUI application might make use of this set to visually flash the fields that have just changed.
        /// </remarks>
        public IPriceMap ChangedPriceFields { get; internal set; }

        public override string ToString()
        {
            return "Venue: " + Venue + ", isin: " + Isin + " => " + AllPriceFields;
        }
    }
}