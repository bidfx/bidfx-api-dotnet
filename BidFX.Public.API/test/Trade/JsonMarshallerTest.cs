﻿using System;
using System.Diagnostics;
using BidFX.Public.API.Trade.Order;
using NUnit.Framework;

namespace BidFX.Public.API.Trade
{
    public class JsonMarshallerTest
    {
        [Test]
        public void TestEmptyFxOrderReturnsEmptyDictionary()
        {
            FxOrder order = new FxOrderBuilder().Build();
            string expected = "{}";
            Assert.AreEqual(expected, JsonMarshaller.ToJSON(order));
        }

        [Test]
        public void TestStringFieldsReturnsQuotedValues()
        {
            FxOrder order = new FxOrderBuilder().SetAccount("FX_ACCT")
                .SetCurrencyPair("GBPUSD")
                .SetDealType("NDS")
                .SetCurrency("GBP")
                .SetFixingDate("2018-01-30")
                .SetHandlingType("automatic")
                .SetReferenceOne("ref 1")
                .SetReferenceTwo("ref 2")
                .SetSettlementDate("2018-01-31")
                .SetSide("Buy")
                .SetTenor("2Y")
                .SetFarCurrency("GBP")
                .SetFarFixingDate("2018-02-27")
                .SetFarSettlementDate("2018-02-28")
                .SetFarTenor("3Y")
                .SetAllocationTemplate("AllocationName")
                .SetStrategyParameter("strat_name", "strat_value")
                .Build();
            string expected = "{" +
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
                           "\"handling_type\":\"automatic\"," +
                           "\"reference\":\"ref 1\"," +
                           "\"reference2\":\"ref 2\"," +
                           "\"settlement_date\":\"2018-01-31\"," +
                           "\"side\":\"Buy\"," +
                           "\"strat_name\":\"strat_value\"," +
                           "\"tenor\":\"2Y\"" +
                           "}";
            Assert.AreEqual(expected, JsonMarshaller.ToJSON(order));
        }

        [Test]
        public void TestSettingDecimalFieldsReturnsNonQuotedValues()
        {
            FxOrder order = new FxOrderBuilder()
                .SetQuantity("1000000.00")
                .SetFarQuantity("2000000")
                .SetPrice("345.32123")
                .Build();
            string expected = "{" +
                           "\"far_quantity\":2000000," +
                           "\"price\":345.32123," +
                           "\"quantity\":1000000.00" +
                           "}";
            Assert.AreEqual(expected, JsonMarshaller.ToJSON(order));
        }
    }
}