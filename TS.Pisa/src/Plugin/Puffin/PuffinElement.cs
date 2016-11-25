using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TS.Pisa.Plugin.Puffin
{
    /// <summary>
    /// This class represents a Puffin XML element.
    /// </summary>
    /// <author>Paul Sweeny</author>
    public class PuffinElement
    {
        private readonly string _tag;

        private readonly List<KeyValuePair<string, PuffinToken>> _attributes =
            new List<KeyValuePair<string, PuffinToken>>();

        private readonly List<PuffinElement> _content = new List<PuffinElement>();

        public PuffinElement(string tag)
        {
            _tag = tag;
        }

        public PuffinElement AddElement(PuffinElement element)
        {
            _content.Add(element);
            return this;
        }

        internal PuffinElement AddAttribute(string name, PuffinToken value)
        {
            if (value == null || !value.IsValueType())
            {
                throw new PuffinSyntaxException("invalid attribute value " + value);
            }
            _attributes.Add(new KeyValuePair<string, PuffinToken>(name, value));
            return this;
        }

        public PuffinElement AddAttribute(string name, int value)
        {
            return AddAttribute(name, PuffinToken.IntegerValue(value));
        }

        public PuffinElement AddAttribute(string name, long value)
        {
            return AddAttribute(name, PuffinToken.LongValue(value));
        }

        public PuffinElement AddAttribute(string name, double value)
        {
            return AddAttribute(name, PuffinToken.DoubleValue(value));
        }

        public PuffinElement AddAttribute(string name, string value)
        {
            return AddAttribute(name, PuffinToken.StringValue(value));
        }

        public PuffinElement AddAttribute(string name, char value)
        {
            return AddAttribute(name, value.ToString());
        }

        public PuffinElement AddAttribute(string name, bool value)
        {
            return AddAttribute(name, PuffinToken.BooleanValue(value));
        }

        public string Tag
        {
            get { return _tag; }
        }

        internal IEnumerable<KeyValuePair<string, PuffinToken>> Attributes
        {
            get { return _attributes; }
        }

        public IEnumerable<PuffinElement> Content
        {
            get { return _content; }
        }

        public bool HasContent()
        {
            return _content.Count != 0;
        }

        public bool HasAttributes()
        {
            return _attributes.Count != 0;
        }

        internal PuffinToken AttributeValue(string name)
        {
            foreach (var attribute in _attributes)
            {
                if (attribute.Key.Equals(name))
                {
                    PuffinToken token = attribute.Value;
                    return token;
                }
            }
            return PuffinToken.NullValueToken;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var formatter = new MessageFormatter(sb);
            formatter.FormatElement(this);
            return sb.ToString();
        }

        public override bool Equals(object o)
        {
            if (o == this)
            {
                return true;
            }
            var element = o as PuffinElement;
            if (element == null) return false;
            if (!_tag.Equals(element._tag))
            {
                return false;
            }
            var eq1 = _attributes.SequenceEqual(element._attributes);
            var eq2 = ContentEqual(_content, element._content);
            return eq1 && eq2;
        }

        private static bool ContentEqual(ICollection<PuffinElement> c1, IList<PuffinElement> c2)
        {
            if (c1.Count == c2.Count)
            {
                return !c1.Where((t, i) => !t.Equals(c2[i])).Any();
            }
            return false;
        }

        public override int GetHashCode()
        {
            var result = _tag.GetHashCode();
            result = 31 * result + _attributes.GetHashCode();
            result = 31 * result + _content.GetHashCode();
            return result;
        }
    }
}