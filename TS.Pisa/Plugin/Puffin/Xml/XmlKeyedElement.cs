namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <summary>
    /// A XmlKeyedElement is a class that represents an XmlElement that contains
    /// nothing but additional XmlElement objects.
    /// </summary>
    /// <remarks>
    /// A XmlKeyedElement is a class that represents an XmlElement that contains
    /// nothing but additional XmlElement objects. These are indexed using a key
    /// taken either from a fixed attribute of each sub-element or from the tag
    /// name of each sub-element.
    /// </remarks>
    /// <author>Paul Sweeny</author>
    public class XmlKeyedElement : XmlElement
    {
        /// <summary>The field with Xml sub-element to use as the primary key.</summary>
        public static readonly XmlName Key = new XmlName("KEY");

        /// <summary>The field with Xml sub-element to use as the notification to delete the element.</summary>
        public static readonly XmlName Delete = new XmlName("DELETE");

        private readonly XmlName _key;

        /// <summary>Construct a new XmlKeyedElement.</summary>
        /// <param name="tag">the tag of the element.</param>
        public XmlKeyedElement(XmlTag tag)
            : this(tag, null)
        {
        }

        /// <summary>Construct a new XmlKeyedElement.</summary>
        /// <param name="tag">the tag of the element.</param>
        /// <param name="key">the key attribute name of the sub-elemenets.</param>
        public XmlKeyedElement(XmlTag tag, XmlName key)
            : base(tag)
        {
            _key = key;
            if (key != null)
            {
                Add(Key, key);
            }
        }

        /// <summary>Construct a new XmlKeyedElement.</summary>
        /// <param name="toCopy">and element to copy.</param>
        public XmlKeyedElement(XmlElement toCopy)
            : base(toCopy.GetTag())
        {
            string key = toCopy.Get(Key, null);
            _key = key == null ? null : new XmlName(key);
            Update(toCopy, false);
        }

        /// <summary>Create the nested content container.</summary>
        public override XmlBasicContentContainer CreateContentContainer()
        {
            return new XmlElementContainer(_key);
        }
    }
}