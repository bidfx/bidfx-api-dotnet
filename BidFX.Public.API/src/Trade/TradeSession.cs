/// Copyright (c) 2018 BidFX Systems LTD. All Rights Reserved.

using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using BidFX.Public.API.Trade.Rest.Json;
using BidFX.Public.API.Trade.REST;
using log4net;

namespace BidFX.Public.API.Trade
{
    public class TradeSession
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public event EventHandler<Order.Order> OrderQueryEventHandler;
        public event EventHandler<Order.Order> OrderSubmitEventHandler;
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
            Log.InfoFormat("TradeSession connecting to {0}", address);
        }

        public long SubmitOrder(Order.Order order)
        {
            long messageId = GetNextMessageId();
            string json = JsonMarshaller.ToJson(order, messageId);
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
            Log.InfoFormat("Submmiting order, messageId: {0}", messageId);
            Log.DebugFormat("Submitting order, messageId: {0}, Json: {1}", messageId, json);
            HttpWebResponse response;
            try
            {
                response = _restClient.SendJSON("POST", "", json);
            }
            catch (Exception e)
            {
                Log.Warn("Unexpected error occurred sending message", e);
                throw;
            }

            try
            {
                Log.InfoFormat("MessageId {0} - Response Received from Server. Processing", messageId);
                string jsonResponse = GetBodyFromResponse(response);
                if (jsonResponse == null)
                {
                    Log.InfoFormat("MessageId {0} - No body to return.", messageId);
                    return;
                }
                else
                {
                    Log.DebugFormat("MessageID {0} - Received Message:\n {1}", messageId, jsonResponse);
                }
                Order.Order order = JsonMarshaller.FromJson(jsonResponse);
                if (OrderSubmitEventHandler != null)
                {
                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat(
                            "Notifying OrderSubmitEventHandler of order submit response, messageId: {0}, orderId: {1}",
                            messageId, order.GetOrderTsId());
                    }
                    OrderSubmitEventHandler(this, order);
                }
                else
                {
                    Log.WarnFormat(
                        "OrderSubmitEventHandler was null dropping order response: {0}",
                        order.ToString());
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
            Order.Order order = JsonMarshaller.FromJson(GetBodyFromResponse(response));
            if (OrderQueryEventHandler != null)
            {
                OrderQueryEventHandler(this, order);
            }
            else
            {
                Log.WarnFormat("OrderQueryEventHandler was null dropping order response, messageId: {0}, orderId: {1}",
                    messageId, order.GetOrderTsId());
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

        private static string GetBodyFromResponse(HttpWebResponse response)
        {
            if (response == null)
            {
                Log.Warn("Response was null");
                return null;
            }

            Stream responseStream = response.GetResponseStream();
            if (responseStream == null)
            {
                Log.Warn("ResponseStream was null");
                return null;
            }
            StreamReader streamReader = new StreamReader(responseStream);
            return streamReader.ReadToEnd();
        }
        
    }
}