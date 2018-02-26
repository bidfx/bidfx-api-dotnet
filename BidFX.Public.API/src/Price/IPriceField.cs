/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

namespace BidFX.Public.API.Price
{
    /// <summary>
    /// Interface for accessing the data in a single price field.
    /// </summary>
    public interface IPriceField
    {
        /// <summary>
        /// The text form of the updated field.
        /// </summary>
        string Text { get; }

        /// <summary>
        /// The raw datatype of the updated field. This will be one of: string, decimal, long, int or bool.
        /// </summary>
        object Value { get; }
    }
}