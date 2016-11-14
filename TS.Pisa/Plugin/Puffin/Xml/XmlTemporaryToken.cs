namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <summary>
    /// This class provides a temporary Xml token used for constructing commonly
    /// used XmlToken instances.
    /// </summary>
    /// <remarks>
    /// This class provides a temporary Xml token used for constructing commonly
    /// used XmlToken instances.  The main difference between a temporary token and
    /// a normal one is that a temporary token does not always own a private copy
    /// of the bytes that define its text.  Hence no byte copying is required to
    /// create a temporary token, and so they are lightweigth and fast to create.
    /// However, since they do not own their data they are not good for long term
    /// storage in a collection - hence the name temporary.
    /// </remarks>
    /// <seealso cref="XmlToken"/>
    /// <author>Paul Sweeny</author>
    public class XmlTemporaryToken
    {
        private readonly int _type;
        private readonly byte[] _text;

        private int _start;
        private int _length;

        private int _hash;

        /// <summary>
        /// Create a new temporary token of a given type sharing its associated
        /// text from a region within a byte array.
        /// </summary>
        /// <param name="type">the type of the token.</param>
        /// <param name="text">a buffer containing the text associated with the token.</param>
        public XmlTemporaryToken(int type, byte[] text)
        {
            _type = type;
            _text = text;
        }

        /// <summary>
        /// Create a new temporary token of a given type sharing its associated
        /// text from a region within a byte array.
        /// </summary>
        /// <remarks>
        /// Create a new temporary token of a given type sharing its associated
        /// text from a region within a byte array.  This constructor is used for
        /// copying a token, hash code any all.
        /// </remarks>
        /// <param name="type">the type of the token.</param>
        /// <param name="text">a buffer containing the text associated with the token.</param>
        /// <param name="hash">the hash code.</param>
        public XmlTemporaryToken(int type, byte[] text, int hash)
        {
            _type = type;
            _text = text;
            _length = text.Length;
            _hash = hash;
        }

        public override bool Equals(object @object)
        {
            if (@object == this)
            {
                return true;
            }
            if (@object != null && @object is XmlTemporaryToken)
            {
                XmlTemporaryToken token = (XmlTemporaryToken) @object;
                if (token._type != _type)
                {
                    return false;
                }
                if (token._length != _length)
                {
                    return false;
                }
                int j = _length + token._start;
                int k = _length + _start;
                for (int i = _length; i > 0; --i)
                {
                    if (token._text[--j] != _text[--k])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>Reset the token to point a new portion of the same buffer.</summary>
        public XmlTemporaryToken Reset(int start, int len)
        {
            _start = start;
            _length = len;
            _hash = 0;
            return this;
        }

        /// <summary>Return the hash code of the token.</summary>
        public override int GetHashCode()
        {
            if (_hash != 0)
            {
                return _hash;
            }
            // The hashCode must be identical to that of the equivalent XmlToken.
            return _hash = XmlToken.ComputeHash(_type, _text, _start, _length);
        }

        /// <summary>Convert to a concrete token.</summary>
        public XmlToken ToToken()
        {
            return new XmlToken(_type, _text, _start, _length, _hash);
        }
    }
}