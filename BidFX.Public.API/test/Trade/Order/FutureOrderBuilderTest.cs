using NUnit.Framework;

namespace BidFX.Public.API.Trade.Order
{
    public class FutureOrderBuilderTest
    {
        private FutureOrderBuilder _orderBuilder;

        [SetUp]
        public void Before()
        {
            _orderBuilder = new FutureOrderBuilder();
        }

        [Test]
        public void TestAssetClassIsSetInOrder()
        {
            FutureOrder futureOrder = _orderBuilder.Build();
            Assert.AreEqual("FUTURE", futureOrder.GetAssetClass());
        }
        
        /**
         * Future Order properties
         */
        [Test]
        public void TestSettingContractDate()
        {
            FutureOrder futureOrder = _orderBuilder.SetContractDate("201802").Build();
            Assert.AreEqual("2018-02", futureOrder.GetContractDate());

            futureOrder = _orderBuilder.SetContractDate(" 2018-02  ").Build();
            Assert.AreEqual("2018-02", futureOrder.GetContractDate());

            futureOrder = _orderBuilder.SetContractDate("   201805 ").Build();
            Assert.AreEqual("2018-05", futureOrder.GetContractDate());

            futureOrder = _orderBuilder.SetContractDate("2019-12").Build();
            Assert.AreEqual("2019-12", futureOrder.GetContractDate());

            futureOrder = _orderBuilder.SetContractDate("2018-5").Build();
            Assert.AreEqual("2018-05", futureOrder.GetContractDate());
            
            futureOrder = _orderBuilder.SetContractDate("20181202").Build();
            Assert.AreEqual("2018-12-02", futureOrder.GetContractDate());

            futureOrder = _orderBuilder.SetContractDate(" 2018-02-18  ").Build();
            Assert.AreEqual("2018-02-18", futureOrder.GetContractDate());

            futureOrder = _orderBuilder.SetContractDate("   20181131 ").Build();
            Assert.AreEqual("2018-11-31", futureOrder.GetContractDate());

            futureOrder = _orderBuilder.SetContractDate("2019-12-3").Build();
            Assert.AreEqual("2019-12-03", futureOrder.GetContractDate());

            futureOrder = _orderBuilder.SetContractDate("2018-5-12").Build();
            Assert.AreEqual("2018-05-12", futureOrder.GetContractDate());
        }

        [Test]
        public void TestSettingNullContractDateClearsContractDate()
        {
            _orderBuilder.SetContractDate("2017-03");
            _orderBuilder.SetContractDate(null);
            FutureOrder futureOrder = _orderBuilder.Build();
            Assert.AreEqual(1, futureOrder.GetJsonMap().Count);
            Assert.IsNull(futureOrder.GetContractDate());
        }

        [Test]
        public void TestSettingEmptyContractDateClearsContractDate()
        {
            _orderBuilder.SetContractDate("2017-03");
            _orderBuilder.SetContractDate("");
            FutureOrder futureOrder = _orderBuilder.Build();
            Assert.AreEqual(1, futureOrder.GetJsonMap().Count);
            Assert.IsNull(futureOrder.GetContractDate());
        }

        [Test]
        public void TestSettingBlankContractDateClearsContractDate()
        {
            _orderBuilder.SetContractDate("2017-03");
            _orderBuilder.SetContractDate("   ");
            FutureOrder futureOrder = _orderBuilder.Build();
            Assert.AreEqual(1, futureOrder.GetJsonMap().Count);
            Assert.IsNull(futureOrder.GetContractDate());
        }

        [Test]
        [ExpectedException("System.ArgumentException", ExpectedMessage = "ContractDate was not in valid format (YYYY-MM): 284")]
        public void TestSettingInvalidContractDateThrowsException()
        {
            _orderBuilder.SetContractDate("284");
        }

