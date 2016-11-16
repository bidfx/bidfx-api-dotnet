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

        private enum State
        {
            FirstByte,
            SecondByte,
            ScanUnseenToken
        }

        /// <summary>Construct a new parser for the given input stream.</summary>
        /// <param name="inStream">the input stream to read Xml data from.</param>
        public XmlBinaryInflater(Stream inStream)
        {
            _inStream = inStream;
        }

        /// <exception cref="XmlSyntaxException"/>
        public XmlElement NextElement()
        {
            var token = NextToken();
            return NextElement(token);
        }

        /// <summary>Analyse the Xml document and then return the next Xml element.</summary>
        /// <param name="token">the most recently read token, normally a start tag.</param>
        /// <returns>the next read XmlElement, or null when there are no more.</returns>
        /// <exception cref="XmlSyntaxException">if there was an error while parsing the Xml.</exception>
        /// <exception cref="XmlSyntaxException"/>
        private XmlElement NextElement(XmlToken token)
        {
            XmlElement element = null;
            if (token == null)
            {
                return element;
            }
            try
            {
                if (!token.IsStartTag())
                {
                    throw new XmlSyntaxException("start tag expected");
                }
                element = new XmlElement(token.GetText());
                XmlElement[] stack = null;
                var nest = 0;
                while ((token = NextToken()) != null)
                {
                    switch (token.TokenType())
                    {
                        case XmlTokenType.EndType:
                        case XmlTokenType.EmptyType:
                        {
                            if (nest == 0)
                            {
                                return element;
                            }
                            element = stack[--nest];
                            break;
                        }
                        case XmlTokenType.StartType:
                        {
                            if (stack == null)
                            {
                                stack = new XmlElement[4];
                            }
                            else
                            {
                                if (nest == stack.Length)
                                {
                                    var old = stack;
                                    stack = new XmlElement[2 * nest];
                                    Array.Copy(old, 0, stack, 0, nest);
                                }
                            }
                            stack[nest] = element;
                            element = new XmlElement(token.GetText());
                            stack[nest++].AddElement(element);
                            break;
                        }
                        case XmlTokenType.NameType:
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
                        case XmlTokenType.IntegerValueType:
                        case XmlTokenType.DoubleValueType:
                        case XmlTokenType.FractionValueType:
                        case XmlTokenType.StringValueType:
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
                throw new XmlSyntaxException(e.Message + (token == null
                                                 ? string.Empty
                                                 : "; reading " + token.DebugString()));
            }
        }

        /// <summary>Create an error message showing where the error occurred in the input.</summary>
        /// <param name="reason">the reason for the error.</param>
        private string Where(string reason)
        {
            var buf = new StringBuilder();
            buf.Append(reason);
            buf.Append('\n');
            var from = _point - 40 - 400;
            var to = _point + 38;
            if (from < 0)
            {
                from = 0;
            }
            if (to > _end)
            {
                to = _end;
            }
            for (var i = from; i < to; ++i)
            {
                if (i == _point)
                {
                    buf.Append("<==here");
                    buf.Append('\n');
                }
                AppendCharacter(buf, _buffer[i] & 255);
            }
            buf.Append('\n');
            return buf.ToString();
        }

        private static void AppendCharacter(StringBuilder buf, int c)
        {
            var value = c.ToString("X");
            if (value.Length == 1)
            {
                buf.Append('0');
            }
            buf.Append(value);
            buf.Append(' ');
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
                                if (b == (byte)XmlTokenType.EndType)
                                {
                                    return _tagStack.Pop().ToEndTag();
                                }
                                if (b == (byte)XmlTokenType.EmptyType)
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
                            var type = (XmlTokenType)_buffer[_mark];
                            if (_mark < --_point)
                            {
                                XmlToken token;
                                var text = MarkedText();
                                switch (type)
                                {
                                    case XmlTokenType.StartType:
                                        token = new XmlToken(type, text, text);
                                        _tagStack.Push(token);
                                        break;
                                    case XmlTokenType.NameType:
                                        token = new XmlToken(type, text, text);
                                        break;
                                    case XmlTokenType.EndType:
                                        token = new XmlToken(type, text, text);
                                        break;
                                    case XmlTokenType.EmptyType:
                                        token = XmlToken.EmptyToken;
                                        break;
                                    case XmlTokenType.ContentType:
                                        token = XmlToken.NullContentToken;
                                        break;
                                    case XmlTokenType.IntegerValueType:
                                        token = new XmlToken(type, text, Convert.ToInt64(text));
                                        break;
                                    case XmlTokenType.DoubleValueType:
                                        token = new XmlToken(type, text, Convert.ToDouble(text));
                                        break;
                                    case XmlTokenType.FractionValueType:
                                        token = new XmlToken(type, text, XmlToken.FractionToDouble(text));
                                        break;
                                    case XmlTokenType.StringValueType:
                                        token = new XmlToken(type, text, text);
                                        break;
                                    default:
                                        throw new XmlSyntaxException("unrecognised token "+type);
                                }
                                _dictionary.Insert(token);
                                return token;
                            }
                            if (type == XmlTokenType.StringValueType)
                            {
                                return XmlToken.NullValueToken;
                            }
                            if (type == XmlTokenType.ContentType)
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