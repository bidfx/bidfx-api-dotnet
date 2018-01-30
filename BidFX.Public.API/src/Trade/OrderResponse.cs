using System.Net;
using BidFX.Public.API.Trade.REST;

namespace BidFX.Public.API.Trade
{
    public class OrderResponse : AbstractRESTResponse
    {
        public long ClientId { get; set; }

        public OrderResponse(HttpWebResponse webResponse) : base(webResponse)
        {
        }
        
        public string GetOrderId()
        {
            return GetField("order_ts_id");
        }
    }
}