using System;
using System.Globalization;

namespace TS.Pisa.Plugin.Puffin
{
    /// <summary>
    /// This class provides an Puffin message token that forms part of a binary compressed XML message in
    /// inter-process communication.
    /// </summary>
    /// <author>Paul Sweeny</author>
    public class PuffinToken
    {
        public static readonly PuffinToken EmptyToken = new PuffinToken(TokenType.TagEndEmptyContent, "", null);
        public static readonly PuffinToken NullValueToken = new PuffinToken(TokenType.AttributeValueString, "", null);
        public static readonly PuffinToken NullContentToken = new PuffinToken(TokenType.NestedContent, "", null);
        public static readonly PuffinToken ZeroToken = IntegerValue(0);

        public string Text { get; private set; }
        public TokenType TokenType { get; private set; }
        public object Value { get; private set; }

        public PuffinToken(TokenType tokenType, string text, object value)
        {
            TokenType = tokenType;
            Text = text;
            Value = value;
        }

        public static PuffinToken StartTag(string value)
        {
            return new PuffinToken(TokenType.TagStart, value, value);
        }

        public static PuffinToken AttributeName(string value)
        {
            return new PuffinToken(TokenType.AttributeName, value, value);
        }

        public static PuffinToken StringValue(string value)
        {
            return new PuffinToken(TokenType.AttributeValueString, value, value);
        }

        public static PuffinToken IntegerValue(int value)
        {
            return new PuffinToken(TokenType.AttributeValueInteger, value.ToString(), value);
        }

        public static PuffinToken LongValue(long value)
        {
            return new PuffinToken(TokenType.AttributeValueInteger, value.ToString(), value);
        }

        public static PuffinToken DoubleValue(double value)
        {
            return new PuffinToken(TokenType.AttributeValueDouble, value.ToString(CultureInfo.InvariantCulture), value);
        }

        public static PuffinToken BooleanValue(bool value)
        {
            return new PuffinToken(TokenType.AttributeValueString, value ? "true" : "false", value);
        }

        public PuffinToken ToEndTag()
        {
            return new PuffinToken(TokenType.TagEnd, Text, Value);
        }

        public bool IsEndTag()
        {
            return TokenType == TokenType.TagEnd;
        }

        public bool IsEmptyTag()
        {
            return TokenType == TokenType.TagEndEmptyContent;
        }

        public bool IsStartTag()
        {
            return TokenType == TokenType.TagStart;
        }

        public bool IsTagContent()
        {
            return TokenType == TokenType.NestedContent;
        }

        public bool IsAttributeName()
        {
            return TokenType == TokenType.AttributeName;
        }

        public bool IsValueType()
        {
            return TokenType >= TokenType.AttributeValueInteger;
        }

        public bool IsNumberType()
        {
            return TokenType == TokenType.AttributeValueInteger ||
                   TokenType == TokenType.AttributeValueDouble ||
                   TokenType == TokenType.AttributeValueFraction;
        }

        public bool IsNull()
        {
            return false;
        }

        public bool IsInteger()
        {
            return TokenType == TokenType.AttributeValueInteger;
        }

        public bool IsDouble()
        {
            return TokenType == TokenType.AttributeValueDouble;
        }

        public bool IsFraction()
        {
            return TokenType == TokenType.AttributeValueFraction;
        }

        public bool IsString()
        {
            return TokenType == TokenType.AttributeValueString;
        }

        public bool IsNegative()
        {
            return Text.StartsWith("-");
        }

        public override string ToString()
        {
            return "token(" + TokenType + " text=\"" + Text + "\", value=" + Value + ")";
        }

        public int ToInteger()
        {
            if (Value is int?)
            {
                return (int) Value;
            }
            try
            {
                if (IsInteger())
                {
                    return Convert.ToInt32(Text);
                }
                return (int) ToDouble();
            }
            catch (FormatException e)
            {
                throw new PuffinSyntaxException("failed to convert token to int double. " + this, e);
            }
        }

        public long ToLong()
        {
            if (Value is long?)
            {
                return (long) Value;
            }
            try
            {
                if (IsInteger())
                {
                    return Convert.ToInt64(Text);
                }
                return (long) ToDouble();
            }
            catch (FormatException e)
            {
                throw new PuffinSyntaxException("failed to convert token to long. " + this, e);
            }
        }

        public double ToDouble()
        {
            if (Value is double?)
            {
                return (double) Value;
            }
            try
            {
                if (IsDouble() || IsInteger())
                {
                    return double.Parse(Text);
                }
                return FractionToDouble(Text);
            }
            catch (FormatException e)
            {
                throw new PuffinSyntaxException("failed to convert token to double. " + this, e);
            }
        }

        public bool ToBoolean()
        {
            try
            {
                return Convert.ToBoolean(Text);
            }
            catch (FormatException e)
            {
                throw new PuffinSyntaxException("failed to convert token to boolean. " + this, e);
            }
        }

        public static double FractionToDouble(string fraction)
        {
            try
            {
                int slash = fraction.IndexOf('/');
                if (slash == -1)
                {
                    if (fraction.IndexOf('.') == -1)
                    {
                        return Convert.ToInt64(fraction);
                    }
                    return double.Parse(fraction);
                }
                double denominator = Convert.ToInt32(fraction.Substring(slash + 1, fraction.Length));
                int space = fraction.IndexOf(' ');
                if (space == -1)
                {
                    return Convert.ToInt32(fraction.Substring(0, slash)) / denominator;
                }
                double whole = Math.Abs(Convert.ToInt32(fraction.Substring(0, space)));
                double numerator = Convert.ToInt32(fraction.Substring(space + 1, slash));
                double abs = whole + numerator / denominator;
                return fraction[0] == '-' ? -abs : abs;
            }
            catch (Exception e)
            {
                throw new FormatException("invalid fraction " + fraction, e);
            }
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