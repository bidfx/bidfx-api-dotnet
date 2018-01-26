using BidFX.Public.API.Price;
using NUnit.Framework;

namespace BidFX.Public.API.Trade.Order
{
    public class FxOrderTest
    {
        private FxOrder _fxOrder;

        [SetUp]
        public void Before()
        {
            _fxOrder = new FxOrder();
        }
        
        [Test]
        [ExpectedException(typeof(IllegalStateException))]
        public void TestCantSetCurrencyPairAfterMakingImmutable()
        {
            _fxOrder.Freeze();
            Assert.IsNull(_fxOrder.GetCurrencyPair());
            _fxOrder.SetCurrencyPair("EURUSD");
        }

        [Test]
        public void TestCanGetCurrencyPairAfterMakingImmutable()
        {
            _fxOrder.SetCurrencyPair("EURUSD");
            _fxOrder.Freeze();
            Assert.AreEqual("EURUSD", _fxOrder.GetCurrencyPair());
        }

        [Test]
        [ExpectedException(typeof(IllegalStateException))]
        public void TestCantGetSideAfterMakingImmutable()
        {
            _fxOrder.Freeze();
            Assert.IsNull(_fxOrder.GetSide());
            _fxOrder.SetSide("Buy");
        }

        [Test]
        public void TestCanGetSideAfterMakingImmutable()
        {
            _fxOrder.SetSide("Buy");
            _fxOrder.Freeze();
            Assert.AreEqual("Buy", _fxOrder.GetSide());
        }

        [Test]
        [ExpectedException(typeof(IllegalStateException))]
        public void TestCantSetDealTypeAfterMakingImmutable()
        {
            _fxOrder.Freeze();
            Assert.IsNull(_fxOrder.GetDealType());
            _fxOrder.SetDealType("Spot");
        }

        [Test]
        public void TestCanGetDealTypeAfterMakingImmutable()
        {
            _fxOrder.SetDealType("Forward");
            _fxOrder.Freeze();
            Assert.AreEqual("Forward", _fxOrder.GetDealType());
        }

        [Test]
        [ExpectedException(typeof(IllegalStateException))]
        public void TestCantSetExecutingVenueAfterMakingImmutable()
        {
            _fxOrder.Freeze();
            Assert.IsNull(_fxOrder.GetExecutingVenue());
            _fxOrder.SetExecutingVenue("TS-SS");
        }

        [Test]
        public void TestCanGetExecutingVenueAfterMakingImmutable()
        {
            _fxOrder.SetExecutingVenue("TS-SS");
            _fxOrder.Freeze();
            Assert.AreEqual("TS-SS", _fxOrder.GetExecutingVenue());
        }

        [Test]
        [ExpectedException(typeof(IllegalStateException))]
        public void TestCantSetHandlingTypeAfterMakingImmutable()
        {
            _fxOrder.Freeze();
            Assert.IsNull(_fxOrder.GetHandlingType());
            _fxOrder.SetHandlingType("automatic");
        }

        [Test]
        public void TestCanGetHandlingTypeAfterMakingImmutable()
        {
            _fxOrder.SetHandlingType("automatic");
            _fxOrder.Freeze();
            Assert.AreEqual("automatic", _fxOrder.GetHandlingType());
        }

        [Test]
        [ExpectedException(typeof(IllegalStateException))]
        public void TestCantGetAccountAfterMakingImmutable()
        {
            _fxOrder.Freeze();
            Assert.IsNull(_fxOrder.GetAccount());
            _fxOrder.SetAccount("FX_ACCT");
        }

        [Test]
        public void TestCanGetAccountAfterMakingImmutable()
        {
            _fxOrder.SetAccount("FX_ACCT");
            _fxOrder.Freeze();
            Assert.AreEqual("FX_ACCT", _fxOrder.GetAccount());
        }
        
        [Test]
        [ExpectedException(typeof(IllegalStateException))]
        public void TestCantSetReference1AfterMakingImmutable()
        {
            _fxOrder.Freeze();
            Assert.IsNull(_fxOrder.GetReference1());
            _fxOrder.SetReference1("reference_one");
        }

        [Test]
        public void TestCanGetReference1AfterMakingImmutable()
        {
            _fxOrder.SetReference1("reference_one");
            _fxOrder.Freeze();
            Assert.AreEqual("reference_one", _fxOrder.GetReference1());
        }
        
        [Test]
        [ExpectedException(typeof(IllegalStateException))]
        public void TestCantSetReference2AfterMakingImmutable()
        {
            _fxOrder.Freeze();
            Assert.IsNull(_fxOrder.GetReference2());
            _fxOrder.SetReference2("reference_two");
        }

