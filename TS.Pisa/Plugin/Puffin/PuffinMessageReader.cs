using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TS.Pisa.Plugin.Puffin
{
    /// <summary>
    /// This class provides a reader for Puffin's binary, compressed, well-formed XML element messages.
    /// </summary>
    /// <author>Paul Sweeny</author>
    public class PuffinMessageReader
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
                var element = new PuffinElement(token.GetText());
                while ((token = NextToken()) != null)
                {
                    switch (token.TokenType())
                    {
                        case LexicalType.TagEnd:
                        case LexicalType.TagEndEmptyContent:
                        {
                            if (_elementStack.Count == 0)
                            {
                                return element;
                            }
                            element = _elementStack.Pop();
                            break;
                        }
                        case LexicalType.TagStart:
                        {
                            _elementStack.Push(element);
                            element = new PuffinElement(token.GetText());
                            _elementStack.Peek().AddElement(element);
                            break;
                        }
                        case LexicalType.AttributeName:
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
                        case LexicalType.AttributeValueInteger:
                        case LexicalType.AttributeValueDouble:
                        case LexicalType.AttributeValueFraction:
                        case LexicalType.AttributeValueString:
                        case LexicalType.NestedContent:
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

        private PuffinToken NextToken()
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
                                if (b == (byte) LexicalType.TagEnd)
                                {
                                    return _tagStack.Pop().ToEndTag();
                                }
                                if (b == (byte) LexicalType.TagEndEmptyContent)
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
                            var type = (LexicalType) _buffer[_mark];
                            if (_mark < --_point)
                            {
                                PuffinToken token;
                                var text = MarkedText();
                                switch (type)
                                {
                                    case LexicalType.TagStart:
                                        token = new PuffinToken(type, text, text);
                                        _tagStack.Push(token);
                                        break;
                                    case LexicalType.AttributeName:
                                        token = new PuffinToken(type, text, text);
                                        break;
                                    case LexicalType.TagEnd:
                                        token = new PuffinToken(type, text, text);
                                        break;
                                    case LexicalType.TagEndEmptyContent:
                                        token = PuffinToken.EmptyToken;
                                        break;
                                    case LexicalType.NestedContent:
                                        token = PuffinToken.NullContentToken;
                                        break;
                                    case LexicalType.AttributeValueInteger:
                                        token = new PuffinToken(type, text, Convert.ToInt64(text));
                                        break;
                                    case LexicalType.AttributeValueDouble:
                                        token = new PuffinToken(type, text, Convert.ToDouble(text));
                                        break;
                                    case LexicalType.AttributeValueFraction:
                                        token = new PuffinToken(type, text, PuffinToken.FractionToDouble(text));
                                        break;
                                    case LexicalType.AttributeValueString:
                                        token = new PuffinToken(type, text, text);
                                        break;
                                    default:
                                        throw new PuffinSyntaxException("unrecognised token " + type);
                                }
                                _dictionary.InsertToken(token);
                                return token;
                            }
                            if (type == LexicalType.AttributeValueString)
                            {
                                return PuffinToken.NullValueToken;
                            }
                            if (type == LexicalType.NestedContent)
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

        private string MarkedText()
        {
            var text = Encoding.ASCII.GetString(_buffer, _mark + 1, _point - _mark);
            return text;
        }
    }
}