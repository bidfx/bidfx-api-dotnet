/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields
{
    /// <summary>
    /// This class provides a data dictionary of field definitions that can be extended safely with new definitions.
    /// </summary>
    internal class ExtendableDataDictionary : IDataDictionary
    {
        private readonly Dictionary<string, FieldDef> _fieldDefByName = new Dictionary<string, FieldDef>();
        private FieldDef[] _fieldDefsByFid;

        public ExtendableDataDictionary() : this(8)
        {
        }

        public ExtendableDataDictionary(int minimumCapacity)
        {
            _fieldDefsByFid = new FieldDef[CapacityPowerOfTwo(minimumCapacity)];
        }


        public FieldDef FieldDefByFid(int fid)
        {
            if (fid < 0)
            {
                return null;
            }

            return fid < _fieldDefsByFid.Length ? _fieldDefsByFid[fid] : null;
        }

        public FieldDef FieldDefByName(string fieldName)
        {
            FieldDef value;
            return _fieldDefByName.TryGetValue(fieldName, out value) ? value : null;
        }

        public void AddFieldDef(FieldDef fieldDef)
        {
            if (!DataDictionaryUtils.IsValid(fieldDef))
            {
                throw new ArgumentException("invalid field def added to dictionary: " + fieldDef);
            }

            EnsureCapactity(fieldDef.Fid + 1);
            FieldDef previous = _fieldDefsByFid[fieldDef.Fid];
            if (previous != null)
            {
                HandleDuplicateFid(previous);
            }

            _fieldDefsByFid[fieldDef.Fid] = fieldDef;
            bool tryGetValue = _fieldDefByName.TryGetValue(fieldDef.Name, out previous);
            if (tryGetValue)
            {
                HandleDuplicateName(previous);
            }

            _fieldDefByName[fieldDef.Name] = fieldDef;
        }

        private void HandleDuplicateFid(FieldDef previous)
        {
            _fieldDefByName.Remove(previous.Name);
        }

        private void HandleDuplicateName(FieldDef previous)
        {
            _fieldDefsByFid[previous.Fid] = null;
        }

        private void EnsureCapactity(int minimumCapacity)
        {
            if (_fieldDefsByFid.Length < minimumCapacity)
            {
                int newCapacity = CapacityPowerOfTwo(minimumCapacity);
                FieldDef[] oldData = _fieldDefsByFid;
                _fieldDefsByFid = new FieldDef[newCapacity];
                oldData.CopyTo(_fieldDefsByFid, 0);
            }
        }

        public List<FieldDef> AllFieldDefs()
        {
            return _fieldDefsByFid.Where(fieldDef => fieldDef != null).ToList();
        }

        public int Size()
        {
            return _fieldDefByName.Count;
        }

        public int NextFreeFid()
        {
            for (int i = _fieldDefsByFid.Length; i-- > 0;)
            {
                FieldDef fieldDef = _fieldDefsByFid[i];
                if (fieldDef != null)
                {
                    return fieldDef.Fid + 1;
                }
            }

            return 0;
        }

        private static int CapacityPowerOfTwo(int minimumCapacity)
        {
            int capacity = 1;
            while (capacity < minimumCapacity)
            {
                capacity <<= 1;
            }

            return capacity;
        }
    }
}