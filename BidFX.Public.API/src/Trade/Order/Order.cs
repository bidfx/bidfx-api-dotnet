using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BidFX.Public.API.Trade.Rest.Json;
using log4net;

namespace BidFX.Public.API.Trade.Order
{
    public class Order : EventArgs, IJsonMarshallable
    {
        private static readonly ILog Log = LogManager.GetLogger("Order");
        
        internal const string Account = "account";
        internal const string Algo = "algo";
        internal const string AllocationTemplate = "allocation_template";
        internal const string AllocationData = "allocation_data";
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

        internal Order(IDictionary<string, object> jsonMap)
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

        public Algo GetAlgo()
        {
            try
            {
                IDictionary<string, object> algoJson = GetComponent<IDictionary<string, object>>(Algo);
                if (algoJson == null) return null;
                string name = (string) algoJson[Trade.Order.Algo.Name];
                IDictionary<string, object> parameters = (IDictionary<string, object>) algoJson[Trade.Order.Algo.Parameters];
                return new Algo(name, parameters);
            }
            catch (InvalidCastException e)
            {
                return GetComponent<Algo>(Algo);
            }
        }
        
        public string GetAllocationTemplate()
        {
            return GetComponent<string>(AllocationTemplate);
        }

        public Allocation GetAllocation()
        {
            IDictionary<string, object> allocationJson = GetComponent<IDictionary<string, object>>(AllocationData);
            return allocationJson == null ? null : new Allocation(allocationJson);
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
            return "{" + string.Join(", ", dictionary.Select(kv => kv.Key + "=" + DeepString(kv.Value))) + "}";
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

            stringBuilder.Append("]");

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
        
        internal static Order FromJson(object jsonObject)
        {
            if (jsonObject is List<object>)
            {
                if (((List<object>) jsonObject).Count == 0)
                {
                    Error error = new Error(null, "Empty list returned from server", null);
                    return new Order(new Dictionary<string, object> {{Errors, new List<Error>{error}}});
                }
                object firstItem = ((List<object>) jsonObject)[0];
                if (!(firstItem is Dictionary<string, object>))
                {
                    throw new ArgumentException("First item was not a map");
                }

                object assetClass = ((Dictionary<string, object>) firstItem)[AssetClass];
                if (!(assetClass is string))
                {
                    throw new ArgumentException("Could not read Asset Class of Result");
                }

                switch ((string) assetClass)
                {
                    case "FX":
                        return new FxOrder((Dictionary<string, object>) firstItem);
                    case "FUTURE":
                        return new FutureOrder((Dictionary<string, object>) firstItem);
                    default:
                        Log.WarnFormat("Unknown assetclass {0}. Creating default order.");
                        return new Order((Dictionary<string, object>) firstItem);
                }
            }
            else if (jsonObject is Dictionary<string, object>)
            {
                object message;
                if (!((Dictionary<string, object>) jsonObject).TryGetValue("message", out message))
                {
                    throw new ArgumentException("Could not get an error message");
                }
                
                Error error = new Error(null, message.ToString(), null);
                return new Order(new Dictionary<string, object> {{Errors, new List<Error>{error}}});
            }
            else
            {
                throw new ArgumentException("Could not read JSON");
            }
        }
    }

}