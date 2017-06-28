namespace BidFX.Public.API.Price.Plugin.Pixie.Fields
{
    public enum FieldType
    {
        Unrecognised = ' ',
        Double = 'D',
        Integer = 'I',
        Long = 'L',
        String = 'S',
        Discard = ' '
    }

    static class FieldTypeMethods
    {
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