using System;
using BidFX.Public.API.Trade;
using BidFX.Public.API.Trade.Order;
using BidFX.Public.API.Trade.Rest.Json;
using NUnit.Framework;

namespace BidFX.Public.API.Test.test.Trade
{
    public class SettlementDateResponseTest
    {
        [Test]
        public void TestDecodeSettlementDateOnlyFromJson()
        {
            long messageId = 2347;
            string json = "{" +
                          "\"settlement_date\": \"2019-04-25\"" +
                          "}";
            SettlementDateResponse settlementDateResponse = SettlementDateResponse.FromJson(messageId, JsonMarshaller.FromJson(json));
            Assert.AreEqual(messageId, settlementDateResponse.GetMessageId());
            Assert.AreEqual("2019-04-25", settlementDateResponse.GetSettlementDate());
            Assert.Null(settlementDateResponse.GetFixingDate());
            Assert.Null(settlementDateResponse.GetFarSettlementDate());
            Assert.Null(settlementDateResponse.GetFarFixingDate());
        }

        [Test]
        public void TestDecodeAllDatesFromJson()
        {
            long messageId = 2347;
            string json = "{"+
            "\"settlement_date\": \"2019-04-25\"," +
            "\"far_settlement_date\": \"2019-10-11\"," +
            "\"fixing_date\": \"2019-04-24\"," +
            "\"far_fixing_date\": \"2019-10-10\"" +
            "}";
            SettlementDateResponse settlementDateResponse = SettlementDateResponse.FromJson(messageId, JsonMarshaller.FromJson(json));
            Assert.AreEqual(messageId, settlementDateResponse.GetMessageId());
            Assert.AreEqual("2019-04-25", settlementDateResponse.GetSettlementDate());
            Assert.AreEqual("2019-10-11", settlementDateResponse.GetFarSettlementDate());
            Assert.AreEqual("2019-04-24", settlementDateResponse.GetFixingDate());
            Assert.AreEqual("2019-10-10", settlementDateResponse.GetFarFixingDate());
        }

        [Test]
        public void TestDecodeFromErrors()
        {
            long messageId = 2347;
            string json = "{" +
            "\"type\": \"Bad Request\"," +
            "\"message\": \"Error while validating URL parameters\"," +
            "\"params\": {"+
                "\"ccy_pair\": \"EURUSD\"," +
                "\"errors\": [" +
                "{" +
                    "\"field\": \"far_tenor\"," +
                    "\"message\": \"Missing required field: far tenor or settlement date must be provided for swaps\"" +
                "}" +
                "]," +
                "\"tenor\": \"2W\"" +
                "}" +
            "}";
            SettlementDateResponse settlementDateResponse = SettlementDateResponse.FromError(messageId, JsonMarshaller.FromJson(json));
            Assert.AreEqual(1, settlementDateResponse.GetErrors().Count);
            Error error = settlementDateResponse.GetErrors()[0];
            Assert.AreEqual("far_tenor", error.GetField());
            Assert.AreEqual("Missing required field: far tenor or settlement date must be provided for swaps",
                error.GetMessage());
            Assert.Null(error.GetValue());
        }
    }
}