/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;

namespace BidFX.Public.API.Price.Subject
{
    internal class SubjectIterator : IEnumerator<SubjectComponent>
    {
        private readonly string[] _components;
        private int _position = -1;

        public SubjectIterator(string[] components)
        {
            _components = components;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            _position++;
            bool moveNext = _components.Length > _position * 2 + 1;
            return moveNext;
        }

        public bool HasNext()
        {
            return _components.Length > (_position + 1) * 2 + 1;
        }

        public void Reset()
        {
            _position = 0;
        }

        private SubjectComponent GetCurrent()
        {
            return new SubjectComponent(_components[2 * _position], _components[2 * _position + 1]);
        }

        public SubjectComponent Current
        {
            get { return GetCurrent(); }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}