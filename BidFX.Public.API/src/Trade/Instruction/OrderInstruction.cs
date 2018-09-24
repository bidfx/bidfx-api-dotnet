using System.Collections.Generic;
using BidFX.Public.API.Trade.Rest.Json;

namespace BidFX.Public.API.Trade.Instruction
{
    public class OrderInstruction : IJsonMarshallable
    {
        internal const string OrderTsId = "order_ts_id";
        internal const string Reason = "reason";
        
        private readonly Dictionary<string, object> _jsonMap;
        
        public OrderInstruction(Dictionary<string, object> jsonMap)
        {
            _jsonMap = jsonMap;
        }

        public IDictionary<string, object> GetJsonMap()
        {
            return _jsonMap;
        }

        public string GetOrderTsId()
        {
            return GetComponent<string>(OrderTsId);
        }

        public string GetReason()
        {
            return GetComponent<string>(Reason);
        }
        
        protected T GetComponent<T>(string key)
        {
            object value;
            return (T) (_jsonMap.TryGetValue(key, out value) ? value : null);
        }
    }
}