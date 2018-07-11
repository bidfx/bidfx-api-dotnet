using System.Collections.Generic;
using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Trade.Instruction
{
    public abstract class OrderInstructionBuilder<T, TO> where T : OrderInstructionBuilder<T, TO>
        where TO : OrderInstruction
    {
        protected readonly Dictionary<string, object> Components = new Dictionary<string, object>();
        
        public T SetOrderTsId(string orderTsId)
        {
            if (Params.IsNullOrEmpty(orderTsId))
            {
                Components.Remove(OrderInstruction.OrderTsId);
                return this as T;
            }
            Components[OrderInstruction.OrderTsId] = orderTsId.Trim();
            return this as T;
        }

        public T SetReason(string reason)
        {
            if (Params.IsNullOrEmpty(reason))
            {
                Components.Remove(OrderInstruction.Reason);
                return this as T;
            }

            Components[OrderInstruction.Reason] = reason.Trim();
            return this as T;
        }
        
        public abstract TO Build();
    }
}