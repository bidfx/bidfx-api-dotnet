using System;
using System.Collections;
using System.Collections.Generic;

namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <summary>
    /// A XmlElementContainer is a class that represents nested content within an XmlElement that contains nothing but
    /// additional XmlElement objects.
    /// </summary>
    /// <remarks>
    /// A XmlElementContainer is a class that represents nested content within an XmlElement that contains nothing but
    /// additional XmlElement objects. These are indexed within the container using a key taken either from a fixed attribute
    /// of each sub-element or from the tag name of each sub-element.
    /// </remarks>
    /// <author>Paul Sweeny</author>
    public class XmlElementContainer : XmlBasicContentContainer
    {
        private readonly XmlName _key;
        private readonly IDictionary<string, XmlElement> _elements = new Dictionary<string, XmlElement>();

        /// <summary>Construct a new XmlElementContainer where sub-element tags are used as index keys.</summary>
        public XmlElementContainer()
        {
            _key = null;
        }

        /// <summary>Construct a new XmlElementContainer.</summary>
        /// <param name="key">the attribute used as the element key.</param>
        public XmlElementContainer(XmlName key)
        {
            _key = key;
        }

        /// <summary>Add a nested element.</summary>
        /// <param name="content">the content to add.</param>
        public override void Add(XmlElement content)
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
        public override void Update(IXmlContentContainer container, bool applyDeletes)
        {
            if (container == this)
            {
                return;
            }
            ICollection<XmlElement> content = container.GetContent();
            lock (content)
            {
                foreach (XmlElement element in content)
                {
                    try
                    {
                        string key = GetKeyOf(element);
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
            return element.Get(XmlKeyedElement.Delete, false);
        }

        public override void ClearDeletes()
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
        public override XmlElement GetElement(string key)
        {
            return _elements.ContainsKey(key) ? _elements[key] : null;
        }

        /// <summary>Get the set of keys used to index the sub-elements of this container.</summary>
        /// <returns>an unmodifiable Set of Strings.</returns>
        public override ICollection<string> GetKeys()
        {
            return _elements.Keys;
        }

        /// <summary>Get a collection of the content.</summary>
        /// <returns>an unmodifiable Collection of XmlElement objects.</returns>
        public override ICollection<XmlElement> GetContent()
        {
            return _elements.Values;
        }

        public override bool Equals(object o)
        {
            if (o == this)
            {
                return true;
            }
            var container = o as XmlElementContainer;
            if (container != null)
            {
                return AreEqual(_elements, container._elements);
            }
            return false;
        }

        private static bool AreEqual(IDictionary<string, XmlElement> d1, IDictionary<string, XmlElement> d2)
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


        /// <summary>Find the minimum delta elements that would be required to update these contents with another.</summary>
        /// <param name="update">the contents to derive the delta from.</param>
        /// <returns>the delta contents or null if no delta is required.</returns>
        public override XmlBasicContentContainer Delta(XmlBasicContentContainer update)
        {
            XmlElementContainer delta = null;
            if (update != null)
            {
                ICollection<string> keySet = update.GetKeys();
                lock (keySet)
                {
                    foreach (var key in keySet)
                    {
                        XmlElement value = update.GetElement(key);
                        XmlElement current = GetElement(key);
                        if (current != null)
                        {
                            value = current.Delta(value);
                            if (value != null && _key != null)
                            {
                                value.Add(_key, current.Get(_key, null));
                            }
                        }
                        if (value != null)
                        {
                            if (delta == null)
                            {
                                delta = new XmlElementContainer(_key);
                            }
                            delta.Add(value);
                        }
                    }
                }
            }
            return delta;
        }

        /// <summary>Get from an element the key used to index the element in this container.</summary>
        private string GetKeyOf(XmlElement element)
        {
            if (_key == null)
            {
                return element.GetTag().ToString();
            }
            return element.Get(_key, null);
        }
    }
}