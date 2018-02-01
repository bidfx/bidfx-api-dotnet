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
        public void TestEmptyCurrencyPairClearsCurrencyPair()
        {
            _orderBuilder.SetCurrencyPair("USDJPY");
            _orderBuilder.SetCurrencyPair("");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
            
        }
        
        [Test]
        public void TestBlankCurrencyPairClearsCurrencyPair()
        {
            _orderBuilder.SetCurrencyPair("USDJPY");
            _orderBuilder.SetCurrencyPair("   ");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestNullCurrencyPairClearsCurrencyPair()
        {
            _orderBuilder.SetCurrencyPair("EURUSD");
            _orderBuilder.SetCurrencyPair(null);
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestValidDealTypeSetsDealType()
        {
            var fxOrder = _orderBuilder.SetDealType("spot").Build();
            Assert.AreEqual("Spot", fxOrder.GetDealType()); //TODO: Check this is the correct case

            fxOrder = _orderBuilder.SetDealType("ndf").Build();
            Assert.AreEqual("NDF", fxOrder.GetDealType()); //TODO: Check this is the correct case
            
            
            fxOrder = _orderBuilder.SetDealType("outright").Build();
            Assert.AreEqual("Outright", fxOrder.GetDealType()); //TODO: Check this is the correct case/word

            fxOrder = _orderBuilder.SetDealType("forward").Build();
            Assert.AreEqual("Outright", fxOrder.GetDealType()); //TODO: Check this is the correct case/word

            fxOrder = _orderBuilder.SetDealType("swap").Build();
            Assert.AreEqual("Swap", fxOrder.GetDealType()); //TODO: Check this is the correct case

            fxOrder = _orderBuilder.SetDealType("nds").Build();
            Assert.AreEqual("NDS", fxOrder.GetDealType()); //TODO: Check this is the correct case
        }

        [Test]
        public void TestNullDealTypeClearsDealType()
        {
            _orderBuilder.SetDealType("Swap");
            _orderBuilder.SetDealType(null);
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestEmptyDealTypeClearsDealType()
        {
            _orderBuilder.SetDealType("NDS");
            _orderBuilder.SetDealType("");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestBlankDealTypeClearsDealType()
        {
            _orderBuilder.SetDealType("Spot");
            _orderBuilder.SetDealType("   ");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        [ExpectedException("System.ArgumentException", ExpectedMessage = "Unsupported deal type: Invalid")]
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
        public void TestNullCurrencyClearsCurrency()
        {
            _orderBuilder.SetCurrency("GBP");
            _orderBuilder.SetCurrency(null);
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestEmptyCurrencyClearsCurrency()
        {
            _orderBuilder.SetCurrency("USD");
            _orderBuilder.SetCurrency("");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestBlankCurrencyClearsCurrency()
        {
            _orderBuilder.SetCurrency("EUR");
            _orderBuilder.SetCurrency("   ");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
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
        public void TestNullAccountClearsAccount()
        {
            _orderBuilder.SetAccount("FX_ACCT");
            _orderBuilder.SetAccount(null);
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestEmptyAccountClearsAccount()
        {
            _orderBuilder.SetAccount("FX_ACCT");
            _orderBuilder.SetAccount("");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestBlankAccountClearsAccount()
        {
            _orderBuilder.SetAccount("FX_ACCT");
            _orderBuilder.SetAccount("   ");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
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
        public void TestSettingNullSideClearsSide()
        {
            _orderBuilder.SetSide("Buy");
            _orderBuilder.SetSide(null);
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }
        
        [Test]
        public void TestSettingEmptySideClearsSide()
        {
            _orderBuilder.SetSide("Sell");
            _orderBuilder.SetSide("");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }
        
        [Test]
        public void TestSettingBlankSideClearsSide()
        {
            _orderBuilder.SetSide("Sell");
            _orderBuilder.SetSide("     ");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestSettingQuantity()
        {
            var fxOrder = _orderBuilder.SetQuantity("1000000.00").Build();
            Assert.AreEqual("1000000.00", fxOrder.GetQuantity());

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
        public void TestNullQuantityClearsQuantity()
        {
            _orderBuilder.SetQuantity("2000000");
            _orderBuilder.SetQuantity(null);
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestEmptyQuantityClearsQuantity()
        {
            _orderBuilder.SetQuantity("30000000");
            _orderBuilder.SetQuantity("");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestBlankQuantityClearsQuantity()
        {
            _orderBuilder.SetQuantity("10000000.00");
            _orderBuilder.SetQuantity("     ");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
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
        public void TestNullTenorClearsTenor()
        {
            _orderBuilder.SetTenor("6M");
            _orderBuilder.SetTenor(null);
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestEmptyTenorClearsTenor()
        {
            _orderBuilder.SetTenor("4M");
            _orderBuilder.SetTenor("");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestBlankTenorThrowsException()
        {
            _orderBuilder.SetTenor("BD");
            _orderBuilder.SetTenor("     ");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestSettingStrategyParameters()
        {
            var fxOrder = _orderBuilder.SetStrategyParameter("s_param_one", "s_value_one")
                .SetStrategyParameter("s_param_two", "s_value_two")
                .Build();
            Assert.AreEqual("s_value_one", fxOrder.GetStrategyParameter("s_param_one"));
            Assert.AreEqual("s_value_two", fxOrder.GetStrategyParameter("s_param_two"));
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingNullStrategyParameterNameThrowsException()
        {
            _orderBuilder.SetStrategyParameter(null, "not_null");
        }
        
        [Test]
        public void TestSettingNullStrategyParameterValueClearsParameter()
        {
            _orderBuilder.SetStrategyParameter("not_null", "not_null_either");
            _orderBuilder.SetStrategyParameter("not_null", null);
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestSettingSameStrategyParameterOverwritesOld()
        {
            var fxOrder = _orderBuilder.SetStrategyParameter("parameter_one", "value_one")
                .SetStrategyParameter("parameter_one", "value_two")
                .Build();
            Assert.AreEqual("value_two", fxOrder.GetStrategyParameter("parameter_one"));
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
        public void TestSettingEmptyParameterValueClearsParameter()
        {
            _orderBuilder.SetStrategyParameter("parameter_one", "not_empty");
            _orderBuilder.SetStrategyParameter("parameter_one", "");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestSettingBlankParameterValueClearsParameter()
        {
            _orderBuilder.SetStrategyParameter("parameter_one", "not_empty");
            _orderBuilder.SetStrategyParameter("parameter_one", "   ");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
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
        public void TestSettingNullExecutingVenueClearsExecutingVenue()
        {
            _orderBuilder.SetExecutingVenue("TS-SS");
            _orderBuilder.SetExecutingVenue(null);
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestSettingEmptyExecutingValueClearsExecutingVenue()
        {
            _orderBuilder.SetExecutingVenue("TS-SS");
            _orderBuilder.SetExecutingVenue("");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestSettingBlankExecutingValueClearsExecutingVenue()
        {
            _orderBuilder.SetExecutingVenue("TS-SS");
            _orderBuilder.SetExecutingVenue("   ");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestSettingHandlingType() //TODO: Do we restrict to "stream", "quote", and "automatic" only?
        {
            var fxOrder = _orderBuilder.SetHandlingType("Stream").Build();
            Assert.AreEqual("stream", fxOrder.GetHandlingType()); //TODO: Check case on this

            fxOrder = _orderBuilder.SetHandlingType("  Quote ").Build();
            Assert.AreEqual("quote", fxOrder.GetHandlingType());  //TODO: Check case on this
        }

        [Test]
        public void TestSettingNullHandlingTypeClearsHandlingType()
        {
            _orderBuilder.SetHandlingType("automatic");
            _orderBuilder.SetHandlingType(null);
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }
        
        [Test]
        public void TestSettingEmptyHandlingTypeClearsHandlingType()
        {
            _orderBuilder.SetHandlingType("stream");
            _orderBuilder.SetHandlingType("");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestSettingBlankHandlingTypeClearsHandlingType()
        {
            _orderBuilder.SetHandlingType("quote");
            _orderBuilder.SetHandlingType("    ");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
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
        public void TestSettingNullAccountClearsAccount()
        {
            _orderBuilder.SetAccount("FX_ACCT");
            _orderBuilder.SetAccount(null);
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestSettingEmptyAccountClearsAccount()
        {
            _orderBuilder.SetAccount("FX_ACCT");
            _orderBuilder.SetAccount("");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestSettingBlankAccountClearsAccount()
        {
            _orderBuilder.SetAccount("FX_ACCT");
            _orderBuilder.SetAccount("   ");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestSettingReferenceOne()
        {
            var fxOrder = _orderBuilder.SetReferenceOne("reference_one").Build();
            Assert.AreEqual("reference_one", fxOrder.GetReference1());
            
            fxOrder = _orderBuilder.SetReferenceOne("  reference_one_with_whitespace ").Build();
            Assert.AreEqual("  reference_one_with_whitespace ", fxOrder.GetReference1());
            
        }

        [Test]
        public void TestSettingNullReferenceOneClearsReferenceOne()
        {
            _orderBuilder.SetReferenceOne("reference_one");
            _orderBuilder.SetReferenceOne(null);
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }
        
        [Test]
        public void TestSettingEmptyReferenceOneClearsReferenceOne()
        {
            _orderBuilder.SetReferenceOne("reference_one");
            _orderBuilder.SetReferenceOne("");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }
        
        [Test]
        public void TestSettingBlankReferenceOneClearsReferenceOne()
        {
            _orderBuilder.SetReferenceOne("reference_one");
            _orderBuilder.SetReferenceOne("   ");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
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
            var fxOrder = _orderBuilder.SetReferenceTwo("reference_two").Build();
            Assert.AreEqual("reference_two", fxOrder.GetReference2());
            
            fxOrder = _orderBuilder.SetReferenceTwo("  reference_two_with_whitespace ").Build();
            Assert.AreEqual("  reference_two_with_whitespace ", fxOrder.GetReference2());
            
        }

        [Test]
        public void TestSettingNullReferenceTwoClearsReferenceTwo()
        {
            _orderBuilder.SetReferenceTwo("reference_one");
            _orderBuilder.SetReferenceTwo(null);
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }
        
        [Test]
        public void TestSettingEmptyReferenceTwoClearsReferenceTwo()
        {
            _orderBuilder.SetReferenceTwo("reference_one");
            _orderBuilder.SetReferenceTwo("");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }
        
        [Test]
        public void TestSettingBlankReferenceTwoClearsReferenceTwo()
        {
            _orderBuilder.SetReferenceTwo("reference_one");
            _orderBuilder.SetReferenceTwo("   ");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestSettingReferenceTwoWithPipe()
        {
            _orderBuilder.SetReferenceTwo("PartA|PartB");
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
        public void TestSettingNullSettlementDateClearsSettlementDate()
        {
            _orderBuilder.SetSettlementDate("2018-02-23");
            _orderBuilder.SetSettlementDate(null);
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestSettingEmptySettlementDateClearsSettlementDate()
        {
            _orderBuilder.SetSettlementDate("2018-02-23");
            _orderBuilder.SetSettlementDate("");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestSettingBlankSettlementDateClearsSettlementDate()
        {
            _orderBuilder.SetSettlementDate("2018-02-23");
            _orderBuilder.SetSettlementDate("   ");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
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
        public void TestSettingNullFixingDateClearsFixingDate()
        {
            _orderBuilder.SetFixingDate("2018-12-12");
            _orderBuilder.SetFixingDate(null);
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }
        
        [Test]
        public void TestSettingEmptyFixingDateClearsFixingDate()
        {
            _orderBuilder.SetFixingDate("2018-12-12");
            _orderBuilder.SetFixingDate("");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }
        
        [Test]
        public void TestSettingBlankFixingDateClearsFixingDate()
        {
            _orderBuilder.SetFixingDate("2018-12-12");
            _orderBuilder.SetFixingDate("  ");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
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
        public void TestSettingNullFarSettlementDateClearsFarSettlementDate()
        {
            _orderBuilder.SetFarSettlementDate("2018-12-12");
            _orderBuilder.SetFarSettlementDate(null);
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }
        
        [Test]
        public void TestSettingEmptyFarSettlementDateClearsFarSettlementDate()
        {
            _orderBuilder.SetFarSettlementDate("2018-12-12");
            _orderBuilder.SetFarSettlementDate("");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }
        
        [Test]
        public void TestSettingBlankFarSettlementDateClearsFarSettlementDate()
        {
            _orderBuilder.SetFarSettlementDate("2018-12-12");
            _orderBuilder.SetFarSettlementDate("   ");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
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
        public void TestSettingNullFarFixingDateClearsFarFixingDate()
        {
            _orderBuilder.SetFarFixingDate("2018-12-12");
            _orderBuilder.SetFarFixingDate(null);
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }
        
        [Test]
        public void TestSettingEmptyFarFixingDateClearsFarFixingDate()
        {
            _orderBuilder.SetFarFixingDate("2018-12-12");
            _orderBuilder.SetFarFixingDate("");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }
        
        [Test]
        public void TestSettingBlankFarFixingDateClearsFarFixingDate()
        {
            _orderBuilder.SetFarFixingDate("2018-12-12");
            _orderBuilder.SetFarFixingDate("   ");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
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
        public void TestSettingNullFarTenorClearsFarTenor()
        {
            _orderBuilder.SetFarTenor("1M");
            _orderBuilder.SetFarTenor(null);
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }
        
        [Test]
        public void TestSettingEmptyFarTenorClearsFarTenor()
        {
            _orderBuilder.SetFarTenor("1M");
            _orderBuilder.SetFarTenor("");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }
        [Test]
        public void TestSettingBlankFarTenorClearsFarTenor()
        {
            _orderBuilder.SetFarTenor("1M");
            _orderBuilder.SetFarTenor("  ");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
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
        public void TestSettingNullFarCurrencyClearsFarCurrency()
        {
            _orderBuilder.SetFarCurrency("GBP");
            _orderBuilder.SetFarCurrency(null);
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestSettingEmptyFarCurrencyClearsFarCurrency()
        {
            _orderBuilder.SetFarCurrency("GBP");
            _orderBuilder.SetFarCurrency("");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestSettingBlankFarCurrencyClearsFarCurrency()
        {
            _orderBuilder.SetFarCurrency("GBP");
            _orderBuilder.SetFarCurrency("    ");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
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
            Assert.AreEqual("1000000.00", fxOrder.GetFarQuantity());

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
        public void TestSettingNullFarQuantityClearsFarQuantity()
        {
            _orderBuilder.SetFarQuantity("1000000.00");
            _orderBuilder.SetFarQuantity(null);
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestSettingEmptyFarQuantityClearsFarQuantity()
        {
            _orderBuilder.SetFarQuantity("1000000.00");
            _orderBuilder.SetFarQuantity("");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestSettingBlankFarQuantityClearsFarQuantity()
        {
            _orderBuilder.SetFarQuantity("1000000.00");
            _orderBuilder.SetFarQuantity("    ");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
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
        public void TestSettingNullAllocationTemplateClearsAllocationTemplate()
        {
            _orderBuilder.SetAllocationTemplate("alloc_template");
            _orderBuilder.SetAllocationTemplate(null);
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestSettingEmptyAllocationTemplateClearsAllocationTemplate()
        {
            _orderBuilder.SetAllocationTemplate("alloc_template");
            _orderBuilder.SetAllocationTemplate("");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test] public void TestSettingBlankAllocationTemplateClearsAllocationTemplate()
        {
            _orderBuilder.SetAllocationTemplate("alloc_template");
            _orderBuilder.SetAllocationTemplate("   ");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }
        
        [Test]
        public void TestSettingPrice()
        {
            var fxOrder = _orderBuilder.SetPrice("1000000.00").Build();
            Assert.AreEqual("1000000.00", fxOrder.GetPrice());

            fxOrder = _orderBuilder.SetPrice("5000000").Build();
            Assert.AreEqual("5000000", fxOrder.GetPrice());
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestNonNumericOrPeridPriceThrowsException()
        {
            _orderBuilder.SetPrice("27281P.00").Build();
        }

        [Test]
        public void TestSettingNullPriceClearsPrice()
        {
            _orderBuilder.SetPrice("1.345");
            _orderBuilder.SetPrice(null);
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }
        
        [Test]
        public void TestSettingEmptyPriceClearsPrice()
        {
            _orderBuilder.SetPrice("1.345");
            _orderBuilder.SetPrice("");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }
        
        [Test]
        public void TestSettingBlankPriceClearsPrice()
        {
            _orderBuilder.SetPrice("1.345");
            _orderBuilder.SetPrice("     ");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }
        
        [Test]
        public void TestSettingPriceType()
        {
            var fxOrder = _orderBuilder.SetPriceType("Limit").Build();
            Assert.AreEqual("Limit", fxOrder.GetPriceType());
            
            fxOrder = _orderBuilder.SetPriceType(" Market  ").Build();
            Assert.AreEqual("Market", fxOrder.GetPriceType());
        }

        [Test]
        public void TestSettingNullPriceTypeClearsPriceType()
        {
            _orderBuilder.SetPriceType("Limit");
            _orderBuilder.SetPriceType(null);
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestSettingEmptyPriceTypeClearsPriceType()
        {
            _orderBuilder.SetPriceType("Limit");
            _orderBuilder.SetPriceType("");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }

        [Test]
        public void TestSettingBlankPriceTypeClearsPriceType()
        {
            _orderBuilder.SetPriceType("Limit");
            _orderBuilder.SetPriceType("  ");
            var fxOrder = _orderBuilder.Build();
            Assert.AreEqual(0, fxOrder.getInternalComponents().Length);
        }
    }
}