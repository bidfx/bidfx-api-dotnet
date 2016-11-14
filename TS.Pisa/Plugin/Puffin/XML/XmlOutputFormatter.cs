using System.IO;
using System.Text;

namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <summary>
    /// An XmlOutputFormatter will format well-formed Xml messages to a given
    /// Stream.
    /// </summary>
    /// <author>Paul Sweeny</author>
    public class XmlOutputFormatter : IXmlFormatter
    {
        private static readonly byte[][] gAllEncodings = new byte[256][];
        private static readonly byte[][] gNonQuoteEncodings = new byte[gAllEncodings.Length][];

        static XmlOutputFormatter()
        {
            for (int i = 0; i < 10; ++i)
            {
                byte[] b = new byte[4];
                b[0] = unchecked((byte) '&');
                b[1] = unchecked((byte) '#');
                b[2] = unchecked((byte) ((byte) ('0') + i));
                b[3] = unchecked((byte) ';');
                gAllEncodings[i] = b;
            }
            for (int i_1 = 10; i_1 < 32; ++i_1)
            {
                byte[] b = new byte[5];
                b[0] = unchecked((byte) '&');
                b[1] = unchecked((byte) '#');
                b[2] = unchecked((byte) ((byte) '0' + i_1 / 10));
                b[3] = unchecked((byte) ((byte) '0' + i_1 % 10));
                b[4] = unchecked((byte) ';');
                gAllEncodings[i_1] = b;
            }
            for (int i_2 = 128; i_2 < 256; ++i_2)
            {
                byte[] b = new byte[6];
                b[0] = unchecked((byte) (byte) ('&'));
                b[1] = unchecked((byte) (byte) ('#'));
                b[2] = unchecked((byte) ((i_2 < 200) ? (byte) ('1') : (byte) ('2')));
                b[3] = unchecked((byte) ((byte) ('0') + (i_2 % 100) / 10));
                b[4] = unchecked((byte) ((byte) ('0') + i_2 % 10));
                b[5] = unchecked((byte) (byte) (';'));
                gAllEncodings[i_2] = b;
            }
            gAllEncodings['\n'] = null;
            gAllEncodings['\r'] = null;
            gAllEncodings['\t'] = null;
            gAllEncodings['<'] = Encoding.ASCII.GetBytes("&lt;");
            gAllEncodings['&'] = Encoding.ASCII.GetBytes("&amp;");
            gAllEncodings['"'] = Encoding.ASCII.GetBytes("&quot;");
            gAllEncodings['\''] = Encoding.ASCII.GetBytes("&apos;");
            for (int i = 0; i < gNonQuoteEncodings.Length; ++i)
            {
                gNonQuoteEncodings[i] = gAllEncodings[i];
            }
            gNonQuoteEncodings['"'] = null;
            gNonQuoteEncodings['\''] = null;
        }

        private Stream _out;

        private bool _hangingElement;
        private char _quote = '"';

        private bool _isEachElementOnNewLine;

        /// <summary>Create a new XmlOutputFormatter.</summary>
        /// <param name="out">the OutputStrean to send formatted Xml to.</param>
        public XmlOutputFormatter(Stream outStream)
        {
            _out = outStream;
        }

        /// <summary>Format an XmlElement.</summary>
        /// <param name="element">the element to be formatted.</param>
        /// <exception cref="System.IO.IOException">if formatting fails due to an I/O error.</exception>
        /// <exception cref="XmlSyntaxException">if formatting fails due to invalid Xml.</exception>
        /// <exception cref="XmlSyntaxException"/>
        public void Format(XmlElement element)
        {
            WriteStartTag(element.GetTag());
            element.FormatAttributes(this);
            if (element.HasContent())
            {
                WriteTagCloseBrace();
                element.FormatContents(this);
                WriteEndTag(element.GetTag());
            }
            else
            {
                WriteEndEmptyTag();
            }
        }

        /// <summary>Format an XmlToken.</summary>
        /// <param name="token">the token to be formatted.</param>
        /// <exception cref="System.IO.IOException">if formatting fails.</exception>
        public void Format(XmlToken token)
        {
            switch (token.TokenType())
            {
                case XmlToken.EndType:
                {
                    WriteEndTag(token);
                    break;
                }
                case XmlToken.EmptyType:
                {
                    WriteEndEmptyTag();
                    break;
                }
                case XmlToken.StartType:
                {
                    WriteStartTag(token);
                    break;
                }
                case XmlToken.ContentType:
                {
                    WriteContent(token);
                    break;
                }
                case XmlToken.NameType:
                {
                    WriteAttributeName(token);
                    break;
                }
                case XmlToken.IntegerValueType:
                case XmlToken.DoubleValueType:
                case XmlToken.FractionValueType:
                {
                    WriteAttributeValueNumber(token);
                    break;
                }
                case XmlToken.StringValueType:
                {
                    WriteAttributeValueString(token);
                    break;
                }
                default:
                {
                    throw new XmlSyntaxException("programming error");
                }
            }
        }

        /// <summary>Flush the data formatter so far.</summary>
        /// <exception cref="System.IO.IOException"/>
        public void Flush()
        {
            _out.Flush();
        }

        /// <summary>
        /// Escape a token so it becomes valid Xml content and then add it to the
        /// message.
        /// </summary>
        /// <param name="token">the token to escape.</param>
        /// <param name="quote">the current quote character for the text being escaped.</param>
        /// <exception cref="System.IO.IOException">if formatting fails.</exception>
        private void Escape(XmlToken token, char quote)
        {
            byte[] text = token.GetTextAsBytes();
            int start = 0;
            for (int i = start; i < text.Length; ++i)
            {
                byte[] encode = Encode(text[i] & 255, quote);
                if (encode != null)
                {
                    _out.Write(text, start, i - start);
                    start = i + 1;
                    _out.Write(encode, 0, encode.Length);
                }
            }
            _out.Write(text, start, text.Length - start);
        }

        /// <summary>Encode a, perhaps special, character as an array of bytes.</summary>
        private byte[] Encode(int c, char quote)
        {
            if (c == quote)
            {
                return gAllEncodings[c];
            }
            return gNonQuoteEncodings[c];
        }

        /// <summary>Set attribute quoting to use single quotes (apostrophe).</summary>
        public void SetSingleQuoteAttributes()
        {
            _quote = '\'';
        }

        /// <summary>Set attribute quoting to use double quotes.</summary>
        public void SetDoubleQuoteAttributes()
        {
            _quote = '"';
        }

        /// <summary>Set formatting of elements one per line on or off.</summary>
        /// <param name="on">true if newlines are required, false otherwise.</param>
        public void SetEachElementOnNewLine(bool on)
        {
            _isEachElementOnNewLine = on;
        }

        /// <exception cref="System.IO.IOException"/>
        public void WriteStartTag(XmlToken token)
        {
            WriteTagOpenBrace();
            var text = token.GetTextAsBytes();
            _out.Write(text, 0, text.Length);
        }

        /// <exception cref="System.IO.IOException"/>
        public void WriteEndTag(XmlToken token)
        {
            WriteTagOpenBrace();
            _out.WriteByte((byte) '/');
            var text = token.GetTextAsBytes();
            _out.Write(text, 0, text.Length);
            WriteTagCloseBrace();
        }

        /// <exception cref="System.IO.IOException"/>
        public void WriteEndEmptyTag()
        {
            _out.WriteByte((byte) '/');
            WriteTagCloseBrace();
        }

        /// <exception cref="System.IO.IOException"/>
        public void WriteContent(XmlToken token)
        {
            if (_hangingElement)
            {
                WriteTagCloseBrace();
            }
            Escape(token, '<');
        }

        /// <exception cref="System.IO.IOException"/>
        public void WriteTagOpenBrace()
        {
            if (_hangingElement)
            {
                WriteTagCloseBrace();
            }
            _out.WriteByte((byte) '<');
            _hangingElement = true;
        }

        /// <exception cref="System.IO.IOException"/>
        public void WriteTagCloseBrace()
        {
            _out.WriteByte((byte) '>');
            _hangingElement = false;
            if (_isEachElementOnNewLine)
            {
                Newline();
            }
        }

        /// <exception cref="System.IO.IOException"/>
        public void WriteAttributeName(XmlToken token)
        {
            _out.WriteByte((byte) ' ');
            var text = token.GetTextAsBytes();
            _out.Write(text, 0, text.Length);
            _out.WriteByte((byte) '=');
        }

        /// <exception cref="System.IO.IOException"/>
        public void WriteAttributeValueNumber(XmlToken token)
        {
            _out.WriteByte((byte) _quote);
            var text = token.GetTextAsBytes();
            _out.Write(text, 0, text.Length);
            _out.WriteByte((byte) _quote);
        }

        /// <exception cref="System.IO.IOException"/>
        public void WriteAttributeValueString(XmlToken token)
        {
            _out.WriteByte((byte) _quote);
            Escape(token, _quote);
            _out.WriteByte((byte) _quote);
        }

        public Stream GetStream()
        {
            return _out;
        }

        /// <exception cref="System.IO.IOException"/>
        private void Newline()
        {
            _out.WriteByte((byte) '\n');
        }
    }
}