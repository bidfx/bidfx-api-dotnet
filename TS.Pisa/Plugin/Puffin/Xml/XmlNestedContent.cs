using System;
using System.Collections.Generic;
using System.Linq;

namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <summary>
    /// This class represents nested content within an XmlElement that contains additional XmlElement objects.
    /// </summary>
    /// <author>Paul Sweeny</author>
    public class XmlNestedContent
    {
        private readonly string _key;
        private readonly IDictionary<string, XmlElement> _elements = new Dictionary<string, XmlElement>();

        /// <summary>Construct a new XmlElementContainer where sub-element tags are used as index keys.</summary>
        public XmlNestedContent()
        {
            _key = null;
        }

        /// <summary>Construct a new XmlElementContainer.</summary>
        /// <param name="key">the attribute used as the element key.</param>
        public XmlNestedContent(string key)
        {
            _key = key;
        }

        /// <summary>Get the first nested Xml element.</summary>
        /// <exception cref="XmlSyntaxException">is there is no initial nested sub-element.</exception>
        /// <exception cref="XmlSyntaxException"/>
        public XmlElement GetFirstElement()
        {
            var content = GetContent();
            if (content == null) throw new XmlSyntaxException("no content available");
            foreach (var element in content)
            {
                return element;
            }
            throw new XmlSyntaxException("no content available");
        }

        /// <summary>Get the nested element with the given key.</summary>
        /// <param name="key">the key (normally tag name) of the element to get.</param>
        public XmlElement GetElement(object key)
        {
            return GetElement(key.ToString());
        }

        /// <summary>Add a nested element.</summary>
        /// <param name="content">the content to add.</param>
        public void Add(XmlElement content)
        {
            if (content == null)
            {
                throw new ArgumentException("cannot add null content");
            }
            var key = GetKeyOf(content);
            _elements[key] = content;
        }

        /// <summary>Update this container with the contents of another container.</summary>
        /// <param name="container">the container to update this container with.</param>
        /// <param name="applyDeletes">
        /// when <code>true</code> any element marked with
        /// deletion attribute in the update container will cause the same element
        /// to be removed from this container (if it exists); when
        /// <code>false</code> a deletion attribute will be treated like any other.
        /// </param>
        public void Update(XmlNestedContent container, bool applyDeletes)
        {
            if (container == this)
            {
                return;
            }
            var content = container.GetContent();
            lock (content)
            {
                foreach (var element in content)
                {
                    try
                    {
                        var key = GetKeyOf(element);
                        if (applyDeletes && IsDeletedSubElement(element))
                        {
                            _elements.Remove(key);
                        }
                        else
                        {
                            if (!_elements.ContainsKey(key) || IsDeletedSubElement(_elements[key]))
                            {
                                _elements[key] = element;
                            }
                            else
                            {
                                _elements[key].Update(element, applyDeletes);
                            }
                        }
                    }
                    catch (InvalidCastException)
                    {
                    }
                }
            }
        }

        // quietly ignore non-XmlElement content
        private static bool IsDeletedSubElement(XmlElement element)
        {
            var token = element.GetAttributeValue(XmlKeyedElement.Delete);
            return token != null && "true".Equals(token.GetText());
        }

        public void ClearDeletes()
        {
            foreach (var entry in _elements)
            {
                if (IsDeletedSubElement(entry.Value))
                {
                    _elements.Remove(entry.Key);
                }
                else
                {
                    entry.Value.ClearDeletes();
                }
            }
        }

        /// <summary>Get the nested element with the given key.</summary>
        /// <param name="key">the key (normally tag name) of the element to get.</param>
        public XmlElement GetElement(string key)
        {
            return _elements.ContainsKey(key) ? _elements[key] : null;
        }

        /// <summary>Get the set of keys used to index the sub-elements of this container.</summary>
        /// <returns>an unmodifiable Set of Strings.</returns>
        public ICollection<string> GetKeys()
        {
            return _elements.Keys;
        }

        /// <summary>Get a collection of the content.</summary>
        /// <returns>an unmodifiable Collection of XmlElement objects.</returns>
        public ICollection<XmlElement> GetContent()
        {
            return _elements.Values;
        }

        public override bool Equals(object o)
        {
            if (o == this)
            {
                return true;
            }
            var container = o as XmlNestedContent;
            return container != null && AreEqual(_elements, container._elements);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private static bool AreEqual(IDictionary<string, XmlElement> d1, IDictionary<string, XmlElement> d2)
        {
            return d1.Count == d2.Count &&
                   d1.All(entry => d2.ContainsKey(entry.Key) && entry.Value.Equals(d2[entry.Key]));
        }


        /// <summary>Find the minimum delta elements that would be required to update these contents with another.</summary>
        /// <param name="update">the contents to derive the delta from.</param>
        /// <returns>the delta contents or null if no delta is required.</returns>
        public XmlNestedContent Delta(XmlNestedContent update)
        {
            if (update == null) return null;
            XmlNestedContent delta = null;
            var keySet = update.GetKeys();
            lock (keySet)
            {
                foreach (var key in keySet)
                {
                    var value = update.GetElement(key);
                    var current = GetElement(key);
                    if (current != null)
                    {
                        value = current.ComputeDelta(value);
                        if (value != null && _key != null)
                        {
                            value.AddAttribute(_key, current.GetAttributeValueAsText(_key));
                        }
                    }
                    if (value == null) continue;
                    if (delta == null)
                    {
                        delta = new XmlNestedContent(_key);
                    }
                    delta.Add(value);
                }
            }
            return delta;
        }

        /// <summary>Get from an element the key used to index the element in this container.</summary>
        private string GetKeyOf(XmlElement element)
        {
            return _key == null ? element.GetTag() : element.GetAttributeValueAsText(_key);
        }
    }
}