using System.Net;
using BidFX.Public.API.Trade.REST;

namespace BidFX.Public.API.Trade
{
    public class OrderResponse : AbstractRESTResponse
    {
        public long MessageId { get; set; }

        public OrderResponse(HttpWebResponse webResponse) : base(webResponse)
        {
        }

        public string GetOrderId()
        {
            return StatusCode == HttpStatusCode.OK && GetSize() > 0 ? GetField("order_ts_id") : "";
        }

        public override string ToString()
        {
            return "MessageId => " + MessageId + ", Body => " + base.ToString();
        }

        public object GetState()
        {
            return StatusCode == HttpStatusCode.OK ? GetField("state") : "";
        }
    }
}