/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using BidFX.Public.API.Enums;
using BidFX.Public.API.Price.Tools;
using Serilog;
using Serilog.Core;

namespace BidFX.Public.API.Price.Subject
{
    /// <summary>
    /// This class provides a mutable subject builder.
    /// </summary>
    public class SubjectBuilder : IComponentHandler, IEnumerable<SubjectComponent>
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(Constants.SourceContextPropertyName, "SubjectBuilder");
        private string[] _components;
        private int _size;

        /// <summary>
        /// Creates a new subject from the components that have been set.
        /// </summary>
        /// <returns></returns>
        public Subject CreateSubject()
        {
            return new Subject(GetComponents());
        }

        /// <summary>
        ///  Create an empty SubjectBuilder
        /// </summary>
        public SubjectBuilder()
        {
            _components = new string[16];
            _size = 0;
        }

        /// <summary>
        /// Create a new SubjectBuilder, pre-populated with components from an existing Subject
        /// </summary>
        /// <param name="subject">The Subject to clone data from</param>
        public SubjectBuilder(Subject subject)
        {
            _size = subject.Size() << 1;
            _components = new string[_size];
            Array.Copy(subject.InternalComponents(), 0, _components, 0, _size);
        }

        /// <summary>
        ///  Create a new SubjectBuilder, pre-populated with components from another SubjectBuilder
        /// </summary>
        /// <param name="subjectBuilder">The SubjectBuilder to clone data from</param>
        public SubjectBuilder(SubjectBuilder subjectBuilder)
        {
            _size = subjectBuilder._size;
            _components = subjectBuilder.GetComponents();
        }

        /// <summary>
        /// Sets a component of the subject.
        /// </summary>
        /// <param name="key">the components key</param>
        /// <param name="value">the components value</param>
        /// <returns>the builder so that calls can be chained</returns>
        public SubjectBuilder SetComponent(string key, string value)
        {
            if (key != null && key.Contains("FixingDate"))
            {
                Log.Information("Received key {key}, not allowed. Not adding component.", key);
                return this;
            }
            ((IComponentHandler) this).SubjectComponent(key, value);
            return this;
        }

        /// <summary>
        ///  Sets a component of the subject, only if the value is not null
        /// </summary>
        /// <param name="key">the components key</param>
        /// <param name="value">the components value</param>
        /// <returns>the builder so that calls can be chained</returns>
        public SubjectBuilder SetIfNotNullOrEmpty(string key, string value)
        {
            return !string.IsNullOrEmpty(value) ? SetComponent(key, value) : this;
        }

        void IComponentHandler.SubjectComponent(string key, string value)
        {
            AddComponent(InternaliseKey(key), InternaliseValue(value));
        }

        private void AddComponent(string key, string value)
        {
            if (IsKeyInOrder(key))
            {
                AddComponentAt(_size, key, value);
            }
            else
            {
                int index = SubjectUtils.BinarySearch(_components, _size, key);
                if (index < 0)
                {
                    AddComponentAt(-1 - index, key, value);
                }
                else
                {
                    _components[index + 1] = value;
                }
            }
        }

        private void AddComponentAt(int index, string key, string value)
        {
            if (_size == _components.Length)
            {
                ExtendCapacity();
            }

            Array.Copy(_components, index, _components, index + 2, _size - index);
            _components[index] = key;
            _components[index + 1] = value;
            _size += 2;
        }

        private void ExtendCapacity()
        {
            int newCapacity = _components.Length << 1;
            string[] oldComponents = _components;
            _components = new string[newCapacity];
            Array.Copy(oldComponents, 0, _components, 0, _size);
        }

        private bool IsKeyInOrder(string key)
        {
            return _size == 0 || string.Compare(_components[_size - 2], key, StringComparison.Ordinal) < 0;
        }

        private static string InternaliseKey(string key)
        {
            string alt = CommonComponents.CommonKey(key);
            if (alt != null)
            {
                return alt;
            }

            SubjectValidator.ValidatePart(key, SubjectPart.Key);
            return key;
        }

        private static string InternaliseValue(string value)
        {
            string alt = CommonComponents.CommonValue(value);
            if (alt != null)
            {
                return alt;
            }

            SubjectValidator.ValidatePart(value, SubjectPart.Value);
            return value;
        }

        internal string[] GetComponents()
        {
            string[] components = new string[_size];
            Array.Copy(_components, 0, components, 0, _size);
            return components;
        }

        /// <summary>
        /// Looks up the value of a variable.
        /// </summary>
        /// <param name="key">the name of the variable to look up</param>
        /// <returns>the variable value of null if the variable is undefined</returns>
        public string LookupValue(string key)
        {
            int index = SubjectUtils.BinarySearch(_components, _size, key);
            return index >= 0 ? _components[index + 1] : null;
        }

        public IEnumerator<SubjectComponent> GetEnumerator()
        {
            return new SubjectIterator(GetComponents());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            _size = 0;
        }

        public override string ToString()
        {
            return new SubjectFormatter().FormatToString(this);
        }
    }
}