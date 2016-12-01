using System.Collections.Generic;

namespace TS.Pisa.Plugin.Puffin
{
    internal class PuffinLevelOnePriceMap : IPriceMap
    {
        private readonly PuffinElement _element;
        private readonly Dictionary<string, IPriceField> _priceFields = new Dictionary<string, IPriceField>();

        public PuffinLevelOnePriceMap(PuffinElement element)
        {
            _element = element;
            foreach (var attribute in element.Attributes)
            {
                _priceFields[attribute.Key] = attribute.Value;
            }
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
            return _element.ToString();
        }
    }
}