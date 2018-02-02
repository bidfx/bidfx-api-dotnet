using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BidFX.Public.API.Price.Subject
{
    /// <summary>
    /// A subject is used to identify information that may be queried or subscribed to.
    /// </summary>
    public class Subject : IEnumerable<SubjectComponent>
    {
        private static readonly SubjectFormatter Formatter = new SubjectFormatter();

        private readonly string[] _components;
        private int _hash;

        /// <summary>
        /// Creates a new subject.
        /// </summary>
        /// <param name="formattedSubject">a formatted subject string</param>
        public Subject(string formattedSubject) : this(BuildComponents(formattedSubject))
        {
        }

        /// <summary>
        /// Creates a subject from an array of components. A dangerous constructor that should only be called by a subject builder to ensure the subject is valid.
        /// </summary>
        /// <param name="components">the components</param>
        internal Subject(string[] components)
        {
            _components = components;
        }

        private static string[] BuildComponents(string formattedSubject)
        {
            SubjectBuilder builder = new SubjectBuilder();
            Formatter.ParseSubject(formattedSubject, builder);
            return builder.GetComponents();
        }

        /// <summary>
        /// Looks up the value of a variable.
        /// </summary>
        /// <param name="key">the name of the variable to look up</param>
        /// <returns>the variable value or null if the variable is undefiened</returns>
        public string GetComponent(string key)
        {
            int index = SubjectUtils.BinarySearch(_components, _components.Length, key);
            return index >= 0 ? _components[index + 1] : null;
        }

        /// <summary>
        /// Tests if this subject is empty - has no components.
        /// </summary>
        /// <returns>true if empty, false otherwise</returns>
        public bool IsEmpty()
        {
            return _components.Length == 0;
        }

        /// <summary>
        /// Gets the size of the subject in terms of its number of component pairs.
        /// </summary>
        /// <returns>the size of the subject</returns>
        public int Size()
        {
            return _components.Length >> 1;
        }

        internal string[] InternalComponents()
        {
            return _components;
        }

        public IEnumerator<SubjectComponent> GetEnumerator()
        {
            return new SubjectIterator(_components);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return Formatter.FormatSubjectComponents(_components);
        }

        protected bool Equals(Subject other)
        {
            return InternalComponents().SequenceEqual(other.InternalComponents());
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

            return obj.GetType() == GetType() && Equals((Subject) obj);
        }

        public override int GetHashCode()
        {
            return _hash == 0 ? _hash = CalculateHash(_components) : _hash;
        }

        private static int CalculateHash(IEnumerable<string> components)
        {
            return components.Aggregate(1, (current, component) => 31 * current + component.GetHashCode());
        }
    }
}