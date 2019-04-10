/// Copyright (c) 2018 BidFX Systems LTD. All Rights Reserved.

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using BidFX.Public.API.Price.Tools;
using BidFX.Public.API.Trade.Instruction;
using BidFX.Public.API.Trade.Rest.Json;
using BidFX.Public.API.Trade.REST;
using log4net;

namespace BidFX.Public.API.Trade
{
    public class TradeSession
    {
        private static readonly ILog Log =
            LogManager.GetLogger("TradeSession");

        public event EventHandler<Order.Order> OrderSubmitEventHandler;
        public event EventHandler<Order.Order> OrderQueryEventHandler;
        public event EventHandler<Order.Order> OrderInstructionEventHandler;
        public event EventHandler<SettlementDateResponse> SettlementDateQueryEventHandler;
        private RESTClient _restClient;
        private static long _nextMessageId;
        public bool Running { get; private set; }
        internal LoginService LoginService { private get; set; }

        public void Start()
        {
            if (!LoginService.LoggedIn)
            {
                throw new IllegalStateException("Not logged in.");
            }
            Uri uri = new UriBuilder
            {
                Scheme = LoginService.Https ? "https" : "http",
                Host = LoginService.Host,
                Port = LoginService.Port
            }.Uri;
            _restClient = new RESTClient(uri, LoginService.Username, LoginService.Password);
            InitialiseNextMessageId();
            Log.InfoFormat("TradeSession connecting to {0}", uri);
            LoginService.OnForcedDisconnectEventHandler += OnForcedDisconnect;
            Running = true;
        }
        
        public void Stop()
        {
            Running = false;
            LoginService.OnForcedDisconnectEventHandler -= OnForcedDisconnect;
        }

        public long SubmitOrder(Order.Order order)
        {
            if (!Running)
            {
                throw new IllegalStateException("TradeSession is not running");
            }
            long messageId = GetNextMessageId();
            string json = JsonMarshaller.ToJson(order, messageId);
            ThreadPool.QueueUserWorkItem(
                delegate { SendOrderViaREST(messageId, json); }
            );
            return messageId;
        }

        public long SubmitQuery(string orderId)
        {
            if (!Running)
            {
                throw new IllegalStateException("TradeSession is not running");
            }
            long messageId = GetNextMessageId();
            ThreadPool.QueueUserWorkItem(
                delegate { SendOrderQueryViaREST(messageId, orderId); }
            );
            return messageId;
        }

        public long SubmitOrderInstruction(OrderInstruction instruction)
        {
            if (!Running)
            {
                throw new IllegalStateException("TradeSession is not running");
            }
            long messageId = GetNextMessageId();
            string json = JsonMarshaller.ToJson(instruction, messageId);
            if (instruction is OrderAmend)
            {
                ThreadPool.QueueUserWorkItem(
                    delegate { SendAmendViaRest(messageId, json); }
                );
            }
            else if (instruction is OrderCancel)
            {
                ThreadPool.QueueUserWorkItem(
                    delegate { SendCancelViaRest(messageId, json); }
                );
            }
            else if (instruction is OrderSubmit)
            {
                ThreadPool.QueueUserWorkItem(
                    delegate { SendSubmitViaRest(messageId, json); }
                );
            }
            else
            {
                Log.ErrorFormat("MessageId {0} - Don't know what to do with Instruction of type {1}", messageId, instruction.GetType());
            }

            return messageId;
        }

        public long QuerySettlementDate(string dealType, string ccyPair, string tenor,
                                        string settlementDate = null, string farTenor = null, string farSettlementDate = null)
        {
            if (!Running)
            {
                throw new IllegalStateException("TradeSession is not running");
            }

            long messageId = GetNextMessageId();
            dealType = ConvertDealTypeForSettlementDateQuery(dealType);
            ccyPair = ccyPair.Trim().ToUpper();
            tenor = tenor.Trim().ToUpper();
            farTenor = farTenor == null ? null : farTenor.Trim().ToUpper();
            settlementDate = settlementDate == null
                ? null
                : DateFormatter.FormatDateWithHyphens("settlementDate", settlementDate, true);
            farSettlementDate = farSettlementDate == null
                ? null
                : DateFormatter.FormatDateWithHyphens("farSettlementDate", farSettlementDate, true);

            ThreadPool.QueueUserWorkItem(
                delegate { SendSettlementDateQueryViaRest(messageId, dealType, ccyPair, tenor, settlementDate, farTenor, farSettlementDate); }
            );

            return messageId;
        }

        private void SendOrderViaREST(long messageId, string json)
        {
            if (!Running)
            {
                throw new IllegalStateException("TradeSession is not running");
            }
            
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Submitting order, messageId: {0}, json: {1}", messageId, json);
            }
            else
            {
                Log.InfoFormat("Submitting order, messageId: {0}", messageId);
            }
            Order.Order order = SendObjectViaRest(messageId, json, @"api/om/v2/order");
            if (order != null && OrderSubmitEventHandler != null)
            {
                OrderSubmitEventHandler.Invoke(this, order);
            }
        }

        private void SendAmendViaRest(long mesageId, string json)
        {
            SendInstructionViaRest(mesageId, json, "amend");
        }

        private void SendCancelViaRest(long messageId, string json)
        {
            SendInstructionViaRest(messageId, json, "cancel");
        }

