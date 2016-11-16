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
    public class BinaryReader
    {
        private readonly byte[] _buffer = new byte[8192];
        private int _end;
        private int _point = -1;
        private int _mark;

        private readonly Stream _inStream;
        private readonly TokenDictionary _dictionary = new TokenDictionary();
        private readonly Stack<XmlToken> _tagStack = new Stack<XmlToken>();
        private readonly Stack<XmlElement> _elementStack = new Stack<XmlElement>();

        private enum State
        {
            FirstByte,
            SecondByte,
            ScanUnseenToken
        }

        public BinaryReader(Stream inStream)
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
                    throw new PuffinSyntaxException("start tag expected");
                }
                var element = new XmlElement(token.GetText());
                while ((token = NextToken()) != null)
                {
                    switch (token.TokenType())
                    {
                        case TokenType.TagEnd:
                        case TokenType.TagEndEmptyContent:
                        {
                            if (_elementStack.Count == 0)
                            {
                                return element;
                            }
                            element = _elementStack.Pop();
                            break;
                        }
                        case TokenType.TagStart:
                        {
                            _elementStack.Push(element);
                            element = new XmlElement(token.GetText());
                            _elementStack.Peek().AddElement(element);
                            break;
                        }
                        case TokenType.AttributeName:
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
                        case TokenType.AttributeValueInteger:
                        case TokenType.AttributeValueDouble:
                        case TokenType.AttributeValueFraction:
                        case TokenType.AttributeValueString:
                        case TokenType.NestedContent:
                        {
                            throw new PuffinSyntaxException("attribute value with no name " + token);
                        }
                        default:
                        {
                            throw new PuffinSyntaxException("unknown token type " + token);
                        }
                    }
                }
                throw new PuffinSyntaxException("unexpected document termination");
            }
            catch (PuffinSyntaxException e)
            {
                throw new PuffinSyntaxException(
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
                        throw new PuffinSyntaxException("input buffer too small " + _buffer.Length);
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
                throw new PuffinSyntaxException(e.Message);
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
                        if (TokenDictionary.IsFirstByteOfToken(b))
                        {
                            state = State.SecondByte;
                        }
                        else
                        {
                            if (TokenDictionary.IsTokenType(b))
                            {
                                if (b == (byte) TokenType.TagEnd)
                                {
                                    return _tagStack.Pop().ToEndTag();
                                }
                                if (b == (byte) TokenType.TagEndEmptyContent)
                                {
                                    _tagStack.Pop();
                                    return XmlToken.EmptyToken;
                                }
                                state = State.ScanUnseenToken;
                            }
                            else
                            {
                                throw new PuffinSyntaxException("token tag expected instead of " + b
                                                             + " ('" + (char) b + "') at position " + _point);
                            }
                        }
                        break;
                    }
                    case State.SecondByte:
                    {
                        XmlToken token;
                        if (TokenDictionary.IsSecondByteOfToken(b))
                        {
                            token = _dictionary.TwoByteToken(_buffer[_mark], b);
                        }
                        else
                        {
                            token = _dictionary.OneByteToken(_buffer[_mark]);
                            --_point;
                        }
                        if (!token.IsStartTag()) return token;
                        _tagStack.Push(token);
                        return token;
                    }
                    case State.ScanUnseenToken:
                    {
                        if (!TokenDictionary.IsPlainText(b))
                        {
                            var type = (TokenType) _buffer[_mark];
                            if (_mark < --_point)
                            {
                                XmlToken token;
                                var text = MarkedText();
                                switch (type)
                                {
                                    case TokenType.TagStart:
                                        token = new XmlToken(type, text, text);
                                        _tagStack.Push(token);
                                        break;
                                    case TokenType.AttributeName:
                                        token = new XmlToken(type, text, text);
                                        break;
                                    case TokenType.TagEnd:
                                        token = new XmlToken(type, text, text);
                                        break;
                                    case TokenType.TagEndEmptyContent:
                                        token = XmlToken.EmptyToken;
                                        break;
                                    case TokenType.NestedContent:
                                        token = XmlToken.NullContentToken;
                                        break;
                                    case TokenType.AttributeValueInteger:
                                        token = new XmlToken(type, text, Convert.ToInt32(text));
                                        break;
                                    case TokenType.AttributeValueDouble:
                                        token = new XmlToken(type, text, Convert.ToDouble(text));
                                        break;
                                    case TokenType.AttributeValueFraction:
                                        token = new XmlToken(type, text, XmlToken.FractionToDouble(text));
                                        break;
                                    case TokenType.AttributeValueString:
                                        token = new XmlToken(type, text, text);
                                        break;
                                    default:
                                        throw new PuffinSyntaxException("unrecognised token " + type);
                                }
                                _dictionary.InsertToken(token);
                                return token;
                            }
                            if (type == TokenType.AttributeValueString)
                            {
                                return XmlToken.NullValueToken;
                            }
                            if (type == TokenType.NestedContent)
                            {
                                return XmlToken.NullContentToken;
                            }
                            throw new PuffinSyntaxException("text of previously unseen token expected");
                        }
                        break;
                    }
                    default:
                    {
                        throw new PuffinSyntaxException("parser programming error");
                    }
                }
            }
            if (state != State.FirstByte)
            {
                throw new PuffinSyntaxException("token completion expected");
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