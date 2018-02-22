using System;
using System.Collections.Generic;
using System.Text;

namespace BidFX.Public.API.Price
{
    internal class PriceMap : IPriceMap
    {
        private readonly Dictionary<string, IPriceField> _priceFields = new Dictionary<string, IPriceField>();

        internal void SetField(string name, IPriceField value)
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
            foreach (KeyValuePair<string, IPriceField> pair in _priceFields)
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
            IPriceField priceField = Field(name);
            if (priceField == null)
            {
                return null;
            }

            return priceField.Value as decimal? ?? ValueParser.ParseDecimal(priceField.Text, null);
        }

        public long? LongField(string name)
        {
            IPriceField priceField = Field(name);
            if (priceField == null)
            {
                return null;
            }

            return priceField.Value as long? ?? ValueParser.ParseLong(priceField.Text, null);
        }

        public int? IntField(string name)
        {
            IPriceField priceField = Field(name);
            if (priceField == null)
            {
                return null;
            }

            return priceField.Value as int? ?? ValueParser.ParseInt(priceField.Text, null);
        }

        public string StringField(string name)
        {
            IPriceField priceField = Field(name);
            return priceField == null ? null : priceField.Text;
        }

        public DateTime? DateTimeField(string name)
        {
            IPriceField priceField = Field(name);
            return priceField == null ? null : priceField.Value as DateTime?;
        }

        public Tick? TickField(string name)
        {
            IPriceField priceField = Field(name);
            return priceField == null ? null : priceField.Value as Tick?;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, IPriceField> pair in _priceFields)
            {
                if (sb.Length > 0)
                {
                    sb.Append(' ');
                }

                sb.Append(pair.Key);
                sb.Append('=').Append('"');
                sb.Append(pair.Value.Text);
                sb.Append('"');
            }

            return sb.ToString();
        }

        internal void MergedPriceMap(IPriceMap priceUpdate, bool replaceAllFields)
        {
            int oldBidLevels = IntField("BidLevels") ?? 0;
            int oldAskLevels = IntField("AskLevels") ?? 0;
            
            if (replaceAllFields)
            {
                _priceFields.Clear();
            }

            foreach (KeyValuePair<string, IPriceField> field in priceUpdate.PriceFields)
            {
                SetField(field.Key, field.Value);
            }
            
            int newBidLevels = IntField("BidLevels") ?? 0;
            int newAskLevels = IntField("AskLevels") ?? 0;
            
            for (int i = newBidLevels + 1; i <= oldBidLevels; i++)
            {
                _priceFields.Remove("Bid" + i);
                _priceFields.Remove("BidSize" + i);
                _priceFields.Remove("BidFirm" + i);
            }
            for (int i = newAskLevels + 1; i <= oldAskLevels; i++)
            {
                _priceFields.Remove("Ask" + i);
                _priceFields.Remove("AskSize" + i);
                _priceFields.Remove("AskFirm" + i);
            }
        }
        
        public void Clear()
        {
            _priceFields.Clear();
        }
    }
}