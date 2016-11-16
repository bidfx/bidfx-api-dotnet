using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <summary>
    /// This class provides an Xml token that may be generated by an Xml parser or
    /// used to represent a component part of a compressed Xml message in
    /// inter-process communication.
    /// </summary>
    /// <remarks>
    /// This class provides an Xml token that may be generated by an Xml parser or
    /// used to represent a component part of a compressed Xml message in
    /// inter-process communication.  An Xml parser typically breaks the incoming
    /// Xml into Tokens, each with a type and associated text string.  This class
    /// provides access to this information.
    /// </remarks>
    /// <author>Paul Sweeny</author>
    public class XmlToken
    {
        public static readonly XmlToken EmptyToken = new XmlToken(XmlTokenType.TagEndEmptyContent, "", null);
        public static readonly XmlToken NullValueToken = new XmlToken(XmlTokenType.AttributeValueString, "", null);
        public static readonly XmlToken NullContentToken = new XmlToken(XmlTokenType.NestedContent, "", null);
        public static readonly XmlToken ZeroToken = IntegerValue(0);

        private readonly XmlTokenType _tokenType;
        private readonly string _text;
        private readonly object _value;

        public XmlToken(XmlTokenType tokenType, string text, object value)
        {
            _tokenType = tokenType;
            _text = text;
            _value = value;
        }

        public static XmlToken StartTag(string value)
        {
            return new XmlToken(XmlTokenType.TagStart, value, value);
        }

        public static XmlToken AttributeName(string value)
        {
            return new XmlToken(XmlTokenType.AttributeName, value, value);
        }

        public static XmlToken StringValue(string value)
        {
            return new XmlToken(XmlTokenType.AttributeValueString, value, value);
        }

        public static XmlToken IntegerValue(int value)
        {
            return new XmlToken(XmlTokenType.AttributeValueInteger, value.ToString(), value);
        }

        public static XmlToken LongValue(long value)
        {
            return new XmlToken(XmlTokenType.AttributeValueInteger, value.ToString(), value);
        }

        public static XmlToken DoubleValue(double value)
        {
            return new XmlToken(XmlTokenType.AttributeValueDouble, value.ToString(CultureInfo.InvariantCulture), value);
        }

        public static XmlToken BooleanValue(bool value)
        {
            return new XmlToken(XmlTokenType.AttributeValueString, value ? "true" : "false", value);
        }

        /// <summary>
        /// Convert a token (normally a start tag) into an end tag token with the
        /// same text.
        /// </summary>
        public XmlToken ToEndTag()
        {
            return new XmlToken(XmlTokenType.TagEnd, _text, _value);
        }

        /// <summary>Get the type of this token.</summary>
        public XmlTokenType TokenType()
        {
            return _tokenType;
        }

        /// <summary>Check if this token is an Xml end-tag token.</summary>
        public bool IsEndTag()
        {
            return _tokenType == XmlTokenType.TagEnd;
        }

        /// <summary>Check if this token is an Xml empty-tag close token.</summary>
        public bool IsEmptyTag()
        {
            return _tokenType == XmlTokenType.TagEndEmptyContent;
        }

        /// <summary>Check if this token is an Xml start-tag token.</summary>
        public bool IsStartTag()
        {
            return _tokenType == XmlTokenType.TagStart;
        }

        /// <summary>Check if this token is an Xml content token.</summary>
        public bool IsTagContent()
        {
            return _tokenType == XmlTokenType.NestedContent;
        }

        /// <summary>Check if this token is an Xml element attribute name token.</summary>
        public bool IsAttributeName()
        {
            return _tokenType == XmlTokenType.AttributeName;
        }

        /// <summary>
        /// Check if this token is an attribute-value type:
        /// integer, double, fraction or string.
        /// </summary>
        public bool IsValueType()
        {
            return _tokenType >= XmlTokenType.AttributeValueInteger;
        }

        /// <summary>
        /// Check if this token is a number type: integer, double
        /// or fraction.
        /// </summary>
        public bool IsNumberType()
        {
            return _tokenType == XmlTokenType.AttributeValueInteger ||
                   _tokenType == XmlTokenType.AttributeValueDouble ||
                   _tokenType == XmlTokenType.AttributeValueFraction;
        }

        /// <summary>Check if this token is null (not present).</summary>
        public bool IsNull()
        {
            return false;
        }

        /// <summary>Check if this token is an integer attribute-value XmlTokenType.</summary>
        public bool IsInteger()
        {
            return _tokenType == XmlTokenType.AttributeValueInteger;
        }

        /// <summary>Check if this token is a double attribute-value XmlTokenType.</summary>
        public bool IsDouble()
        {
            return _tokenType == XmlTokenType.AttributeValueDouble;
        }

        /// <summary>Check if this token is a fraction attribute-value XmlTokenType.</summary>
        public bool IsFraction()
        {
            return _tokenType == XmlTokenType.AttributeValueFraction;
        }

        /// <summary>Check if this token is a string attribute-value XmlTokenType.</summary>
        public bool IsString()
        {
            return _tokenType == XmlTokenType.AttributeValueString;
        }

        /// <summary>Check if this token is negative.</summary>
        public bool IsNegative()
        {
            return _text.StartsWith("-");
        }

        /// <summary>Get the text associated with this token.</summary>
        /// <returns>the associated text or "" if the token has no text.</returns>
        public string GetText()
        {
            return _text;
        }

        public object GetValue()
        {
            return _value;
        }

        /// <summary>
        /// Get the length of this token in bytes, this is the same as the length
        /// of the text associated with the token.
        /// </summary>
        public int Length()
        {
            return _text.Length;
        }

        /// <summary>Convert the value of this token to a String (same as getText).</summary>
        public override string ToString()
        {
            return _text;
        }

        /// <summary>Convert the value of this token to an int.</summary>
        /// <exception cref="System.FormatException">
        /// if the token does not contain a parsable
        /// integer.
        /// </exception>
        public int ToInteger()
        {
            if (IsInteger())
            {
                return Convert.ToInt32(ToString());
            }
            return (int) ToDouble();
        }

        /// <summary>Convert the value of this token to a long.</summary>
        /// <exception cref="System.FormatException">
        /// if the token does not contain a parsable
        /// long.
        /// </exception>
        public long ToLong()
        {
            if (IsInteger())
            {
                return Convert.ToInt64(ToString());
            }
            return (long) ToDouble();
        }

        /// <summary>Convert the value of this token to a double.</summary>
        /// <exception cref="System.FormatException">
        /// if the token does not contain a parsable
        /// double or fraction.
        /// </exception>
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
            catch (XmlSyntaxException)
            {
            }
            throw new FormatException("invalid fraction " + fraction);
        }

        private double TextAsDouble()
        {
            return double.Parse(ToString());
        }

        /// <summary>Convert the value of this token to a boolean.</summary>
        /// <seealso cref="bool.()"/>
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
            if (!(o is XmlToken)) return false;
            var token = (XmlToken) o;
            return _tokenType == token._tokenType && _text.Equals(token._text);
        }

        /// <summary>Create a debug string representation of this Token.</summary>
        public string DebugString()
        {
            return "token " + _tokenType + " [" + ToString() + ']';
        }

        /// <summary>Get the text associated with this token as a byte array.</summary>
        /// <remarks>
        /// Get the text associated with this token as a byte array.  The returned
        /// value is not a copy so beware not to muck with it.
        /// </remarks>
        public byte[] GetTextAsBytes()
        {
            return Encoding.ASCII.GetBytes(_text);
        }

        /// <summary>Return the hash code of the token.</summary>
        public override int GetHashCode()
        {
            return _tokenType.GetHashCode() + (_text.GetHashCode() << 4);
        }
    }
}