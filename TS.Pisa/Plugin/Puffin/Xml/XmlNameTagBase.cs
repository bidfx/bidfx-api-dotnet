using System;

namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <summary>An XmlNameTagBase is a common base class for XmlName and XmlTag.</summary>
    /// <author>Paul Sweeny</author>
    public class XmlNameTagBase
    {
        internal readonly XmlToken _token;

        /// <summary>Create a new XmlNameTagBase.</summary>
        /// <param name="name">the name of the token.</param>
        /// <param name="type">the type of the name/tag token.</param>
        public XmlNameTagBase(string name, int type)
        {
            if (!IsValidName(name))
            {
                throw new ArgumentException("invalid Xml name: " + name);
            }
            _token = XmlToken.GetInstance(type, name);
        }

        /// <summary>Create a new XmlNameTagBase.</summary>
        /// <param name="name">the name of the token.</param>
        public XmlNameTagBase(XmlToken name)
        {
            _token = name;
        }

        /// <summary>Test if the given name is a valid Xml tag or attrinute name.</summary>
        /// <param name="name">the name to test.</param>
        public static bool IsValidName(string name)
        {
            if (name == null)
            {
                return false;
            }
            if (name.Length == 0)
            {
                return false;
            }
            char[] c = name.ToCharArray();
            if (!IsNameFirstChar(c[0]))
            {
                return false;
            }
            for (int i = 1; i < c.Length; ++i)
            {
                if (!IsNameChar(c[i]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>Test if the given character is a valid name character.</summary>
        private static bool IsNameChar(char c)
        {
            return char.IsLetterOrDigit(c) || c == '_' || c == ':' || c == '.' || c == '-';
        }

        /// <summary>Test if the given character is a valid first name character.</summary>
        private static bool IsNameFirstChar(char c)
        {
            return char.IsLetter(c) || c == '_' || c == ':';
        }

        /// <summary>Convert this to an XmlToken.</summary>
        public virtual XmlToken ToToken()
        {
            return _token;
        }

        public override bool Equals(object @object)
        {
            if (@object == this)
            {
                return true;
            }
            if (@object != null && @object is XmlNameTagBase)
            {
                XmlNameTagBase name = (XmlNameTagBase) @object;
                return _token.Equals(name._token);
            }
            return false;
        }

        /// <summary>Return the hash code of the token.</summary>
        public override int GetHashCode()
        {
            return _token.GetHashCode();
        }

        /// <summary>Convert this to String.</summary>
        public override string ToString()
        {
            return _token.ToString();
        }
    }
}