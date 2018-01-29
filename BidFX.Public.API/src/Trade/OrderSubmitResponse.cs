using BidFX.Public.API.Trade.REST;

namespace BidFX.Public.API.Trade
{
    public class OrderSubmitResponse : AbstractRESTResponse
    {
        public long ClientId { get; set; }
    }
}