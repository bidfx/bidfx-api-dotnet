using System.Collections.Generic;
using BidFX.Public.API.Trade.Order;
using NUnit.Framework;

namespace BidFX.Public.API.Trade.Rest.Json
{
    public class JsonMarshallerTest
    {
        [Test]
        public void TestEmptyFxOrderReturnsJustCorrelationIdAndAssetClass()
        {
            FxOrder order = new FxOrderBuilder().Build();
            const string expected = "[{\"asset_class\":\"FX\",\"correlation_id\":\"451\"}]";
            Assert.AreEqual(expected, JsonMarshaller.ToJson(order, 451));
        }

        [Test]
        public void TestStringFieldsReturnsQuotedValues()
        {
            FxOrder order = new FxOrderBuilder().SetAccount("FX_ACCT")
                .SetCurrencyPair("GBPUSD")
                .SetDealType("NDS")
                .SetCurrency("GBP")
                .SetFixingDate("2018-01-30")
                .SetHandlingType("AUTOMATIC")
                .SetReferenceOne("ref 1")
                .SetReferenceTwo("ref 2")
                .SetSettlementDate("2018-01-31")
                .SetSide("BUY")
                .SetTenor("2Y")
                .SetOrderType("LIMIT")
                .SetFarCurrency("GBP")
                .SetFarFixingDate("2018-02-27")
                .SetFarSettlementDate("2018-02-28")
                .SetFarTenor("3Y")
                .SetAllocationTemplate("AllocationName")
                .SetStrategyParameter("strategy_name", "strat_value")
                .Build();
            const string expected = "[{" +
                                    "\"account\":\"FX_ACCT\"," +
                                    "\"allocation_template\":\"AllocationName\"," +
                                    "\"asset_class\":\"FX\"," +
                                    "\"ccy_pair\":\"GBPUSD\"," +
                                    "\"correlation_id\":\"321\"," +
                                    "\"deal_type\":\"NDS\"," +
                                    "\"dealt_ccy\":\"GBP\"," +
                                    "\"far_dealt_ccy\":\"GBP\"," +
                                    "\"far_fixing_date\":\"2018-02-27\"," +
                                    "\"far_settlement_date\":\"2018-02-28\"," +
                                    "\"far_tenor\":\"3Y\"," +
                                    "\"fixing_date\":\"2018-01-30\"," +
                                    "\"handling_type\":\"AUTOMATIC\"," +
                                    "\"order_type\":\"LIMIT\"," +
                                    "\"reference1\":\"ref 1\"," +
                                    "\"reference2\":\"ref 2\"," +
                                    "\"settlement_date\":\"2018-01-31\"," +
                                    "\"side\":\"BUY\"," +
                                    "\"strategy_name\":\"strat_value\"," +
                                    "\"tenor\":\"2Y\"" +
                                    "}]";
            Assert.AreEqual(expected, JsonMarshaller.ToJson(order, 321));
        }

        [Test]
        public void TestSettingDecimalFieldsReturnsNonQuotedValues()
        {
            FxOrder order = new FxOrderBuilder()
                .SetQuantity(1000000.00m)
                .SetFarQuantity(2000000)
                .SetPrice(345.32123m)
                .Build();
            const string expected = "[{" +
                                    "\"asset_class\":\"FX\"," +
                                    "\"correlation_id\":\"123\"," +
                                    "\"far_quantity\":2000000," +
                                    "\"price\":345.32123," +
                                    "\"quantity\":1000000.00" +
                                    "}]";
            Assert.AreEqual(expected, JsonMarshaller.ToJson(order, 123));
        }

