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
        public static readonly PuffinToken EmptyToken = new PuffinToken(LexicalType.TagEndEmptyContent, "", null);
        public static readonly PuffinToken NullValueToken = new PuffinToken(LexicalType.AttributeValueString, "", null);
        public static readonly PuffinToken NullContentToken = new PuffinToken(LexicalType.NestedContent, "", null);
        public static readonly PuffinToken ZeroToken = IntegerValue(0);

        private readonly LexicalType _lexicalType;
        private readonly string _text;
        private readonly object _value;

        public PuffinToken(LexicalType lexicalType, string text, object value)
        {
            _lexicalType = lexicalType;
            _text = text;
            _value = value;
        }

        public static PuffinToken StartTag(string value)
        {
            return new PuffinToken(LexicalType.TagStart, value, value);
        }

        public static PuffinToken AttributeName(string value)
        {
            return new PuffinToken(LexicalType.AttributeName, value, value);
        }

        public static PuffinToken StringValue(string value)
        {
            return new PuffinToken(LexicalType.AttributeValueString, value, value);
        }

        public static PuffinToken IntegerValue(int value)
        {
            return new PuffinToken(LexicalType.AttributeValueInteger, value.ToString(), value);
        }

        public static PuffinToken LongValue(long value)
        {
            return new PuffinToken(LexicalType.AttributeValueInteger, value.ToString(), value);
        }

        public static PuffinToken DoubleValue(double value)
        {
            return new PuffinToken(LexicalType.AttributeValueDouble, value.ToString(CultureInfo.InvariantCulture), value);
        }

        public static PuffinToken BooleanValue(bool value)
        {
            return new PuffinToken(LexicalType.AttributeValueString, value ? "true" : "false", value);
        }

        public PuffinToken ToEndTag()
        {
            return new PuffinToken(LexicalType.TagEnd, _text, _value);
        }

        public LexicalType TokenType()
        {
            return _lexicalType;
        }

        public bool IsEndTag()
        {
            return _lexicalType == LexicalType.TagEnd;
        }

        public bool IsEmptyTag()
        {
            return _lexicalType == LexicalType.TagEndEmptyContent;
        }

        public bool IsStartTag()
        {
            return _lexicalType == LexicalType.TagStart;
        }

        public bool IsTagContent()
        {
            return _lexicalType == LexicalType.NestedContent;
        }

        public bool IsAttributeName()
        {
            return _lexicalType == LexicalType.AttributeName;
        }

        public bool IsValueType()
        {
            return _lexicalType >= LexicalType.AttributeValueInteger;
        }

        public bool IsNumberType()
        {
            return _lexicalType == LexicalType.AttributeValueInteger ||
                   _lexicalType == LexicalType.AttributeValueDouble ||
                   _lexicalType == LexicalType.AttributeValueFraction;
        }

        public bool IsNull()
        {
            return false;
        }

        public bool IsInteger()
        {
            return _lexicalType == LexicalType.AttributeValueInteger;
        }

        public bool IsDouble()
        {
            return _lexicalType == LexicalType.AttributeValueDouble;
        }

        public bool IsFraction()
        {
            return _lexicalType == LexicalType.AttributeValueFraction;
        }

        public bool IsString()
        {
            return _lexicalType == LexicalType.AttributeValueString;
        }

        public bool IsNegative()
        {
            return _text.StartsWith("-");
        }

        public string GetText()
        {
            return _text;
        }

        public object GetValue()
        {
            return _value;
        }

        public int Length()
        {
            return _text.Length;
        }

        public override string ToString()
        {
            return _text;
        }

        public int ToInteger()
        {
            if (IsInteger())
            {
                return Convert.ToInt32(ToString());
            }
            return (int) ToDouble();
        }

        public long ToLong()
        {
            if (IsInteger())
            {
                return Convert.ToInt64(ToString());
            }
            return (long) ToDouble();
        }

        public double ToDouble()
        {
            if (IsDouble() || IsInteger())
            {
                return TextAsDouble();
            }
            return FractionToDouble(ToString());
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

        private double TextAsDouble()
        {
            return double.Parse(ToString());
        }

        public bool ToBoolean()
        {
            return Convert.ToBoolean(ToString());
        }

        public override bool Equals(object o)
        {
            if (o == this)
            {
                return true;
            }
            if (!(o is PuffinToken)) return false;
            var token = (PuffinToken) o;
            return _lexicalType == token._lexicalType && _text.Equals(token._text);
        }

        public string DebugString()
        {
            return "token " + _lexicalType + " [" + ToString() + ']';
        }

        public override int GetHashCode()
        {
            return _lexicalType.GetHashCode() + (_text.GetHashCode() << 4);
        }
    }
}