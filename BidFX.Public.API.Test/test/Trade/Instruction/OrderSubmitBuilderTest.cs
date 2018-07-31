using NUnit.Framework;

namespace BidFX.Public.API.Trade.Instruction
{
    public class OrderSubmitBuilderTest
    {
        private OrderSubmitBuilder _instructionBuilder;

        [SetUp]
        public void Before()
        {
            _instructionBuilder = new OrderSubmitBuilder();
        }

        [Test]
        public void TestSettingOrderTsId()
        {
            OrderSubmit orderSubmit = _instructionBuilder.SetOrderTsId("123-GHT-ABC-3832-API").Build();
            Assert.AreEqual("123-GHT-ABC-3832-API", orderSubmit.GetOrderTsId());

            orderSubmit = _instructionBuilder.SetOrderTsId("   123-456-abc  ").Build();
            Assert.AreEqual("123-456-abc", orderSubmit.GetOrderTsId());
        }

        [Test]
        public void TestSettingBlankOrderTsIdClearsOrderTsId()
        {
            _instructionBuilder.SetOrderTsId("123-456").Build();
            _instructionBuilder.SetOrderTsId("   ").Build();
            Assert.IsNull(_instructionBuilder.Build().GetOrderTsId());
        }

        [Test]
        public void TestSettingEmptyOrderTsIdClearsOrderTsId()
        {
            _instructionBuilder.SetOrderTsId("123-456").Build();
            _instructionBuilder.SetOrderTsId("").Build();
            Assert.IsNull(_instructionBuilder.Build().GetOrderTsId());
        }

        [Test]
        public void TestSettingNullOrderTsIdClearsOrderTsId()
        {
            _instructionBuilder.SetOrderTsId("123-456").Build();
            _instructionBuilder.SetOrderTsId(null);
            Assert.IsNull(_instructionBuilder.Build().GetOrderTsId());
        }
        
        [Test]
        public void TestSettingReason()
        {
            OrderSubmit orderSubmit = _instructionBuilder.SetReason("This is my reason").Build();
            Assert.AreEqual("This is my reason", orderSubmit.GetReason());

            orderSubmit = _instructionBuilder.SetReason("   This is another reason  ").Build();
            Assert.AreEqual("This is another reason", orderSubmit.GetReason());
        }

        [Test]
        public void TestSettingBlankReasonClearsReason()
        {
            _instructionBuilder.SetReason("123-456").Build();
            _instructionBuilder.SetReason("   ").Build();
            Assert.IsNull(_instructionBuilder.Build().GetReason());
        }

        [Test]
        public void TestSettingEmptyReasonClearsReason()
        {
            _instructionBuilder.SetReason("123-456").Build();
            _instructionBuilder.SetReason("").Build();
            Assert.IsNull(_instructionBuilder.Build().GetReason());
        }

        [Test]
        public void TestSettingNullReasonClearsReason()
        {
            _instructionBuilder.SetReason("123-456").Build();
            _instructionBuilder.SetReason(null);
            Assert.IsNull(_instructionBuilder.Build().GetReason());
        }
    }
}