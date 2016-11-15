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

        private readonly IDictionary<string, XmlToken> _attributes = new Dictionary<string, XmlToken>();
        private XmlNestedContent _contents;

        /// <summary>Create a new XmlElement.</summary>
        /// <param name="tag">the tag of the element.</param>
        public XmlElement(string tag)
        {
            _tag = tag;
        }

        public XmlElement AddElement(XmlElement element)
        {
            if (_contents == null)
            {
                _contents = CreateNestedContent();
            }
            _contents.Add(element);
            return this;
        }

        protected virtual XmlNestedContent CreateNestedContent()
        {
            return new XmlNestedContent();
        }

        public XmlElement AddAttribute(string name, XmlToken value)
        {
            if (!value.IsValueType())
            {
                throw new XmlSyntaxException("invalid attribute value " + value);
            }
            _attributes[name] = value;
            return this;
        }

        public XmlElement AddAttribute(string name, int value)
        {
            _attributes[name] = value == 0 ? XmlToken.ZeroToken : XmlToken.IntegerValue(value);
            return this;
        }

        public XmlElement AddAttribute(string name, long value)
        {
            _attributes[name] = value == 0 ? XmlToken.ZeroToken : XmlToken.LongValue(value);
            return this;
        }

        public XmlElement AddAttribute(string name, double value)
        {
            _attributes[name] = value == 0.0 ? XmlToken.ZeroToken : XmlToken.DoubleValue(value);
            return this;
        }

        public XmlElement AddAttribute(string name, string value)
        {
            if (value == null)
            {
                throw new ArgumentException("null Xml attribute value");
            }
            _attributes[name] = XmlToken.StringValue(value);
            return this;
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
            _attributes[name] = XmlToken.BooleanValue(value);
            return this;
        }

        /// <summary>Get the tag name.</summary>
        public string GetTag()
        {
            return _tag;
        }

        /// <summary>Test if this element has nested content.</summary>
        public bool HasContent()
        {
            return _contents != null;
        }

        /// <summary>Test if this element has any attributes.</summary>
        public bool HasAttributes()
        {
            return _attributes.Count != 0;
        }

        /// <summary>Get a collection of the attribute names if this element.</summary>
        /// <returns>
        /// an unmodifiable list of
        /// <see cref="XmlName"/>
        /// s.
        /// </returns>
        public IList<string> GetAttributeNames()
        {
            IList<string> names = new List<string>(_attributes.Count);
            lock (_attributes)
            {
                foreach (var key in _attributes.Keys)
                {
                    names.Add(key);
                }
            }
            return names;
        }

        /// <summary>Get the value of an Xml attribute.</summary>
        /// <param name="name">the name of the attribute.</param>
        /// <returns>the attribute value or null.</returns>
        public XmlToken GetAttributeValue(string name)
        {
            return _attributes.ContainsKey(name) ? _attributes[name] : null;
        }

        /// <summary>Remove an Xml attribute.</summary>
        /// <param name="name">the name of the attribute.</param>
        public void RemoveAttribute(string name)
        {
            _attributes.Remove(name);
        }

        /// <summary>Get the nested contect of this Xml element.</summary>
        /// <returns>the content list or null if there is none.</returns>
        public XmlNestedContent GetNestedContent()
        {
            return _contents;
        }

        /// <summary>Update this element with another one.</summary>
        /// <remarks>
        /// Update this element with another one.  Attributes that are part of the
        /// update element but not part of this element are added to this one.
        /// Where there are duplicates then the attributes from the update element
        /// overwrite those in this element.  Any content in the update element
        /// replaces the content of this element.
        /// </remarks>
        /// <param name="update">the element to update this one with.</param>
        public void Update(XmlElement update)
        {
            Update(update, false);
        }

        /// <summary>Update this element with another one.</summary>
        /// <remarks>
        /// Update this element with another one.  Attributes that are part of the
        /// update element but not part of this element are added to this one.
        /// Where there are duplicates then the attributes from the update element
        /// overwrite those in this element.  Any content in the update element
        /// replaces the content of this element.
        /// </remarks>
        /// <param name="update">the element to update this one with.</param>
        /// <param name="applyDeletes">
        /// when <code>true</code> any nested element marked
        /// with deletion attribute in the update will cause the same nested element
        /// to be removed from this element (if it exists); when <code>false</code>
        /// a deletion attribute will be treated like any other.
        /// </param>
        public void Update(XmlElement update, bool applyDeletes)
        {
            if (update == null)
            {
                throw new ArgumentException("cannot update with null");
            }
            lock (update)
            {
                foreach (var entry in update._attributes)
                {
                    _attributes[entry.Key] = entry.Value;
                }
                if (update.HasContent())
                {
                    if (_contents == null)
                    {
                        _contents = CreateNestedContent();
                    }
                    _contents.Update(update._contents, applyDeletes);
                }
            }
        }

        /// <summary>Removes nested sub-elements marked with the deletion attribute.</summary>
        public void ClearDeletes()
        {
            if (_contents != null)
            {
                _contents.ClearDeletes();
            }
        }

        /// <summary>
        /// Find the minimum delta element that would be required to update this
        /// element with another.
        /// </summary>
        /// <remarks>
        /// Find the minimum delta element that would be required to update this
        /// element with another.  The result is an element containing only those
        /// attributes and contents from the update that are either not present in
        /// or are different from those of this element.
        /// </remarks>
        /// <param name="update">the element to derive the delta from.</param>
        /// <returns>the delta element or null if no delta is required.</returns>
        public XmlElement ComputeDelta(XmlElement update)
        {
            XmlElement delta = null;
            lock (_attributes)
            {
                foreach (var entry in update._attributes)
                {
                    if (!_attributes.ContainsKey(entry.Key) || !entry.Value.Equals(_attributes[entry.Key]))
                    {
                        if (delta == null)
                        {
                            delta = new XmlElement(_tag);
                        }
                        delta._attributes[entry.Key] = entry.Value;
                    }
                }
            }
            XmlNestedContent dc = update._contents;
            if (HasContent())
            {
                dc = _contents.Delta(dc);
            }
            if (dc != null)
            {
                if (delta == null)
                {
                    delta = new XmlElement(_tag);
                }
                delta._contents = dc;
            }
            return delta;
        }

        /// <summary>Format the list of Xml attributes.</summary>
        /// <param name="formatter">the formatter to apply.</param>
        /// <exception cref="System.IO.IOException">if formatting fails due to an I/O error.</exception>
        /// <exception cref="XmlSyntaxException">if formatting fails due to invalid Xml.</exception>
        /// <exception cref="XmlSyntaxException"/>
        public void FormatAttributes(XmlFormatter formatter)
        {
            foreach (var entry in _attributes)
            {
                formatter.FormatAttribute(entry.Key, entry.Value);
            }
        }

        /// <summary>Format the list of Xml content.</summary>
        /// <param name="formatter">the formatter to apply.</param>
        /// <exception cref="System.IO.IOException">if formatting fails due to an I/O error.</exception>
        /// <exception cref="XmlSyntaxException">if formatting fails due to invalid Xml.</exception>
        /// <exception cref="XmlSyntaxException"/>
        public void FormatContents(XmlFormatter formatter)
        {
            if (!HasContent()) return;
            var content = _contents.GetContent();
            lock (content)
            {
                foreach (var subElement in content)
                {
                    formatter.FormatElement(subElement);
                }
            }
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
            var xmlElement = o as XmlElement;
            if (xmlElement == null) return false;
            var element = xmlElement;
            if (_contents == null != (element._contents == null))
            {
                return false;
            }
            if (!_tag.Equals(element._tag))
            {
                return false;
            }
            if (!AreEqual(_attributes, element._attributes))
            {
                return false;
            }
            return _contents == null || _contents.Equals(element._contents);
        }

        private static bool AreEqual(IDictionary<string, XmlToken> d1, IDictionary<string, XmlToken> d2)
        {
            return d1.Count == d2.Count &&
                   d1.All(entry => d2.ContainsKey(entry.Key) && entry.Value.Equals(d2[entry.Key]));
        }


        public override int GetHashCode()
        {
            var result = _tag.GetHashCode();
            result = 31 * result + _attributes.GetHashCode();
            var contentsHashCode = 0;
            if (_contents != null)
            {
                contentsHashCode = _contents.GetHashCode();
            }
            result = 31 * result + contentsHashCode;
            return result;
        }

        public string GetAttributeValueAsText(string key)
        {
            if (GetAttributeValue(key) != null)
            {
                return GetAttributeValue(key).GetText();
            }
            return null;
        }
    }
}