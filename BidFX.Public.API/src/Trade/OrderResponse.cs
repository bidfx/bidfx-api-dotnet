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
            if (_statusCode == HttpStatusCode.OK)
            {
                return _statusCode.ToString();
            }

            return null;
        }

        public override string ToString()
        {
            return "MessageId => " + MessageId + " " + base.ToString();
        }
    }
}