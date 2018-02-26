/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

using System;
using System.Collections.Generic;

namespace BidFX.Public.API.Price
{
    /// <summary>
    /// Interface for accessing a map of recently updated price fields as name-values pairs.
    /// </summary>
    public interface IPriceMap
    {
        /// <summary>
        /// Allows iteration over all of the price field names in the map.
        /// </summary>
        IEnumerable<string> FieldNames { get; }

        /// <summary>
        /// Allows iteration over all of the price field name-value pairs in the map.
        /// </summary>
        IEnumerable<KeyValuePair<string, IPriceField>> PriceFields { get; }

        /// <summary>
        /// Gets a field.
        /// </summary>
        /// <param name="name">the name of the field <see cref="FieldName">common field names</see> </param>
        /// <returns>the prive field or null if the field is not present.</returns>
        IPriceField Field(string name);

        /// <summary>
        /// Gets a field as a decimal value.
        /// </summary>
        /// <param name="name">the name of the field <see cref="FieldName">common field names</see> </param>
        /// <returns>the value or null if the field is not present or cannot be converted to decimal.</returns>
        decimal? DecimalField(string name);

        /// <summary>
        /// Gets a field as a long value.
        /// </summary>
        /// <param name="name">the name of the field <see cref="FieldName">common field names</see> </param>
        /// <returns>the value or null if the field is not present or cannot be converted to long.</returns>
        long? LongField(string name);

        /// <summary>
        /// Gets a field as an integer value.
        /// </summary>
        /// <param name="name">the name of the field <see cref="FieldName">common field names</see> </param>
        /// <returns>the value or null if the field is not present or cannot be converted to int.</returns>
        int? IntField(string name);

        /// <summary>
        /// Gets a field as a string value.
        /// </summary>
        /// <param name="name">the name of the field <see cref="FieldName">common field names</see> </param>
        /// <returns>the value or null if the field is not present.</returns>
        string StringField(string name);

        /// <summary>
        /// Gets a field as a date-time value.
        /// </summary>
        /// <param name="name">the name of the field <see cref="FieldName">common field names</see> </param>
        /// <returns>the value or null if the field is not present.</returns>
        DateTime? DateTimeField(string name);

        /// <summary>
        /// Gets a field as a tick value.
        /// </summary>
        /// <param name="name">the name of the field <see cref="FieldName">common field names</see> </param>
        /// <returns>the value or null if the field is not present.</returns>
        Tick? TickField(string name);
    }
}