using System;
using System.Net;
using System.Threading;
using BidFX.Public.API.Trade.Order;
using BidFX.Public.API.Trade.REST;

namespace BidFX.Public.API.Trade
{
    public class TradeManager
    {
        public event EventHandler<OrderQueryResponse> OrderQueryEventHandler;
        public event EventHandler<OrderSubmitResponse> OrderSubmitEventHandler;
        private RESTClient _restClient;
        private Random _random = new Random();

        public TradeManager()
        {
            _restClient = new RESTClient();
        }
        
        public long SubmitOrder(FxOrder fxOrder)
        {
            var clientID = (long) _random.Next() << 32 | _random.Next();
            var json = JsonMarshaller.ToJSON(fxOrder);
            ThreadPool.QueueUserWorkItem(
                delegate
                {
                    SendOrderViaREST(clientID, "POST", "/api/v1/fx/submit", json);
                }, null
            );
            return 0L;
        }

        public long SubmitQuery(string OrderID)
        {
            //TODO
            return 0L;
        }

        public long SubmitQuery(long ClientID)
        {
            var returnID = new Random();
            //TODO
            return 0L;
        }

        private void SendOrderViaREST(long ClientID, string method, string path, string json)
        {
            var response = _restClient.sendJSON(method, path, json);
            var orderSubmitResponse = new OrderSubmitResponse {ClientId = ClientID};
            //TODO populate orderresponse with response;
            this.OrderSubmitEventHandler(this, orderSubmitResponse);
        }
        
    }
}