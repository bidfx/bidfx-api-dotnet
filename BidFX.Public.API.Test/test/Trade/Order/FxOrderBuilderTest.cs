using System;
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
                public void TestCurrencyPairsWithInvalidLengthThrowsException()
        {
ArgumentException exception = Assert.Throws<ArgumentException>(() => _orderBuilder.SetCurrencyPair("GBPUSD2"));
            Assert.AreEqual("CurrencyPair must be in format 'XXXYYY': GBPUSD2", exception.Message);

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
        public void TestInvalidDealTypeThrowsException()
        {
            ArgumentException exception = Assert.Throws<ArgumentException>(() => _orderBuilder.SetDealType("Invalid").Build());
            Assert.AreEqual("Unsupported DealType: Invalid", exception.Message);
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
        public void TestNonThreeLetterCurrencyThrowsException()
        {
            ArgumentException exception = Assert.Throws<ArgumentException>(() => _orderBuilder.SetCurrency("GBPD"));
            Assert.AreEqual("Currency must be in format 'XXX': GBPD", exception.Message);
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
        public void TestSettingNullStrategyParameterNameThrowsException()
        {
            ArgumentException exception = Assert.Throws<ArgumentException>(() => _orderBuilder.SetStrategyParameter(null, "not_null"));
            Assert.AreEqual("Method parameter may not be null", exception.Message);
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
                public void TestSettingEmptyParameterNameThrowsException()
        {
ArgumentException exception = Assert.Throws<ArgumentException>(() => _orderBuilder.SetStrategyParameter("", "value_one"));
            Assert.AreEqual("Method parameter may not be blank", exception.Message);

        }

        [Test]
                public void TestSettingBlankParameterNameThrowsException()
        {
ArgumentException exception = Assert.Throws<ArgumentException>(() => _orderBuilder.SetStrategyParameter("   ", "value_two"));
            Assert.AreEqual("Method parameter may not be blank", exception.Message);

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
                public void TestSettingInvalidSettlementDateThrowsException()
        {
ArgumentException exception = Assert.Throws<ArgumentException>(() => _orderBuilder.SetSettlementDate("291"));
            Assert.AreEqual("SettlementDate was not in valid format (YYYY-MM-DD): 291", exception.Message);

        }

        [Test]
                public void TestSettingSettlementDateWithBadCharactersThrowsException()
        {
ArgumentException exception = Assert.Throws<ArgumentException>(() => _orderBuilder.SetSettlementDate("yyyy-mm-dd"));
            Assert.AreEqual("SettlementDate was not in valid format (YYYY-MM-DD): yyyy-mm-dd", exception.Message);

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
        public void TestSettingInvalidFixingDateThrowsException()
        {
            ArgumentException exception = Assert.Throws<ArgumentException>(() => _orderBuilder.SetFixingDate("291"));
            Assert.AreEqual("FixingDate was not in valid format (YYYY-MM-DD): 291", exception.Message);
        }

        [Test]
                public void TestSettingFixingDateWithBadCharactersThrowsException()
        {
ArgumentException exception = Assert.Throws<ArgumentException>(() => _orderBuilder.SetFixingDate("yyyy-mm-dd"));
            Assert.AreEqual("FixingDate was not in valid format (YYYY-MM-DD): yyyy-mm-dd", exception.Message);

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
        public void TestSettingInvalidFarSettlementDateThrowsException()
        {
            ArgumentException exception = Assert.Throws<ArgumentException>(() => _orderBuilder.SetFarSettlementDate("291"));
            Assert.AreEqual("FarSettlementDate was not in valid format (YYYY-MM-DD): 291", exception.Message);
        }

        [Test]
        public void TestSettingFarSettlementDateWithBadCharactersThrowsException()
        {
            ArgumentException exception = Assert.Throws<ArgumentException>(() => _orderBuilder.SetFarSettlementDate("yyyy-mm-dd"));
            Assert.AreEqual("FarSettlementDate was not in valid format (YYYY-MM-DD): yyyy-mm-dd", exception.Message);
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
        public void TestSettingInvalidFarFixingDateThrowsException()
        {
            ArgumentException exception = Assert.Throws<ArgumentException>(() => _orderBuilder.SetFarFixingDate("291"));
            Assert.AreEqual("FarFixingDate was not in valid format (YYYY-MM-DD): 291", exception.Message);
        }

        [Test]
                public void TestSettingFarFixingDateWithBadCharactersThrowsException()
        {
ArgumentException exception = Assert.Throws<ArgumentException>(() => _orderBuilder.SetFarFixingDate("yyyy-mm-dd"));
            Assert.AreEqual("FarFixingDate was not in valid format (YYYY-MM-DD): yyyy-mm-dd", exception.Message);

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
                public void TestNonThreeLetterFarCurrencyThrowsException()
        {
ArgumentException exception = Assert.Throws<ArgumentException>(() => _orderBuilder.SetFarCurrency("GBPD"));
            Assert.AreEqual("FarCurrency must be in format 'AAA': GBPD", exception.Message);

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
                public void TestNonNumericOrPeridFarQuantityThrowsException()
        {
ArgumentException exception = Assert.Throws<ArgumentException>(() => _orderBuilder.SetFarQuantity(-2700).Build());
            Assert.AreEqual("FarQuantity can not be negative: -2700", exception.Message);

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
        public void TestSettingInvalidFarSideThrowsException()
        {
            ArgumentException exception = Assert.Throws<ArgumentException>(() => _orderBuilder.SetFarSide("Invalid"));
            Assert.AreEqual("Side must be either 'BUY' or 'SELL': Invalid", exception.Message);
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