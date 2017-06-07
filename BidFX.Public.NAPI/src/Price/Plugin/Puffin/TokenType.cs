namespace BidFX.Public.NAPI.Price.Plugin.Puffin
{
    internal enum TokenType
    {
        TagEnd = 0,
        TagEndEmptyContent = 1,
        TagStart = 2,
        NestedContent = 3,
        AttributeName = 4,
        IntegerValue = 5,
        DecimalValue = 6,
        FractionValue = 7,
        StringValue = 8
    }
}