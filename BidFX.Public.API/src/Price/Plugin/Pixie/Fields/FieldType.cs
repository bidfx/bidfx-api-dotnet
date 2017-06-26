using System;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields
{
    public enum FieldType
    {
        Double = 'D',
        Integer = 'I',
        Long = 'L',
        String = 'S',
        Discard = ' ',
        Unrecognised = ' '
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
                case (char) FieldType.Discard:
                    return FieldType.Discard;
//                case (char) FieldType.Unrecognised:
//                    return FieldType.Unrecognised;
                default:
                    throw new ArgumentException("unrecognised field type code: " + code + " ('" + (char) code +
                                                "').");
            }
        }
    }
}