using BidFX.Public.API.Trade.REST;

namespace BidFX.Public.API.Trade
{
    public class OrderSubmitCallback : IRESTCallback<OrderSubmitResponse>
    {
        public void Notfiy()
        {
            //TODO
        }

        public bool HasResponse()
        {
            //TODO
            return false;
        }

        public OrderSubmitResponse GetResponse()
        {
            //TODO
            return null;
        }
        
        public OrderSubmitResponse GetResponse(long timeout)
        {
            //TODO
            return null;
        }
    }
}