using System.Collections.Generic;

namespace BidFX.Public.API.Trade.Order
{
    public class AllocationBuilder
    {
        private readonly IDictionary<string, object> _components = new Dictionary<string, object>();

        public AllocationBuilder()
        {
            _components[Allocation.AllocationType] = Allocation.PreTrade;
        }

        /// <summary>
        /// Set whether the template is pre-trade or post-trade
        /// </summary>
        /// <param name="pretrade">set true for a pre-trade allocation template, and false for a post-trade allocation template</param>
        public AllocationBuilder SetPretrade(bool pretrade)
        {
            _components[Allocation.AllocationType] = pretrade ? Allocation.PreTrade : Allocation.PostTrade;
            return this;
        }

        public AllocationBuilder SetAutoAllocate(bool autoAllocate)
        {
            _components[Allocation.AutoAllocate] = autoAllocate;
            return this;
        }

        public AllocationBuilder SetEntires(IEnumerable<AllocationTemplateEntry> entries)
        {
            _components[Allocation.Entries] = entries;
            return this;
        }
        
        public Allocation Build()
        {
            return new Allocation(_components);
        }
    }
}