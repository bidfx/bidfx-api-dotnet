using System.Collections.Generic;

namespace TS.Pisa
{
    /// <summary>
    /// This interface is used for accessing values within a map of price fields.
    /// </summary>
    public interface IPriceMap
    {
        /// <summary>
        /// Gets the complete set of price field names.
        /// </summary>
        IEnumerable<string> FieldNames { get; }

        /// <summary>
        /// Gets the price fields as a map.
        /// </summary>
        IEnumerable<KeyValuePair<string, IPriceField>> PriceFields { get; }

        /// <summary>
        /// Returns the current value of a price field as an IPriceField
        /// </summary>
        /// <param name="name">the name of the price field to return</param>
        /// <returns>the IPriceField for that price field or null if the field is not present</returns>
        IPriceField Field(string name);

        /// <summary>
        /// Return the current value of a price field as a decimal
        /// </summary>
        /// <param name="name">the name of the price field to return</param>
        /// <returns>the field value as a decimal or null if the field is not present</returns>
        decimal? DecimalField(string name);

        /// <summary>
        /// Return the current value of a price field as a long
        /// </summary>
        /// <param name="name">the name of the price field to return</param>
        /// <returns>the field value as a long or null if the field is not present</returns>
        long? LongField(string name);

        /// <summary>
        /// Return the current value of a price field as a int
        /// </summary>
        /// <param name="name">the name of the price field to return</param>
        /// <returns>the field value as a int or null if the field is not present</returns>
        int? IntField(string name);

        /// <summary>
        /// Return the current value of a price field as a string
        /// </summary>
        /// <param name="name">the name of the price field to return</param>
        /// <returns>the field value as a string or null if the field is not present</returns>
        string StringField(string name);
    }
}