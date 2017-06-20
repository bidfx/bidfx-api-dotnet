using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BidFX.Public.API.Price.Subject
{
    public class Subject : IEnumerable<SubjectComponent>
    {
        private static readonly SubjectFormatter Formatter = new SubjectFormatter();
        
        private readonly string[] _components;
        private int _hash;

        public Subject(string formattedSubject) : this(BuildComponents(formattedSubject))
        {
            
        }

        public Subject(string[] components)
        {
            _components = components;
        }

        private static string[] BuildComponents(string formattedSubject)
        {
            var builder = new SubjectBuilder();
            Formatter.ParseSubject(formattedSubject, builder);
            return builder.GetComponents();
        }

        public string LookupValue(string key)
        {
            var index = SubjectUtils.BinarySearch(_components, _components.Length, key);
            return index >= 0 ? _components[index + 1] : null;
        }

        public bool IsEmpty()
        {
            return _components.Length == 0;
        }

        public int Size()
        {
            return _components.Length >> 1;
        }

        public string[] InternalComponents()
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
            return _components.SequenceEqual(other._components);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
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