using System;
using System.Collections.Generic;

namespace BidFX.Public.API.Trade.Order
{
    public class Error
    {
        internal const string Field = "field";
        internal const string Message = "message";
        internal const string Value = "value";

        private readonly IDictionary<string, object> _jsonMap;

        public Error(IDictionary<string, object> jsonMap)
        {
            _jsonMap = new SortedDictionary<string, object>(jsonMap);
        }

        internal Error(string field, string message, object value)
        {
            _jsonMap = new SortedDictionary<string, object>();
            _jsonMap[Field] = field;
            _jsonMap[Message] = message;
            _jsonMap[Value] = value;
        }

        public string GetField()
        {
            return GetComponent<string>(Field);
        }

        public string GetMessage()
        {
            return GetComponent<string>(Message);
        }

        public object GetValue()
        {
            return GetComponent<object>(Value);
        }

        private T GetComponent<T>(string key)
        {
            object value;
            return (T) (_jsonMap.TryGetValue(key, out value) ? value : null);
        }

        public override string ToString()
        {
            return Order.DeepStringDictionary(_jsonMap);
        }

        internal static Error FromJson(object jsonObject)
        {
            if (jsonObject is Dictionary<string, object>)
            {
                return new Error((Dictionary<string, object>) jsonObject);
            }
            
            throw new ArgumentException("jsonObject not a dictionary");
        }
    }
}