        [Test]
        public void TestSuccessfulFutureRestReponseIsParsedCorrectly()
        {
            const string json = "["+
            "{" +
                "\"alternate_owner\": \"lasman\"," +
                "\"asset_class\":\"FUTURE\"," +
                "\"correlation_id\":\"CID2\"," +
                "\"creation_date\":\"2018-07-09T16:57:16.374Z\"," +
                "\"deactivation_date\":\"2018-07-10T02:57:16.374Z\"," +
                "\"done_for_day\":false," +
                "\"errant_quantity\":0," +
                "\"executed_quantity\":0," +
                "\"executed_value\":0," +
                "\"executing_broker\":\"TS-SS\"," +
                "\"fully_executed\":false," +
                "\"instrument_code\":\"ZVBH9 Index\"," +
                "\"instrument_code_type\":\"BLOOMBERG\"," +
                "\"instrument_ts_id\":\"2241264189\"," +
                "\"leaves_quantity\":1000," +
                "\"order_ts_id\":\"20180709-3102162260-640-319-API\"," +
                "\"order_type\":\"LIMIT\"," +
                "\"outstanding_quantity\":0," +
                "\"owner\":\"lasman\"," +
                "\"price\":99," +
                "\"quantity\":1000," +
                "\"reference1\":\"REF1\"," +
                "\"reference2\":\"REF2\"," +
                "\"released_quantity\":0," +
                "\"settlement_currency\":\"USD\"," +
                "\"settlement_date\":\"2018-07-09T10:00:00.000Z\"," +
                "\"side\":\"BUY\"," +
                "\"state\":\"REGISTERED\"," +
                "\"strategy_state\":\"NONE\"," +
                "\"time_in_force_type\":\"DAY\"," +
                "\"unexecuted_quantity\":1000," +
                "\"uuid\": 595038130165" +
            "}" +
            "]";
            Order.Order order = JsonMarshaller.FromJson(json);
            Assert.IsInstanceOf<FutureOrder>(order);
            FutureOrder futureOrder = (FutureOrder) order;
            Assert.AreEqual("lasman", futureOrder.GetAlternateOwner());
            Assert.AreEqual("FUTURE", futureOrder.GetAssetClass());
            Assert.AreEqual("CID2", futureOrder.GetMessageId());
            Assert.AreEqual("2018-07-09T16:57:16.374Z", futureOrder.GetCreationDate());
            Assert.AreEqual("2018-07-10T02:57:16.374Z", futureOrder.GetDeactivationDate());
            Assert.AreEqual(false, futureOrder.GetDoneForDay());
            Assert.AreEqual(0, futureOrder.GetErrantQuantity());
            Assert.AreEqual(0, futureOrder.GetExecutedQuantity());
            Assert.AreEqual("TS-SS", futureOrder.GetExecutingBroker());
            Assert.AreEqual(false, futureOrder.GetFullyExecuted());
            Assert.AreEqual("ZVBH9 Index", futureOrder.GetInstrumentCode());
            Assert.AreEqual("BLOOMBERG", futureOrder.GetInstrumentCodeType());
            Assert.AreEqual("2241264189", futureOrder.GetInstrumentTsId());
            Assert.AreEqual(1000m, futureOrder.GetLeavesQuantity());
            Assert.AreEqual("20180709-3102162260-640-319-API", futureOrder.GetOrderTsId());
            Assert.AreEqual("LIMIT", futureOrder.GetOrderType());
            Assert.AreEqual(0m, futureOrder.GetOutstandingQuantity());
            Assert.AreEqual("lasman", futureOrder.GetOwner());
            Assert.AreEqual(99m, futureOrder.GetPrice());
            Assert.AreEqual(1000m, futureOrder.GetQuantity());
            Assert.AreEqual("REF1", futureOrder.GetReference1());
            Assert.AreEqual("REF2", futureOrder.GetReference2());
            Assert.AreEqual(0, futureOrder.GetReleasedQuantity());
            Assert.AreEqual("USD", futureOrder.GetSettlementCurrency());
            Assert.AreEqual("2018-07-09T10:00:00.000Z", futureOrder.GetSettlementDate());
            Assert.AreEqual("BUY", futureOrder.GetSide());
            Assert.AreEqual("REGISTERED", futureOrder.GetState());
            Assert.AreEqual("NONE", futureOrder.GetStrategyState());
            Assert.AreEqual("DAY", futureOrder.GetTimeInForceType());
            Assert.AreEqual(1000, futureOrder.GetUnexecutedQuantity());
            Assert.AreEqual(595038130165m, futureOrder.GetUUID());
        }

