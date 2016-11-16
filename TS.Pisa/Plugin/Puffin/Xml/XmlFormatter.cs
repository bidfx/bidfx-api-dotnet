using System.Text;

namespace TS.Pisa.Plugin.Puffin.Xml
{
    public class XmlFormatter
    {
        private static readonly string[] Encodings = new string[256];

        static XmlFormatter()
        {
            for (var i = 0; i < 23; ++i)
            {
                Encodings[i] = "&#" + i + ";";
            }
            for (var i = 128; i < 256; ++i)
            {
                Encodings[i] = "&#" + i + ";";
            }
            Encodings['\n'] = null;
            Encodings['\r'] = null;
            Encodings['\t'] = null;
            Encodings['&'] = "&amp;";
            Encodings['"'] = "&quot;";
            Encodings['\''] = "&apos;";
        }

        private readonly StringBuilder _builder;

        public XmlFormatter(StringBuilder builder)
        {
            _builder = builder;
        }

        public static string FormatToString(XmlElement element)
        {
            var sb = new StringBuilder();
            var formatter = new XmlFormatter(sb);
            formatter.FormatElement(element);
            return sb.ToString();
        }

        public void FormatElement(XmlElement element)
        {
            _builder.Append('<' + element.Tag);

            foreach (var attribute in element.Attributes)
            {
                FormatAttribute(attribute.Key, attribute.Value);
            }
            if (element.HasContent())
            {
                _builder.Append('>');
                foreach (var subElement in element.Content)
                {
                    FormatElement(subElement);
                }
                _builder.Append("</" + element.Tag + ">");
            }
            else
            {
                _builder.Append("/>");
            }
        }

        private void FormatAttribute(string name, XmlToken value)
        {
            _builder.Append(' ' + name + "=\"");
            switch (value.TokenType())
            {
                case XmlTokenType.AttributeValueInteger:
                case XmlTokenType.AttributeValueDouble:
                case XmlTokenType.AttributeValueFraction:
                    _builder.Append(value.GetText());
                    break;
                case XmlTokenType.AttributeValueString:
                    EscapeToken(value);
                    break;
                case XmlTokenType.TagEnd:
                case XmlTokenType.TagEndEmptyContent:
                case XmlTokenType.TagStart:
                case XmlTokenType.NestedContent:
                case XmlTokenType.AttributeName:
                    break;
                default:
                    throw new PuffinSyntaxException("unexpect attribute value type " + value);
            }
            _builder.Append('"');
        }

        private void EscapeToken(XmlToken token)
        {
            var text = token.GetTextAsBytes();
            foreach (var c in text)
            {
                var encoded = Encodings[c];
                if (encoded == null)
                {
                    _builder.Append((char) c);
                }
                else
                {
                    _builder.Append(encoded);
                }
            }
        }
    }
}