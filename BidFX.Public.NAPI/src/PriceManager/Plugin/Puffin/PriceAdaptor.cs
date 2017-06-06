using System;
using BidFX.Public.NAPI.PriceManager.Tools;

namespace BidFX.Public.NAPI.PriceManager.Plugin.Puffin
{
    internal class PriceAdaptor
    {
        public static IPriceMap ToPriceMap(PuffinElement element)
        {
            if (element == null) return null;
            var priceMap = new PriceMap();
            foreach (var attribute in element.Attributes)
            {
                priceMap.SetField(attribute.Key, AdaptPriceField(attribute.Key, attribute.Value));
            }
            return priceMap;
        }

        internal static IPriceField AdaptPriceField(string name, IPriceField field)
        {
            if (IsTimeField(name))
            {
                if (field.Value is long)
                {
                    field = JavaDateTimeField((long) field.Value);
                }
                else if (field.Value is string)
                {
                    field = GuessDateTimeField((string) field.Value, field);
                }
            }
            else if (IsDateField(name))
            {
                if (field.Value is int)
                {
                    field = IntDateTimeField((int) field.Value);
                }
                else if (field.Value is string)
                {
                    field = GuessDateTimeField((string) field.Value, field);
                }
            }
            else if (field.Value is string && IsTickField(name))
            {
                field = AdaptTickField((string) field.Value);
            }
            return field;
        }

        private static IPriceField AdaptTickField(string value)
        {
            switch (value)
            {
                case "^":
                case "Up":
                case "WasUp":
                    return new PriceField("^", Tick.Up);
                case "v":
                case "Down":
                case "WasDown":
                    return new PriceField("v", Tick.Down);
                default:
                    return new PriceField("=", Tick.Flat);
            }
        }

        private static bool IsTickField(string name)
        {
            return name.EndsWith("Tick");
        }

        private static bool IsDateField(string name)
        {
            return name.EndsWith("Date");
        }

        private static bool IsTimeField(string name)
        {
            return name.EndsWith("Time") || name.Equals(FieldName.TimeOfUpdate);
        }

        private static IPriceField JavaDateTimeField(long javaTime)
        {
            var dateTime = JavaTime.ToDateTime(javaTime);
            var isoDate = JavaTime.IsoDateFormat(dateTime);
            return new PriceField(isoDate, dateTime);
        }

        private static IPriceField IntDateTimeField(int yyyymmdd)
        {
            var year = yyyymmdd / 10000;
            var month = yyyymmdd / 100 % 100;
            var day = yyyymmdd % 100;
            var dateTime = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
            return new PriceField(dateTime.ToString("yyyy-MM-dd"), dateTime);
        }

        private static IPriceField GuessDateTimeField(string value, IPriceField field)
        {
            DateTime dateTime;
            if (DateTime.TryParse(value, out dateTime))
            {
                if (IsTimeOnly(value))
                {
                    if (dateTime.CompareTo(DateTime.UtcNow) >= 0)
                    {
                        dateTime = dateTime.Subtract(TimeSpan.FromDays(1));
                    }
                }
                var isoDate = JavaTime.IsoDateFormat(dateTime);
                return new PriceField(isoDate, dateTime);
            }
            return field;
        }

        private static bool IsTimeOnly(string s)
        {
            return s.Length > 4 && s[2] == ':';
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
                default:
                    return SubscriptionStatus.UNAVAILABLE;
            }
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