        [Test]
        public void TestCanGetReference2AfterMakingImmutable()
        {
            _fxOrder.SetReference2("reference_two");
            _fxOrder.Freeze();
            Assert.AreEqual("reference_two", _fxOrder.GetReference2());
        }
        
        [Test]
        [ExpectedException(typeof(IllegalStateException))]
        public void TestCantSetCurrencyAfterMakingImmuatable()
        {
            _fxOrder.Freeze();
            Assert.IsNull(_fxOrder.GetCurrency());
            _fxOrder.SetCurrency("EUR");
        }
        
        [Test]
        public void TestCanGetCurrencyAfterMakingImmutable()
        {
            _fxOrder.SetCurrency("EUR");
            _fxOrder.Freeze();
            Assert.AreEqual("EUR", _fxOrder.GetCurrency());
        }

        [Test]
        [ExpectedException(typeof(IllegalStateException))]
        public void TestCantSetQuantityAfterMakingImmutable()
        {
            _fxOrder.Freeze();
            Assert.IsNull(_fxOrder.GetQuantity());
            _fxOrder.SetQuantity("2000000.00");
        }

        [Test]
        public void TestCanGetQuantityAfterMakingImmutable()
        {
            _fxOrder.SetQuantity("3000000.00");
            _fxOrder.Freeze();
            Assert.AreEqual("3000000.00", _fxOrder.GetQuantity());
        }

        [Test]
        [ExpectedException(typeof(IllegalStateException))]
        public void TestCantSetTenorAfterMakingImmutable()
        {
            _fxOrder.Freeze();
            Assert.IsNull(_fxOrder.GetTenor());
            _fxOrder.SetTenor("2M");
        }

        [Test]
        public void TestCanGetTenorAfterMakingImmutable()
        {
            _fxOrder.SetTenor("2M");
            _fxOrder.Freeze();
            Assert.AreEqual("2M", _fxOrder.GetTenor());
        }

        [Test]
        [ExpectedException(typeof(IllegalStateException))]
        public void TestCantSetSettlementDateAfterMakingImmutable()
        {
            _fxOrder.Freeze();
            Assert.IsNull(_fxOrder.GetSettlementDate());
            _fxOrder.SetSettlementDate("2018-02-13");
        }

        [Test]
        public void TestCanGetSettlementDateAfterMakingImmutable()
        {
            _fxOrder.SetSettlementDate("2018-02-13");
            _fxOrder.Freeze();
            Assert.AreEqual("2018-02-13", _fxOrder.GetSettlementDate());
        }
        
        [Test]
        [ExpectedException(typeof(IllegalStateException))]
        public void TestCantSetFixingDateAfterMakingImmutable()
        {
            _fxOrder.Freeze();
            Assert.IsNull(_fxOrder.GetFixingDate());
            _fxOrder.SetFixingDate("2018-02-15");
        }

        [Test]
        public void TestCanGetFixingDateAfterMakingImmutable()
        {
            _fxOrder.SetFixingDate("2018-02-15");
            _fxOrder.Freeze();
            Assert.AreEqual("2018-02-15", _fxOrder.GetFixingDate());
        }
        
        [Test]
        [ExpectedException(typeof(IllegalStateException))]
        public void TestCantSetFarCurrencyAfterMakingImmuatable()
        {
            _fxOrder.Freeze();
            Assert.IsNull(_fxOrder.GetFarCurrency());
            _fxOrder.SetFarCurrency("EUR");
        }
        
        [Test]
        public void TestCanGetFarCurrencyAfterMakingImmutable()
        {
            _fxOrder.SetFarCurrency("EUR");
            _fxOrder.Freeze();
            Assert.AreEqual("EUR", _fxOrder.GetFarCurrency());
        }

        [Test]
        [ExpectedException(typeof(IllegalStateException))]
        public void TestCantSetFarQuantityAfterMakingImmutable()
        {
            _fxOrder.Freeze();
            Assert.IsNull(_fxOrder.GetFarQuantity());
            _fxOrder.SetFarQuantity("2000000.00");
        }

        [Test]
        public void TestCanGetFarQuantityAfterMakingImmutable()
        {
            _fxOrder.SetFarQuantity("3000000.00");
            _fxOrder.Freeze();
            Assert.AreEqual("3000000.00", _fxOrder.GetFarQuantity());
        }

