using System.Collections.Generic;
using System.Net;
using BidFX.Public.API.Trade.REST;
using Newtonsoft.Json;

namespace BidFX.Public.API.Trade
{
    public class OrderResponse : AbstractRESTResponse
    {
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
        /// <returns>The error given to the order, or null if there was no error.</returns>
        public List<string> GetErrors()
        {
            string errors = GetField("errors");
            List<Dictionary<string, string>> list = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(errors);
            return list.ConvertAll(error => error["message"]);
        }
    }
}