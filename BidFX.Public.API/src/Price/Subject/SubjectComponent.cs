/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

using System.Text;

namespace BidFX.Public.API.Price.Subject
{
    /// <summary>
    /// A component part of a subject.
    /// </summary>
    public class SubjectComponent
    {
        private readonly string _key;
        private readonly string _value;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="key">the subject components key</param>
        /// <param name="value">the subject components value</param>
        public SubjectComponent(string key, string value)
        {
            _key = key;
            _value = value;
        }

        /// <summary>
        /// The key name of the component
        /// </summary>
        public string Key
        {
            get { return _key; }
        }

        /// <summary>
        /// The value of the component
        /// </summary>
        public string Value
        {
            get { return _value; }
        }

        public override string ToString()
        {
            return new StringBuilder().Append(_key).Append('=').Append(_value).ToString();
        }

        protected bool Equals(SubjectComponent other)
        {
            return string.Equals(_key, other._key) && string.Equals(_value, other._value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((SubjectComponent) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_key != null ? _key.GetHashCode() : 0) * 397) ^ (_value != null ? _value.GetHashCode() : 0);
            }
        }
    }
}