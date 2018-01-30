using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using BidFX.Public.API.Trade.Order;
using BidFX.Public.API.Trade.REST;

namespace BidFX.Public.API.Trade
{
    public class TradeManager
    {
        public event EventHandler<OrderResponse> OrderQueryEventHandler;
        public event EventHandler<OrderResponse> OrderSubmitEventHandler;
        private readonly RESTClient _restClient;
        private static readonly Random Random = new Random();

        public TradeManager(string username, string password)
        {
            _restClient = new RESTClient("http://uatdev.tradingscreen.com:443", username, password); //TODO; Set REST endpoint variably.
        }
        
        public long SubmitOrder(FxOrder fxOrder)
        {
            var clientId = GenerateMessageId();
            var json = JsonMarshaller.ToJSON(fxOrder);
            ThreadPool.QueueUserWorkItem(
                delegate
                {
                    SendOrderViaREST(clientId, json);
                }
            );
            return 0L;
        }

        public long SubmitQuery(string orderId)
        {
            var messageId = GenerateMessageId();
            ThreadPool.QueueUserWorkItem(
                delegate
                {
                    SendQueryViaREST(messageId, orderId);
                }
            );
            return 0L;
        }

        private void SendOrderViaREST(long messageId, string json)
        {
            var response = _restClient.SendJSON("POST", "/api/v1/fx/submit", json);
            var orderResponse = new OrderResponse(response) {ClientId = messageId};
//            _clientIdToOrderId[ClientID] = orderSubmitResponse.GetOrderId();
            if (OrderSubmitEventHandler != null)
            {
                OrderSubmitEventHandler(this, orderResponse);
            }
        }

        private void SendQueryViaREST(long messageId, string orderId)
        {
            var response = _restClient.SendMessage("GET", "/api/v1/fx/query?order_ts_id=" + orderId);
            var orderResponse = new OrderResponse(response) {ClientId = messageId};
            if (OrderQueryEventHandler != null)
            {
                OrderQueryEventHandler(this, orderResponse);
            }
        }

        private static long GenerateMessageId()
        {
            long nextLong;
            lock (Random)
            {
                nextLong = Random.Next(int.MinValue, int.MaxValue) << 32 | Random.Next(int.MinValue, int.MaxValue);
            }
            return nextLong;
        }
    }
}