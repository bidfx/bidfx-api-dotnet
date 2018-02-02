using System;
using System.Net;
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
            string address = "https://" + Host + ":" + Port;
            _restClient = new RESTClient(address, Username, Password);
            InitialiseNextMessageId();
            Log.InfoFormat("TradeManager connecting to {0}", address);
        }

        public long SubmitOrder(FxOrder fxOrder)
        {
            long messageId = GetNextMessageId();
            string json = JsonMarshaller.ToJSON(fxOrder);
            ThreadPool.QueueUserWorkItem(
                delegate { SendOrderViaREST(messageId, json); }
            );
            return messageId;
        }

        public long SubmitQuery(string orderId)
        {
            long messageId = GetNextMessageId();
            ThreadPool.QueueUserWorkItem(
                delegate { SendQueryViaREST(messageId, orderId); }
            );
            return messageId;
        }

        private void SendOrderViaREST(long messageId, string json)
        {
            Log.InfoFormat("Submmiting order, messageId {0}", messageId);
            Log.DebugFormat("Submitting order, messageId: {0}, Json: {1}", messageId, json);
            HttpWebResponse response = _restClient.SendJSON("POST", "", json);
            try
            {
                OrderResponse orderResponse = new OrderResponse(response) {MessageId = messageId};
                if (OrderSubmitEventHandler != null)
                {
                    Log.DebugFormat(
                        "Notifying OrderSubmitEventHandler of order submit response, messageId: {0}, orderId: {1}",
                        messageId, orderResponse.GetOrderId());
                    OrderSubmitEventHandler(this, orderResponse);
                }
                else
                {
                    Log.WarnFormat(
                        "OrderSubmitEventHandler was null dropping order response, messageId: {0}, orderId: {1}",
                        messageId, orderResponse.GetOrderId());
                }
            }
            catch (Exception e)
            {
                Log.Warn("Unexpected error occurred parsing response", e);
                throw;
            }
        }

        private void SendQueryViaREST(long messageId, string orderId)
        {
            Log.DebugFormat("Querying orderId {0}, messageId {1}", orderId, messageId);
            HttpWebResponse response = _restClient.SendMessage("GET", "?order_ts_id=" + orderId);
            OrderResponse orderResponse = new OrderResponse(response) {MessageId = messageId};
            if (OrderQueryEventHandler != null)
            {
                OrderQueryEventHandler(this, orderResponse);
            }
            else
            {
                Log.WarnFormat("OrderQueryEventHandler was null dropping order response, messageId: {0}, orderId: {1}",
                    messageId, orderResponse.GetOrderId());
            }
        }

        private static long GetNextMessageId()
        {
            return Interlocked.Increment(ref _nextMessageId);
        }


        private static void InitialiseNextMessageId()
        {
            int hashCode = Environment.TickCount.GetHashCode();
            const long mask = (1L << 16) - 1;
            _nextMessageId = Math.Abs((hashCode & mask) << 32);
        }
    }
}