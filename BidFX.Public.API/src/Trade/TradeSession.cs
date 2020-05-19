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
using Serilog;
using Serilog.Events;

namespace BidFX.Public.API.Trade
{
    public class TradeSession
    {
        private static readonly ILogger Log = Logger.ForContext<TradeSession>();
        public event EventHandler<Order.Order> OrderSubmitEventHandler;
        public event EventHandler<Order.Order> OrderQueryEventHandler;
        public event EventHandler<Order.Order> OrderInstructionEventHandler;
        public event EventHandler<SettlementDateResponse> SettlementDateQueryEventHandler;
        private RESTClient _restClient;
        private static long _nextMessageId;
        public bool Running { get; private set; }
        private readonly UserInfo UserInfo;

        public TradeSession(UserInfo userInfo)
        {
            UserInfo = userInfo;
        }
        
        public void Start()
        {
            Uri uri = new UriBuilder
            {
                Scheme = UserInfo.Https ? "https" : "http",
                Host = UserInfo.Host,
                Port = UserInfo.Port
            }.Uri;
            _restClient = new RESTClient(uri, UserInfo.Username, UserInfo.Password);
            InitialiseNextMessageId();
            Log.Information("TradeSession connecting to {uri}", uri);
            Running = true;
        }
        
        public void Stop()
        {
            Running = false;
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
                Log.Error("MessageId {messageId} - Don't know what to do with Instruction of type {type}", messageId, instruction.GetType());
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
            
            if (Log.IsEnabled(LogEventLevel.Debug))
            {
                Log.Debug("Submitting order, messageId: {messageId}, json: {json}", messageId, json);
            }
            else
            {
                Log.Information("Submitting order, messageId: {messageId}", messageId);
            }
            Order.Order order = SendObjectViaRest(messageId, json, @"api/om/v2/order");
            if (order != null && OrderSubmitEventHandler != null)
            {
                Log.Debug("Notifying OrderSubmitEventHandler of order submit response, messageId: {messageId}, orderId: {orderId}", messageId, order.GetOrderTsId());
                OrderSubmitEventHandler.Invoke(this, order);
            }
            else if (OrderSubmitEventHandler == null)
            {
                Log.Information("No OrderSubmitEventHandler registered, dropping messageId {messsageId}, orderId {orderId}", messageId, order.GetOrderTsId());
            }
            else
            {
                Log.Information("Order response was null, messageId {messageId}", messageId);
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
            if (Log.IsEnabled(LogEventLevel.Debug))
            {
                Log.Debug("Submitting instruction, messageId: {messageId}, json: {json}", messageId, json);
            }
            else
            {
                Log.Information("Submitting instruction, messageId: {messageId}", messageId);
            }

            Order.Order  order= SendObjectViaRest(messageId, json, @"api/om/v2/order/amend");
            if (order != null && OrderInstructionEventHandler != null)
            {
                Log.Debug("Notifying OrderInstructionEventHandler of order instruction response, messageId: {messageId}, orderId: {orderId}", messageId, order.GetOrderTsId());
                OrderInstructionEventHandler.Invoke(this, order);
            }
            else if (OrderSubmitEventHandler == null)
            {
                Log.Information("No OrderInstructionEventHandler registered, dropping messageId {messageId}, orderId {orderId}", messageId, order.GetOrderTsId());
            }
            else
            {
                Log.Information("Order Instruction response was null, messageId {messageId}", messageId);
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
                Log.Warning(e, "Unexpected error occurred sending message");
                if (response != null)
                {
                    response.Close();
                }
                return null;
            }

            try
            {
                Log.Information("MessageId {messageId} - Response Received from Server. Processing", messageId);
                string jsonResponse = GetBodyFromResponse(response);
                response.Close();
                if (jsonResponse == null)
                {
                    Log.Information("MessageId {messageId} - No body to return.", messageId);
                    return null;
                }
                else
                {
                    Log.Debug("MessageID {messageId} - Received Message:\n {json}", messageId, jsonResponse);
                }
                Order.Order order = Order.Order.FromJson(JsonMarshaller.FromJson(jsonResponse));
                if (order != null)
                {
                    order.SetMessageId(messageId);
                }
                return order;
            }
            catch (Exception e)
            {
                Log.Warning(e, "Unexpected error occurred parsing response");
                return null;
            }
        }

        private void SendOrderQueryViaREST(long messageId, string orderId)
        {
            Log.Debug("Querying orderId {orderId}, messageId {messageId}", orderId, messageId);
            using (HttpWebResponse response = _restClient.SendMessage("GET", "api/om/v2/order", "order_ts_id=" + orderId))
            {
                Order.Order order = Order.Order.FromJson(JsonMarshaller.FromJson(GetBodyFromResponse(response)));
                if (order == null)
                {
                    Log.Information("No valid order returned, messageId {messageId}, orderId: {orderId}", messageId, orderId);
                    return;
                }
                
                order.SetMessageId(messageId);
                if (OrderQueryEventHandler != null)
                {
                    OrderQueryEventHandler(this, order);
                }
                else
                {
                    Log.Warning(
                        "OrderQueryEventHandler was null dropping order response, messageId: {messageId}, orderId: {orderId}",
                        messageId, order.GetOrderTsId());
                }
            }
        }

        private void SendSettlementDateQueryViaRest(long messageId, string dealType, string ccyPair, string tenor, string settlementDate, string farTenor, string farSettlementDate)
        {
            StringBuilder query = new StringBuilder(); 
            query.Append("deal_type=").Append(dealType).Append("&ccy_pair=").Append(ccyPair);
            
            if (!string.IsNullOrWhiteSpace(tenor))
            {
                query.Append("&tenor=").Append(tenor);
            }
            if (!string.IsNullOrWhiteSpace(settlementDate))
            {
                query.Append("&settlement_date=").Append(settlementDate);
            }
            if (!string.IsNullOrWhiteSpace(farTenor))
            {
                query.Append("&far_tenor=").Append(farTenor);
            }

            if (!string.IsNullOrWhiteSpace(farSettlementDate))
            {
                query.Append("&far_settlement_date=").Append(farSettlementDate);
            }
            SendSettlementDateQueryViaRest(messageId, query.ToString());
        }

        private void SendSettlementDateQueryViaRest(long messageId, string query)
        {
            try
            {
                Log.Debug("Sending settlement date query {query}, messageId {messageId}", query, messageId);
                using (HttpWebResponse response = _restClient.SendMessage("GET", "api/fx-dates/v1/fx-date", query))
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
                        Log.Warning(
                            "SettlementDateQueryEventHandler was null, dropping query response, messageId: {messageId}",
                            messageId);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Warning(e, "Unexpected error occurred parsing response");
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
                Log.Warning("Response was null");
                return null;
            }

            Stream responseStream = response.GetResponseStream();
            if (responseStream == null)
            {
                Log.Warning("ResponseStream was null");
                return null;
            }
            StreamReader streamReader = new StreamReader(responseStream);
            return streamReader.ReadToEnd();
        }
    }
}