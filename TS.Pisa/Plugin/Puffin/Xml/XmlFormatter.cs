using System.IO;
using System.Text;

namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <summary>
    /// An XmlOutputFormatter will format well-formed Xml messages to a given
    /// Stream.
    /// </summary>
    /// <author>Paul Sweeny</author>
    public class XmlFormatter
    {
        private static readonly byte[][] AllEncodings = new byte[256][];
        private static readonly byte[][] NonQuoteEncodings = new byte[AllEncodings.Length][];

        static XmlFormatter()
        {
            for (var i = 0; i < 10; ++i)
            {
                var b = new byte[4];
                b[0] = unchecked((byte) '&');
                b[1] = unchecked((byte) '#');
                b[2] = unchecked((byte) ((byte) ('0') + i));
                b[3] = unchecked((byte) ';');
                AllEncodings[i] = b;
            }
            for (var i = 10; i < 32; ++i)
            {
                var b = new byte[5];
                b[0] = unchecked((byte) '&');
                b[1] = unchecked((byte) '#');
                b[2] = unchecked((byte) ((byte) '0' + i / 10));
                b[3] = unchecked((byte) ((byte) '0' + i % 10));
                b[4] = unchecked((byte) ';');
                AllEncodings[i] = b;
            }
            for (var i = 128; i < 256; ++i)
            {
                var b = new byte[6];
                b[0] = unchecked((byte) '&');
                b[1] = unchecked((byte) '#');
                b[2] = unchecked(i < 200 ? (byte) '1' : (byte) '2');
                b[3] = unchecked((byte) ((byte) '0' + i % 100 / 10));
                b[4] = unchecked((byte) ((byte) '0' + i % 10));
                b[5] = unchecked((byte) ';');
                AllEncodings[i] = b;
            }
            AllEncodings['\n'] = null;
            AllEncodings['\r'] = null;
            AllEncodings['\t'] = null;
            AllEncodings['<'] = Encoding.ASCII.GetBytes("&lt;");
            AllEncodings['&'] = Encoding.ASCII.GetBytes("&amp;");
            AllEncodings['"'] = Encoding.ASCII.GetBytes("&quot;");
            AllEncodings['\''] = Encoding.ASCII.GetBytes("&apos;");
            for (var i = 0; i < NonQuoteEncodings.Length; ++i)
            {
                NonQuoteEncodings[i] = AllEncodings[i];
            }
            NonQuoteEncodings['"'] = null;
            NonQuoteEncodings['\''] = null;
        }

        private readonly Stream _outStream;
        private bool _hangingElement;

        /// <summary>Create a new XmlOutputFormatter.</summary>
        /// <param name="outStream">the OutputStrean to send formatted Xml to.</param>
        public XmlFormatter(Stream outStream)
        {
            _outStream = outStream;
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
            _outStream.Flush();
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
            var text = token.GetTextAsBytes();
            var start = 0;
            for (var i = start; i < text.Length; ++i)
            {
                var encode = Encode(text[i] & 0xff, quote);
                if (encode == null) continue;
                _outStream.Write(text, start, i - start);
                start = i + 1;
                _outStream.Write(encode, 0, encode.Length);
            }
            _outStream.Write(text, start, text.Length - start);
        }

        private static byte[] Encode(int c, char quote)
        {
            return c == quote ? AllEncodings[c] : NonQuoteEncodings[c];
        }

        private void WriteStartTag(XmlToken token)
        {
            WriteTagOpenBrace();
            var text = token.GetTextAsBytes();
            _outStream.Write(text, 0, text.Length);
        }

        private void WriteEndTag(XmlToken token)
        {
            WriteTagOpenBrace();
            _outStream.WriteByte((byte) '/');
            var text = token.GetTextAsBytes();
            _outStream.Write(text, 0, text.Length);
            WriteTagCloseBrace();
        }

        private void WriteEndEmptyTag()
        {
            _outStream.WriteByte((byte) '/');
            WriteTagCloseBrace();
        }

        private void WriteContent(XmlToken token)
        {
            if (_hangingElement)
            {
                WriteTagCloseBrace();
            }
            Escape(token, '<');
        }

        private void WriteTagOpenBrace()
        {
            if (_hangingElement)
            {
                WriteTagCloseBrace();
            }
            _outStream.WriteByte((byte) '<');
            _hangingElement = true;
        }

        private void WriteTagCloseBrace()
        {
            _outStream.WriteByte((byte) '>');
            _hangingElement = false;
        }

        private void WriteAttributeName(XmlToken token)
        {
            _outStream.WriteByte((byte) ' ');
            var text = token.GetTextAsBytes();
            _outStream.Write(text, 0, text.Length);
            _outStream.WriteByte((byte) '=');
        }

        private void WriteAttributeValueNumber(XmlToken token)
        {
            _outStream.WriteByte((byte) '"');
            var text = token.GetTextAsBytes();
            _outStream.Write(text, 0, text.Length);
            _outStream.WriteByte((byte) '"');
        }

        private void WriteAttributeValueString(XmlToken token)
        {
            _outStream.WriteByte((byte) '"');
            Escape(token, '"');
            _outStream.WriteByte((byte) '"');
        }

    }
}