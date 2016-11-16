using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <summary>
    /// This class provides a compressed Xml language inflate tokenizer for well-formed Xml expressions.
    /// </summary>
    /// <author>Paul Sweeny</author>
    public class XmlBinaryInflater
    {
        private readonly byte[] _buffer = new byte[8192];
        private int _end;
        private int _point = -1;
        private int _mark;

        private readonly Stream _inStream;
        private readonly XmlDictionary _dictionary = new XmlDictionary();
        private readonly Stack<XmlToken> _tagStack = new Stack<XmlToken>();
        private readonly Stack<XmlElement> _elementStack = new Stack<XmlElement>();

        private enum State
        {
            FirstByte,
            SecondByte,
            ScanUnseenToken
        }

        public XmlBinaryInflater(Stream inStream)
        {
            _inStream = inStream;
        }

        public XmlElement NextElement()
        {
            var token = NextToken();
            return NextElement(token);
        }

        private XmlElement NextElement(XmlToken token)
        {
            if (token == null)
            {
                return null;
            }
            try
            {
                if (!token.IsStartTag())
                {
                    throw new XmlSyntaxException("start tag expected");
                }
                var element = new XmlElement(token.GetText());
                while ((token = NextToken()) != null)
                {
                    switch (token.TokenType())
                    {
                        case XmlTokenType.TagEnd:
                        case XmlTokenType.TagEndEmptyContent:
                        {
                            if (_elementStack.Count == 0)
                            {
                                return element;
                            }
                            element = _elementStack.Pop();
                            break;
                        }
                        case XmlTokenType.TagStart:
                        {
                            _elementStack.Push(element);
                            element = new XmlElement(token.GetText());
                            _elementStack.Peek().AddElement(element);
                            break;
                        }
                        case XmlTokenType.AttributeName:
                        {
                            var name = token;
                            token = NextToken();
                            if (token == null)
                            {
                                break;
                            }
                            element.AddAttribute(name.GetText(), token);
                            break;
                        }
                        case XmlTokenType.AttributeValueInteger:
                        case XmlTokenType.AttributeValueDouble:
                        case XmlTokenType.AttributeValueFraction:
                        case XmlTokenType.AttributeValueString:
                        case XmlTokenType.NestedContent:
                        {
                            throw new XmlSyntaxException("attribute value with no name " + token);
                        }
                        default:
                        {
                            throw new XmlSyntaxException("unknown token type " + token);
                        }
                    }
                }
                throw new XmlSyntaxException("unexpected document termination");
            }
            catch (XmlSyntaxException e)
            {
                throw new XmlSyntaxException(
                    e.Message + (token == null ? string.Empty : "; reading " + token.DebugString()));
            }
        }

        private bool FillBuffer()
        {
            try
            {
                if (_mark > 0)
                {
                    _end -= _mark;
                    _point -= _mark;
                    if (_end < 0)
                    {
                        _end = 0;
                    }
                    if (_end > 0)
                    {
                        Array.Copy(_buffer, _mark, _buffer, 0, _end);
                    }
                    _mark = 0;
                    var got = _inStream.Read(_buffer, _end, _buffer.Length - _end);
                    if (got == -1)
                    {
                        return false;
                    }
                    _end += got;
                }
                while (_point >= _end)
                {
                    if (_point >= _buffer.Length)
                    {
                        throw new XmlSyntaxException("input buffer too small " + _buffer.Length);
                    }
                    var got = _inStream.Read(_buffer, _end, _buffer.Length - _end);
                    if (got == -1)
                    {
                        return false;
                    }
                    _end += got;
                }
                return true;
            }
            catch (IOException e)
            {
                throw new XmlSyntaxException(e.Message);
            }
        }

        private XmlToken NextToken()
        {
            var state = State.FirstByte;
            for (_mark = ++_point; _point < _end || FillBuffer(); ++_point)
            {
                var b = _buffer[_point];
                switch (state)
                {
                    case State.FirstByte:
                    {
                        if (XmlDictionary.IsFirstByteOfToken(b))
                        {
                            state = State.SecondByte;
                        }
                        else
                        {
                            if (XmlDictionary.IsTokenType(b))
                            {
                                if (b == (byte) XmlTokenType.TagEnd)
                                {
                                    return _tagStack.Pop().ToEndTag();
                                }
                                if (b == (byte) XmlTokenType.TagEndEmptyContent)
                                {
                                    _tagStack.Pop();
                                    return XmlToken.EmptyToken;
                                }
                                state = State.ScanUnseenToken;
                            }
                            else
                            {
                                throw new XmlSyntaxException("token tag expected instead of " + b
                                                             + " ('" + (char) b + "') at position " + _point);
                            }
                        }
                        break;
                    }
                    case State.SecondByte:
                    {
                        XmlToken token;
                        if (XmlDictionary.IsSecondByteOfToken(b))
                        {
                            token = _dictionary.GetToken(_buffer[_mark], b);
                        }
                        else
                        {
                            token = _dictionary.GetToken(_buffer[_mark]);
                            --_point;
                        }
                        if (!token.IsStartTag()) return token;
                        _tagStack.Push(token);
                        return token;
                    }
                    case State.ScanUnseenToken:
                    {
                        if (!XmlDictionary.IsPlainText(b))
                        {
                            var type = (XmlTokenType) _buffer[_mark];
                            if (_mark < --_point)
                            {
                                XmlToken token;
                                var text = MarkedText();
                                switch (type)
                                {
                                    case XmlTokenType.TagStart:
                                        token = new XmlToken(type, text, text);
                                        _tagStack.Push(token);
                                        break;
                                    case XmlTokenType.AttributeName:
                                        token = new XmlToken(type, text, text);
                                        break;
                                    case XmlTokenType.TagEnd:
                                        token = new XmlToken(type, text, text);
                                        break;
                                    case XmlTokenType.TagEndEmptyContent:
                                        token = XmlToken.EmptyToken;
                                        break;
                                    case XmlTokenType.NestedContent:
                                        token = XmlToken.NullContentToken;
                                        break;
                                    case XmlTokenType.AttributeValueInteger:
                                        token = new XmlToken(type, text, Convert.ToInt32(text));
                                        break;
                                    case XmlTokenType.AttributeValueDouble:
                                        token = new XmlToken(type, text, Convert.ToDouble(text));
                                        break;
                                    case XmlTokenType.AttributeValueFraction:
                                        token = new XmlToken(type, text, XmlToken.FractionToDouble(text));
                                        break;
                                    case XmlTokenType.AttributeValueString:
                                        token = new XmlToken(type, text, text);
                                        break;
                                    default:
                                        throw new XmlSyntaxException("unrecognised token " + type);
                                }
                                _dictionary.Insert(token);
                                return token;
                            }
                            if (type == XmlTokenType.AttributeValueString)
                            {
                                return XmlToken.NullValueToken;
                            }
                            if (type == XmlTokenType.NestedContent)
                            {
                                return XmlToken.NullContentToken;
                            }
                            throw new XmlSyntaxException("text of previously unseen token expected");
                        }
                        break;
                    }
                    default:
                    {
                        throw new XmlSyntaxException("parser programming error");
                    }
                }
            }
            if (state != State.FirstByte)
            {
                throw new XmlSyntaxException("token completion expected");
            }
            return null;
        }

        private string MarkedText()
        {
            var text = Encoding.ASCII.GetString(_buffer, _mark + 1, _point - _mark);
            return text;
        }
    }
}