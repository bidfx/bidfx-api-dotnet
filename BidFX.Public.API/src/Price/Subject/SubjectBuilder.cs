using System;
using System.Collections;
using System.Collections.Generic;

namespace BidFX.Public.API.Price.Subject
{
    public class SubjectBuilder : IComponentHandler, IEnumerable<SubjectComponent>
    {
        private string[] _components = new string[16];
        private int _size = 0;
        public bool AutoRefresh { get; set; }

        public Subject CreateSubject()
        {
            return new Subject(GetComponents(), AutoRefresh);
        }

        public SubjectBuilder SetComponent(string key, string value)
        {
            SubjectComponent(key, value);
            return this;
        }

        public void SubjectComponent(string key, string value)
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
                var index = SubjectUtils.BinarySearch(_components, _size, key);
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
            if (_size == _components.Length) ExtendCapacity();
            Array.Copy(_components, index, _components, index + 2, _size - index);
            _components[index] = key;
            _components[index + 1] = value;
            _size += 2;
        }

        private void ExtendCapacity()
        {
            var newCapacity = _components.Length << 1;
            var oldComponents = _components;
            _components = new string[newCapacity];
            Array.Copy(oldComponents, 0, _components, 0, _size);
        }

        private bool IsKeyInOrder(string key)
        {
            return _size == 0 || string.Compare(_components[_size - 2], key, StringComparison.Ordinal) < 0;
        }

        private static string InternaliseKey(string key)
        {
            var alt = CommonComponents.CommonKey(key);
            if (alt != null) return alt;
            SubjectValidator.ValidatePart(key, SubjectPart.Key);
            return key;
        }

        private static string InternaliseValue(string value)
        {
            var alt = CommonComponents.CommonKey(value);
            if (alt != null) return alt;
            SubjectValidator.ValidatePart(value, SubjectPart.Value);
            return value;
        }

        public string[] GetComponents()
        {
            var components = new string[_size];
            Array.Copy(_components, 0, components, 0, _size);
            return components;
        }

        public string LookupValue(string key)
        {
            var index = SubjectUtils.BinarySearch(_components, _size, key);
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

        public static Subject CreateLevelOneSpotRfsSubject(string source, string symbol, string currency,
            string quantity, string user, string account)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, symbol, currency, quantity, user, account, "1", "RFS", "Spot", source);
            return subjectBuilder
                .SetComponent("Customer",
                    "0001") //need to remove this once highway supports getting this itself for level 1
                .SetComponent(SubjectComponentName.Tenor, "Spot").CreateSubject();
        }

        public static Subject CreateLevelTwoSpotRfsSubject(string symbol, string currency, string quantity, string user,
            string account)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, symbol, currency, quantity, user, account, "2", "RFS", "Spot", "FXTS");
            return subjectBuilder
                .SetComponent(SubjectComponentName.Tenor, "Spot").CreateSubject();
        }

        public static void AddBasicComponents(SubjectBuilder subjectBuilder, string symbol, string currency,
            string quantity, string user, string account, string level, string quoteStyle, string subClass,
            string source)
        {
            subjectBuilder
                .SetComponent(SubjectComponentName.Account, account)
                .SetComponent(SubjectComponentName.AssetClass, "Fx")
                .SetComponent(SubjectComponentName.Currency, currency)
                .SetComponent(SubjectComponentName.QuoteStyle, quoteStyle)
                .SetComponent(SubjectComponentName.Exchange, "OTC")
                .SetComponent(SubjectComponentName.Source, source)
                .SetComponent(SubjectComponentName.SubClass, subClass)
                .SetComponent(SubjectComponentName.Level, level)
                .SetComponent(SubjectComponentName.Symbol, symbol)
                .SetComponent(SubjectComponentName.Quantity, quantity)
                .SetComponent(SubjectComponentName.User, user);
        }
    }
}