        private void SendSubmitViaRest(long messageId, string json)
        {
            SendInstructionViaRest(messageId, json, "submit");
        }

        private void SendInstructionViaRest(long messageId, string json, string path)
        {
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Submitting instruction, messageId: {0}, json: {1}", messageId, json);
            }
            else
            {
                Log.InfoFormat("Submitting instruction, messageId: {0}", messageId);
            }

            Order.Order  order= SendObjectViaRest(messageId, json, @"api/om/v2/order/amend");
            if (order != null && OrderInstructionEventHandler != null)
            {
                order.SetMessageId(messageId);
                OrderInstructionEventHandler.Invoke(this, order);
            }
        }

        private Order.Order SendObjectViaRest(long messageId, string json, string path)
        {
            HttpWebResponse response = null;
            try
            {
                response = _restClient.SendJSON("POST", path, json);
            }
            catch (Exception e)
            {
                Log.Warn("Unexpected error occurred sending message", e);
                if (response != null)
                {
                    response.Close();
                }
                return null;
            }

            try
            {
                Log.InfoFormat("MessageId {0} - Response Received from Server. Processing", messageId);
                string jsonResponse = GetBodyFromResponse(response);
                response.Close();
                if (jsonResponse == null)
                {
                    Log.InfoFormat("MessageId {0} - No body to return.", messageId);
                    return null;
                }
                else
                {
                    Log.DebugFormat("MessageID {0} - Received Message:\n {1}", messageId, jsonResponse);
                }
                Order.Order order = Order.Order.FromJson(JsonMarshaller.FromJson(jsonResponse));
                if (OrderSubmitEventHandler != null)
                {
                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat(
                            "Notifying OrderSubmitEventHandler of order submit response, messageId: {0}, orderId: {1}",
                            messageId, order.GetOrderTsId());
                    }
                    return order;
                }
                else
                {
                    Log.WarnFormat(
                        "OrderSubmitEventHandler was null dropping order response: {0}",
                        order.ToString());
                    return null;
                }
            }
            catch (Exception e)
            {
                Log.Warn("Unexpected error occurred parsing response", e);
                return null;
            }
        }

        private void SendOrderQueryViaREST(long messageId, string orderId)
        {
            Log.DebugFormat("Querying orderId {0}, messageId {1}", orderId, messageId);
            using (HttpWebResponse response = _restClient.SendMessage("GET", "api/om/v2/order", "order_ts_id=" + orderId))
            {
                Order.Order order = Order.Order.FromJson(JsonMarshaller.FromJson(GetBodyFromResponse(response)));
                order.SetMessageId(messageId);
                if (OrderQueryEventHandler != null)
                {
                    OrderQueryEventHandler(this, order);
                }
                else
                {
                    Log.WarnFormat(
                        "OrderQueryEventHandler was null dropping order response, messageId: {0}, orderId: {1}",
                        messageId, order.GetOrderTsId());
                }
            }
        }

        private void SendSettlementDateQueryViaRest(long messageId, string dealType, string ccyPair, string tenor, string settlementDate, string farTenor, string farSettlementDate)
        {
            StringBuilder query = new StringBuilder(); 
            query.Append("deal_type=").Append(dealType).Append("&ccy_pair=").Append(ccyPair);
            
            if (tenor != null)
            {
                query.Append("&tenor=").Append(tenor);
            }
            if (settlementDate != null)
            {
                query.Append("&settlement_date=").Append(settlementDate);
            }
            if (farTenor != null)
            {
                query.Append("&far_tenor=").Append(farTenor);
            }

            if (farSettlementDate != null)
            {
                query.Append("&far_settlement_date=").Append(farSettlementDate);
            }
            SendSettlementDateQueryViaRest(messageId, query.ToString());
        }

        private void SendSettlementDateQueryViaRest(long messageId, string query)
        {
            try
            {
                Log.DebugFormat("Sending settlement date query {0}, messageId {1}", query, messageId);
                using (HttpWebResponse response = _restClient.SendMessage("GET", "api/fx-dates/v1", query))
                {
                    SettlementDateResponse settlementDateResponse;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        settlementDateResponse =
                            SettlementDateResponse.FromJson(messageId, JsonMarshaller.FromJson(GetBodyFromResponse(response)));
                    }
                    else
                    {
                        settlementDateResponse =
                            SettlementDateResponse.FromError(messageId, JsonMarshaller.FromJson(GetBodyFromResponse(response)));
                    }

                    if (SettlementDateQueryEventHandler != null)
                    {
                        SettlementDateQueryEventHandler(this, settlementDateResponse);
                    }
                    else
                    {
                        Log.WarnFormat(
                            "SettlementDateEQueryEventHandler was null, dropping query response, messageId: {0}",
                            messageId);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Warn("Unexpected error occurred parsing response", e);
            }
            
        }

        private string ConvertDealTypeForSettlementDateQuery(string dealType)
        {
            dealType = dealType.Trim().ToUpper();
            switch (dealType)
            {
                case "FWD":
                case "FORWARD":
                case "OUTRIGHT":
                    return "FWD";
                default:
                    return dealType;
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

        private void OnForcedDisconnect(object sender, DisconnectEventArgs e)
        {
            Running = false;
            LoginService.OnForcedDisconnectEventHandler -= OnForcedDisconnect;
        }
    }
}