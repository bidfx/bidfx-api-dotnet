/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Price.Subject
{
    internal class SubjectFormatter
    {
        private const char DefaultComponentSeperator = ',';
        private const char DefaultKeyValueSeperator = '=';

        private readonly char _componentSeperator;
        private readonly char _keyValueSeperator;
        private readonly NumericCharacterEntity _numericCharacterEntity = new NumericCharacterEntity();

        private static readonly ThreadLocal<StringBuilder> ThreadLocalBuilder =
            new ThreadLocal<StringBuilder>(() => new StringBuilder());

        public SubjectFormatter() : this(DefaultComponentSeperator, DefaultKeyValueSeperator)
        {
        }

        public SubjectFormatter(char componenetSeperator, char keyValueSeperator)
        {
            _componentSeperator = componenetSeperator;
            _keyValueSeperator = keyValueSeperator;
            _numericCharacterEntity.AddCharacterEncoding(componenetSeperator);
            _numericCharacterEntity.AddCharacterEncoding(' ');
        }

        public string FormatToString(IEnumerable<SubjectComponent> iterable)
        {
            StringBuilder builder = ThreadLocalBuilder.Value;
            builder.Length = 0;
            int count = 0;
            foreach (SubjectComponent entry in iterable)
            {
                if (count++ != 0)
                {
                    builder.Append(_componentSeperator);
                }

                builder.Append(entry.Key)
                    .Append(_keyValueSeperator)
                    .Append(_numericCharacterEntity.EncodeString(entry.Value));
            }

            return builder.ToString();
        }

        public string FormatSubjectComponents(string[] components)
        {
            if (components.Length == 0)
            {
                return "";
            }

            StringBuilder builder = ThreadLocalBuilder.Value;
            builder.Length = 0;
            char equal = _keyValueSeperator;
            char comma = _componentSeperator;
            NumericCharacterEntity entity = _numericCharacterEntity;

            builder.Append(components[0]).Append(equal).Append(entity.EncodeString(components[1]));
            for (int i = 2; i < components.Length; i += 2)
            {
                builder.Append(comma).Append(components[i]).Append(equal)
                    .Append(entity.EncodeString(components[i + 1]));
            }

            return builder.ToString();
        }

        public void ParseSubject(string formattedSubject, IComponentHandler handler)
        {
            if (formattedSubject == null)
            {
                throw new ArgumentNullException("null subject string");
            }

            if (handler == null)
            {
                throw new ArgumentNullException("null subject component handler");
            }

            if (formattedSubject.Length == 0)
            {
                throw new IllegalSubjectException("blank subject string");
            }

            StringCharIterator iterator = new StringCharIterator(formattedSubject, _componentSeperator);
            try
            {
                while (iterator.HasNext())
                {
                    string key = iterator.Next(_keyValueSeperator);
                    string encoded = iterator.Next(_componentSeperator);
                    string value = _numericCharacterEntity.DecodeString(encoded);
                    handler.SubjectComponent(key, value);
                }
            }
            catch (Exception e)
            {
                throw new IllegalSubjectException("Subject parse error: " + e.Message);
            }
        }
    }
}