        [Test]
        public void TestSuccessfulFxRestResponseIsParsedCorrectly()
        {
            const string json = 
            "[{" +
                "\"account\": \"FX_ACCT\"," +
                "\"alternate_owner\": \"lasman\"," +
                "\"asset_class\": \"FX\"," +
                "\"ccy_pair\": \"EURGBP\"," +
                "\"deal_type\": \"NDS\"," +
                "\"dealt_ccy\": \"GBP\"," +
                "\"far_dealt_ccy\": \"GBP\"," +
                "\"far_fixing_date\": \"2019-01-10\"," +
                "\"far_quantity\": 150000," +
                "\"far_settlement_date\": \"2019-01-14\"," +
                "\"far_side\": \"BUY\"," +
                "\"far_tenor\": \"6M\"," +
                "\"fixing_date\": \"2018-07-24\"," +
                "\"handling_type\": \"AUTOMATIC\"," +
                "\"order_ts_id\": \"20180710-1052056868-524-782-API\"," +
                "\"order_type\": \"LIMIT\"," +
                "\"owner\": \"lasman\"," +
                "\"price\": 1.44321," +
                "\"quantity\": 150000," +
                "\"settlement_date\": \"2018-07-26\"," +
                "\"side\": \"SELL\"," +
                "\"state\": \"REGISTERED\"," +
                "\"tenor\": \"2W\"," +
                "\"uuid\": 190741670917 " +
            "}]";
            Order.Order order = JsonMarshaller.FromJson(json);
            Assert.IsInstanceOf<FxOrder>(order);
            FxOrder fxOrder = (FxOrder) order;
            Assert.AreEqual("FX_ACCT", fxOrder.GetAccount());
            Assert.AreEqual("lasman", fxOrder.GetAlternateOwner());
            Assert.AreEqual("FX", fxOrder.GetAssetClass());
            Assert.AreEqual("EURGBP", fxOrder.GetCurrencyPair());
            Assert.AreEqual("NDS", fxOrder.GetDealType());
            Assert.AreEqual("GBP", fxOrder.GetCurrency());
            Assert.AreEqual("GBP", fxOrder.GetFarCurrency());
            Assert.AreEqual("2019-01-10", fxOrder.GetFarFixingDate());
            Assert.AreEqual(150000, fxOrder.GetFarQuantity());
            Assert.AreEqual("2019-01-14", fxOrder.GetFarSettlementDate());
            Assert.AreEqual("BUY", fxOrder.GetFarSide());
            Assert.AreEqual("6M", fxOrder.GetFarTenor());
            Assert.AreEqual("2018-07-24", fxOrder.GetFixingDate());
            Assert.AreEqual("AUTOMATIC", fxOrder.GetHandlingType());
            Assert.AreEqual("20180710-1052056868-524-782-API", fxOrder.GetOrderTsId());
            Assert.AreEqual("LIMIT", fxOrder.GetOrderType());
            Assert.AreEqual("lasman", fxOrder.GetOwner());
            Assert.AreEqual(1.44321m, fxOrder.GetPrice());
            Assert.AreEqual(150000, fxOrder.GetQuantity());
            Assert.AreEqual("2018-07-26", fxOrder.GetSettlementDate());
            Assert.AreEqual("SELL", fxOrder.GetSide());
            Assert.AreEqual("REGISTERED", fxOrder.GetState());
            Assert.AreEqual("2W", fxOrder.GetTenor());
            Assert.AreEqual(190741670917, fxOrder.GetUUID());
        }
        
