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

        public static SubscriptionStatus ToStatus(int statusCode)
        {
            switch (statusCode)
            {
                case 0:
                    return SubscriptionStatus.OK; // OK
                case 1:
                    return SubscriptionStatus.PENDING; // PENDING
                case 2:
                    return SubscriptionStatus.TIMEOUT; // TIMEOUT
                case 3:
                    return SubscriptionStatus.STALE; // LINE_DOWN
                case 4:
                    return SubscriptionStatus.CLOSED; // CLIENT_SYNTAX
                case 5:
                    return SubscriptionStatus.CLOSED; // SERVER_SYNTAX
                case 6:
                    return SubscriptionStatus.CLOSED; // CLIENT_VERSION
                case 7:
                    return SubscriptionStatus.CLOSED; // SERVER_VERSION
                case 8:
                    return SubscriptionStatus.CLOSED; // SUBJECT_MALFORMED
                case 9:
                    return SubscriptionStatus.UNAVAILABLE; // SUBJECT_UNKNOWN
                case 10:
                    return SubscriptionStatus.CLOSED; // SUBJECT_UNSUPPORTED
                case 11:
                    return SubscriptionStatus.UNAVAILABLE; // SOURCE_UNAVAILABLE
                case 12:
                    return SubscriptionStatus.PROHIBITED; // ACCESS_DENIED
                case 13:
                    return SubscriptionStatus.STALE; // PRICE_STALE
                case 14:
                    return SubscriptionStatus.UNAVAILABLE; // PRICE_UNAVAILABLE
                case 15:
                    return SubscriptionStatus.CLOSED; // PRICE_DELETED
                case 16:
                    return SubscriptionStatus.CLOSED; // RESOURCE_LOOKUP
                case 17:
                    return SubscriptionStatus.REJECTED; // REMOTE_FEED
                case 18:
                    return SubscriptionStatus.EXHAUSTED; // SUBSCRIPTIONS_LIMIT
            }
            return SubscriptionStatus.UNAVAILABLE;
        }
    }
}