using System;
using System.Collections;
using System.ComponentModel;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using BidFX.Public.API.Trade.Order;
using BidFX.Public.API.Trade.REST;
using log4net;

namespace BidFX.Public.API.Trade
{
    public class TradeManager
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public event EventHandler<OrderResponse> OrderQueryEventHandler;
        public event EventHandler<OrderResponse> OrderSubmitEventHandler;
        private RESTClient _restClient;
        private static long _nextMessageId;
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        
        public void Start()
        {
            var address = "https://" + Host + ":" + Port;
            _restClient = new RESTClient(address, Username, Password);
            initialiseNextMessageId();
            Log.InfoFormat("TradeManager connecting to {0}", address);
        }

        private void initialiseNextMessageId()
        {
            long hashCode = Environment.TickCount.GetHashCode();
            long mask = (1L << 16) - 1;
            _nextMessageId = Math.Abs((hashCode & mask) << 32);
        }
        
        public long SubmitOrder(FxOrder fxOrder)
        {
            var messageId = GetNextMessageId();
            var json = JsonMarshaller.ToJSON(fxOrder);
            ThreadPool.QueueUserWorkItem(
                delegate
                {
                    SendOrderViaREST(messageId, json);
                }
            );
            return messageId;
        }

        public long SubmitQuery(string orderId)
        {
            var messageId = GetNextMessageId();
            ThreadPool.QueueUserWorkItem(
                delegate
                {
                    SendQueryViaREST(messageId, orderId);
                }
            );
            return messageId;
        }

        private void SendOrderViaREST(long messageId, string json)
        {
            Log.InfoFormat("Submmiting order, messageId {0}", messageId);
            Log.DebugFormat("Submitting order, messageId: {0}, Json: {1}", messageId, json);
            var response = _restClient.SendJSON("POST", "", json);
            var orderResponse = new OrderResponse(response) {MessageId = messageId};
//            _clientIdToOrderId[ClientID] = orderSubmitResponse.GetOrderId();
            if (OrderSubmitEventHandler != null)
            {
                OrderSubmitEventHandler(this, orderResponse);
            }
        }

        private void SendQueryViaREST(long messageId, string orderId)
        {
            Log.DebugFormat("Querying orderId {0}, messageId {1}", orderId, messageId);
            var response = _restClient.SendMessage("GET", "?order_ts_id=" + orderId);
            var orderResponse = new OrderResponse(response) {MessageId = messageId};
            if (OrderQueryEventHandler != null)
            {
                OrderQueryEventHandler(this, orderResponse);
            }
        }

        private static long GetNextMessageId()
        {
            return Interlocked.Increment(ref _nextMessageId);
        }
    }
}