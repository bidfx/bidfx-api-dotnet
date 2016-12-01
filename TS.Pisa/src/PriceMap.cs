using System.Collections.Generic;
using System.Text;

namespace TS.Pisa
{
    internal class PriceMap : IPriceMap
    {
        private readonly Dictionary<string, IPriceField> _priceFields = new Dictionary<string, IPriceField>();

        internal void Set(string name, IPriceField value)
        {
            _priceFields[name] = value;
        }

        public IEnumerable<string> FieldNames
        {
            get { return _priceFields.Keys; }
        }

        public IEnumerable<KeyValuePair<string, IPriceField>> PriceFields
        {
            get { return _priceFields; }
        }

        public IPriceField Field(string name)
        {
            foreach (var pair in _priceFields)
            {
                if (name.Equals(pair.Key))
                {
                    return pair.Value;
                }
            }
            return null;
        }

        public decimal? DecimalField(string name)
        {
            var priceField = Field(name);
            if (priceField == null) return null;
            return priceField.Value as decimal? ?? ValueParser.ParseDecimal(priceField.Text, null);
        }

        public long? LongField(string name)
        {
            var priceField = Field(name);
            if (priceField == null) return null;
            return priceField.Value as long? ?? ValueParser.ParseLong(priceField.Text, null);
        }

        public int? IntField(string name)
        {
            var priceField = Field(name);
            if (priceField == null) return null;
            return priceField.Value as int? ?? ValueParser.ParseInt(priceField.Text, null);
        }

        public string StringField(string name)
        {
            var priceField = Field(name);
            return priceField == null ? null : priceField.Text;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var pair in _priceFields)
            {
                sb.Append(' ');
                sb.Append(pair.Key);
                sb.Append('=').Append('"');
                sb.Append(pair.Value.Text);
                sb.Append('"');
            }
            return sb.ToString();
        }

        internal void MergedPriceMap(IPriceMap priceUpdate, bool replaceAllFields)
        {
            if (replaceAllFields)
            {
                _priceFields.Clear();
            }
            foreach (var field in priceUpdate.PriceFields)
            {
                Set(field.Key, field.Value);
            }
        }

        public void Clear()
        {
            _priceFields.Clear();
        }
    }
}