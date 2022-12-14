/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

using System;

namespace BidFX.Public.API.Price
{
    /// <summary>
    /// Describes a single price update event (or tick) on a price subscription.
    /// </summary>
    public class PriceUpdateEvent : EventArgs
    {
        /// <summary>
        /// The subject of the subscription.
        /// </summary>
        public Subject.Subject Subject { get; internal set; }

        /// <summary>
        /// The complete map of price fields containing all current prices published by the venue.
        /// </summary>
        /// <remarks>
        /// This map will contain all of the recently changed fields plus any other that remain current
        /// but have not changed in this price update.
        /// </remarks>
        public IPriceMap AllPriceFields { get; internal set; }

        /// <summary>
        /// The map of price fields that changed in the current price update event.
        /// </summary>
        /// <remarks>
        /// This will be a subset of the full set of published price fields.
        /// A GUI application might make use of this set to visually flash the fields that have just changed.
        /// </remarks>
        public IPriceMap ChangedPriceFields { get; internal set; }

        public override string ToString()
        {
            return Subject + " AllPriceFields{" + AllPriceFields + "} ChangedPriceFields{" + ChangedPriceFields + "}";
        }
    }
}