using System.Collections.Generic;

namespace BidFX.Public.API.Trade.Order
{
    public class Execution
    {
        private const string Eti = "eti";
        private const string ExecutedQuantity = "executed_quantity";
        private const string ExecutedValue = "executed_value";
        private const string OrderTsId = "order_ts_id";
        private const string Price = "price";
        private const string Quantity = "quantity";
        private const string Side = "side";
        private const string State = "state";
        private const string TsId = "ts_id";

        private readonly SortedDictionary<string, object> _jsonMap;

        public Execution(IDictionary<string, object> jsonMap)
        {
            if (jsonMap == null)
            {
                _jsonMap = new SortedDictionary<string, object>();
            }
            else
            {
                _jsonMap = new SortedDictionary<string, object>(jsonMap);
            }
        }
        
        public string GetEti()
        {
            return GetComponent<string>(Eti);
        }

        public decimal? GetExecutedQuantity()
        {
            return GetComponent<decimal?>(ExecutedQuantity);
        }

        public decimal? GetExecutedValue()
        {
            return GetComponent<decimal?>(ExecutedValue);
        }

        public string GetOrderTsId()
        {
            return GetComponent<string>(OrderTsId);
        }

        public decimal? GetPrice()
        {
            return GetComponent<decimal?>(Price);
        }

        public decimal? GetQuantity()
        {
            return GetComponent<decimal?>(Quantity);
        }

        public string GetSide()
        {
            return GetComponent<string>(Side);
        }

        public object GetState()
        {
            return GetComponent<string>(State);
        }

        public string GetTsId()
        {
            return GetComponent<string>(TsId);
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
    }
}