using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BidFX.Public.NAPI.Price.Plugin.Puffin
{
    /// <summary>
    /// This class provides a reader for Puffin's binary, compressed, well-formed XML element messages.
    /// </summary>
    /// <author>Paul Sweeny</author>
    internal class PuffinMessageReader
    {
        private readonly byte[] _buffer = new byte[8192];
        private int _end;
        private int _point = -1;
        private int _mark;

        private readonly Stream _inStream;
        private readonly TokenDictionary _dictionary = new TokenDictionary();
        private readonly Stack<PuffinToken> _tagStack = new Stack<PuffinToken>();
        private readonly Stack<PuffinElement> _elementStack = new Stack<PuffinElement>();

        private enum State
        {
            FirstByte,
            SecondByte,
            ScanUnseenToken
        }

        public PuffinMessageReader(Stream inStream)
        {
            _inStream = inStream;
        }

        public PuffinElement ReadMessage()
        {
            var token = NextToken();
            return NextElement(token);
        }

        private PuffinElement NextElement(PuffinToken token)
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
                var element = new PuffinElement(token.Text);
                while ((token = NextToken()) != null)
                {
                    switch (token.TokenType)
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
                            element = new PuffinElement(token.Text);
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
                            element.AddAttribute(name.Text, token);
                            break;
                        }
                        case TokenType.IntegerValue:
                        case TokenType.DecimalValue:
                        case TokenType.FractionValue:
                        case TokenType.StringValue:
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
                    e.Message + (token == null ? string.Empty : "; reading " + token));
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
                    if (got == 0)
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
                    if (got == 0)
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

        private PuffinToken NextToken()
        {
            var state = State.FirstByte;
            for (_mark = ++_point; _point < _end || FillBuffer(); ++_point)
            {
                var b = _buffer[_point];
                //if (Log.IsDebugEnabled) Log.Debug("read byte " + b+" ("+(char)b+") at state "+state);

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
                                    return PuffinToken.EmptyToken;
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
                        PuffinToken token;
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
                                PuffinToken token;
                                var text = MarkedText();
                                switch (type)
                                {
                                    case TokenType.TagStart:
                                        token = new PuffinToken(type, text, text);
                                        _tagStack.Push(token);
                                        break;
                                    case TokenType.AttributeName:
                                        token = new PuffinToken(type, text, text);
                                        break;
                                    case TokenType.TagEnd:
                                        token = new PuffinToken(type, text, text);
                                        break;
                                    case TokenType.TagEndEmptyContent:
                                        token = PuffinToken.EmptyToken;
                                        break;
                                    case TokenType.NestedContent:
                                        token = PuffinToken.NullContentToken;
                                        break;
                                    case TokenType.IntegerValue:
                                        token = IntegerToken(text);
                                        break;
                                    case TokenType.DecimalValue:
                                        token = new PuffinToken(type, text, ValueParser.ParseDecimal(text, 0m));
                                        break;
                                    case TokenType.FractionValue:
                                        token = FractionToken(text);
                                        break;
                                    case TokenType.StringValue:
                                        token = new PuffinToken(type, text, text);
                                        break;
                                    default:
                                        throw new PuffinSyntaxException("unrecognised token " + type);
                                }
                                _dictionary.InsertToken(token);
                                return token;
                            }
                            if (type == TokenType.StringValue)
                            {
                                return PuffinToken.NullValueToken;
                            }
                            if (type == TokenType.NestedContent)
                            {
                                return PuffinToken.NullContentToken;
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

        private static PuffinToken IntegerToken(string text)
        {
            if (text.Length < 10)
            {
                return new PuffinToken(TokenType.IntegerValue, text, ValueParser.ParseInt(text, 0));
            }
            return new PuffinToken(TokenType.IntegerValue, text, ValueParser.ParseLong(text, 0L));
        }

        private static PuffinToken FractionToken(string text)
        {
            return new PuffinToken(TokenType.DecimalValue, text, ValueParser.ParseFraction(text));
        }

        private string MarkedText()
        {
            var text = Encoding.ASCII.GetString(_buffer, _mark + 1, _point - _mark);
            return text;
        }
    }
}