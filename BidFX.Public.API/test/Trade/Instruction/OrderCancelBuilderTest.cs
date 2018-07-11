using NUnit.Framework;

namespace BidFX.Public.API.Trade.Instruction
{
    public class OrderCancelBuilderTest
    {
        private OrderCancelBuilder _instructionBuilder;

        [SetUp]
        public void Before()
        {
            _instructionBuilder = new OrderCancelBuilder();
        }

        [Test]
        public void TestSettingDone()
        {
            OrderCancel orderCancel = _instructionBuilder.SetDone(true).Build();
            Assert.AreEqual(true, orderCancel.GetDone());

            orderCancel = _instructionBuilder.SetDone(false).Build();
            Assert.AreEqual(false, orderCancel.GetDone());
        }

        [Test]
        public void TestSettingDoneToNullClearsDone()
        {
            _instructionBuilder.SetDone(true);
            _instructionBuilder.SetDone(null);
            
            Assert.IsNull(_instructionBuilder.Build().GetDone());
        }
    }
}