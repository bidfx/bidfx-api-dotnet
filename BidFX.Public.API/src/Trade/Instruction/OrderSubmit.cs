using System.Collections.Generic;

namespace BidFX.Public.API.Trade.Instruction
{
    public class OrderSubmit : OrderInstruction
    {
        public OrderSubmit(Dictionary<string, object> jsonMap) : base(jsonMap)
        {
        }
    }
}