using System.Collections.Generic;

namespace BidFX.Public.API.Trade.Order
{
    public class AllocationBuilder
    {
        private readonly IDictionary<string, object> _components = new Dictionary<string, object>();

        public AllocationBuilder SetPretrade(bool? pretrade)
        {
            if (pretrade == null)
            {
                _components.Remove(Allocation.AllocationType);
            }
            else
            {
                _components[Allocation.AllocationType] = pretrade.Value ? Allocation.PreTrade : Allocation.PostTrade;
            }

            return this;
        }

        public AllocationBuilder SetAutoAllocate(bool? autoAllocate)
        {
            if (autoAllocate == null)
            {
                _components.Remove(Allocation.AutoAllocate);
            }
            else
            {
                _components[Allocation.AutoAllocate] = autoAllocate.Value;
            }

            return this;
        }

        public AllocationBuilder SetEntries(IEnumerable<AllocationTemplateEntry> entries)
        {
            if (entries == null)
            {
                _components.Remove(Allocation.Entries);
            }
            else
            {
                _components[Allocation.Entries] = entries;
            }
            return this;
        }
        
        public Allocation Build()
        {
            return new Allocation(_components);
        }
    }
}