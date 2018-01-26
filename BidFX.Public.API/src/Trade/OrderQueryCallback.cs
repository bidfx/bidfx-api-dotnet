using BidFX.Public.API.Trade.REST;

namespace BidFX.Public.API.Trade
{
    public class OrderQueryCallback : IRESTCallback<OrderQueryResponse>
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

        public OrderQueryResponse GetResponse()
        {
            //TODO
            return null;
        }

        public OrderQueryResponse GetResponse(long timeout)
        {
            //TODO
            return null;
        }
    }
}