using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <summary>
    /// An XmlElement is a class that represents a single element within an
    /// XmlMessage.
    /// </summary>
    /// <author>Paul Sweeny</author>
    public class XmlElement
    {
        private readonly string _tag;
        private readonly List<KeyValuePair<string, XmlToken>> _attributes = new List<KeyValuePair<string, XmlToken>>();
        private readonly List<XmlElement> _content = new List<XmlElement>();

        /// <summary>Create a new XmlElement.</summary>
        /// <param name="tag">the tag of the element.</param>
        public XmlElement(string tag)
        {
            _tag = tag;
        }

        public XmlElement AddElement(XmlElement element)
        {
            _content.Add(element);
            return this;
        }

        public XmlElement AddAttribute(string name, XmlToken value)
        {
            if (value == null || !value.IsValueType())
            {
                throw new XmlSyntaxException("invalid attribute value " + value);
            }
            _attributes.Add(new KeyValuePair<string, XmlToken>(name, value));
            return this;
        }

        public XmlElement AddAttribute(string name, int value)
        {
            return AddAttribute(name, XmlToken.IntegerValue(value));
        }

        public XmlElement AddAttribute(string name, long value)
        {
            return AddAttribute(name, XmlToken.LongValue(value));
        }

        public XmlElement AddAttribute(string name, double value)
        {
            return AddAttribute(name, XmlToken.DoubleValue(value));
        }

        public XmlElement AddAttribute(string name, string value)
        {
            return AddAttribute(name, XmlToken.StringValue(value));
        }

        /// <summary>Add an Xml attribute to this element.</summary>
        /// <param name="name">the name of the attribute.</param>
        /// <param name="value">the value of the attribute.</param>
        /// <returns>this element.</returns>
        public XmlElement AddAttribute(string name, char value)
        {
            return AddAttribute(name, value.ToString());
        }

        /// <summary>Add an Xml attribute to this element.</summary>
        /// <param name="name">the name of the attribute.</param>
        /// <param name="value">the value of the attribute.</param>
        /// <returns>this element.</returns>
        public XmlElement AddAttribute(string name, bool value)
        {
            return AddAttribute(name, XmlToken.BooleanValue(value));
        }

        public string Tag
        {
            get { return _tag; }
        }

        public IEnumerable<KeyValuePair<string, XmlToken>> Attributes
        {
            get { return _attributes; }
        }

        public IEnumerable<XmlElement> Content
        {
            get { return _content; }
        }


        /// <summary>Test if this element has nested content.</summary>
        public bool HasContent()
        {
            return _content.Count != 0;
        }

        /// <summary>Test if this element has any attributes.</summary>
        public bool HasAttributes()
        {
            return _attributes.Count != 0;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var formatter = new XmlFormatter(sb);
            formatter.FormatElement(this);
            return sb.ToString();
        }

        public override bool Equals(object o)
        {
            if (o == this)
            {
                return true;
            }
            var element = o as XmlElement;
            if (element == null) return false;
            if (!_tag.Equals(element._tag))
            {
                return false;
            }
            var eq1 = _attributes.SequenceEqual(element._attributes);
            var eq2 = ContentEqual(_content, element._content);
            return eq1 && eq2;
        }

        private static bool ContentEqual(ICollection<XmlElement> c1, IList<XmlElement> c2)
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