using System.IO;
using System.Text;

namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <summary>
    /// An XmlTokenizer provides a base-class for tokenizing well-formed Xml
    /// expressions.
    /// </summary>
    /// <author>Paul Sweeny</author>
    public abstract class AbstractXmlTokenizer : IXmlTokenizer
    {
        /// <summary>This object is used in place of null.</summary>
        private const int Eof = -1;

        protected readonly byte[] _buffer;

        protected int _end;
        protected int _point = -1;

        protected int _mark;

        /// <summary>Construct a new tokenizer.</summary>
        /// <param name="xml">the Xml buffer to tokenize.</param>
        protected AbstractXmlTokenizer(byte[] xml)
        {
            _buffer = xml;
        }

        /// <exception cref="XmlSyntaxException"/>
        public abstract XmlToken NextToken();

        /// <exception cref="XmlSyntaxException"/>
        public virtual XmlElement NextElement()
        {
            return NextElement(NextToken());
        }

        /// <summary>Analyse the Xml document and then return the next Xml element.</summary>
        /// <param name="token">the most recently read token, normally a start tag.</param>
        /// <returns>the next read XmlElement, or null when there are no more.</returns>
        /// <exception cref="XmlSyntaxException">if there was an error while parsing the Xml.</exception>
        /// <exception cref="XmlSyntaxException"/>
        public virtual XmlElement NextElement(XmlToken token)
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
                element = new XmlElement(token);
                XmlElement[] stack = null;
                int nest = 0;
                while ((token = NextToken()) != null)
                {
                    switch (token.TokenType())
                    {
                        case XmlToken.EndType:
                        case XmlToken.EmptyType:
                        {
                            if (nest == 0)
                            {
                                return element;
                            }
                            element = stack[--nest];
                            break;
                        }
                        case XmlToken.StartType:
                        {
                            if (stack == null)
                            {
                                stack = new XmlElement[4];
                            }
                            else
                            {
                                if (nest == stack.Length)
                                {
                                    XmlElement[] old = stack;
                                    stack = new XmlElement[2 * nest];
                                    System.Array.Copy(old, 0, stack, 0, nest);
                                }
                            }
                            stack[nest] = element;
                            element = new XmlElement(token);
                            stack[nest++].Add(element);
                            break;
                        }
                        case XmlToken.NameType:
                        {
                            XmlToken name = token;
                            token = NextToken();
                            if (token == null)
                            {
                                break;
                            }
                            element.Add(name, token);
                            break;
                        }
                        case XmlToken.IntegerValueType:
                        case XmlToken.DoubleValueType:
                        case XmlToken.FractionValueType:
                        case XmlToken.StringValueType:
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
                throw Error(e.Message + (token == null ? string.Empty : "; reading " + token.DebugString()));
            }
        }

        /// <summary>Create an error message.</summary>
        /// <param name="reason">the reason for the error.</param>
        /// <returns>an XmlSyntaxException for the error.</returns>
        protected static XmlSyntaxException Error(string reason)
        {
            return new XmlSyntaxException(reason);
        }

        /// <summary>Create an error message showing where the error occurred in the input.</summary>
        /// <param name="reason">the reason for the error.</param>
        public virtual string Where(string reason)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(reason);
            buf.Append('\n');
            int from = _point - 40 - 400;
            int to = _point + 38;
            if (from < 0)
            {
                from = 0;
            }
            if (to > _end)
            {
                to = _end;
            }
            for (int i = from; i < to; ++i)
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

        /// <summary>Apend a character to a StringBuffer, transform it if non-readable.</summary>
        public virtual void AppendCharacter(StringBuilder buf, int c)
        {
            if (c < 32 || c > 126 || c == '\\')
            {
                AppendNonVisibleCharacter(buf, c);
            }
            else
            {
                buf.Append((char) c);
            }
        }

        /// <summary>Apend a non-visible character to a StringBuffer.</summary>
        public virtual void AppendNonVisibleCharacter(StringBuilder buf, int c)
        {
            buf.Append('\\');
            switch (c)
            {
                case '\b':
                {
                    buf.Append('b');
                    break;
                }
                case '\t':
                {
                    buf.Append('t');
                    break;
                }
                case '\n':
                {
                    buf.Append('n');
                    break;
                }
                case '\r':
                {
                    buf.Append('r');
                    break;
                }
                case '\f':
                {
                    buf.Append('f');
                    break;
                }
                case '\\':
                {
                    buf.Append('\\');
                    break;
                }
                default:
                {
                    AppendHexCharacter(buf, c);
                    break;
                }
            }
        }

        public virtual void AppendHexCharacter(StringBuilder buf, int c)
        {
            string value = c.ToString("X");
            if (value.Length == 1)
            {
                buf.Append('0');
            }
            buf.Append(value);
        }

        /// <summary>Fill the buffer from the input stream.</summary>
        /// <param name="input">the input stream to fill from.</param>
        /// <returns>true if there is more data to read and false otherwise.</returns>
        /// <exception cref="XmlSyntaxException">
        /// if an I/O error occurred while refilling the
        /// buffer.
        /// </exception>
        /// <exception cref="XmlSyntaxException"/>
        public virtual bool FillBuffer(Stream input)
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
                        System.Array.Copy(_buffer, _mark, _buffer, 0, _end);
                    }
                    _mark = 0;
                    int got = input.Read(_buffer, _end, _buffer.Length - _end);
                    if (got == Eof)
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
                    int got = input.Read(_buffer, _end, _buffer.Length - _end);
                    if (got == Eof)
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

        public virtual string BufferAsString()
        {
            return Encoding.GetEncoding(28591).GetString(_buffer, _mark, _end - _mark);
        }
    }
}