        [Test]
        [ExpectedException("System.ArgumentException", ExpectedMessage = "ContractDate was not in valid format (YYYY-MM): yyyy-mm")]
        public void TestSettingContractDateWithBadCharactersThrowsException()
        {
            _orderBuilder.SetContractDate("yyyy-mm");
        }

        /**
         * Security Order properties
         */
        
        [Test]
        public void TestSettingInstrumentCode()
        {
            FutureOrder futureOrder = _orderBuilder.SetInstrumentCode("FDXU8").Build();
            Assert.AreEqual("FDXU8", futureOrder.GetInstrumentCode());

            futureOrder = _orderBuilder.SetInstrumentCode("  FDXM9 ").Build();
            Assert.AreEqual("FDXM9", futureOrder.GetInstrumentCode());
        }

        [Test]
        public void TestEmptyInstrumentCodeClearsInstrumentCode()
        {
            _orderBuilder.SetInstrumentCode("FDXU8");
            _orderBuilder.SetInstrumentCode("");
            FutureOrder futureOrder = _orderBuilder.Build();
            Assert.AreEqual(1, futureOrder.GetJsonMap().Count);
            Assert.IsNull(futureOrder.GetInstrumentCode());
        }

        [Test]
        public void TestBlankInstrumentCodeClearsInstrumentCode()
        {
            _orderBuilder.SetInstrumentCode("FDXU8");
            _orderBuilder.SetInstrumentCode("    ");
            FutureOrder futureOrder = _orderBuilder.Build();
            Assert.AreEqual(1, futureOrder.GetJsonMap().Count);
            Assert.IsNull(futureOrder.GetInstrumentCode());
        }

        [Test]
        public void TestNullInstrumentCodeClearsInstrumentCode()
        {
            _orderBuilder.SetInstrumentCode("FDXU8");
            _orderBuilder.SetInstrumentCode(null);
            FutureOrder futureOrder = _orderBuilder.Build();
            Assert.AreEqual(1, futureOrder.GetJsonMap().Count);
            Assert.IsNull(futureOrder.GetInstrumentCode());
        }

        [Test]
        public void TestSettingInstrumentCodeType()
        {
            FutureOrder futureOrder = _orderBuilder.SetInstrumentCodeType("BLOOMBERG").Build();
            Assert.AreEqual("BLOOMBERG", futureOrder.GetInstrumentCodeType());

            futureOrder = _orderBuilder.SetInstrumentCodeType(" RIC  ").Build();
            Assert.AreEqual("RIC", futureOrder.GetInstrumentCodeType());
        }
        
        [Test]
        public void TestEmptyInstrumentCodeTypeClearsInstrumentCodeType()
        {
            _orderBuilder.SetInstrumentCodeType("BLOOMBERG");
            _orderBuilder.SetInstrumentCodeType("");
            FutureOrder futureOrder = _orderBuilder.Build();
            Assert.AreEqual(1, futureOrder.GetJsonMap().Count);
            Assert.IsNull(futureOrder.GetInstrumentCodeType());
        }

        [Test]
        public void TestBlankInstrumentCodeTypeClearsInstrumentCodeType()
        {
            _orderBuilder.SetInstrumentCodeType("BLOOMBERG");
            _orderBuilder.SetInstrumentCodeType("    ");
            FutureOrder futureOrder = _orderBuilder.Build();
            Assert.AreEqual(1, futureOrder.GetJsonMap().Count);
            Assert.IsNull(futureOrder.GetInstrumentCodeType());
        }

        [Test]
        public void TestNullInstrumentCodeTypeClearsInstrumentCodeType()
        {
            _orderBuilder.SetInstrumentCodeType("BLOOMBERG");
            _orderBuilder.SetInstrumentCodeType(null);
            FutureOrder futureOrder = _orderBuilder.Build();
            Assert.AreEqual(1, futureOrder.GetJsonMap().Count);
            Assert.IsNull(futureOrder.GetInstrumentCodeType());
        }
    }
}