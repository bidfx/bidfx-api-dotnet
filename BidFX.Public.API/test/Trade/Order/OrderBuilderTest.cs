﻿using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace BidFX.Public.API.Trade.Order
{
    public class OrderBuilderTest
    {
        private OrderBuilder<TestOrderBuilderImpl, TestOrderImpl> _orderBuilder;

        [SetUp]
        public void Before()
        {
            _orderBuilder = new TestOrderBuilderImpl();
        }

        [Test]
        public void TestAssetClassIsSetInOrder()
        {
            Order order = _orderBuilder.Build();
            Assert.AreEqual("TEST", order.GetAssetClass());
        }

        [Test]
        public void TestSettingExecutingAccount()
        {
            Order order = _orderBuilder.SetAccount("FX_ACCT").Build();
            Assert.AreEqual("FX_ACCT", order.GetAccount());

            order = _orderBuilder.SetAccount("  AN_ACCOUNT ").Build();
            Assert.AreEqual("AN_ACCOUNT", order.GetAccount());
        }

        [Test]
        public void TestNullAccountClearsAccount()
        {
            _orderBuilder.SetAccount("FX_ACCT");
            _orderBuilder.SetAccount(null);
            Order order = _orderBuilder.Build();
            Assert.AreEqual(1, order.GetInternalComponents().Count);
        }

        [Test]
        public void TestEmptyAccountClearsAccount()
        {
            _orderBuilder.SetAccount("FX_ACCT");
            _orderBuilder.SetAccount("");
            Order order = _orderBuilder.Build();
            Assert.AreEqual(1, order.GetInternalComponents().Count);
        }

        [Test]
        public void TestBlankAccountClearsAccount()
        {
            _orderBuilder.SetAccount("FX_ACCT");
            _orderBuilder.SetAccount("   ");
            Order order = _orderBuilder.Build();
            Assert.AreEqual(1, order.GetInternalComponents().Count);
        }

        [Test]
        public void TestSettingSide()
        {
            Order order = _orderBuilder.SetSide("buy").Build();
            Assert.AreEqual("BUY", order.GetSide());

            order = _orderBuilder.SetSide("sell").Build();
            Assert.AreEqual("SELL", order.GetSide());

            order = _orderBuilder.SetSide("  buy").Build();
            Assert.AreEqual("BUY", order.GetSide());

            order = _orderBuilder.SetSide(" sell  ").Build();
            Assert.AreEqual("SELL", order.GetSide());
        }

        [Test]
        [ExpectedException("System.ArgumentException", ExpectedMessage =
            "Side must be either 'BUY' or 'SELL': Invalid")]
        public void TestSettingInvalidSideThrowsException()
        {
            _orderBuilder.SetSide("Invalid");
        }

        [Test]
        public void TestSettingNullSideClearsSide()
        {
            _orderBuilder.SetSide("Buy");
            _orderBuilder.SetSide(null);
            Order order = _orderBuilder.Build();
            Assert.AreEqual(1, order.GetInternalComponents().Count);
        }

        [Test]
        public void TestSettingEmptySideClearsSide()
        {
            _orderBuilder.SetSide("Sell");
            _orderBuilder.SetSide("");
            Order order = _orderBuilder.Build();
            Assert.AreEqual(1, order.GetInternalComponents().Count);
        }

        [Test]
        public void TestSettingBlankSideClearsSide()
        {
            _orderBuilder.SetSide("Sell");
            _orderBuilder.SetSide("     ");
            Order order = _orderBuilder.Build();
            Assert.AreEqual(1, order.GetInternalComponents().Count);
        }

        [Test]
        public void TestSettingQuantity()
        {
            Order order = _orderBuilder.SetQuantity(1000000.00m).Build();
            Assert.AreEqual(1000000.00m, order.GetQuantity());

            order = _orderBuilder.SetQuantity(5000000).Build();
            Assert.AreEqual(5000000m, order.GetQuantity());
        }

        [Test]
        public void TestNullQuantityClearsQuantity()
        {
            _orderBuilder.SetQuantity(2000000);
            _orderBuilder.SetQuantity(null);
            Order order = _orderBuilder.Build();
            Assert.AreEqual(1, order.GetInternalComponents().Count);
        }

        [Test]
        [ExpectedException("System.ArgumentException", ExpectedMessage =
            "Quantity can not be negative: -5")]
        public void TestNegativeQuantityThrowsException()
        {
            _orderBuilder.SetQuantity(-5);
        }

        [Test]
        public void TestSettingHandlingType()
        {
            Order order = _orderBuilder.SetHandlingType("STREAM").Build();
            Assert.AreEqual("RFS", order.GetHandlingType());

            order = _orderBuilder.SetHandlingType("RFS").Build();
            Assert.AreEqual("RFS", order.GetHandlingType());

            order = _orderBuilder.SetHandlingType("  Quote ").Build();
            Assert.AreEqual("RFQ", order.GetHandlingType());

            order = _orderBuilder.SetHandlingType("  rfq ").Build();
            Assert.AreEqual("RFQ", order.GetHandlingType());

            order = _orderBuilder.SetHandlingType("Automatic").Build();
            Assert.AreEqual("AUTOMATIC", order.GetHandlingType());
        }

        [Test]
        public void TestSettingNullHandlingTypeClearsHandlingType()
        {
            _orderBuilder.SetHandlingType("automatic");
            _orderBuilder.SetHandlingType(null);
            Order order = _orderBuilder.Build();
            Assert.AreEqual(1, order.GetInternalComponents().Count);
        }

        [Test]
        public void TestSettingEmptyHandlingTypeClearsHandlingType()
        {
            _orderBuilder.SetHandlingType("stream");
            _orderBuilder.SetHandlingType("");
            Order order = _orderBuilder.Build();
            Assert.AreEqual(1, order.GetInternalComponents().Count);
        }

        [Test]
        public void TestSettingBlankHandlingTypeClearsHandlingType()
        {
            _orderBuilder.SetHandlingType("quote");
            _orderBuilder.SetHandlingType("    ");
            Order order = _orderBuilder.Build();
            Assert.AreEqual(1, order.GetInternalComponents().Count);
        }

        [Test]
        public void TestSettingReferenceOne()
        {
            Order order = _orderBuilder.SetReferenceOne("reference_one").Build();
            Assert.AreEqual("reference_one", order.GetReference1());

            order = _orderBuilder.SetReferenceOne("  reference_one_with_whitespace ").Build();
            Assert.AreEqual("  reference_one_with_whitespace ", order.GetReference1());
        }

        [Test]
        public void TestSettingNullReferenceOneClearsReferenceOne()
        {
            _orderBuilder.SetReferenceOne("reference_one");
            _orderBuilder.SetReferenceOne(null);
            Order order = _orderBuilder.Build();
            Assert.AreEqual(1, order.GetInternalComponents().Count);
        }

        [Test]
        public void TestSettingEmptyReferenceOneClearsReferenceOne()
        {
            _orderBuilder.SetReferenceOne("reference_one");
            _orderBuilder.SetReferenceOne("");
            Order order = _orderBuilder.Build();
            Assert.AreEqual(1, order.GetInternalComponents().Count);
        }

        [Test]
        public void TestSettingBlankReferenceOneClearsReferenceOne()
        {
            _orderBuilder.SetReferenceOne("reference_one");
            _orderBuilder.SetReferenceOne("   ");
            Order order = _orderBuilder.Build();
            Assert.AreEqual(1, order.GetInternalComponents().Count);
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingReferenceOneWithPipe()
        {
            _orderBuilder.SetReferenceOne("PartA|PartB");
        }

        [Test]
        public void TestSettingReferenceTwo()
        {
            Order order = _orderBuilder.SetReferenceTwo("reference_two").Build();
            Assert.AreEqual("reference_two", order.GetReference2());

            order = _orderBuilder.SetReferenceTwo("  reference_two_with_whitespace ").Build();
            Assert.AreEqual("  reference_two_with_whitespace ", order.GetReference2());
        }

        [Test]
        public void TestSettingNullReferenceTwoClearsReferenceTwo()
        {
            _orderBuilder.SetReferenceTwo("reference_one");
            _orderBuilder.SetReferenceTwo(null);
            Order order = _orderBuilder.Build();
            Assert.AreEqual(1, order.GetInternalComponents().Count);
        }

        [Test]
        public void TestSettingEmptyReferenceTwoClearsReferenceTwo()
        {
            _orderBuilder.SetReferenceTwo("reference_one");
            _orderBuilder.SetReferenceTwo("");
            Order order = _orderBuilder.Build();
            Assert.AreEqual(1, order.GetInternalComponents().Count);
        }

        [Test]
        public void TestSettingBlankReferenceTwoClearsReferenceTwo()
        {
            _orderBuilder.SetReferenceTwo("reference_one");
            _orderBuilder.SetReferenceTwo("   ");
            Order order = _orderBuilder.Build();
            Assert.AreEqual(1, order.GetInternalComponents().Count);
        }

        [Test]
        [ExpectedException("System.ArgumentException", ExpectedMessage = "References can not contain pipes (|)")]
        public void TestSettingReferenceTwoWithPipe()
        {
            _orderBuilder.SetReferenceTwo("PartA|PartB");
        }

        [Test]
        public void TestSettingPrice()
        {
            Order order = _orderBuilder.SetPrice(1000000.00m).Build();
            Assert.AreEqual(1000000.00m, order.GetPrice());

            order = _orderBuilder.SetPrice(5000000).Build();
            Assert.AreEqual(5000000m, order.GetPrice());
        }

        [Test]
        [ExpectedException("System.ArgumentException", ExpectedMessage = "Price can not be negative: -2")]
        public void TestNegativePriceThrowsException()
        {
            _orderBuilder.SetPrice(-2).Build();
        }

        [Test]
        public void TestSettingNullPriceClearsPrice()
        {
            _orderBuilder.SetPrice(1.345m);
            _orderBuilder.SetPrice(null);
            Order order = _orderBuilder.Build();
            Assert.AreEqual(1, order.GetInternalComponents().Count);
        }

        [Test]
        public void TestSettingOrderType()
        {
            Order order = _orderBuilder.SetOrderType("Limit").Build();
            Assert.AreEqual("LIMIT", order.GetOrderType());

            order = _orderBuilder.SetOrderType(" Market  ").Build();
            Assert.AreEqual("MARKET", order.GetOrderType());
        }

        [Test]
        public void TestSettingNullOrderTypeClearsOrderType()
        {
            _orderBuilder.SetOrderType("Limit");
            _orderBuilder.SetOrderType(null);
            Order order = _orderBuilder.Build();
            Assert.AreEqual(1, order.GetInternalComponents().Count);
        }

        [Test]
        public void TestSettingEmptyOrderTypeClearsOrderType()
        {
            _orderBuilder.SetOrderType("Limit");
            _orderBuilder.SetOrderType("");
            Order order = _orderBuilder.Build();
            Assert.AreEqual(1, order.GetInternalComponents().Count);
        }

        [Test]
        public void TestSettingBlankOrderTypeClearsOrderType()
        {
            _orderBuilder.SetOrderType("Limit");
            _orderBuilder.SetOrderType("  ");
            Order order = _orderBuilder.Build();
            Assert.AreEqual(1, order.GetInternalComponents().Count);
        }
    }

    internal class TestOrderBuilderImpl : OrderBuilder<TestOrderBuilderImpl, TestOrderImpl>
    {
        public TestOrderBuilderImpl() : base("TEST")
        {
        }

        public override TestOrderImpl Build()
        {
            return new TestOrderImpl(Components);
        }
    }

    internal class TestOrderImpl : Order
    {
        public TestOrderImpl(Dictionary<string, object> components) : base(components)
        {
        }
    }
}