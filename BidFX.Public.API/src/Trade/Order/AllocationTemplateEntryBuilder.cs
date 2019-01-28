namespace BidFX.Public.API.Trade.Order
{
    public class AllocationTemplateEntryBuilder
    {
        public AllocationTemplateEntryBuilder SetRatio(double ratio)
        {
            return this;
        }

        public AllocationTemplateEntryBuilder SetClearingAccount(string account)
        {
            return this;
        }

        public AllocationTemplateEntryBuilder SetClearingBroker(string account)
        {
            return this;
        }
        
        public AllocationTemplateEntry Build()
        {
            return null;
        }
    }
}