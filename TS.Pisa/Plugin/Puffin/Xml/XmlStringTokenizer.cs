using System;
using System.IO;
using System.Text;

namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <summary>
    /// This class provides an Xml language tokenizer for well-formed Xml
    /// expressions.
    /// </summary>
    /// <remarks>
    /// This class provides an Xml language tokenizer for well-formed Xml
    /// expressions.  This is basically a cut-down Xml lexical analyser.  It cannot
    /// handle all elements of Xml but it can handle most; in particular it can
    /// handle the typical Xml documents that are used for Tradingscreen
    /// inter-process communication messaging.  It is designed to be fast rather
    /// than comprehensive.
    /// <p> The most notable Xml features that are not supported include: unicode
    /// characters, directives, processing instructions and comments.  None of
    /// these are required for IPC.
    /// </remarks>
    /// <author>Paul Sweeny</author>
    public class XmlStringTokenizer : AbstractXmlTokenizer
    {
        private const int Initial = 0;
        private const int Tag = 1;
        private const int Content = 2;
        private const int AttributeList = 3;
        private const int AttributeName = 4;
        private const int AttributeValue = 5;
        private const int EndTag = 6;
        private const int WsInEndTag = 7;
        private const int EmptyTag = 8;
        private const int RefNeedMp = 1;
        private const int RefNeedP = 2;
        private const int RefNeedU = 3;
        private const int RefNeedO = 4;
        private const int RefNeedS = 5;
        private const int RefNeedT = 6;
        private const int RefNeedSemi = 7;
        private const int Numeric = 8;
        private const int Decimal = 9;
        private const int HexStart = 10;
        private const int Hex = 11;
        private const int RefComplete = 12;
        private const int NumStart = 0;
        private const int NumSign = 1;
        private const int NumInteger = 2;
        private const int NumPoint = 3;
        private const int NumDouble = 4;
        private const int NumExponent = 5;
        private const int NumExpSign = 6;
        private const int NumExpDouble = 7;
        private const int FractionStart = 0;
        private const int Numerator = 1;
        private const int Denominator = 2;
        private const int Fraction = 3;
        private const int CharRange = 128;
        private static bool[] gWhitespace = new bool[CharRange];

        private static bool[] gContent = new bool[CharRange];
        private static bool[] gFName = new bool[CharRange];

        private static bool[] gName = new bool[CharRange];

        static XmlStringTokenizer()
        {
            for (var c = 0; c < CharRange; ++c)
            {
                gWhitespace[c] = c == ' ' || c == '\t' || c == '\n' || c == '\r';
                gContent[c] = c != '<' && c != '&';
                gFName[c] = (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_' || c == ':';
                gName[c] = gFName[c] || (c >= '0' && c <= '9') || c == '-' || c == '.';
            }
        }

        private int _state = Initial;

        private readonly XmlTemporaryToken _startTmp;
        private readonly XmlTemporaryToken _nameTmp;

        private readonly XmlTemporaryToken _endTmp;

        /// <summary>Construct a new tokenizer.</summary>
        /// <param name="xml">the Xml document to tokenize.</param>
        /// <param name="end">the end of the Xml buffer.</param>
        public XmlStringTokenizer(byte[] xml, int end)
            : base(xml)
        {
            _end = end;
            _startTmp = new XmlTemporaryToken(XmlToken.StartType, _buffer);
            _nameTmp = new XmlTemporaryToken(XmlToken.NameType, _buffer);
            _endTmp = new XmlTemporaryToken(XmlToken.EndType, _buffer);
        }

        /// <summary>Construct a new parser for the given Xml string.</summary>
        /// <param name="xml">the Xml document to tokenize.</param>
        public XmlStringTokenizer(string xml)
            : this(Encoding.ASCII.GetBytes(xml), xml.Length)
        {
        }

        /// <summary>Parse and then return the next token from the Xml input stream.</summary>
        /// <returns>the next read XmlToken.</returns>
        /// <exception cref="XmlSyntaxException">if there was an error while parsing the Xml.</exception>
        /// <exception cref="XmlSyntaxException"/>
        public override XmlToken NextToken()
        {
            try
            {
                for (_mark = ++_point; _point < _end || FillBuffer(); ++_point)
                {
                    int b = _buffer[_point];
                    switch (_state)
                    {
                        case Initial:
                        {
                            if (b == '<')
                            {
                                _state = Tag;
                            }
                            else
                            {
                                if (gWhitespace[b])
                                {
                                    continue;
                                }
                                else
                                {
                                    --_point;
                                    _state = Content;
                                }
                            }
                            break;
                        }
                        case Tag:
                        {
                            if (gFName[b])
                            {
                                b = ParseName();
                                if (b == '>')
                                {
                                    _state = Content;
                                }
                                else
                                {
                                    if (b == '/')
                                    {
                                        _state = EmptyTag;
                                    }
                                    else
                                    {
                                        if (gWhitespace[b])
                                        {
                                            _state = AttributeList;
                                        }
                                        else
                                        {
                                            throw Error("tag name expected");
                                        }
                                    }
                                }
                                return XmlToken.GetInstance(_startTmp.Reset(_mark, _point - _mark));
                            }
                            else
                            {
                                if (b == '/')
                                {
                                    _state = EndTag;
                                }
                                else
                                {
                                    throw Error("tag or end-tag name expected");
                                }
                            }
                            break;
                        }
                        case Content:
                        {
                            _state = Tag;
                            if (b == '<')
                            {
                                continue;
                            }
                            _mark = _point;
                            return ParseContent('<', XmlToken.ContentType);
                        }
                        case AttributeList:
                        {
                            if (gFName[b])
                            {
                                b = ParseName();
                                if (b == '=')
                                {
                                    _state = AttributeName;
                                }
                                else
                                {
                                    throw Error("attribute name and equals expected");
                                }
                                return XmlToken.GetInstance(_nameTmp.Reset(_mark, _point - _mark));
                            }
                            else
                            {
                                if (b == '>')
                                {
                                    _state = Content;
                                }
                                else
                                {
                                    if (b == '/')
                                    {
                                        _state = EmptyTag;
                                    }
                                    else
                                    {
                                        if (gWhitespace[b])
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            throw Error("whitespace or tag close expected");
                                        }
                                    }
                                }
                            }
                            break;
                        }
                        case AttributeName:
                        {
                            if (b == '"' || b == '\'')
                            {
                                _state = AttributeValue;
                                ++_point;
                                return ParseValue(b);
                            }
                            throw Error("attribute value expected");
                        }
                        case AttributeValue:
                        {
                            if (gWhitespace[b])
                            {
                                _state = AttributeList;
                            }
                            else
                            {
                                if (b == '>')
                                {
                                    _state = Content;
                                }
                                else
                                {
                                    if (b == '/')
                                    {
                                        _state = EmptyTag;
                                    }
                                    else
                                    {
                                        throw Error("whitespace or tag close expected");
                                    }
                                }
                            }
                            break;
                        }
                        case EndTag:
                        {
                            if (gFName[b])
                            {
                                b = ParseName();
                                if (b == '>')
                                {
                                    _state = Content;
                                }
                                else
                                {
                                    if (gWhitespace[b])
                                    {
                                        _state = WsInEndTag;
                                    }
                                    else
                                    {
                                        throw Error("end-tag name expected");
                                    }
                                }
                                return XmlToken.GetInstance(_endTmp.Reset(_mark, _point - _mark));
                            }
                            throw Error("end-tag name expected");
                        }
                        case WsInEndTag:
                        {
                            if (b == '>')
                            {
                                _state = Content;
                            }
                            else
                            {
                                if (gWhitespace[b])
                                {
                                    continue;
                                }
                                else
                                {
                                    throw Error("whitespace or end-tag close expected");
                                }
                            }
                            break;
                        }
                        case EmptyTag:
                        {
                            if (b == '>')
                            {
                                _state = Content;
                            }
                            else
                            {
                                throw Error("empty-tag close expected");
                            }
                            return XmlToken.EmptyToken;
                        }
                        default:
                        {
                            throw new XmlParseException("parser programming error");
                        }
                    }
                }
                return null;
            }
            catch (IndexOutOfRangeException)
            {
                int b = _buffer[_point];
                if (b < 0 || b > 126)
                {
                    throw Error("unexpected control character");
                }
                throw;
            }
        }

        /// <summary>Parse an Xml name, such as an attribute name.</summary>
        /// <exception cref="XmlSyntaxException">if there was an error while parsing the Xml.</exception>
        /// <exception cref="XmlSyntaxException"/>
        private int ParseName()
        {
            for (_mark = _point; _point < _end || FillBuffer(); ++_point)
            {
                int b = _buffer[_point];
                if (!gName[b])
                {
                    return b;
                }
            }
            throw Error("unexpected end of document while parsing name");
        }

        /// <summary>Parse Xml message content.</summary>
        /// <param name="terminator">the character that terminates a run of Xml content.</param>
        /// <param name="type">the token type to apply content to.</param>
        /// <returns>the resulting XmlToken.</returns>
        /// <exception cref="XmlSyntaxException">if there was an error while parsing the Xml.</exception>
        /// <exception cref="XmlSyntaxException"/>
        private XmlToken ParseContent(int terminator, int type)
        {
            MemoryStream fragments = null;
            for (; _point < _end || FillBuffer(); ++_point)
            {
                int b = _buffer[_point];
                if (b == terminator)
                {
                    if (fragments == null)
                    {
                        return new XmlToken(type, _buffer, _mark, _point - _mark);
                    }
                    else
                    {
                        fragments.Write(_buffer, _mark, _point - _mark);
                        byte[] buf = fragments.ToArray();
                        return new XmlToken(type, buf, 0, buf.Length);
                    }
                }
                if (gContent[b])
                {
                    continue;
                }
                if (b == '&')
                {
                    if (fragments == null)
                    {
                        fragments = new MemoryStream();
                    }
                    fragments.Write(_buffer, _mark, _point - _mark);
                    fragments.WriteByte((byte) CharacterReference());
                    _mark = _point;
                    ++_mark;
                    continue;
                }
                throw Error("content or tag expected");
            }
            if (NothingButWhitespace())
            {
                return null;
            }
            throw Error("unexpected end of document while parsing content");
        }

        /// <summary>Check if the marked text is nothing but whitespace.</summary>
        private bool NothingButWhitespace()
        {
            for (int i = _mark; i < _point; ++i)
            {
                if (!gWhitespace[_buffer[i]])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>Parse an Xml attribute value.</summary>
        /// <param name="terminator">the character that terminates the value.</param>
        /// <returns>the resulting XmlToken.</returns>
        /// <exception cref="XmlSyntaxException">if there was an error while parsing the Xml.</exception>
        /// <exception cref="XmlSyntaxException"/>
        private XmlToken ParseValue(int terminator)
        {
            int state = NumStart;
            for (_mark = _point; _point < _end || FillBuffer(); ++_point)
            {
                int b = _buffer[_point];
                switch (state)
                {
                    case NumStart:
                    {
                        if (b <= '9' && b >= '0')
                        {
                            state = NumInteger;
                            continue;
                        }
                        if (b == '-' || b == '+')
                        {
                            state = NumSign;
                            continue;
                        }
                        if (b == '.')
                        {
                            state = NumPoint;
                            continue;
                        }
                        break;
                    }
                    case NumSign:
                    {
                        if (b <= '9' && b >= '0')
                        {
                            state = NumInteger;
                            continue;
                        }
                        if (b == '.')
                        {
                            state = NumPoint;
                            continue;
                        }
                        break;
                    }
                    case NumInteger:
                    {
                        if (b <= '9' && b >= '0')
                        {
                            continue;
                        }
                        if (b == terminator)
                        {
                            return new XmlToken(XmlToken.IntegerValueType, _buffer, _mark, _point - _mark);
                        }
                        if (b == '.')
                        {
                            state = NumDouble;
                            continue;
                        }
                        if (b == ' ')
                        {
                            return ParseFraction(terminator);
                        }
                        break;
                    }
                    case NumPoint:
                    {
                        if (b <= '9' && b >= '0')
                        {
                            state = NumDouble;
                            continue;
                        }
                        break;
                    }
                    case NumDouble:
                    {
                        if (b <= '9' && b >= '0')
                        {
                            continue;
                        }
                        if (b == terminator)
                        {
                            return new XmlToken(XmlToken.DoubleValueType, _buffer, _mark, _point - _mark);
                        }
                        if (b == 'e' || b == 'E')
                        {
                            state = NumExponent;
                            continue;
                        }
                        break;
                    }
                    case NumExponent:
                    {
                        if (b <= '9' && b >= '0')
                        {
                            state = NumExpDouble;
                            continue;
                        }
                        if (b == '-' || b == '+')
                        {
                            state = NumExpSign;
                            continue;
                        }
                        break;
                    }
                    case NumExpSign:
                    {
                        if (b <= '9' && b >= '0')
                        {
                            state = NumExpDouble;
                            continue;
                        }
                        break;
                    }
                    case NumExpDouble:
                    {
                        if (b <= '9' && b >= '0')
                        {
                            continue;
                        }
                        if (b == terminator)
                        {
                            return new XmlToken(XmlToken.DoubleValueType, _buffer, _mark, _point - _mark);
                        }
                        break;
                    }
                    default:
                    {
                        throw new XmlParseException("parser programming error");
                    }
                }
                if (b == terminator && _mark == _point)
                {
                    return XmlToken.NullValueToken;
                }
                return ParseContent(terminator, XmlToken.StringValueType);
            }
            throw Error("unexpected end of document while parsing value");
        }

        /// <summary>Parse an Xml attribute fraction value.</summary>
        /// <param name="terminator">the character that terminates the value.</param>
        /// <returns>the resulting XmlToken.</returns>
        /// <exception cref="XmlSyntaxException">if there was an error while parsing the Xml.</exception>
        /// <exception cref="XmlSyntaxException"/>
        private XmlToken ParseFraction(int terminator)
        {
            int state = FractionStart;
            for (++_point; _point < _end || FillBuffer(); ++_point)
            {
                int b = _buffer[_point];
                switch (state)
                {
                    case FractionStart:
                    {
                        if (b <= '9' && b >= '0')
                        {
                            state = Numerator;
                            continue;
                        }
                        break;
                    }
                    case Numerator:
                    {
                        if (b <= '9' && b >= '0')
                        {
                            continue;
                        }
                        if (b == '/')
                        {
                            state = Denominator;
                        }
                        continue;
                    }
                    case Denominator:
                    {
                        //break;
                        if (b <= '9' && b >= '0')
                        {
                            state = Fraction;
                            continue;
                        }
                        break;
                    }
                    case Fraction:
                    {
                        if (b <= '9' && b >= '0')
                        {
                            continue;
                        }
                        if (b == terminator)
                        {
                            return new XmlToken(XmlToken.FractionValueType, _buffer, _mark, _point - _mark);
                        }
                        break;
                    }
                    default:
                    {
                        throw new XmlParseException("parser programming error");
                    }
                }
                return ParseContent(terminator, XmlToken.StringValueType);
            }
            throw Error("unexpected end of document while parsing fraction");
        }

        /// <summary>Parse an Xml character reference such as "&lt;".</summary>
        /// <returns>the character equivalent of the reference.</returns>
        /// <exception cref="XmlSyntaxException">if there was an error while parsing the Xml.</exception>
        /// <exception cref="XmlSyntaxException"/>
        private int CharacterReference()
        {
            int value = 0;
            int state = Initial;
            while (++_point < _end || FillBuffer())
            {
                int b = _buffer[_point];
                switch (state)
                {
                    case Initial:
                    {
                        if (b == '#')
                        {
                            state = Numeric;
                        }
                        else
                        {
                            if (b == 'a')
                            {
                                state = RefNeedMp;
                            }
                            else
                            {
                                if (b == 'q')
                                {
                                    state = RefNeedU;
                                    value = '"';
                                }
                                else
                                {
                                    if (b == 'l')
                                    {
                                        state = RefNeedT;
                                        value = '<';
                                    }
                                    else
                                    {
                                        if (b == 'g')
                                        {
                                            state = RefNeedT;
                                            value = '>';
                                        }
                                        else
                                        {
                                            throw Error("character entity reference expected");
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                    case RefNeedMp:
                    {
                        if (b == 'm')
                        {
                            state = RefNeedP;
                            value = '&';
                        }
                        else
                        {
                            if (b == 'p')
                            {
                                state = RefNeedO;
                                value = '\'';
                            }
                            else
                            {
                                throw Error("character entity reference expected");
                            }
                        }
                        break;
                    }
                    case RefNeedP:
                    {
                        if (b == 'p')
                        {
                            state = RefNeedSemi;
                        }
                        else
                        {
                            throw Error("character entity reference expected");
                        }
                        break;
                    }
                    case RefNeedU:
                    {
                        if (b == 'u')
                        {
                            state = RefNeedO;
                        }
                        else
                        {
                            throw Error("character entity reference expected");
                        }
                        break;
                    }
                    case RefNeedO:
                    {
                        if (b == 'o')
                        {
                            state = (value == '"') ? RefNeedT : RefNeedS;
                        }
                        else
                        {
                            throw Error("character entity reference expected");
                        }
                        break;
                    }
                    case RefNeedS:
                    {
                        if (b == 's')
                        {
                            state = RefNeedSemi;
                        }
                        else
                        {
                            throw Error("character entity reference expected");
                        }
                        break;
                    }
                    case RefNeedT:
                    {
                        if (b == 't')
                        {
                            state = RefNeedSemi;
                        }
                        else
                        {
                            throw Error("character entity reference expected");
                        }
                        break;
                    }
                    case RefNeedSemi:
                    {
                        if (b == ';')
                        {
                            return value;
                        }
                        throw Error("character entity reference expected");
                    }
                    case Numeric:
                    {
                        // break;
                        if (b == 'x')
                        {
                            state = HexStart;
                        }
                        else
                        {
                            if (b <= '9' && b >= '0')
                            {
                                value = b - '0';
                                state = Decimal;
                            }
                            else
                            {
                                throw Error("numeric character reference expected");
                            }
                        }
                        break;
                    }
                    case Decimal:
                    {
                        if (b <= '9' && b >= '0')
                        {
                            value = value * 10 + b - '0';
                        }
                        else
                        {
                            if (b == ';')
                            {
                                return value;
                            }
                            else
                            {
                                throw Error("decimal character reference expected");
                            }
                        }
                        break;
                    }
                    case HexStart:
                    {
                        if (b == ';')
                        {
                            b = 0;
                        }
                        // error
                        state = Hex;
                        goto case Hex;
                    }
                    case Hex:
                    {
                        // fall through
                        if (b <= '9' && b >= '0')
                        {
                            value = value * 16 + b - '0';
                        }
                        else
                        {
                            if (b >= 'a' && b <= 'f')
                            {
                                value = value * 16 + b - 'a';
                            }
                            else
                            {
                                if (b >= 'A' && b <= 'F')
                                {
                                    value = value * 16 + b - 'A';
                                }
                                else
                                {
                                    if (b == ';')
                                    {
                                        return value;
                                    }
                                    else
                                    {
                                        throw Error("hex character reference expected");
                                    }
                                }
                            }
                        }
                        break;
                    }
                    default:
                    {
                        throw new XmlParseException("parser programming error");
                    }
                }
            }
            throw Error("unexpected end of document while parsing char reference");
        }

        /// <summary>Skip whitespace at the point.</summary>
        /// <exception cref="XmlSyntaxException"/>
        public virtual void SkipWhitespace()
        {
            for (_mark = ++_point; _point < _end || FillBuffer(); ++_point)
            {
                if (!gWhitespace[_buffer[_point]])
                {
                    break;
                }
            }
            --_point;
        }

        /// <summary>Fill the buffer.</summary>
        /// <returns>true if there is more data to read and false otherwise.</returns>
        /// <exception cref="XmlSyntaxException">
        /// if an I/O error occurred while refilling the
        /// buffer.
        /// </exception>
        /// <exception cref="XmlSyntaxException"/>
        public virtual bool FillBuffer()
        {
            return false;
        }
    }
}