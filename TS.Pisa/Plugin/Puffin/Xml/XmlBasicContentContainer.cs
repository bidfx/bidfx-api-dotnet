using System;
using System.Collections;
using System.Collections.Generic;

namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <summary>
    /// A XmlBasicContentContainer is an abstract base class that represents nested
    /// content within an XmlElement.
    /// </summary>
    /// <author>Paul Sweeny</author>
    public abstract class XmlBasicContentContainer : IXmlContentContainer
    {
        /// <summary>Add a nested element.</summary>
        /// <param name="content">the content to add.</param>
        public abstract void Add(XmlElement content);

        /// <summary>Update this container with the contents of another container.</summary>
        /// <param name="container">the content to update this content with.</param>
        public virtual void Update(IXmlContentContainer container, bool applyDeletes)
        {
        }

        /// <summary>Removes nested sub-elements marked with the deletion attribute.</summary>
        public virtual void ClearDeletes()
        {
        }

        /// <summary>Get the first nested Xml element.</summary>
        /// <exception cref="XmlSyntaxException">is there is no initial nested sub-element.</exception>
        /// <exception cref="XmlSyntaxException"/>
        public virtual XmlElement GetFirstElement()
        {
            ICollection<XmlElement> content = GetContent();
            if (content != null)
            {
                foreach (var element in content)
                {
                    return element;
                }
            }
            throw new XmlSyntaxException("no content available");
        }

        /// <summary>Get the nested element with the given key.</summary>
        /// <param name="key">the key (normally tag name) of the element to get.</param>
        public virtual XmlElement GetElement(object key)
        {
            return GetElement(key.ToString());
        }

        /// <summary>
        /// Find the minimum delta elements that would be required to update these
        /// contents with another.
        /// </summary>
        /// <param name="update">the contents to derive the delta from.</param>
        /// <returns>the delta contents or null if no delta is required.</returns>
        public virtual XmlBasicContentContainer Delta(XmlBasicContentContainer update)
        {
            return null;
        }

        public abstract ICollection<XmlElement> GetContent();

        public abstract XmlElement GetElement(string arg1);
        public abstract ICollection<string> GetKeys();
    }
}