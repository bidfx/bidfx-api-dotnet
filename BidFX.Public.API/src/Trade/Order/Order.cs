using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BidFX.Public.API.Trade.Rest.Json;

namespace BidFX.Public.API.Trade.Order
{
    public class Order : EventArgs, IJsonMarshallable
    {
        internal const string AssetClass = "asset_class";
        internal const string Account = "account";
        internal const string Currency = "dealt_ccy";
        internal const string Side = "side";
        internal const string Quantity = "quantity";
        internal const string HandlingType = "handling_type";
        internal const string Reference1 = "reference1";
        internal const string Reference2 = "reference2";
        internal const string Price = "price";
        internal const string OrderType = "order_type";
        internal const string AlternateOwner = "alternate_owner";
        internal const string MessageId = "correlation_id";
        internal const string CreationDate = "creation_date";
        internal const string DeactivationDate = "deactivation_date";
        internal const string DoneForDay = "done_for_day";
        internal const string ErrantQuantity = "errant_quantity";
        internal const string ExecutedQuantity = "executed_quantity";
        internal const string ExecutingBroker = "executing_broker";
        internal const string FullyExecuted = "fully_executed";
        internal const string LeavesQuantity = "leaves_quantity";
        internal const string OrderTsId = "order_ts_id";
        internal const string OutstandingQuantity = "outstanding_quantity";
        internal const string Owner = "owner";
        internal const string ReleasedQuantity = "released_quantity";
        internal const string SettlementCurrency = "settlement_currency";
        internal const string SettlementDate = "settlement_date";
        internal const string State = "state";
        internal const string StrategyState = "strategy_state";
        internal const string TimeInForceType = "time_in_force_type";
        internal const string UnexecutedQuantity = "unexecuted_quantity";
        internal const string UUID = "uuid";
        internal const string Errors = "errors";

        private readonly SortedDictionary<string, object> _jsonMap;

        public Order(IDictionary<string, object> jsonMap)
        {
            _jsonMap = new SortedDictionary<string, object>(jsonMap);
        }

        public IDictionary<string, object> GetJsonMap()
        {
            return _jsonMap;
        }

        public string GetAssetClass()
        {
            return GetComponent<string>(AssetClass);
        }

        public string GetAccount()
        {
            return GetComponent<string>(Account);
        }

        public string GetSide()
        {
            return GetComponent<string>(Side);
        }

        public string GetHandlingType()
        {
            return GetComponent<string>(HandlingType);
        }

        public string GetReference1()
        {
            return GetComponent<string>(Reference1);
        }

        public string GetReference2()
        {
            return GetComponent<string>(Reference2);
        }

        public decimal? GetPrice()
        {
            return GetComponent<decimal?>(Price);
        }

        public string GetOrderType()
        {
            return GetComponent<string>(OrderType);
        }

        public decimal? GetQuantity()
        {
            return GetComponent<decimal?>(Quantity);
        }

        public string GetAlternateOwner()
        {
            return GetComponent<string>(AlternateOwner);
        }

        public string GetMessageId()
        {
            return GetComponent<string>(MessageId);
        }

        public string GetCreationDate()
        {
            return GetComponent<string>(CreationDate);
        }

        public string GetDeactivationDate()
        {
            return GetComponent<string>(DeactivationDate);
        }

        public bool? GetDoneForDay()
        {
            return GetComponent<bool?>(DoneForDay);
        }

        public decimal? GetErrantQuantity()
        {
            return GetComponent<decimal?>(ErrantQuantity);
        }

        public decimal? GetExecutedQuantity()
        {
            return GetComponent<decimal?>(ExecutedQuantity);
        }

        public string GetExecutingBroker()
        {
            return GetComponent<string>(ExecutingBroker);
        }

        public bool? GetFullyExecuted()
        {
            return GetComponent<bool?>(FullyExecuted);
        }

        public decimal? GetLeavesQuantity()
        {
            return GetComponent<decimal?>(LeavesQuantity);
        }

        public string GetOrderTsId()
        {
            return GetComponent<string>(OrderTsId);
        }

        public decimal? GetOutstandingQuantity()
        {
            return GetComponent<decimal?>(OutstandingQuantity);
        }

        public string GetOwner()
        {
            return GetComponent<string>(Owner);
        }

        public decimal? GetReleasedQuantity()
        {
            return GetComponent<decimal?>(ReleasedQuantity);
        }

        public string GetSettlementCurrency()
        {
            return GetComponent<string>(SettlementCurrency);
        }

        public string GetSettlementDate()
        {
            return GetComponent<string>(SettlementDate);
        }

        public string GetState()
        {
            return GetComponent<string>(State);
        }

        public string GetStrategyState()
        {
            return GetComponent<string>(StrategyState);
        }

        public string GetTimeInForceType()
        {
            return GetComponent<string>(TimeInForceType);
        }

        public decimal? GetUnexecutedQuantity()
        {
            return GetComponent<decimal?>(UnexecutedQuantity);
        }

        public decimal? GetUUID()
        {
            return GetComponent<decimal?>(UUID);
        }

        public List<Error> GetErrors()
        {
            return GetComponent<List<Error>>(Errors);
        }

        protected T GetComponent<T>(string key)
        {
            object value;
            return (T) (_jsonMap.TryGetValue(key, out value) ? value : null);
        }

        public override string ToString()
        {
            return DeepStringDictionary(_jsonMap);
        }

        private static string DeepStringDictionary(IDictionary<string, object> dictionary)
        {
            return "[" + string.Join(", ", dictionary.Select(kv => kv.Key + "=" + DeepString(kv.Value))) + "]";
        }

        private static string DeepStringEnumerable(IEnumerable<object> collection)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("[");
            string delim = "";
            foreach (object o in collection)
            {
                stringBuilder.Append(delim);
                stringBuilder.Append(DeepString(o));
                delim = ", ";
            }

            return stringBuilder.ToString();
        }

        private static string DeepString(object o)
        {
            if (o is IDictionary<string, object>)
            {
                return DeepStringDictionary((IDictionary<string, object>) o);
            }
            else if (o is IEnumerable<object>)
            {
                return DeepStringEnumerable((IEnumerable<object>) o);
            }
            else
            {
                return o == null ? "null" : o.ToString();
            }
        }
    }

}