using System;
using System.Collections.Generic;
using BidFX.Public.API.Price.Tools;

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
            if (Params.IsNullOrEmpty(account))
            {
                throw new ArgumentException("Clearing Account can not be empty");
            }

            _components[AllocationTemplateEntry.ClearingAccount] = account.Trim();
            return this;
        }

        public AllocationTemplateEntryBuilder SetClearingBroker(string broker)
        {
            if (Params.IsNullOrEmpty(broker))
            {
                throw new ArgumentException("Clearing Broker can not be empty");
            }

            _components[AllocationTemplateEntry.ClearingBroker] = broker.Trim();
            return this;
        }
        
        public AllocationTemplateEntry Build()
        {
            return new AllocationTemplateEntry(_components);
        }
    }
}