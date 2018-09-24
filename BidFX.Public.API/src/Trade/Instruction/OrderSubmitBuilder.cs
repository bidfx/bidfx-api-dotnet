namespace BidFX.Public.API.Trade.Instruction
{
    public class OrderSubmitBuilder : OrderInstructionBuilder<OrderSubmitBuilder, OrderSubmit>
    {
        public override OrderSubmit Build()
        {
            return new OrderSubmit(Components);
        }
    }
}