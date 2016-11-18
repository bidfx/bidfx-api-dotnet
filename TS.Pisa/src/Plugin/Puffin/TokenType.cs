namespace TS.Pisa.Plugin.Puffin
{
    public enum TokenType
    {
        TagEnd = 0,
        TagEndEmptyContent = 1,
        TagStart = 2,
        NestedContent = 3,
        AttributeName = 4,
        AttributeValueInteger = 5,
        AttributeValueDouble = 6,
        AttributeValueFraction = 7,
        AttributeValueString = 8
    }
}