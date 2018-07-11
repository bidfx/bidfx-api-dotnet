using NUnit.Framework;

namespace BidFX.Public.API.Trade.Order
{
    public class FxOrderBuilderTest
    {
        private FxOrderBuilder _orderBuilder;

        [SetUp]
        public void Before()
        {
            _orderBuilder = new FxOrderBuilder();
        }

        [Test]
        public void TestAssetClassIsSetInOrder()
        {
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual("FX", fxOrder.GetAssetClass());
        }

        [Test]
        public void TestSettingCurrencyPair()
        {
            FxOrder fxOrder = _orderBuilder.SetCurrencyPair("GBPUSD").Build();
            Assert.AreEqual("GBPUSD", fxOrder.GetCurrencyPair());

            fxOrder = _orderBuilder.SetCurrencyPair("  EURAUD ").Build();
            Assert.AreEqual("EURAUD", fxOrder.GetCurrencyPair());
        }

        [Test]
        [ExpectedException("System.ArgumentException", ExpectedMessage = "CurrencyPair must be in format 'XXXYYY': GBPUSD2")]
        public void TestCurrencyPairsWithInvalidLengthThrowsException()
        {
            _orderBuilder.SetCurrencyPair("GBPUSD2");
        }

        [Test]
        public void TestEmptyCurrencyPairClearsCurrencyPair()
        {
            _orderBuilder.SetCurrencyPair("USDJPY");
            _orderBuilder.SetCurrencyPair("");
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
            Assert.IsNull(fxOrder.GetCurrencyPair());
        }

        [Test]
        public void TestBlankCurrencyPairClearsCurrencyPair()
        {
            _orderBuilder.SetCurrencyPair("USDJPY");
            _orderBuilder.SetCurrencyPair("   ");
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
            Assert.IsNull(fxOrder.GetCurrencyPair());
        }

        [Test]
        public void TestNullCurrencyPairClearsCurrencyPair()
        {
            _orderBuilder.SetCurrencyPair("EURUSD");
            _orderBuilder.SetCurrencyPair(null);
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
            Assert.IsNull(fxOrder.GetCurrencyPair());
        }

        [Test]
        public void TestValidDealTypeSetsDealType()
        {
            FxOrder fxOrder = _orderBuilder.SetDealType("spot").Build();
            Assert.AreEqual("SPOT", fxOrder.GetDealType());

            fxOrder = _orderBuilder.SetDealType("ndf").Build();
            Assert.AreEqual("NDF", fxOrder.GetDealType());

            fxOrder = _orderBuilder.SetDealType("outright").Build();
            Assert.AreEqual("FWD", fxOrder.GetDealType());

            fxOrder = _orderBuilder.SetDealType("forward").Build();
            Assert.AreEqual("FWD", fxOrder.GetDealType());

            fxOrder = _orderBuilder.SetDealType("swap").Build();
            Assert.AreEqual("SWAP", fxOrder.GetDealType());

            fxOrder = _orderBuilder.SetDealType("nds").Build();
            Assert.AreEqual("NDS", fxOrder.GetDealType());
        }

