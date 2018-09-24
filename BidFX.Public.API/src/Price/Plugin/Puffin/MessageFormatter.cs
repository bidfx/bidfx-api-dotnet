using System.Collections.Generic;
using System.Text;

namespace BidFX.Public.API.Price.Plugin.Puffin
{
    internal class MessageFormatter
    {
        private static readonly string[] Encodings = new string[256];

        static MessageFormatter()
        {
            for (int i = 0; i < 23; ++i)
            {
                Encodings[i] = "&#" + i + ";";
            }

            for (int i = 128; i < 256; ++i)
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

        public MessageFormatter(StringBuilder builder)
        {
            _builder = builder;
        }

        public static string FormatToString(PuffinElement element)
        {
            StringBuilder sb = new StringBuilder();
            MessageFormatter formatter = new MessageFormatter(sb);
            formatter.FormatElement(element);
            return sb.ToString();
        }

        public void FormatElement(PuffinElement element)
        {
            _builder.Append('<' + element.Tag);

            foreach (KeyValuePair<string, PuffinToken> attribute in element.Attributes)
            {
                FormatAttribute(attribute.Key, attribute.Value);
            }

            if (element.HasContent())
            {
                _builder.Append('>');
                foreach (PuffinElement subElement in element.Content)
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

        private void FormatAttribute(string name, PuffinToken value)
        {
            _builder.Append(' ' + name + "=\"");
            switch (value.TokenType)
            {
                case TokenType.IntegerValue:
                case TokenType.DecimalValue:
                case TokenType.FractionValue:
                    _builder.Append(value.Text);
                    break;
                case TokenType.StringValue:
                    EscapeToken(value);
                    break;
                case TokenType.TagEnd:
                case TokenType.TagEndEmptyContent:
                case TokenType.TagStart:
                case TokenType.NestedContent:
                case TokenType.AttributeName:
                    break;
                default:
                    throw new PuffinSyntaxException("unexpect attribute value type " + value);
            }

            _builder.Append('"');
        }

        private void EscapeToken(IPriceField token)
        {
            foreach (char c in token.Text)
            {
                string encoded = Encodings[c];
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
    }
}