        [Test]
        [ExpectedException(typeof(IllegalStateException))]
        public void TestCantSetFarTenorAfterMakingImmutable()
        {
            _fxOrder.Freeze();
            Assert.IsNull(_fxOrder.GetFarTenor());
            _fxOrder.SetFarTenor("2M");
        }

        [Test]
        public void TestCanGetFarTenorAfterMakingImmutable()
        {
            _fxOrder.SetFarTenor("2M");
            _fxOrder.Freeze();
            Assert.AreEqual("2M", _fxOrder.GetFarTenor());
        }

        [Test]
        [ExpectedException(typeof(IllegalStateException))]
        public void TestCantSetFarSettlementDateAfterMakingImmutable()
        {
            _fxOrder.Freeze();
            Assert.IsNull(_fxOrder.GetFarSettlementDate());
            _fxOrder.SetFarSettlementDate("2018-02-13");
        }

        [Test]
        public void TestCanGetFarSettlementDateAfterMakingImmutable()
        {
            _fxOrder.SetFarSettlementDate("2018-02-13");
            _fxOrder.Freeze();
            Assert.AreEqual("2018-02-13", _fxOrder.GetFarSettlementDate());
        }
        
        [Test]
        [ExpectedException(typeof(IllegalStateException))]
        public void TestCantSetFarFixingDateAfterMakingImmutable()
        {
            _fxOrder.Freeze();
            Assert.IsNull(_fxOrder.GetFarFixingDate());
            _fxOrder.SetFarFixingDate("2018-02-15");
        }

        [Test]
        public void TestCanGetFarFixingDateAfterMakingImmutable()
        {
            _fxOrder.SetFarFixingDate("2018-02-15");
            _fxOrder.Freeze();
            Assert.AreEqual("2018-02-15", _fxOrder.GetFarFixingDate());
        }

        [Test]
        [ExpectedException(typeof(IllegalStateException))]
        public void TestCantSetAllocationTemplateAfterMakingImmutable()
        {
            _fxOrder.Freeze();
            Assert.IsNull(_fxOrder.GetAllocationTemplate());
            _fxOrder.SetAllocationTemplate("alloc_template");
        }

        [Test]
        public void TestCanGetAllocationTemplateAfterMakingImmutable()
        {
            _fxOrder.SetAllocationTemplate("alloc_template");
            _fxOrder.Freeze();
            Assert.AreEqual("alloc_template", _fxOrder.GetAllocationTemplate());
        }

        [Test]
        [ExpectedException(typeof(IllegalStateException))]
        public void TestCantSetStrategyParameterAfterMakingImmutable()
        {
            _fxOrder.Freeze();
            Assert.IsNull(_fxOrder.GetStrategyParameters());
            _fxOrder.SetStrategyParameter("parameter_name", "parameter_value");
        }

        [Test]
        public void TestSettingMultipleStrategyParameters()
        {
            _fxOrder.SetStrategyParameter("param_one", "value_one");
            _fxOrder.SetStrategyParameter("param_two", "value_two");
            _fxOrder.SetStrategyParameter("param_three", "value_three");
            _fxOrder.Freeze();
            var strategyParams = _fxOrder.GetStrategyParameters();
            Assert.AreEqual(3, strategyParams.Count);
            Assert.AreEqual("value_one", strategyParams["param_one"]);
            Assert.AreEqual("value_two", strategyParams["param_two"]);
            Assert.AreEqual("value_three", strategyParams["param_three"]);
        }

        [Test]
        public void TestOverwritingStrategyParameter()
        {
            _fxOrder.SetStrategyParameter("param_one", "value_one");
            _fxOrder.SetStrategyParameter("param_two", "value_two");
            _fxOrder.Freeze();
            var strategyParams = _fxOrder.GetStrategyParameters();
            Assert.AreEqual(1, strategyParams.Count);
            Assert.AreEqual("value_two", strategyParams["param_one"]);
        }

        [Test]
        public void TestReturnedStrategyParametersAreACopy()
        {
            _fxOrder.SetStrategyParameter("param_one", "value_one");
            var strategyParams1 = _fxOrder.GetStrategyParameters();
            Assert.AreEqual("value_one", strategyParams1["param_one"]);
            
            strategyParams1["param_one"] = "value_two";
            var strategyParams2 = _fxOrder.GetStrategyParameters();
            Assert.AreEqual("value_one", strategyParams2["param_one"]);

            _fxOrder.SetStrategyParameter("param_one", "value_three");
            Assert.AreEqual("value_one", strategyParams2["param_one"]);
            
            Assert.AreNotSame(strategyParams1, strategyParams2);
        }
    }
}