namespace BidFX.Public.API.Trade.Instruction
{
    public class OrderCancelBuilder : OrderInstructionBuilder<OrderCancelBuilder, OrderCancel>
    {
        public OrderCancelBuilder SetDone(bool? done)
        {
            if (!done.HasValue)
            {
                Components.Remove(OrderCancel.Done);
                return this;
            }
            Components[OrderCancel.Done] = done;
            return this;
        }
        
        public override OrderCancel Build()
        {
            return new OrderCancel(Components);
        }
    }
}