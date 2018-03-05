using System;
using System.Diagnostics;
using BidFX.Public.API.Trade.Order;
using NUnit.Framework;

namespace BidFX.Public.API.Trade
{
    public class JsonMarshallerTest
    {
        [Test]
        public void TestEmptyFxOrderReturnsJustCorrelationId()
        {
            FxOrder order = new FxOrderBuilder().Build();
            const string expected = "[{\"correlation_id\":\"451\"}]";
            Assert.AreEqual(expected, JsonMarshaller.ToJSON(order, 451));
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
                                    "\"ccy_pair\":\"GBPUSD\"," +
                                    "\"deal_type\":\"NDS\"," +
                                    "\"dealt_ccy\":\"GBP\"," +
                                    "\"far_dealt_ccy\":\"GBP\"," +
                                    "\"far_fixing_date\":\"2018-02-27\"," +
                                    "\"far_settlement_date\":\"2018-02-28\"," +
                                    "\"far_tenor\":\"3Y\"," +
                                    "\"fixing_date\":\"2018-01-30\"," +
                                    "\"handling_type\":\"AUTOMATIC\"," +
                                    "\"reference\":\"ref 1\"," +
                                    "\"reference2\":\"ref 2\"," +
                                    "\"settlement_date\":\"2018-01-31\"," +
                                    "\"side\":\"BUY\"," +
                                    "\"strategy_name\":\"strat_value\"," +
                                    "\"tenor\":\"2Y\"," +
                                    "\"correlation_id\":\"321\"" +
                                    "}]";
            Assert.AreEqual(expected, JsonMarshaller.ToJSON(order, 321));
        }

        [Test]
        public void TestSettingDecimalFieldsReturnsNonQuotedValues()
        {
            FxOrder order = new FxOrderBuilder()
                .SetQuantity("1000000.00")
                .SetFarQuantity("2000000")
                .SetPrice("345.32123")
                .Build();
            const string expected = "[{" +
                                    "\"far_quantity\":2000000," +
                                    "\"price\":345.32123," +
                                    "\"quantity\":1000000.00," +
                                    "\"correlation_id\":\"123\"" +
                                    "}]";
            Assert.AreEqual(expected, JsonMarshaller.ToJSON(order, 123));
        }
    }
}