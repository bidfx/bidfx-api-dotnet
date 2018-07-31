using NUnit.Framework;

namespace BidFX.Public.API.Trade.Instruction
{
    public class OrderAmendBuilderTest
    {
        private OrderAmendBuilder _instructionBuilder;

        [SetUp]
        public void Before()
        {
            _instructionBuilder = new OrderAmendBuilder();
        }

        [Test]
        public void TestSettingOwner()
        {
            OrderAmend orderAmend = _instructionBuilder.SetOwner("lasman").Build();
            Assert.AreEqual("lasman", orderAmend.GetOwner());

            orderAmend = _instructionBuilder.SetOwner("   dtang ").Build();
            Assert.AreEqual("dtang", orderAmend.GetOwner());
        }

        [Test]
        public void TestSettingBlankOwnerClearsOwner()
        {
            _instructionBuilder.SetOwner("lasman");
            _instructionBuilder.SetOwner("    ");
            Assert.IsNull(_instructionBuilder.Build().GetOwner());
        }

        [Test]
        public void TestSettingEmptyOwnerClearsOwner()
        {
            _instructionBuilder.SetOwner("lasman");
            _instructionBuilder.SetOwner("");
            Assert.IsNull(_instructionBuilder.Build().GetOwner());
        }

        [Test]
        public void TestSettingNullOwnerClearsOwner()
        {
            _instructionBuilder.SetOwner("lasman");
            _instructionBuilder.SetOwner(null);
            Assert.IsNull(_instructionBuilder.Build().GetOwner());
        }
        
        [Test]
        public void TestSettingAggregationLevelOne()
        {
            OrderAmend orderAmend = _instructionBuilder.SetAggregationLevelOne("lasman").Build();
            Assert.AreEqual("lasman", orderAmend.GetAggregationLevelOne());

            orderAmend = _instructionBuilder.SetAggregationLevelOne("   dtang ").Build();
            Assert.AreEqual("dtang", orderAmend.GetAggregationLevelOne());
        }

        [Test]
        public void TestSettingBlankAggregationLevelOneClearsAggregationLevelOne()
        {
            _instructionBuilder.SetAggregationLevelOne("lasman");
            _instructionBuilder.SetAggregationLevelOne("    ");
            Assert.IsNull(_instructionBuilder.Build().GetAggregationLevelOne());
        }

        [Test]
        public void TestSettingEmptyAggregationLevelOneClearsAggregationLevelOne()
        {
            _instructionBuilder.SetAggregationLevelOne("lasman");
            _instructionBuilder.SetAggregationLevelOne("");
            Assert.IsNull(_instructionBuilder.Build().GetAggregationLevelOne());
        }

        [Test]
        public void TestSettingNullAggregationLevelOneClearsAggregationLevelOne()
        {
            _instructionBuilder.SetAggregationLevelOne("lasman");
            _instructionBuilder.SetAggregationLevelOne(null);
            Assert.IsNull(_instructionBuilder.Build().GetAggregationLevelOne());
        }
        
        [Test]
        public void TestSettingAggregationLevelTwo()
        {
            OrderAmend orderAmend = _instructionBuilder.SetAggregationLevelTwo("lasman").Build();
            Assert.AreEqual("lasman", orderAmend.GetAggregationLevelTwo());

            orderAmend = _instructionBuilder.SetAggregationLevelTwo("   dtang ").Build();
            Assert.AreEqual("dtang", orderAmend.GetAggregationLevelTwo());
        }

        [Test]
        public void TestSettingBlankAggregationLevelTwoClearsAggregationLevelTwo()
        {
            _instructionBuilder.SetAggregationLevelTwo("lasman");
            _instructionBuilder.SetAggregationLevelTwo("    ");
            Assert.IsNull(_instructionBuilder.Build().GetAggregationLevelTwo());
        }

        [Test]
        public void TestSettingEmptyAggregationLevelTwoClearsAggregationLevelTwo()
        {
            _instructionBuilder.SetAggregationLevelTwo("lasman");
            _instructionBuilder.SetAggregationLevelTwo("");
            Assert.IsNull(_instructionBuilder.Build().GetAggregationLevelTwo());
        }

        [Test]
        public void TestSettingNullAggregationLevelTwoClearsAggregationLevelTwo()
        {
            _instructionBuilder.SetAggregationLevelTwo("lasman");
            _instructionBuilder.SetAggregationLevelTwo(null);
            Assert.IsNull(_instructionBuilder.Build().GetAggregationLevelTwo());
        }
        
        [Test]
        public void TestSettingAggregationLevelThree()
        {
            OrderAmend orderAmend = _instructionBuilder.SetAggregationLevelThree("lasman").Build();
            Assert.AreEqual("lasman", orderAmend.GetAggregationLevelThree());

            orderAmend = _instructionBuilder.SetAggregationLevelThree("   dtang ").Build();
            Assert.AreEqual("dtang", orderAmend.GetAggregationLevelThree());
        }

        [Test]
        public void TestSettingBlankAggregationLevelThreeClearsAggregationLevelThree()
        {
            _instructionBuilder.SetAggregationLevelThree("lasman");
            _instructionBuilder.SetAggregationLevelThree("    ");
            Assert.IsNull(_instructionBuilder.Build().GetAggregationLevelThree());
        }

        [Test]
        public void TestSettingEmptyAggregationLevelThreeClearsAggregationLevelThree()
        {
            _instructionBuilder.SetAggregationLevelThree("lasman");
            _instructionBuilder.SetAggregationLevelThree("");
            Assert.IsNull(_instructionBuilder.Build().GetAggregationLevelThree());
        }

        [Test]
        public void TestSettingNullAggregationLevelThreeClearsAggregationLevelThree()
        {
            _instructionBuilder.SetAggregationLevelThree("lasman");
            _instructionBuilder.SetAggregationLevelThree(null);
            Assert.IsNull(_instructionBuilder.Build().GetAggregationLevelThree());
        }

        [Test]
        public void TestSettingPrice()
        {
            OrderAmend orderAmend = _instructionBuilder.SetPrice(1.234m).Build();
            Assert.AreEqual(1.234m, orderAmend.GetPrice());

            orderAmend = _instructionBuilder.SetPrice(0).Build();
            Assert.AreEqual(0, orderAmend.GetPrice());
        }

        [Test]
        public void TestSettingNullPriceClearsPrice()
        {
            _instructionBuilder.SetPrice(1.5324m);
            _instructionBuilder.SetPrice(null);
            Assert.IsNull(_instructionBuilder.Build().GetPrice());
        }

        [Test]
        public void TestSettingQuantity()
        {
            OrderAmend orderAmend = _instructionBuilder.SetQuantity(1234).Build();
            Assert.AreEqual(1234, orderAmend.GetQuantity());

            orderAmend = _instructionBuilder.SetQuantity(0).Build();
            Assert.AreEqual(0, orderAmend.GetQuantity());
        }

        [Test]
        public void TestSettingNullQuantityClearsQuantity()
        {
            _instructionBuilder.SetQuantity(15324);
            _instructionBuilder.SetQuantity(null);
            Assert.IsNull(_instructionBuilder.Build().GetQuantity());
        }
    }
}