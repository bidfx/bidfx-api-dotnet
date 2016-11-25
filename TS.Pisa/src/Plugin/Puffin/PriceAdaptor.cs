using System;
using System.Collections.Generic;

namespace TS.Pisa.Plugin.Puffin
{
    internal class PriceAdaptor
    {
        public static IPriceMap ToPriceMap(PuffinElement element)
        {
            if (element == null) return null;
            return new LevelOnePriceMap(element);
        }

        public class LevelOnePriceMap : IPriceMap
        {
            private readonly PuffinElement _element;
            private readonly Dictionary<string, IPriceField> _priceFields = new Dictionary<string, IPriceField>();

            public LevelOnePriceMap(PuffinElement element)
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

            public double? DoubleField(string name)
            {
                var priceField = Field(name);
                if (priceField == null) return null;
                return priceField.Value as double? ?? ParseDouble(priceField);
            }

            public long? LongField(string name)
            {
                var priceField = Field(name);
                if (priceField == null) return null;
                return priceField.Value as long? ?? ParseLong(priceField);
            }

            public int? IntField(string name)
            {
                var priceField = Field(name);
                if (priceField == null) return null;
                return priceField.Value as int? ?? ParseInt(priceField);
            }

            public string StringField(string name)
            {
                var priceField = Field(name);
                return priceField == null ? null : priceField.Text;
            }

            private static double? ParseDouble(IPriceField priceField)
            {
                try
                {
                    return double.Parse(priceField.Text);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            private static long? ParseLong(IPriceField priceField)
            {
                try
                {
                    return long.Parse(priceField.Text);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            private static int? ParseInt(IPriceField priceField)
            {
                try
                {
                    return int.Parse(priceField.Text);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public override string ToString()
            {
                return _element.ToString();
            }
        }

        public static PriceStatus ToStatus(int statusCode)
        {
            switch (statusCode)
            {
                case 0:
                    return PriceStatus.OK; // OK
                case 1:
                    return PriceStatus.PENDING; // PENDING
                case 2:
                    return PriceStatus.TIMEOUT; // TIMEOUT
                case 3:
                    return PriceStatus.STALE; // LINE_DOWN
                case 4:
                    return PriceStatus.CLOSED; // CLIENT_SYNTAX
                case 5:
                    return PriceStatus.CLOSED; // SERVER_SYNTAX
                case 6:
                    return PriceStatus.CLOSED; // CLIENT_VERSION
                case 7:
                    return PriceStatus.CLOSED; // SERVER_VERSION
                case 8:
                    return PriceStatus.CLOSED; // SUBJECT_MALFORMED
                case 9:
                    return PriceStatus.UNAVAILABLE; // SUBJECT_UNKNOWN
                case 10:
                    return PriceStatus.CLOSED; // SUBJECT_UNSUPPORTED
                case 11:
                    return PriceStatus.UNAVAILABLE; // SOURCE_UNAVAILABLE
                case 12:
                    return PriceStatus.PROHIBITED; // ACCESS_DENIED
                case 13:
                    return PriceStatus.STALE; // PRICE_STALE
                case 14:
                    return PriceStatus.UNAVAILABLE; // PRICE_UNAVAILABLE
                case 15:
                    return PriceStatus.CLOSED; // PRICE_DELETED
                case 16:
                    return PriceStatus.CLOSED; // RESOURCE_LOOKUP
                case 17:
                    return PriceStatus.REJECTED; // REMOTE_FEED
                case 18:
                    return PriceStatus.EXHAUSTED; // SUBSCRIPTIONS_LIMIT
            }
            return PriceStatus.UNAVAILABLE;
        }
    }
}