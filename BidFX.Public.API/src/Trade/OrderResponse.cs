using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using BidFX.Public.API.Trade.REST;
using log4net;
using Newtonsoft.Json;

namespace BidFX.Public.API.Trade
{
    public class OrderResponse : AbstractRESTResponse
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public long MessageId { get; internal set; }

        public OrderResponse(HttpWebResponse webResponse) : base(webResponse)
        {
        }

        /**
         * For testing
         */
        internal OrderResponse(string json, HttpStatusCode statusCode = HttpStatusCode.OK) : base(json,statusCode)
        {
        }

        /// <summary>
        /// Get the unique TSId assigned to the order. This can be used to query the state of an order.
        /// </summary>
        /// <returns>The TSId assigned to the order, or null if no TSId was assigned.</returns>
        public string GetOrderId()
        {
            return GetField("order_ts_id");
        }

        public override string ToString()
        {
            return "MessageId => " + MessageId + ", Body => " + base.ToString();
        }

        /// <summary>
        /// Get the last known state of the order.
        /// </summary>
        /// <returns>The state assigned to the order, or null if no state was assigned.</returns>
        public string GetState()
        {
            return GetField("state");
        }

        /// <summary>
        /// Get the errors attached to the order, if there is one.
        /// </summary>
        /// <returns>A list of errors given to the order. The list is empty if there were no errors.</returns>
        public List<string> GetErrors()
        {
            string errors = GetField("errors");
            if (errors == null)
            {
                string type = GetField("type");
                string message = GetField("message");
                if (type == null)
                {
                    return new List<string>();
                }

                Dictionary<string, string> error = new Dictionary<string, string>
                    {
                        {"type", type}, 
                        {"message", message}
                    };

                return new List<string>
                {
                    ConvertErrorDictionaryToString(error)
                };
            }
            
            try
            {
                List<Dictionary<string, string>> list = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(errors);
                return list.ConvertAll(ConvertErrorDictionaryToString);
            }
            catch (JsonSerializationException e)
            {
                Log.Error("Could not deserialize errors, returning as string", e);
                return new List<string> {errors};
            }
        }

        private string ConvertErrorDictionaryToString(Dictionary<string, string> error)
        {
            if (error.ContainsKey("type"))
            {
                return error["type"] + " - " + error["message"];
            }

            if (error.ContainsKey("message"))
            {
                return error["message"];
            }

            return string.Join(", ", error.Values.ToList());
        }
    }
}