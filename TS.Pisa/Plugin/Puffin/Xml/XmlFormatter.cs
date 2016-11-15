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
        private static readonly string[] AllEncodings = new string[256];
        private static readonly string[] NonQuoteEncodings = new string[AllEncodings.Length];

        static XmlFormatter()
        {
            for (var i = 0; i < 23; ++i)
            {
                AllEncodings[i] = "&#" + i + ";";
            }
            for (var i = 128; i < 256; ++i)
            {
                AllEncodings[i] = "&#" + i + ";";
            }
            AllEncodings['\n'] = null;
            AllEncodings['\r'] = null;
            AllEncodings['\t'] = null;
            AllEncodings['<'] = "&lt;";
            AllEncodings['&'] = "&amp;";
            AllEncodings['"'] = "&quot;";
            AllEncodings['\''] = "&apos;";
            for (var i = 0; i < NonQuoteEncodings.Length; ++i)
            {
                NonQuoteEncodings[i] = AllEncodings[i];
            }
            NonQuoteEncodings['"'] = null;
            NonQuoteEncodings['\''] = null;
        }

        private readonly StringBuilder _builder;
        private bool _hangingElement;

        public XmlFormatter(StringBuilder builder)
        {
            _builder = builder;
        }

        public void FormatElement(XmlElement element)
        {
            WriteStartTag(element.GetTag());
            element.FormatAttributes(this);
            if (element.HasContent())
            {
                WriteTagCloseBrace();
                var content = element.GetNestedContent().GetContent();
                foreach (var subElement in content)
                {
                    FormatElement(subElement);
                }
                WriteEndTag(element.GetTag());
            }
            else
            {
                WriteEndEmptyTag();
            }
        }

        public void FormatAttribute(string name, XmlToken value)
        {
            WriteAttributeName(name);
            switch (value.TokenType())
            {
                case XmlTokenType.EmptyType:
                case XmlTokenType.IntegerValueType:
                case XmlTokenType.DoubleValueType:
                case XmlTokenType.FractionValueType:
                    WriteAttributeValueNumber(value);
                    break;
                case XmlTokenType.StringValueType:
                    WriteAttributeValueString(value);
                    break;
                default:
                    throw new XmlSyntaxException("unexpect attribute value type " + value);
            }
        }

        private void EscapeToken(XmlToken token)
        {
            var text = token.GetTextAsBytes();
            foreach (var c in text)
            {
                var encoded = Encode(c, '"');
                if (encoded == null)
                {
                    _builder.Append(c);
                }
                else
                {
                    _builder.Append(encoded);
                }
            }
        }

        private static string Encode(int c, char quote)
        {
            return c == quote ? AllEncodings[c] : NonQuoteEncodings[c];
        }

        private void WriteStartTag(string token)
        {
            WriteTagOpenBrace();
            _builder.Append(token);
        }

        private void WriteEndTag(string token)
        {
            WriteTagOpenBrace();
            _builder.Append('/');
            _builder.Append(token);
            WriteTagCloseBrace();
        }

        private void WriteEndEmptyTag()
        {
            _builder.Append('/');
            WriteTagCloseBrace();
        }

        private void WriteTagOpenBrace()
        {
            if (_hangingElement)
            {
                WriteTagCloseBrace();
            }
            _builder.Append('<');
            _hangingElement = true;
        }

        private void WriteTagCloseBrace()
        {
            _builder.Append('>');
            _hangingElement = false;
        }

        private void WriteAttributeName(string name)
        {
            _builder.Append(' ');
            _builder.Append(name);
            _builder.Append('=');
        }

        private void WriteAttributeValueNumber(XmlToken token)
        {
            _builder.Append('"');
            _builder.Append(token.GetText());
            _builder.Append('"');
        }

        private void WriteAttributeValueString(XmlToken token)
        {
            _builder.Append('"');
            EscapeToken(token);
            _builder.Append('"');
        }
    }
}