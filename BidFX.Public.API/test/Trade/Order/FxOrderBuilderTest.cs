using System;
using BidFX.Public.API.Price;
using NUnit.Framework;

namespace BidFX.Public.API.Trade.Order
{
    public class FxOrderBuilderTest
    {
        private FxOrderBuilder _orderBuilder;

        [SetUp]
        private void Before()
        {
            _orderBuilder = new FxOrderBuilder();
        }

        [Test]
        [ExpectedException(typeof(IllegalStateException))]
        public void TestBuiltFxOrderIsImmutable()
        {
            var fxOrder = _orderBuilder.Build();
            fxOrder.SetCurrency("EURUSD");
        }
        
        [Test]
        public void TestSettingCurrencyPair()
        {
            var fxOrder = _orderBuilder.SetCurrencyPair("GBPUSD").Build();
            Assert.AreEqual("GBPUSD", fxOrder.GetCurrencyPair());
            
            fxOrder = _orderBuilder.SetCurrencyPair("  EURAUD ").Build();
            Assert.AreEqual("EURAUD", fxOrder.GetCurrencyPair());
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestCurrencyPairsWithInvalidLengthThrowsException()
        {
            _orderBuilder.SetCurrencyPair("GBPUSD2");
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestNullCurrencyPairsThrowsException()
        {
            _orderBuilder.SetCurrencyPair(null);
        }

        [Test]
        public void TestValidDealTypeSetsDealType()
        {
            var fxOrder = _orderBuilder.SetDealType("spot").Build();
            Assert.AreEqual("Spot", fxOrder.GetDealType()); //TODO: Check this is the correct case

            fxOrder = _orderBuilder.SetDealType("ndf").Build();
            Assert.AreEqual("NDF", fxOrder.GetDealType()); //TODO: Check this is the correct case

            fxOrder = _orderBuilder.SetDealType("forward").Build();
            Assert.AreEqual("Forward", fxOrder.GetDealType()); //TODO: Check this is the correct case/word

            fxOrder = _orderBuilder.SetDealType("swap").Build();
            Assert.AreEqual("Swap", fxOrder.GetDealType()); //TODO: Check this is the correct case

            fxOrder = _orderBuilder.SetDealType("nds").Build();
            Assert.AreEqual("NDS", fxOrder.GetDealType()); //TODO: Check this is the correct case
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestNullDealTypeThrowsException()
        {
            _orderBuilder.SetDealType(null);
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestEmptyDealTypeThrowsException()
        {
            _orderBuilder.SetDealType("");
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestBlankDealTypeThrowsException()
        {
            _orderBuilder.SetDealType("   ");
        }

        [Test]
        [ExpectedException("System.ArgumentException", ExpectedMessage = "Unsupported DealType")]
        public void TestInvalidDealTypeThrowsException()
        {
           var fxOrder = _orderBuilder.SetDealType("Invalid").Build();
           Assert.Null(fxOrder.GetDealType());
        }

        [Test]
        public void TestSettingCurrency()
        {
            var fxOrder = _orderBuilder.SetCurrency("GBP").Build();
            Assert.AreEqual("GBP", fxOrder.GetCurrency());

            fxOrder = _orderBuilder.SetCurrency("  EUR ").Build();
            Assert.AreEqual("EUR", fxOrder.GetCurrency());
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestNullCurrencyThrowsException()
        {
            _orderBuilder.SetCurrency(null);
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestEmptyCurrencyThrowsException()
        {
            _orderBuilder.SetCurrency("");
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestBlankCurrencyThrowsException()
        {
            _orderBuilder.SetCurrency("   ");
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestNonThreeLetterCurrencyThrowsException()
        {
            _orderBuilder.SetCurrency("GBPD");
        }

        [Test]
        public void TestSettingExecutingAccount()
        {
            var fxOrder = _orderBuilder.SetAccount("FX_ACCT").Build();
            Assert.AreEqual("FX_ACCT", fxOrder.GetAccount());

            fxOrder = _orderBuilder.SetAccount("  AN_ACCOUNT ").Build();
            Assert.AreEqual("AN_ACCOUNT", fxOrder.GetAccount());
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestNullAccountThrowsException()
        {
            _orderBuilder.SetAccount(null);
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestEmptyAccountThrowsException()
        {
            _orderBuilder.SetAccount("");
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestBlankAccountThrowsException()
        {
            _orderBuilder.SetAccount("   ");
        }

        [Test]
        public void TestSettingSide()
        {
            var fxOrder = _orderBuilder.SetSide("buy").Build();
            Assert.AreEqual("Buy", fxOrder.GetSide()); //TODO: Check this is the correct case
            
            fxOrder = _orderBuilder.SetSide("sell").Build();
            Assert.AreEqual("Sell", fxOrder.GetSide());

            fxOrder = _orderBuilder.SetSide("  buy").Build();
            Assert.AreEqual("Buy", fxOrder.GetSide());
            
            fxOrder = _orderBuilder.SetSide(" sell  ").Build();
            Assert.AreEqual("Sell", fxOrder.GetSide());
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingInvalidSideThrowsException()
        {
            _orderBuilder.SetSide("Invalid");
        }

        [Test]
        [ExpectedException("System.ArgumentExeception")]
        public void TestSettingNullSideThrowsException()
        {
            _orderBuilder.SetSide(null);
        }
        
        [Test]
        [ExpectedException("System.ArgumentExeception")]
        public void TestSettingEmptySideThrowsException()
        {
            _orderBuilder.SetSide("");
        }
        
        [Test]
        [ExpectedException("System.ArgumentExeception")]
        public void TestSettingBlankSideThrowsException()
        {
            _orderBuilder.SetSide("     ");
        }

        [Test]
        public void TestSettingQuantity()
        {
            var fxOrder = _orderBuilder.SetQuantity("1000000.00").Build();
            Assert.AreEqual("1000000.00", fxOrder);

            fxOrder = _orderBuilder.SetQuantity("5000000").Build();
            Assert.AreEqual("5000000", fxOrder.GetQuantity());
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestNonNumericOrPeridQuantityThrowsException()
        {
            _orderBuilder.SetQuantity("27281P.00").Build();
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestNullQuantityThrowsException()
        {
            _orderBuilder.SetQuantity(null);
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestEmptyQuantityThrowsException()
        {
            _orderBuilder.SetQuantity("");
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestBlankQuantityThrowsException()
        {
            _orderBuilder.SetQuantity("     ");
        }

        [Test]
        public void TestSettingTenor()
        {
            var fxOrder = _orderBuilder.SetTenor("2M").Build();
            Assert.AreEqual("2M", fxOrder.GetTenor());
            
            fxOrder = _orderBuilder.SetTenor(" 1Y  ").Build();
            Assert.AreEqual("1Y", fxOrder.GetTenor());
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestNullTenorThrowsException()
        {
            _orderBuilder.SetTenor(null);
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestEmptyTenorThrowsException()
        {
            _orderBuilder.SetTenor("");
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestBlankTenorThrowsException()
        {
            _orderBuilder.SetTenor("     ");
        }

        [Test]
        public void TestSettingStrategyParameters()
        {
            var fxOrder = _orderBuilder.SetStrategyParameter("s_param_one", "s_value_one")
                .SetStrategyParameter("s_param_two", "s_value_two")
                .Build();
            var strategyParams = fxOrder.GetStrategyParameters();
            Assert.AreEqual("s_value_one", strategyParams["s_param_one"]);
            Assert.AreEqual("s_value_two", strategyParams["s_param_two"]);
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingNullStrategyParameterNameThrowsException()
        {
            _orderBuilder.SetStrategyParameter(null, "not_null");
        }
        
        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingNullStrategyParameterValueThrowsException()
        {
            _orderBuilder.SetStrategyParameter("not_null", null);
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingNullStrategyParametersAfterSettingAValidThrowsException()
        {
            _orderBuilder.SetStrategyParameter("not_null_key", "not_null_vale");
            _orderBuilder.SetStrategyParameter("another_not_null_key", null);
        }

        [Test]
        public void TestSettingSameStrategyParameterOverwritesOld()
        {
            var fxOrder = _orderBuilder.SetStrategyParameter("parameter_one", "value_one")
                .SetStrategyParameter("parameter_one", "value_two")
                .Build();
            Assert.AreEqual(1, fxOrder.GetStrategyParameters().Count);
            Assert.AreEqual("value_two", fxOrder.GetStrategyParameters()["parameter_one"]);
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingEmptyParameterNameThrowsException()
        {
            _orderBuilder.SetStrategyParameter("", "value_one");
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingBlankParameterNameThrowsException()
        {
            _orderBuilder.SetStrategyParameter("   ", "value_two");
        }

        [Test]
        public void TestSettingEmptyParameterValueIsAllowed()
        {
            var fxOrder = _orderBuilder.SetStrategyParameter("parameter_one", "").Build();
            Assert.AreEqual("", fxOrder.GetStrategyParameters()["parameter_one"]);
        }

        [Test]
        public void TestSettingBlankParameterTrimsToEmptyString()
        {
            var fxOrder = _orderBuilder.SetStrategyParameter("parameter_one", "    ").Build();
            Assert.AreEqual("", fxOrder.GetStrategyParameters()["parameter_one"]);
        }


        [Test]
        public void TestSettingExecutingVenue()
        {
            var fxOrder = _orderBuilder.SetExecutingVenue("executing_venue_abc").Build();
            Assert.AreEqual("executing_venue_abc", fxOrder.GetExecutingVenue());

            fxOrder = _orderBuilder.SetExecutingVenue("   executing_venue_def ").Build();
            Assert.AreEqual("executing_venue_def", fxOrder.GetExecutingVenue());
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingNullExecutingVenueThrowsException()
        {
            _orderBuilder.SetExecutingVenue(null);
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingEmptyExecutingValueThrowsException()
        {
            _orderBuilder.SetExecutingVenue("");
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingBlankExecutingValueThrowsException()
        {
            _orderBuilder.SetExecutingVenue("   ");
        }

        [Test]
        public void TestSettingHandlingType() //TODO: Do we restrict to "stream", "quote", and "automatic" only?
        {
            var fxOrder = _orderBuilder.SetHandlingType("stream").Build();
            Assert.AreEqual("stream", fxOrder.GetHandlingType());

            fxOrder = _orderBuilder.SetHandlingType("  quote ").Build();
            Assert.AreEqual("quote", fxOrder.GetHandlingType());
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingNullHandlingTypeThrowsException()
        {
            _orderBuilder.SetHandlingType(null);
        }
        
        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingEmptyHandlingTypeThrowsException()
        {
            _orderBuilder.SetHandlingType("");
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingBlankHandlingTypeThrowsException()
        {
            _orderBuilder.SetHandlingType("    ");
        }

        [Test]
        public void TestSettingAccount()
        {
            var fxOrder = _orderBuilder.SetAccount("FX_ACCT").Build();
            Assert.AreEqual("FX_ACCT", fxOrder.GetAccount());

            fxOrder = _orderBuilder.SetAccount("  ANOTHER_ACCOUNT ").Build();
            Assert.AreEqual("ANOTHER_ACCOUNT", fxOrder.GetAccount());
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingNullAccountThrowsException()
        {
            _orderBuilder.SetAccount(null);
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingEmptyAccountThrowsException()
        {
            _orderBuilder.SetAccount("");
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingBlankAccountThrowsException()
        {
            _orderBuilder.SetAccount("   ");
        }

        [Test]
        public void TestSettingReferences()
        {
            var fxOrder = _orderBuilder.SetReference("reference_one", "reference_two").Build();
            Assert.AreEqual("reference_one", fxOrder.GetReference1());
            Assert.AreEqual("reference_two", fxOrder.GetReference2());

            //TODO: Check if references will maintain leading and trailing whitespace
            fxOrder = _orderBuilder.SetReference("  reference_three ", " reference_four ").Build();
            Assert.AreEqual("  reference_three ", fxOrder.GetReference1());
            Assert.AreEqual(" reference_four ", fxOrder.GetReference2());

            fxOrder = _orderBuilder.SetReference("reference_five", "").Build();
            Assert.AreEqual("reference_five", fxOrder.GetReference1());
            Assert.AreEqual("", fxOrder.GetReference2());
            
            fxOrder = _orderBuilder.SetReference("", "reference_eight").Build();
            Assert.AreEqual("", fxOrder.GetReference1());
            Assert.AreEqual("reference_eight", fxOrder.GetReference2());
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingNullFirstReferenceThrowsException()
        {
            _orderBuilder.SetReference(null, "reference_two");
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingNullSecondReferenceThrowsException()
        {
            _orderBuilder.SetReference("reference_one", null);
        }

        [Test]
        public void TestSettingSettlementDate()
        {
            var fxOrder = _orderBuilder.SetSettlementDate("20180207").Build();
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
        [ExpectedException("System.ArgumentException")]
        public void TestSettingNullSettlementDateThrowsException()
        {
            _orderBuilder.SetSettlementDate(null);
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingInvalidSettlementDateThrowsException()
        {
            _orderBuilder.SetSettlementDate("291");
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingSettlementDateWithBadCharactersThrowsException()
        {
            _orderBuilder.SetSettlementDate("yyyy-mm-dd");
        }
        
        [Test]
        public void TestSettingFixingDate()
        {
            var fxOrder = _orderBuilder.SetFixingDate("20180207").Build();
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
        [ExpectedException("System.ArgumentException")]
        public void TestSettingNullFixingDateThrowsException()
        {
            _orderBuilder.SetFixingDate(null);
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingInvalidFixingDateThrowsException()
        {
            _orderBuilder.SetFixingDate("291");
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingFixingDateWithBadCharactersThrowsException()
        {
            _orderBuilder.SetFixingDate("yyyy-mm-dd");
        }
        
        [Test]
        public void TestSettingFarSettlementDate()
        {
            var fxOrder = _orderBuilder.SetFarSettlementDate("20180207").Build();
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
        [ExpectedException("System.ArgumentException")]
        public void TestSettingNullFarSettlementDateThrowsException()
        {
            _orderBuilder.SetFarSettlementDate(null);
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingInvalidFarSettlementDateThrowsException()
        {
            _orderBuilder.SetFarSettlementDate("291");
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingFarSettlementDateWithBadCharactersThrowsException()
        {
            _orderBuilder.SetFarSettlementDate("yyyy-mm-dd");
        }
        
        [Test]
        public void TestSettingFarFixingDate()
        {
            var fxOrder = _orderBuilder.SetFarFixingDate("20180207").Build();
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
        [ExpectedException("System.ArgumentException")]
        public void TestSettingNullFarFixingDateThrowsException()
        {
            _orderBuilder.SetFarFixingDate(null);
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingInvalidFarFixingDateThrowsException()
        {
            _orderBuilder.SetFarFixingDate("291");
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingFarFixingDateWithBadCharactersThrowsException()
        {
            _orderBuilder.SetFarFixingDate("yyyy-mm-dd");
        }
        
        [Test]
        public void TestSettingFarTenor()
        {
            var fxOrder = _orderBuilder.SetFarTenor("2M").Build();
            Assert.AreEqual("2M", fxOrder.GetFarTenor());
            
            fxOrder = _orderBuilder.SetFarTenor(" 1Y  ").Build();
            Assert.AreEqual("1Y", fxOrder.GetFarTenor());
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestNullFarTenorThrowsException()
        {
            _orderBuilder.SetFarTenor(null);
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestEmptyFarTenorThrowsException()
        {
            _orderBuilder.SetFarTenor("");
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestBlankFarTenorThrowsException()
        {
            _orderBuilder.SetFarTenor("     ");
        }
        
        [Test]
        public void TestSettingFarCurrency()
        {
            var fxOrder = _orderBuilder.SetFarCurrency("GBP").Build();
            Assert.AreEqual("GBP", fxOrder.GetFarCurrency());

            fxOrder = _orderBuilder.SetFarCurrency("  EUR ").Build();
            Assert.AreEqual("EUR", fxOrder.GetFarCurrency());
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestNullFarCurrencyThrowsException()
        {
            _orderBuilder.SetFarCurrency(null);
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestEmptyFarCurrencyThrowsException()
        {
            _orderBuilder.SetFarCurrency("");
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestBlankFarCurrencyThrowsException()
        {
            _orderBuilder.SetFarCurrency("   ");
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestNonThreeLetterFarCurrencyThrowsException()
        {
            _orderBuilder.SetFarCurrency("GBPD");
        }
        
        [Test]
        public void TestSettingFarQuantity()
        {
            var fxOrder = _orderBuilder.SetFarQuantity("1000000.00").Build();
            Assert.AreEqual("1000000.00", fxOrder);

            fxOrder = _orderBuilder.SetFarQuantity("5000000").Build();
            Assert.AreEqual("5000000", fxOrder.GetFarQuantity());
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestNonNumericOrPeridFarQuantityThrowsException()
        {
            _orderBuilder.SetFarQuantity("27281P.00").Build();
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestNullFarQuantityThrowsException()
        {
            _orderBuilder.SetFarQuantity(null);
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestEmptyFarQuantityThrowsException()
        {
            _orderBuilder.SetFarQuantity("");
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestBlankFarQuantityThrowsException()
        {
            _orderBuilder.SetFarQuantity("     ");
        }

        [Test]
        public void TestSettingAllocationTemplate()
        {
            var fxOrder = _orderBuilder.SetAllocationTemplate("alloc_template").Build();
            Assert.AreEqual("alloc_template", fxOrder.GetAllocationTemplate());
            
            fxOrder = _orderBuilder.SetAllocationTemplate(" alloc_template_2  ").Build();
            Assert.AreEqual("alloc_template_2", fxOrder.GetAllocationTemplate());
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingNullAllocationTemplateThrowsException()
        {
            _orderBuilder.SetAllocationTemplate(null);
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingEmptyAllocationTemplateThrowsException()
        {
            _orderBuilder.SetAllocationTemplate("");
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingBlankAllocationTemplateThrowsException()
        {
            _orderBuilder.SetAllocationTemplate("    ");
        }
    }
}