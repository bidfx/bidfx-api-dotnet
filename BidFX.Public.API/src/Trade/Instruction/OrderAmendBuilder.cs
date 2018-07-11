using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Trade.Instruction
{
    public class OrderAmendBuilder : OrderInstructionBuilder<OrderAmendBuilder, OrderAmend>
    {
        public OrderAmendBuilder SetOwner(string owner)
        {
            if (Params.IsNullOrEmpty(owner))
            {
                Components.Remove(OrderAmend.Owner);
                return this;
            }
            Components[OrderAmend.Owner] = owner.Trim();
            return this;
        }

        public OrderAmendBuilder SetPrice(decimal? price)
        {
            if (!price.HasValue)
            {
                Components.Remove(OrderAmend.Price);
                return this;
            }
            Components[OrderAmend.Price] = price;
            return this;
        }

        public OrderAmendBuilder SetQuantity(decimal? quantity)
        {
            if (!quantity.HasValue)
            {
                Components.Remove(OrderAmend.Quantity);
            }
            Components[OrderAmend.Quantity] = quantity;
            return this;
        }
        
        public OrderAmendBuilder SetAggregationLevelOne(string aggregationLevel)
        {
            if (Params.IsNullOrEmpty(aggregationLevel))
            {
                Components.Remove(OrderAmend.AggregationLevel1);
                return this;
            }
            Components[OrderAmend.AggregationLevel1] = aggregationLevel.Trim();
            return this;
        }
        
        public OrderAmendBuilder SetAggregationLevelTwo(string aggregationLevel)
        {
            if (Params.IsNullOrEmpty(aggregationLevel))
            {
                Components.Remove(OrderAmend.AggregationLevel2);
                return this;
            }
            Components[OrderAmend.AggregationLevel2] = aggregationLevel.Trim();
            return this;
        }
        
        public OrderAmendBuilder SetAggregationLevelThree(string aggregationLevel)
        {
            if (Params.IsNullOrEmpty(aggregationLevel))
            {
                Components.Remove(OrderAmend.AggregationLevel3);
                return this;
            }
            Components[OrderAmend.AggregationLevel3] = aggregationLevel.Trim();
            return this;
        }

        public override OrderAmend Build()
        {
            return new OrderAmend(Components);
        }
    }
}