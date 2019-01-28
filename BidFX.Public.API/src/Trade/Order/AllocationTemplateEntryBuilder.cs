using System.Collections.Generic;

namespace BidFX.Public.API.Trade.Order
{
    public class AllocationTemplateEntryBuilder
    {
        private readonly IDictionary<string, object> _components = new Dictionary<string, object>();
        
        public AllocationTemplateEntryBuilder SetRatio(decimal ratio)
        {
            _components[AllocationTemplateEntry.AllocRatio] = ratio;
            return this;
        }

        public AllocationTemplateEntryBuilder SetClearingAccount(string account)
        {
            _components[AllocationTemplateEntry.ClearingAccount] = account;
            return this;
        }

        public AllocationTemplateEntryBuilder SetClearingBroker(string broker)
        {
            _components[AllocationTemplateEntry.ClearingBroker] = broker;
            return this;
        }
        
        public AllocationTemplateEntry Build()
        {
            return new AllocationTemplateEntry(_components);
        }
    }
}