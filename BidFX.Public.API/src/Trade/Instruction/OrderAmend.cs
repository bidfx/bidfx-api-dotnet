using System.Collections.Generic;

namespace BidFX.Public.API.Trade.Instruction
{
    public class OrderAmend : OrderInstruction
    {
        internal const string Owner = "owner";
        internal const string Price = "price";
        internal const string Quantity = "quantity";
        internal const string AggregationLevel1 = "aggregation_level_1";
        internal const string AggregationLevel2 = "aggregation_level_2";
        internal const string AggregationLevel3 = "aggregation_level_3";

        public OrderAmend(Dictionary<string, object> components) : base(components)
        {}
        
        public string GetOrderTsId()
        {
            return GetComponent<string>(OrderTsId);
        }

        public string GetOwner()
        {
            return GetComponent<string>(Owner);
        }

        public decimal? GetPrice()
        {
            return GetComponent<decimal?>(Price);
        }
        
        public decimal? GetQuantity()
        {
            return GetComponent<decimal?>(Quantity);
        }

        public string GetAggregationLevelOne()
        {
            return GetComponent<string>(AggregationLevel1);
        }

        public string GetAggregationLevelTwo()
        {
            return GetComponent<string>(AggregationLevel2);
        }

        public string GetAggregationLevelThree()
        {
            return GetComponent<string>(AggregationLevel3);
        }
    }
}