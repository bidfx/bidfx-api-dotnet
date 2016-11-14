using System;
using System.Collections.Generic;
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
    public class XmlToken : IXmlFormatable
    {
        /// <summary>The Xml end-tag token type.</summary>
        public const int EndType = 0;

        /// <summary>The Xml empty-tag close token type.</summary>
        public const int EmptyType = 1;

        /// <summary>The Xml start-tag token type.</summary>
        public const int StartType = 2;

        public const int ContentType = 3;

        /// <summary>The Xml element attribute name token type.</summary>
        public const int NameType = 4;

        /// <summary>The Xml element integer attribute value token type.</summary>
        public const int IntegerValueType = 5;

        /// <summary>The Xml element double attribute value token type.</summary>
        public const int DoubleValueType = 6;

        /// <summary>The Xml element fraction value token type.</summary>
        public const int FractionValueType = 7;

        /// <summary>The Xml element string attribute value token type.</summary>
        public const int StringValueType = 8;

        /// <summary>One greater than the largest token type.</summary>
        public const int MaxType = 9;

        private static readonly string[] gTokenType = new string[]
        {
            "END-TAG", "EMPTY-ELEMENT", "START-TAG", "CONTENT", "NAME", "INTEGER-VALUE", "DOUBLE-VALUE",
            "FRACTION-VALUE", "STRING-VALUE"
        };

        private static readonly byte[] Blank = new byte[0];

        private static readonly IDictionary<XmlTemporaryToken, XmlToken> gCommonTokenPool =
            new Dictionary<XmlTemporaryToken, XmlToken>();

        public static readonly XmlToken EmptyToken = new XmlToken(EmptyType, Blank, 0, 0, EmptyType);
        public static readonly XmlToken NullValueToken = new XmlToken(StringValueType, Blank, 0, 0, StringValueType);

        public static readonly XmlToken NullContentToken = new XmlToken(ContentType, Blank, 0, 0, ContentType);
        public static readonly XmlToken ZeroToken = new XmlToken(0);

        private readonly int _type;
        private readonly byte[] _text;

        private int _hash;

        /// <summary>
        /// Create a new token of a given type by copying its associated text from
        /// a region within a character array.
        /// </summary>
        /// <param name="type">the type of the token.</param>
        /// <param name="text">a buffer containing the text associated with the token.</param>
        /// <param name="start">the start index of the text within the buffer.</param>
        /// <param name="length">the length of the text associated with the token.</param>
        public XmlToken(int type, char[] text, int start, int length)
        {
            //private final byte[] _text;
            if (type < 0 || type >= MaxType)
            {
                throw new XmlSyntaxException("bad token type: " + type);
            }
            _type = type;
            _text = new byte[length];
            for (int i = 0; i < length; ++i)
            {
                _text[i] = unchecked((byte) text[start++]);
            }
        }

        /// <summary>
        /// Create a new token of a given type by copying its associated text from
        /// a region within a byte array.
        /// </summary>
        /// <param name="type">the type of the token.</param>
        /// <param name="buf">a buffer containing the text associated with the token.</param>
        /// <param name="start">the start index of the text within the buffer.</param>
        /// <param name="length">the length of the text associated with the token.</param>
        public XmlToken(int type, byte[] buf, int start, int length)
            : this(type, buf, start, length, 0)
        {
        }

        /// <summary>
        /// Create a new token of a given type by copying its associated text from
        /// a region within a byte array.
        /// </summary>
        /// <param name="type">the type of the token.</param>
        /// <param name="buf">a buffer containing the text associated with the token.</param>
        /// <param name="start">the start index of the text within the buffer.</param>
        /// <param name="length">the length of the text associated with the token.</param>
        /// <param name="hash">the hash code for the token.</param>
        public XmlToken(int type, byte[] buf, int start, int length, int hash)
        {
            _type = type;
            _text = new byte[length];
            System.Array.Copy(buf, start, _text, 0, length);
            _hash = hash;
        }

        /// <summary>Create a new token of a given type and associated text.</summary>
        /// <param name="type">the type of the token.</param>
        /// <param name="text">the text associated with the token.</param>
        public XmlToken(int type, string text)
        {
            //mText = new byte[text.length()];
            //text.getBytes(0, _text.length, _text, 0);
            _type = type;
            _text = Encoding.ASCII.GetBytes(text);
        }

        /// <summary>Create a new token representing an attribute-value.</summary>
        /// <param name="value">the value of the attribute.</param>
        public XmlToken(string value)
            : this(StringValueType, value)
        {
        }

        /// <summary>Create a new token representing an attribute-value.</summary>
        /// <param name="value">the value of the attribute.</param>
        public XmlToken(int value)
            : this(IntegerValueType, value.ToString())
        {
        }

        /// <summary>Create a new token representing an attribute-value.</summary>
        /// <param name="value">the value of the attribute.</param>
        public XmlToken(long value)
            : this(IntegerValueType, value.ToString())
        {
        }

        /// <summary>Create a new token representing an attribute-value.</summary>
        /// <param name="value">the value of the attribute.</param>
        public XmlToken(double value)
            : this(DoubleValueType, ToString(value))
        {
        }

        private static string ToString(double value)
        {
            return value.ToString();
        }

        /// <summary>Create a new token representing an attribute-value.</summary>
        /// <param name="value">the value of the attribute.</param>
        public XmlToken(bool value)
            : this(StringValueType, value?"true":"false")
        {
        }

        /// <summary>
        /// Convert a token (normally a start tag) into an end tag token with the
        /// same text.
        /// </summary>
        public XmlToken ToEndTag()
        {
            return new XmlToken(this);
        }

        private XmlToken(XmlToken startTag)
        {
            _type = EndType;
            _text = startTag._text;
        }

        /// <summary>Get the type of this token.</summary>
        public int TokenType()
        {
            return _type;
        }

        /// <summary>Check if this token is an Xml end-tag token.</summary>
        public bool IsEndTag()
        {
            return _type == EndType;
        }

        /// <summary>Check if this token is an Xml empty-tag close token.</summary>
        public bool IsEmptyTag()
        {
            return _type == EmptyType;
        }

        /// <summary>Check if this token is an Xml start-tag token.</summary>
        public bool IsStartTag()
        {
            return _type == StartType;
        }

        /// <summary>Check if this token is an Xml content token.</summary>
        public bool IsTagContent()
        {
            return _type == ContentType;
        }

        /// <summary>Check if this token is an Xml element attribute name token.</summary>
        public bool IsAttributeName()
        {
            return _type == NameType;
        }

        /// <summary>
        /// Check if this token is an attribute-value type:
        /// integer, double, fraction or string.
        /// </summary>
        public bool IsValueType()
        {
            return _type >= IntegerValueType;
        }

        /// <summary>
        /// Check if this token is a number type: integer, double
        /// or fraction.
        /// </summary>
        public bool IsNumberType()
        {
            return _type == IntegerValueType || _type == DoubleValueType || _type == FractionValueType;
        }

        /// <summary>Check if this token is null (not present).</summary>
        public bool IsNull()
        {
            return false;
        }

        /// <summary>Check if this token is an integer attribute-value type.</summary>
        public bool IsInteger()
        {
            return _type == IntegerValueType;
        }

        /// <summary>Check if this token is a double attribute-value type.</summary>
        public bool IsDouble()
        {
            return _type == DoubleValueType;
        }

        /// <summary>Check if this token is a fraction attribute-value type.</summary>
        public bool IsFraction()
        {
            return _type == FractionValueType;
        }

        /// <summary>Check if this token is a string attribute-value type.</summary>
        public bool IsString()
        {
            return _type == StringValueType;
        }

        /// <summary>Check if this token is negative.</summary>
        public bool IsNegative()
        {
            return _text[0] == '-';
        }

        /// <summary>Get the text associated with this token.</summary>
        /// <returns>the associated text or "" if the token has no text.</returns>
        public string GetText()
        {
            return ToString();
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
            return _text.Length == 0 ? string.Empty : Encoding.ASCII.GetString(_text, 0, _text.Length);
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

        public override bool Equals(object @object)
        {
            if (@object == this)
            {
                return true;
            }
            if (@object != null && @object is XmlToken)
            {
                XmlToken token = (XmlToken) @object;
                if (token._type != _type)
                {
                    return false;
                }
                if (token._text.Length != _text.Length)
                {
                    return false;
                }
                for (int i = _text.Length; i-- > 0;)
                {
                    if (token._text[i] != _text[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>Create a debug string representation of this Token.</summary>
        public string DebugString()
        {
            return "token " + gTokenType[_type] + " [" + ToString() + ']';
        }

        /// <summary>Get the text associated with this token as a byte array.</summary>
        /// <remarks>
        /// Get the text associated with this token as a byte array.  The returned
        /// value is not a copy so beware not to muck with it.
        /// </remarks>
        public byte[] GetTextAsBytes()
        {
            return _text;
        }

        /// <summary>Return the hash code of the token.</summary>
        public override int GetHashCode()
        {
            if (_hash != 0)
            {
                return _hash;
            }
            return _hash = ComputeHash(_type, _text, 0, _text.Length);
        }

        /// <summary>Compute the hash code.</summary>
        /// <remarks>
        /// Compute the hash code.  This method is separated out from hashCode() so
        /// that it may be shared by XmlTemporaryToken.
        /// </remarks>
        /// <param name="type">the type of the token.</param>
        /// <param name="buf">a buffer containing the text associated with the token.</param>
        /// <param name="pos">the start position of the text within the buffer.</param>
        /// <param name="length">the length of the text associated with the token.</param>
        public static int ComputeHash(int type, byte[] buf, int pos, int length)
        {
            int i = 0;
            for (; i < length && i < 7; ++i)
            {
                type = (type << 4) + buf[pos++];
            }
            for (; i < length; ++i)
            {
                type = ((type ^ ((int) (((uint) type) >> 28))) << 4) + buf[pos++];
            }
            return type;
        }

        /// <summary>Get the token instance that matches the given temporary token.</summary>
        /// <remarks>
        /// Get the token instance that matches the given temporary token.  The
        /// returned instance is taken from a central pool of commonly used tokens.
        /// If is it not already in this pool then it will be created and added to
        /// it first.
        /// </remarks>
        /// <param name="temp">the temporary token to compare against pooled tokens.</param>
        public static XmlToken GetInstance(XmlTemporaryToken temp)
        {
            XmlToken token;
            gCommonTokenPool.TryGetValue(temp, out token);
            if (token == null)
            {
                token = temp.ToToken();
                gCommonTokenPool.Add(token.ToTempToken(), token);
            }
            return token;
        }

        /// <summary>Get the token instance that matches the given type and text.</summary>
        /// <remarks>
        /// Get the token instance that matches the given type and text.  The
        /// returned instance is taken from a central pool of commonly used tokens.
        /// If is it not already in this pool then it will be created and added to
        /// it first.
        /// </remarks>
        /// <param name="type">the type of the token to obtain.</param>
        /// <param name="text">the text of the token to obtain.</param>
        public static XmlToken GetInstance(int type, string text)
        {
            XmlToken token = new XmlToken(type, text);
            XmlTemporaryToken temp = token.ToTempToken();
            XmlToken found;
            gCommonTokenPool.TryGetValue(temp, out found);
            if (found == null)
            {
                found = token;
                gCommonTokenPool.Add(temp, found);
            }
            return found;
        }

        /// <summary>Format this object using the given formatter.</summary>
        /// <exception cref="System.IO.IOException">if formatting fails due to an I/O error.</exception>
        /// <exception cref="XmlSyntaxException">if formatting fails due to invalid Xml.</exception>
        /// <exception cref="XmlSyntaxException"/>
        public void FormatUsing(IXmlFormatter formatter)
        {
            formatter.Format(this);
        }

        private XmlTemporaryToken ToTempToken()
        {
            return new XmlTemporaryToken(_type, _text, _hash);
        }
    }
}