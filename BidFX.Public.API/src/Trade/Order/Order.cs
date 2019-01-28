using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BidFX.Public.API.Trade.Rest.Json;

namespace BidFX.Public.API.Trade.Order
{
    public class Order : EventArgs, IJsonMarshallable
    {
        internal const string Account = "account";
        internal const string AllocationTemplate = "allocation_template";
        internal const string Allocations = "allocations";
        internal const string AlternateOwner = "alternate_owner";
        internal const string AssetClass = "asset_class";
        internal const string Currency = "dealt_ccy";
        internal const string Side = "side";
        internal const string Quantity = "quantity";
        internal const string HandlingType = "handling_type";
        internal const string Reference1 = "reference1";
        internal const string Reference2 = "reference2";
        internal const string Price = "price";
        internal const string OrderInstruction = "order_instruction";
        internal const string OrderType = "order_type";
        internal const string SettlementDate = "settlement_date";
        private const string MessageId = "correlation_id";
        //Read only properties
        internal const string Errors = "errors";
        private const string Executions = "executions";
        private const string OrderTsId = "order_ts_id";
        private const string Owner = "owner";
        private const string State = "state";
        private const string UUID = "uuid";

        private readonly SortedDictionary<string, object> _jsonMap;

        public Order(IDictionary<string, object> jsonMap)
        {
            ProcessErrors(jsonMap);
            ProcessExecutions(jsonMap);
            
            _jsonMap = new SortedDictionary<string, object>(jsonMap);
        }

        private static void ProcessErrors(IDictionary<string, object> jsonMap)
        {
            object errorsObject;
            jsonMap.TryGetValue(Errors, out errorsObject);
            List<object> errors = errorsObject as List<object>;
            if (errors == null)
            {
                return;
            }
            List<Error> errorsList = errors.ConvertAll(errorMap => new Error(errorMap as Dictionary<string, object>));
            jsonMap[Errors] = errorsList;
        }

        private static void ProcessExecutions(IDictionary<string, object> jsonMap)
        {
            object executionsObject;
            jsonMap.TryGetValue(Executions, out executionsObject);
            List<object> executions = executionsObject as List<object>;
            if (executions == null)
            {
                return;
            }

            List<Execution> executionsList = executions.ConvertAll(execution => new Execution(execution as Dictionary<string, object> ));
            jsonMap[Executions] = executionsList;
        }

        public IDictionary<string, object> GetJsonMap()
        {
            return _jsonMap;
        }

        public string GetAccount()
        {
            return GetComponent<string>(Account);
        }

        public string GetAllocationTemplate()
        {
            return GetComponent<string>(AllocationTemplate);
        }

        public List<AllocationTemplateEntry> GetAllocations()
        {
            return GetComponent<List<AllocationTemplateEntry>>(Allocations);
        }

        public string GetAssetClass()
        {
            return GetComponent<string>(AssetClass);
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

        public List<Error> GetErrors()
        {
            return GetComponent<List<Error>>(Errors);
        }

        public List<Execution> GetExecutions()
        {
            return GetComponent<List<Execution>>(Executions);
        }
        
        public long? GetMessageId()
        {
            long id;
            if (long.TryParse(GetComponent<string>(MessageId), out id))
            {
                return id;
            }
            return null;

        }

        public string GetOrderTsId()
        {
            return GetComponent<string>(OrderTsId);
        }

        public string GetOwner()
        {
            return GetComponent<string>(Owner);
        }

        public string GetSettlementDate()
        {
            return GetComponent<string>(SettlementDate);
        }

        public string GetState()
        {
            return GetComponent<string>(State);
        }

        public decimal? GetUUID()
        {
            return GetComponent<decimal?>(UUID);
        }


        protected T GetComponent<T>(string key)
        {
            object value;
            return (T) (_jsonMap.TryGetValue(key, out value) ? value : null);
        }

        internal void SetMessageId(long messageId)
        {
            _jsonMap[MessageId] = messageId.ToString();
        }
        
        public override string ToString()
        {
            return DeepStringDictionary(_jsonMap);
        }

        internal static string DeepStringDictionary(IDictionary<string, object> dictionary)
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