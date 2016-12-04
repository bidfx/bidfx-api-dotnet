using TS.Pisa.Tools;

namespace TS.Pisa.Plugin.Puffin
{
    internal class PriceAdaptor
    {
        public static IPriceMap ToPriceMap(PuffinElement element)
        {
            if (element == null) return null;
            var priceMap = new PriceMap();
            foreach (var attribute in element.Attributes)
            {
                AddField(priceMap, attribute.Key, attribute.Value);
            }
            return priceMap;
        }

        private static void AddField(PriceMap priceMap, string name, IPriceField value)
        {
            if (value.Value is long && IsTimeField(name))
            {
                value = DateTimeField((long)value.Value);
            }
            priceMap.SetField(name, value);
        }

        private static IPriceField DateTimeField(long javaTime)
        {
            var dateTime = JavaTime.ToDateTime(javaTime);
            var isoDate = JavaTime.IsoDateFormat(dateTime);
            return new PriceField(isoDate, dateTime);
        }

        private static bool IsTimeField(string name)
        {
            return name.EndsWith("Time") || name.Equals(FieldName.TimeOfUpdate);
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

    internal class PriceField : IPriceField
    {
        public string Text { get; private set; }
        public object Value { get; private set; }

        public PriceField(string text, object value)
        {
            Text = text;
            Value = value;
        }
    }
}