        [Test]
        public void TestRestOrderResponseWithErrorIsParsedCorrectly()
        {
            const string json = "[{" +
                "\"account\": \"FX_ACCT\"," +
                "\"alternate_owner\": \"lasman\"," +
                "\"asset_class\": \"FX\"," +
                "\"ccy_pair\": \"EURGBP\"," +
                "\"deal_type\": \"NDS\"," +
                "\"errors\": [" +
                "{" +
                "    \"field\": \"dealt_ccy\"," +
                "    \"message\": \"Missing required field\"" +
                "}," +
                "{" +
                "    \"field\": \"far_dealt_ccy\"," +
                "    \"message\": \"Missing required field\"" +
                "}," +
                "{" +
                "    \"field\": \"far_side\"," +
                "    \"message\": \"Missing required field\"" +
                "}" +
                "]," +
                "\"far_fixing_date\": \"2019-01-10\"," +
                "\"far_quantity\": 150000," +
                "\"far_settlement_date\": \"2019-01-14\"," +
                "\"far_tenor\": \"6M\"," +
                "\"fixing_date\": \"2018-07-24\"," +
                "\"handling_type\": \"AUTOMATIC\"," +
                "\"order_ts_id\": \"20180710-1627231301-751-720-API\"," +
                "\"order_type\": \"LIMIT\"," +
                "\"owner\": \"dtang\"," +
                "\"price\": 1.44321," +
                "\"quantity\": 120000," +
                "\"settlement_date\": \"2018-07-26\"," +
                "\"tenor\": \"2W\"" +
            "}]";
            Order.Order order = JsonMarshaller.FromJson(json);
            Assert.IsInstanceOf<FxOrder>(order);
            FxOrder fxOrder = (FxOrder) order;
            Assert.AreEqual("FX_ACCT", fxOrder.GetAccount());
            Assert.AreEqual("lasman", fxOrder.GetAlternateOwner());
            Assert.AreEqual("FX", fxOrder.GetAssetClass());
            Assert.AreEqual("EURGBP", fxOrder.GetCurrencyPair());
            Assert.AreEqual("NDS", fxOrder.GetDealType());
            Assert.AreEqual("2019-01-10", fxOrder.GetFarFixingDate());
            Assert.AreEqual(150000, fxOrder.GetFarQuantity());
            Assert.AreEqual("2019-01-14", fxOrder.GetFarSettlementDate());
            Assert.AreEqual("6M", fxOrder.GetFarTenor());
            Assert.AreEqual("2018-07-24", fxOrder.GetFixingDate());
            Assert.AreEqual("AUTOMATIC", fxOrder.GetHandlingType());
            Assert.AreEqual("20180710-1627231301-751-720-API", fxOrder.GetOrderTsId());
            Assert.AreEqual("LIMIT", fxOrder.GetOrderType());
            Assert.AreEqual("dtang", fxOrder.GetOwner());
            Assert.AreEqual(1.44321m, fxOrder.GetPrice());
            Assert.AreEqual(120000, fxOrder.GetQuantity());
            Assert.AreEqual("2018-07-26", fxOrder.GetSettlementDate());
            Assert.AreEqual("2W", fxOrder.GetTenor());
            List<Error> errors = fxOrder.GetErrors();
            Assert.AreEqual(3, errors.Count);
            Error error = errors[0];
            Assert.AreEqual("dealt_ccy", error.Field);
            Assert.AreEqual("Missing required field", error.Message);
            error = errors[1];
            Assert.AreEqual("far_dealt_ccy", error.Field);
            Assert.AreEqual("Missing required field", error.Message);
            error = errors[2];
            Assert.AreEqual("far_side", error.Field);
            Assert.AreEqual("Missing required field", error.Message);
        }

        [Test]
        public void TestSingleErrorRestResponseIsParsedCorrectly()
        {
            const string json = "{" +
            "\"type\": \"Bad Request\"," +
            "\"message\": \"Failed to deserialize body text expecting List of Order\"" +
            "}";
            Order.Order order = JsonMarshaller.FromJson(json);
            Assert.AreEqual(1, order.GetErrors().Count);
            Assert.AreEqual("Failed to deserialize body text expecting List of Order", order.GetErrors()[0].Message);
            Assert.IsNull(order.GetErrors()[0].Field);
        }
    }
}