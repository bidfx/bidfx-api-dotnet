using System.Collections.Generic;

namespace BidFX.Public.API.Trade.Instruction
{
    public class OrderCancel : OrderInstruction
    {
        internal const string Done = "done";
        
        public OrderCancel(Dictionary<string, object> jsonMap) : base(jsonMap)
        {
        }

        public bool? GetDone()
        {
            return GetComponent<bool?>(Done);
        }
    }
}