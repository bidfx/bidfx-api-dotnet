using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        private readonly XmlToken _tag;

        private readonly IDictionary<XmlToken, XmlToken> _attributes = new Dictionary<XmlToken, XmlToken>(8);
        private XmlBasicContentContainer _contents;

        /// <summary>Create a new XmlElement.</summary>
        /// <param name="tag">the tag of the element.</param>
        public XmlElement(XmlTag tag)
        {
            if (tag == null)
            {
                throw new ArgumentException("null Xml tag");
            }
            _tag = tag._token;
        }

        /// <summary>Create a new XmlElement.</summary>
        /// <param name="tag">the tag of the element.</param>
        /// <exception cref="System.ArgumentException">
        /// is the given token is not of type
        /// <see cref="XmlToken.StartType"/>
        /// .
        /// </exception>
        public XmlElement(XmlToken tag)
        {
            if (!tag.IsStartTag())
            {
                throw new ArgumentException("invalid tag type " + tag);
            }
            _tag = tag;
        }

        /// <summary>Create a new XmlElement.</summary>
        /// <param name="tag">the tag of the element.</param>
        public XmlElement(string tag)
            : this(new XmlTag(tag))
        {
        }

        /// <summary>Add an Xml element to the end of the nested content of this element.</summary>
        /// <param name="element">the element to add.</param>
        /// <returns>this element.</returns>
        public virtual XmlElement Add(XmlElement element)
        {
            lock (this)
            {
                if (_contents == null)
                {
                    _contents = CreateContentContainer();
                }
                _contents.Add(element);
                return this;
            }
        }

        /// <summary>Add an Xml attribute to this element.</summary>
        /// <param name="name">the name of the attribute.</param>
        /// <param name="value">the value of the attribute.</param>
        /// <returns>this element.</returns>
        /// <exception cref="XmlSyntaxException">
        /// is the given name token is not of type
        /// <see cref="XmlToken.NameType"/>
        /// or if the value is not a value type.
        /// </exception>
        /// <exception cref="XmlSyntaxException"/>
        public virtual XmlElement Add(XmlToken name, XmlToken value)
        {
            lock (this)
            {
                if (!name.IsAttributeName())
                {
                    throw new XmlSyntaxException("invalid attribute name " + name);
                }
                if (!value.IsValueType())
                {
                    throw new XmlSyntaxException("invalid attribute value " + value);
                }
                _attributes[name] = value;
                return this;
            }
        }

        /// <summary>Add an Xml attribute to this element.</summary>
        /// <param name="name">the name of the attribute.</param>
        /// <param name="value">the value of the attribute.</param>
        /// <returns>this element.</returns>
        /// <exception cref="XmlSyntaxException">is the given value token is not a value type.</exception>
        /// <exception cref="XmlSyntaxException"/>
        public virtual XmlElement Add(XmlName name, XmlToken value)
        {
            lock (this)
            {
                if (!value.IsValueType())
                {
                    throw new XmlSyntaxException("invalid attribute value " + value);
                }
                _attributes[name._token] = value;
                return this;
            }
        }

        /// <summary>Add an Xml attribute to this element.</summary>
        /// <param name="name">the name of the attribute.</param>
        /// <param name="value">the value of the attribute.</param>
        /// <returns>this element.</returns>
        public virtual XmlElement Add(XmlName name, int value)
        {
            lock (this)
            {
                _attributes[name._token] = value == 0 ? XmlToken.ZeroToken : new XmlToken(value);
                return this;
            }
        }

        /// <summary>Add an Xml attribute to this element.</summary>
        /// <param name="name">the name of the attribute.</param>
        /// <param name="value">the value of the attribute.</param>
        /// <returns>this element.</returns>
        public virtual XmlElement Add(XmlName name, long value)
        {
            lock (this)
            {
                _attributes[name._token] = value == 0 ? XmlToken.ZeroToken : new XmlToken(value);
                return this;
            }
        }

        /// <summary>Add an Xml attribute to this element.</summary>
        /// <param name="name">the name of the attribute.</param>
        /// <param name="value">the value of the attribute.</param>
        /// <returns>this element.</returns>
        public virtual XmlElement Add(XmlName name, double value)
        {
            lock (this)
            {
                _attributes[name._token] = value == 0.0 ? XmlToken.ZeroToken : new XmlToken(value);
                return this;
            }
        }

        /// <summary>Add an Xml attribute to this element.</summary>
        /// <param name="name">the name of the attribute.</param>
        /// <param name="value">the value of the attribute.</param>
        /// <returns>this element.</returns>
        public virtual XmlElement Add(XmlName name, object value)
        {
            lock (this)
            {
                if (value == null)
                {
                    throw new ArgumentException("null Xml attribute value");
                }
                _attributes[name._token] = new XmlToken(value.ToString());
                return this;
            }
        }

        /// <summary>Add an Xml attribute to this element.</summary>
        /// <param name="name">the name of the attribute.</param>
        /// <param name="value">the value of the attribute.</param>
        /// <returns>this element.</returns>
        public virtual XmlElement Add(XmlName name, char value)
        {
            lock (this)
            {
                return Add(name, value.ToString());
            }
        }

        /// <summary>Add an Xml attribute to this element.</summary>
        /// <param name="name">the name of the attribute.</param>
        /// <param name="value">the value of the attribute.</param>
        /// <returns>this element.</returns>
        public virtual XmlElement Add(XmlName name, bool value)
        {
            lock (this)
            {
                _attributes[name._token] = new XmlToken(value);
                return this;
            }
        }

        /// <summary>Get the tag name.</summary>
        public virtual XmlToken GetTag()
        {
            lock (this)
            {
                return _tag;
            }
        }

        /// <summary>Test if this element is of the given type.</summary>
        public virtual bool IsA(XmlTag type)
        {
            lock (this)
            {
                return _tag.Equals(type._token);
            }
        }

        /// <summary>Test if this element has nested content.</summary>
        public virtual bool HasContent()
        {
            lock (this)
            {
                return _contents != null;
            }
        }

        /// <summary>Test if this element has any attributes.</summary>
        public virtual bool HasAttributes()
        {
            lock (this)
            {
                return _attributes.Count != 0;
            }
        }

        /// <summary>Get a collection of the attribute names if this element.</summary>
        /// <returns>
        /// an unmodifiable list of
        /// <see cref="XmlName"/>
        /// s.
        /// </returns>
        public virtual IList<XmlName> GetAttributeNames()
        {
            lock (this)
            {
                IList<XmlName> names = new List<XmlName>(_attributes.Count);
                lock (_attributes)
                {
                    foreach (var key in _attributes.Keys)
                    {
                        names.Add(new XmlName(key));
                    }
                }
                return names;
            }
        }

        /// <summary>Get the value of an Xml attribute.</summary>
        /// <param name="name">the name of the attribute.</param>
        /// <returns>the attribute value or null.</returns>
        public virtual XmlToken Get(XmlName name)
        {
            lock (this)
            {
                return _attributes.ContainsKey(name._token) ? _attributes[name._token] : null;
            }
        }

        /// <summary>Get the value of an Xml attribute.</summary>
        /// <param name="name">the name of the attribute.</param>
        /// <param name="defaultValue">the default value if the name is not present.</param>
        /// <returns>the attribute value or the default.</returns>
        public virtual int Get(XmlName name, int defaultValue)
        {
            lock (this)
            {
                return _attributes.ContainsKey(name._token) ? _attributes[name._token].ToInteger() : defaultValue;
            }
        }

        /// <summary>Get the value of an Xml attribute.</summary>
        /// <param name="name">the name of the attribute.</param>
        /// <param name="defaultValue">the default value if the name is not present.</param>
        /// <returns>the attribute value or the default.</returns>
        public virtual long Get(XmlName name, long defaultValue)
        {
            lock (this)
            {
                return _attributes.ContainsKey(name._token) ? _attributes[name._token].ToLong() : defaultValue;
            }
        }

        /// <summary>Get the value of an Xml attribute.</summary>
        /// <param name="name">the name of the attribute.</param>
        /// <param name="defaultValue">the default value if the name is not present.</param>
        /// <returns>the attribute value or the default.</returns>
        public virtual double Get(XmlName name, double defaultValue)
        {
            lock (this)
            {
                return _attributes.ContainsKey(name._token) ? _attributes[name._token].ToDouble() : defaultValue;
            }
        }

        /// <summary>Get the value of an Xml attribute.</summary>
        /// <param name="name">the name of the attribute.</param>
        /// <param name="defaultValue">the default value if the name is not present.</param>
        /// <returns>the attribute value or the default.</returns>
        public virtual string Get(XmlName name, string defaultValue)
        {
            lock (this)
            {
                return _attributes.ContainsKey(name._token) ? _attributes[name._token].ToString() : defaultValue;
            }
        }

        /// <summary>Get the value of an Xml attribute.</summary>
        /// <param name="name">the name of the attribute.</param>
        /// <param name="defaultValue">the default value if the name is not present.</param>
        /// <returns>the attribute value or the default.</returns>
        public virtual bool Get(XmlName name, bool defaultValue)
        {
            lock (this)
            {
                return _attributes.ContainsKey(name._token) ? _attributes[name._token].ToBoolean() : defaultValue;
            }
        }

        /// <summary>Remove an Xml attribute.</summary>
        /// <param name="name">the name of the attribute.</param>
        public virtual void Remove(XmlName name)
        {
            lock (this)
            {
                _attributes.Remove(name._token);
            }
        }

        /// <summary>Get the nested contect of this Xml element.</summary>
        /// <returns>the content list or null if there is none.</returns>
        public virtual IXmlContentContainer GetContents()
        {
            lock (this)
            {
                return _contents;
            }
        }

        /// <summary>Create the nested content container.</summary>
        public virtual XmlBasicContentContainer CreateContentContainer()
        {
            return new XmlElementContainer();
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
        public virtual void Update(XmlElement update)
        {
            lock (this)
            {
                Update(update, false);
            }
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
        public virtual void Update(XmlElement update, bool applyDeletes)
        {
            lock (this)
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
                            _contents = CreateContentContainer();
                        }
                        _contents.Update(update._contents, applyDeletes);
                    }
                }
            }
        }

        /// <summary>Removes nested sub-elements marked with the deletion attribute.</summary>
        public virtual void ClearDeletes()
        {
            lock (this)
            {
                _contents?.ClearDeletes();
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
        public virtual XmlElement Delta(XmlElement update)
        {
            lock (this)
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
                XmlBasicContentContainer dc = update._contents;
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
        }

        /// <summary>Format the list of Xml attributes.</summary>
        /// <param name="formatter">the formatter to apply.</param>
        /// <exception cref="System.IO.IOException">if formatting fails due to an I/O error.</exception>
        /// <exception cref="XmlSyntaxException">if formatting fails due to invalid Xml.</exception>
        /// <exception cref="XmlSyntaxException"/>
        public virtual void FormatAttributes(IXmlFormatter formatter)
        {
            lock (_attributes)
            {
                foreach (var entry in _attributes)
                {
                    formatter.Format(entry.Key);
                    formatter.Format(entry.Value);
                }
            }
        }

        /// <summary>Format the list of Xml content.</summary>
        /// <param name="formatter">the formatter to apply.</param>
        /// <exception cref="System.IO.IOException">if formatting fails due to an I/O error.</exception>
        /// <exception cref="XmlSyntaxException">if formatting fails due to invalid Xml.</exception>
        /// <exception cref="XmlSyntaxException"/>
        public virtual void FormatContents(IXmlFormatter formatter)
        {
            if (HasContent())
            {
                ICollection<XmlElement> content = _contents.GetContent();
                lock (content)
                {
                    foreach (var subElement in content)
                    {
                        formatter.Format(subElement);
                    }
                }
            }
        }

        /// <summary>Format this object using the given formatter.</summary>
        /// <param name="formatter">the formatter to use to format this object.</param>
        /// <exception cref="System.IO.IOException">if formatting fails due to an I/O error.</exception>
        /// <exception cref="XmlSyntaxException">if formatting fails due to invalid Xml.</exception>
        /// <exception cref="XmlSyntaxException"/>
        public virtual void FormatUsing(IXmlFormatter formatter)
        {
            lock (this)
            {
                formatter.Format(this);
            }
        }

        public override string ToString()
        {
            lock (this)
            {
                var stream = new MemoryStream();
                var formatter = new XmlOutputFormatter(stream);
                try
                {
                    formatter.Format(this);
                    formatter.Flush();
                }
                catch (Exception)
                {
                }
                return Encoding.ASCII.GetString(stream.ToArray(), 0, stream.ToArray().Length);
            }
        }

        public override bool Equals(object o)
        {
            lock (this)
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
        }

        private static bool AreEqual(IDictionary<XmlToken, XmlToken> d1, IDictionary<XmlToken, XmlToken> d2)
        {
            if (d1.Count != d2.Count) return false;
            foreach (var entry in d1)
            {
                if (!d2.ContainsKey(entry.Key) || !entry.Value.Equals(d2[entry.Key]))
                {
                    return false;
                }
            }
            return true;
        }


        public override int GetHashCode()
        {
            int result;
            result = _tag.GetHashCode();
            result = 31 * result + _attributes.GetHashCode();
            result = 31 * result + (_contents != null ? _contents.GetHashCode() : 0);
            return result;
        }
    }
}