namespace TS.Pisa
{
    /// <summary>
    /// This is an interface that represents a single key-value pair of field name and field value
    /// </summary>
    public interface IPriceField
    {
        /// <summary>
        /// The field name
        /// </summary>
        string Text { get; }

        /// <summary>
        /// The field value
        /// </summary>
        object Value { get; }
    }
}