using System.Collections.Generic;

namespace BidFX.Public.API.Trade.Order
{
    public class AllocationTemplateEntry
    {
        internal const string AllocRatio = "alloc_ratio";
        internal const string ClearingAccount = "clearing_account";
        internal const string ClearingBroker = "clearing_broker";
        internal const string Quantity = "quantity";
        internal const string FarQuantity = "far_quantity";

        private readonly SortedDictionary<string, object> _jsonMap;
        
        public decimal? GetRatio()
        {
            return GetComponent<decimal?>(AllocRatio);
        }

        public string GetClearingAccount()
        {
            return GetComponent<string>(ClearingAccount);
        }

        public string GetClearingBroker()
        {
            return GetComponent<string>(ClearingBroker);
        }

        public decimal? GetQuantity()
        {
            return GetComponent<decimal?>(Quantity);
        }

        public decimal? GetFarQuantity()
        {
            return GetComponent<decimal?>(FarQuantity);
        }

        public AllocationTemplateEntry(IDictionary<string, object> jsonMap)
        {
            _jsonMap = new SortedDictionary<string, object>(jsonMap);
        }

        public IDictionary<string, object> GetJsonMap()
        {
            return _jsonMap;
        }
        
        private T GetComponent<T>(string key)
        {
            object value;
            return (T) (_jsonMap.TryGetValue(key, out value) ? value : null);
        }
    }
}