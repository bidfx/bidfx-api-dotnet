using System.Globalization;

namespace BidFX.Public.API.Price.Plugin.Puffin
{
    /// <summary>
    /// This class provides an Puffin message token that forms part of a binary compressed XML message in
    /// inter-process communication.
    /// </summary>
    /// <author>Paul Sweeny</author>
    internal class PuffinToken : IPriceField
    {
        public static readonly PuffinToken EmptyToken = new PuffinToken(TokenType.TagEndEmptyContent, "", null);
        public static readonly PuffinToken NullValueToken = new PuffinToken(TokenType.StringValue, "", null);
        public static readonly PuffinToken NullContentToken = new PuffinToken(TokenType.NestedContent, "", null);

        public string Text { get; private set; }
        public TokenType TokenType { get; private set; }
        public object Value { get; private set; }

        internal PuffinToken(TokenType tokenType, string text, object value)
        {
            TokenType = tokenType;
            Text = text;
            Value = value;
        }

        public static PuffinToken StringValue(string value)
        {
            return new PuffinToken(TokenType.StringValue, value, value);
        }

        public static PuffinToken IntegerValue(int value)
        {
            return new PuffinToken(TokenType.IntegerValue, value.ToString(), value);
        }

        public static PuffinToken LongValue(long value)
        {
            return new PuffinToken(TokenType.IntegerValue, value.ToString(), value);
        }

        public static PuffinToken DoubleValue(double value)
        {
            return new PuffinToken(TokenType.DecimalValue, value.ToString(CultureInfo.InvariantCulture), value);
        }

        public static PuffinToken BooleanValue(bool value)
        {
            return new PuffinToken(TokenType.StringValue, value ? "true" : "false", value);
        }

        public PuffinToken ToEndTag()
        {
            return new PuffinToken(TokenType.TagEnd, Text, Value);
        }

        public bool IsStartTag()
        {
            return TokenType == TokenType.TagStart;
        }

        public bool IsValueType()
        {
            return TokenType >= TokenType.IntegerValue;
        }

        public override string ToString()
        {
            if (ReferenceEquals(Text, Value))
            {
                return "token(" + TokenType + " text=\"" + Text + "\")";
            }
            return "token(" + TokenType + " text=\"" + Text + "\", value=" + Value + ")";
        }

        public override bool Equals(object o)
        {
            if (o == this)
            {
                return true;
            }
            if (!(o is PuffinToken)) return false;
            var token = (PuffinToken) o;
            return TokenType == token.TokenType && Text.Equals(token.Text);
        }

        public override int GetHashCode()
        {
            return TokenType.GetHashCode() + (Text.GetHashCode() << 4);
        }
    }
}