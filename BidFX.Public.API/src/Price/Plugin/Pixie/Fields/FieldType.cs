namespace BidFX.Public.API.Price.Plugin.Pixie.Fields
{
    internal enum FieldType
    {
        /// <summary>
        /// An unrecognised field type used by a client when the client cannot recognise the field type sent to it from
        /// the server, perhaps due to a protocol version mismatch.
        /// </summary>
        Unrecognised = ' ',
        /// <summary>
        /// A double field type.
        /// </summary>
        Double = 'D',
        /// <summary>
        /// A integer field type.
        /// </summary>
        Integer = 'I',
        /// <summary>
        /// A long field type.
        /// </summary>
        Long = 'L',
        /// <summary>
        /// A string field type.
        /// </summary>
        String = 'S'
    }

    static class FieldTypeMethods
    {
        /// <summary>
        /// Gets the enum based on its code used in the Pixie wire format.
        /// </summary>
        /// <param name="code">the code letter of the type</param>
        /// <returns>the enum</returns>
        public static FieldType ValueOf(int code)
        {
            switch (code)
            {
                case (char) FieldType.Double:
                    return FieldType.Double;
                case (char) FieldType.Integer:
                    return FieldType.Integer;
                case (char) FieldType.Long:
                    return FieldType.Long;
                case (char) FieldType.String:
                    return FieldType.String;
                default:
                    return FieldType.Unrecognised;
            }
        }
    }
}