        [Test]
        public void TestNullDealTypeClearsDealType()
        {
            _orderBuilder.SetDealType("Swap");
            _orderBuilder.SetDealType(null);
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        public void TestEmptyDealTypeClearsDealType()
        {
            _orderBuilder.SetDealType("NDS");
            _orderBuilder.SetDealType("");
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        public void TestBlankDealTypeClearsDealType()
        {
            _orderBuilder.SetDealType("Spot");
            _orderBuilder.SetDealType("   ");
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        [ExpectedException("System.ArgumentException", ExpectedMessage = "Unsupported DealType: Invalid")]
        public void TestInvalidDealTypeThrowsException()
        {
            FxOrder fxOrder = _orderBuilder.SetDealType("Invalid").Build();
            Assert.Null(fxOrder.GetDealType());
        }

        [Test]
        public void TestSettingCurrency()
        {
            FxOrder fxOrder = _orderBuilder.SetCurrency("GBP").Build();
            Assert.AreEqual("GBP", fxOrder.GetCurrency());

            fxOrder = _orderBuilder.SetCurrency("  EUR ").Build();
            Assert.AreEqual("EUR", fxOrder.GetCurrency());
        }

        [Test]
        public void TestNullCurrencyClearsCurrency()
        {
            _orderBuilder.SetCurrency("GBP");
            _orderBuilder.SetCurrency(null);
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
            Assert.IsNull(fxOrder.GetCurrency());
        }

        [Test]
        public void TestEmptyCurrencyClearsCurrency()
        {
            _orderBuilder.SetCurrency("USD");
            _orderBuilder.SetCurrency("");
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
            Assert.IsNull(fxOrder.GetCurrency());
        }

        [Test]
        public void TestBlankCurrencyClearsCurrency()
        {
            _orderBuilder.SetCurrency("EUR");
            _orderBuilder.SetCurrency("   ");
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
            Assert.IsNull(fxOrder.GetCurrency());
        }

        [Test]
        [ExpectedException("System.ArgumentException", ExpectedMessage= "Currency must be in format 'XXX': GBPD" )]
        public void TestNonThreeLetterCurrencyThrowsException()
        {
            _orderBuilder.SetCurrency("GBPD");
        }

        [Test]
        public void TestSettingTenor()
        {
            FxOrder fxOrder = _orderBuilder.SetTenor("2M").Build();
            Assert.AreEqual("2M", fxOrder.GetTenor());

            fxOrder = _orderBuilder.SetTenor(" 1Y  ").Build();
            Assert.AreEqual("1Y", fxOrder.GetTenor());
        }

        [Test]
        public void TestNullTenorClearsTenor()
        {
            _orderBuilder.SetTenor("6M");
            _orderBuilder.SetTenor(null);
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        public void TestEmptyTenorClearsTenor()
        {
            _orderBuilder.SetTenor("4M");
            _orderBuilder.SetTenor("");
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        public void TestBlankTenorClearsTenor()
        {
            _orderBuilder.SetTenor("BD");
            _orderBuilder.SetTenor("     ");
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        public void TestSettingStrategyParameters()
        {
            FxOrder fxOrder = _orderBuilder.SetStrategyParameter("s_param_one", "s_value_one")
                .SetStrategyParameter("s_param_two", "s_value_two")
                .Build();
            Assert.AreEqual("s_value_one", fxOrder.GetStrategyParameter("s_param_one"));
            Assert.AreEqual("s_value_two", fxOrder.GetStrategyParameter("s_param_two"));
        }

        [Test]
        [ExpectedException("System.ArgumentException" , ExpectedMessage = "Method parameter may not be null")]
        public void TestSettingNullStrategyParameterNameThrowsException()
        {
            _orderBuilder.SetStrategyParameter(null, "not_null");
        }

        [Test]
        public void TestSettingNullStrategyParameterValueClearsParameter()
        {
            _orderBuilder.SetStrategyParameter("not_null", "not_null_either");
            _orderBuilder.SetStrategyParameter("not_null", null);
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        public void TestSettingSameStrategyParameterOverwritesOld()
        {
            FxOrder fxOrder = _orderBuilder.SetStrategyParameter("parameter_one", "value_one")
                .SetStrategyParameter("parameter_one", "value_two")
                .Build();
            Assert.AreEqual("value_two", fxOrder.GetStrategyParameter("parameter_one"));
        }

        [Test]
        [ExpectedException("System.ArgumentException", ExpectedMessage = "Method parameter may not be blank")]
        public void TestSettingEmptyParameterNameThrowsException()
        {
            _orderBuilder.SetStrategyParameter("", "value_one");
        }

        [Test]
        [ExpectedException("System.ArgumentException", ExpectedMessage = "Method parameter may not be blank")]
        public void TestSettingBlankParameterNameThrowsException()
        {
            _orderBuilder.SetStrategyParameter("   ", "value_two");
        }

        [Test]
        public void TestSettingEmptyParameterValueClearsParameter()
        {
            _orderBuilder.SetStrategyParameter("parameter_one", "not_empty");
            _orderBuilder.SetStrategyParameter("parameter_one", "");
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        public void TestSettingBlankParameterValueClearsParameter()
        {
            _orderBuilder.SetStrategyParameter("parameter_one", "not_empty");
            _orderBuilder.SetStrategyParameter("parameter_one", "   ");
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        public void TestSettingSettlementDate()
        {
            FxOrder fxOrder = _orderBuilder.SetSettlementDate("20180207").Build();
            Assert.AreEqual("2018-02-07", fxOrder.GetSettlementDate());

            fxOrder = _orderBuilder.SetSettlementDate(" 2018-05-21  ").Build();
            Assert.AreEqual("2018-05-21", fxOrder.GetSettlementDate());

            fxOrder = _orderBuilder.SetSettlementDate("  20180319 ").Build();
            Assert.AreEqual("2018-03-19", fxOrder.GetSettlementDate());

            fxOrder = _orderBuilder.SetSettlementDate("2018-12-12").Build();
            Assert.AreEqual("2018-12-12", fxOrder.GetSettlementDate());

            fxOrder = _orderBuilder.SetSettlementDate("2018-1-5").Build();
            Assert.AreEqual("2018-01-05", fxOrder.GetSettlementDate());
        }

        [Test]
        public void TestSettingNullSettlementDateClearsSettlementDate()
        {
            _orderBuilder.SetSettlementDate("2018-02-23");
            _orderBuilder.SetSettlementDate(null);
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        public void TestSettingEmptySettlementDateClearsSettlementDate()
        {
            _orderBuilder.SetSettlementDate("2018-02-23");
            _orderBuilder.SetSettlementDate("");
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        public void TestSettingBlankSettlementDateClearsSettlementDate()
        {
            _orderBuilder.SetSettlementDate("2018-02-23");
            _orderBuilder.SetSettlementDate("   ");
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        [ExpectedException("System.ArgumentException", ExpectedMessage = "SettlementDate was not in valid format (YYYY-MM-DD): 291")]
        public void TestSettingInvalidSettlementDateThrowsException()
        {
            _orderBuilder.SetSettlementDate("291");
        }

        [Test]
        [ExpectedException("System.ArgumentException", ExpectedMessage = "SettlementDate was not in valid format (YYYY-MM-DD): yyyy-mm-dd")]
        public void TestSettingSettlementDateWithBadCharactersThrowsException()
        {
            _orderBuilder.SetSettlementDate("yyyy-mm-dd");
        }

        [Test]
        public void TestSettingFixingDate()
        {
            FxOrder fxOrder = _orderBuilder.SetFixingDate("20180207").Build();
            Assert.AreEqual("2018-02-07", fxOrder.GetFixingDate());

            fxOrder = _orderBuilder.SetFixingDate(" 2018-05-21  ").Build();
            Assert.AreEqual("2018-05-21", fxOrder.GetFixingDate());

            fxOrder = _orderBuilder.SetFixingDate("  20180319 ").Build();
            Assert.AreEqual("2018-03-19", fxOrder.GetFixingDate());

            fxOrder = _orderBuilder.SetFixingDate("2018-12-12").Build();
            Assert.AreEqual("2018-12-12", fxOrder.GetFixingDate());

            fxOrder = _orderBuilder.SetFixingDate("2018-1-5").Build();
            Assert.AreEqual("2018-01-05", fxOrder.GetFixingDate());
        }

        [Test]
        public void TestSettingNullFixingDateClearsFixingDate()
        {
            _orderBuilder.SetFixingDate("2018-12-12");
            _orderBuilder.SetFixingDate(null);
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        public void TestSettingEmptyFixingDateClearsFixingDate()
        {
            _orderBuilder.SetFixingDate("2018-12-12");
            _orderBuilder.SetFixingDate("");
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        public void TestSettingBlankFixingDateClearsFixingDate()
        {
            _orderBuilder.SetFixingDate("2018-12-12");
            _orderBuilder.SetFixingDate("  ");
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        [ExpectedException("System.ArgumentException" , ExpectedMessage = "FixingDate was not in valid format (YYYY-MM-DD): 291")]
        public void TestSettingInvalidFixingDateThrowsException()
        {
            _orderBuilder.SetFixingDate("291");
        }

        [Test]
        [ExpectedException("System.ArgumentException", ExpectedMessage = "FixingDate was not in valid format (YYYY-MM-DD): yyyy-mm-dd")]
        public void TestSettingFixingDateWithBadCharactersThrowsException()
        {
            _orderBuilder.SetFixingDate("yyyy-mm-dd");
        }

        [Test]
        public void TestSettingFarSettlementDate()
        {
            FxOrder fxOrder = _orderBuilder.SetFarSettlementDate("20180207").Build();
            Assert.AreEqual("2018-02-07", fxOrder.GetFarSettlementDate());

            fxOrder = _orderBuilder.SetFarSettlementDate(" 2018-05-21  ").Build();
            Assert.AreEqual("2018-05-21", fxOrder.GetFarSettlementDate());

            fxOrder = _orderBuilder.SetFarSettlementDate("  20180319 ").Build();
            Assert.AreEqual("2018-03-19", fxOrder.GetFarSettlementDate());

            fxOrder = _orderBuilder.SetFarSettlementDate("2018-12-12").Build();
            Assert.AreEqual("2018-12-12", fxOrder.GetFarSettlementDate());

            fxOrder = _orderBuilder.SetFarSettlementDate("2018-1-5").Build();
            Assert.AreEqual("2018-01-05", fxOrder.GetFarSettlementDate());
        }

        [Test]
        public void TestSettingNullFarSettlementDateClearsFarSettlementDate()
        {
            _orderBuilder.SetFarSettlementDate("2018-12-12");
            _orderBuilder.SetFarSettlementDate(null);
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        public void TestSettingEmptyFarSettlementDateClearsFarSettlementDate()
        {
            _orderBuilder.SetFarSettlementDate("2018-12-12");
            _orderBuilder.SetFarSettlementDate("");
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        public void TestSettingBlankFarSettlementDateClearsFarSettlementDate()
        {
            _orderBuilder.SetFarSettlementDate("2018-12-12");
            _orderBuilder.SetFarSettlementDate("   ");
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        [ExpectedException("System.ArgumentException" , ExpectedMessage = "FarSettlementDate was not in valid format (YYYY-MM-DD): 291")]
        public void TestSettingInvalidFarSettlementDateThrowsException()
        {
            _orderBuilder.SetFarSettlementDate("291");
        }

        [Test]
        [ExpectedException("System.ArgumentException" , ExpectedMessage = "FarSettlementDate was not in valid format (YYYY-MM-DD): yyyy-mm-dd")]
        public void TestSettingFarSettlementDateWithBadCharactersThrowsException()
        {
            _orderBuilder.SetFarSettlementDate("yyyy-mm-dd");
        }

        [Test]
        public void TestSettingFarFixingDate()
        {
            FxOrder fxOrder = _orderBuilder.SetFarFixingDate("20180207").Build();
            Assert.AreEqual("2018-02-07", fxOrder.GetFarFixingDate());

            fxOrder = _orderBuilder.SetFarFixingDate(" 2018-05-21  ").Build();
            Assert.AreEqual("2018-05-21", fxOrder.GetFarFixingDate());

            fxOrder = _orderBuilder.SetFarFixingDate("  20180319 ").Build();
            Assert.AreEqual("2018-03-19", fxOrder.GetFarFixingDate());

            fxOrder = _orderBuilder.SetFarFixingDate("2018-12-12").Build();
            Assert.AreEqual("2018-12-12", fxOrder.GetFarFixingDate());

            fxOrder = _orderBuilder.SetFarFixingDate("2018-1-5").Build();
            Assert.AreEqual("2018-01-05", fxOrder.GetFarFixingDate());
        }

        [Test]
        public void TestSettingNullFarFixingDateClearsFarFixingDate()
        {
            _orderBuilder.SetFarFixingDate("2018-12-12");
            _orderBuilder.SetFarFixingDate(null);
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        public void TestSettingEmptyFarFixingDateClearsFarFixingDate()
        {
            _orderBuilder.SetFarFixingDate("2018-12-12");
            _orderBuilder.SetFarFixingDate("");
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        public void TestSettingBlankFarFixingDateClearsFarFixingDate()
        {
            _orderBuilder.SetFarFixingDate("2018-12-12");
            _orderBuilder.SetFarFixingDate("   ");
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        [ExpectedException("System.ArgumentException" , ExpectedMessage = "FarFixingDate was not in valid format (YYYY-MM-DD): 291")]
        public void TestSettingInvalidFarFixingDateThrowsException()
        {
            _orderBuilder.SetFarFixingDate("291");
        }

        [Test]
        [ExpectedException("System.ArgumentException", ExpectedMessage = "FarFixingDate was not in valid format (YYYY-MM-DD): yyyy-mm-dd")]
        public void TestSettingFarFixingDateWithBadCharactersThrowsException()
        {
            _orderBuilder.SetFarFixingDate("yyyy-mm-dd");
        }

        [Test]
        public void TestSettingFarTenor()
        {
            FxOrder fxOrder = _orderBuilder.SetFarTenor("2M").Build();
            Assert.AreEqual("2M", fxOrder.GetFarTenor());

            fxOrder = _orderBuilder.SetFarTenor(" 1Y  ").Build();
            Assert.AreEqual("1Y", fxOrder.GetFarTenor());
        }

        [Test]
        public void TestSettingNullFarTenorClearsFarTenor()
        {
            _orderBuilder.SetFarTenor("1M");
            _orderBuilder.SetFarTenor(null);
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        public void TestSettingEmptyFarTenorClearsFarTenor()
        {
            _orderBuilder.SetFarTenor("1M");
            _orderBuilder.SetFarTenor("");
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        public void TestSettingBlankFarTenorClearsFarTenor()
        {
            _orderBuilder.SetFarTenor("1M");
            _orderBuilder.SetFarTenor("  ");
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        public void TestSettingFarCurrency()
        {
            FxOrder fxOrder = _orderBuilder.SetFarCurrency("GBP").Build();
            Assert.AreEqual("GBP", fxOrder.GetFarCurrency());

            fxOrder = _orderBuilder.SetFarCurrency("  EUR ").Build();
            Assert.AreEqual("EUR", fxOrder.GetFarCurrency());
        }

        [Test]
        public void TestSettingNullFarCurrencyClearsFarCurrency()
        {
            _orderBuilder.SetFarCurrency("GBP");
            _orderBuilder.SetFarCurrency(null);
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        public void TestSettingEmptyFarCurrencyClearsFarCurrency()
        {
            _orderBuilder.SetFarCurrency("GBP");
            _orderBuilder.SetFarCurrency("");
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        public void TestSettingBlankFarCurrencyClearsFarCurrency()
        {
            _orderBuilder.SetFarCurrency("GBP");
            _orderBuilder.SetFarCurrency("    ");
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        [ExpectedException("System.ArgumentException", ExpectedMessage = "FarCurrency must be in format 'AAA': GBPD")]
        public void TestNonThreeLetterFarCurrencyThrowsException()
        {
            _orderBuilder.SetFarCurrency("GBPD");
        }

        [Test]
        public void TestSettingFarQuantity()
        {
            FxOrder fxOrder = _orderBuilder.SetFarQuantity(1000000.00m).Build();
            Assert.AreEqual(1000000.00m, fxOrder.GetFarQuantity());

            fxOrder = _orderBuilder.SetFarQuantity(5000000).Build();
            Assert.AreEqual(5000000m, fxOrder.GetFarQuantity());
        }

        [Test]
        [ExpectedException("System.ArgumentException", ExpectedMessage = "FarQuantity can not be negative: -2700")]
        public void TestNonNumericOrPeridFarQuantityThrowsException()
        {
            _orderBuilder.SetFarQuantity(-2700).Build();
        }

        [Test]
        public void TestSettingNullFarQuantityClearsFarQuantity()
        {
            _orderBuilder.SetFarQuantity(1000000.00m);
            _orderBuilder.SetFarQuantity(null);
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        public void TestSettingAllocationTemplate()
        {
            FxOrder fxOrder = _orderBuilder.SetAllocationTemplate("alloc_template").Build();
            Assert.AreEqual("alloc_template", fxOrder.GetAllocationTemplate());

            fxOrder = _orderBuilder.SetAllocationTemplate(" alloc_template_2  ").Build();
            Assert.AreEqual("alloc_template_2", fxOrder.GetAllocationTemplate());
        }

        [Test]
        public void TestSettingNullAllocationTemplateClearsAllocationTemplate()
        {
            _orderBuilder.SetAllocationTemplate("alloc_template");
            _orderBuilder.SetAllocationTemplate(null);
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        public void TestSettingEmptyAllocationTemplateClearsAllocationTemplate()
        {
            _orderBuilder.SetAllocationTemplate("alloc_template");
            _orderBuilder.SetAllocationTemplate("");
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }

        [Test]
        public void TestSettingBlankAllocationTemplateClearsAllocationTemplate()
        {
            _orderBuilder.SetAllocationTemplate("alloc_template");
            _orderBuilder.SetAllocationTemplate("   ");
            FxOrder fxOrder = _orderBuilder.Build();
            Assert.AreEqual(1, fxOrder.GetJsonMap().Count);
        }
        
        [Test]
        public void TestSettingFarSide()
        {
            FxOrder order = _orderBuilder.SetFarSide("buy").Build();
            Assert.AreEqual("BUY", order.GetFarSide());

            order = _orderBuilder.SetFarSide("sell").Build();
            Assert.AreEqual("SELL", order.GetFarSide());

            order = _orderBuilder.SetFarSide("  buy").Build();
            Assert.AreEqual("BUY", order.GetFarSide());

            order = _orderBuilder.SetFarSide(" sell  ").Build();
            Assert.AreEqual("SELL", order.GetFarSide());
        }

        [Test]
        [ExpectedException("System.ArgumentException", ExpectedMessage =
            "Side must be either 'BUY' or 'SELL': Invalid")]
        public void TestSettingInvalidFarSideThrowsException()
        {
            _orderBuilder.SetFarSide("Invalid");
        }

        [Test]
        public void TestSettingNullFarSideClearsSide()
        {
            _orderBuilder.SetFarSide("Buy");
            _orderBuilder.SetFarSide(null);
            FxOrder order = _orderBuilder.Build();
            Assert.AreEqual(1, order.GetJsonMap().Count);
            Assert.IsNull(order.GetFarSide());
        }

        [Test]
        public void TestSettingEmptyFarSideClearsSide()
        {
            _orderBuilder.SetFarSide("Sell");
            _orderBuilder.SetFarSide("");
            FxOrder order = _orderBuilder.Build();
            Assert.AreEqual(1, order.GetJsonMap().Count);
            Assert.IsNull(order.GetFarSide());
        }

        [Test]
        public void TestSettingBlankFarSideClearsSide()
        {
            _orderBuilder.SetFarSide("Sell");
            _orderBuilder.SetFarSide("     ");
            FxOrder order = _orderBuilder.Build();
            Assert.AreEqual(1, order.GetJsonMap().Count);
            Assert.IsNull(order.GetFarSide());
        }
    }
}