using System.Collections.Generic;
using BidFX.Public.API.Trade.Rest.Json;

namespace BidFX.Public.API.Trade.Order
{
    public class Allocation : IJsonMarshallable
    {
        public const string AllocationType = "allocation_type";
        public const string AutoAllocate = "auto_allocate";
        public const string Entries = "entries";
        public const string PreTrade = "PRE_TRADE";
        public const string PostTrade = "POST_TRADE";
        
        private const string TemplateName = "template_name";
        
        private readonly IDictionary<string, object> _jsonMap;
        
        public Allocation(IDictionary<string, object> jsonMap)
        {
            _jsonMap = new SortedDictionary<string, object>(jsonMap);
        }

        public List<AllocationTemplateEntry> GetEntries()
        {
            List<object> entriesJson = GetComponent<List<object>>(Entries);
            if (entriesJson == null)
            {
                return null;
            }
            
            List<AllocationTemplateEntry> allocations = new List<AllocationTemplateEntry>();
            foreach (object allocation in entriesJson)
            {
                allocations.Add(new AllocationTemplateEntry((IDictionary<string, object>) allocation));
            }

            return allocations;
        }

        public bool? IsAutoAllocate()
        {
            return GetComponent<bool?>(AutoAllocate);
        }

        public bool? IsPreTrade()
        {
            string allocationType = GetComponent<string>(AllocationType);
            if (allocationType == null)
            {
                return null;
            }
            return allocationType.Equals(PreTrade);
        }

        public string GetTemplateName()
        {
            return GetComponent<string>(TemplateName);
        }
        
        public IDictionary<string, object> GetJsonMap()
        {
            return _jsonMap;
        }
        
        private T GetComponent<T>(string key)
        {
            object value;
            return (T) (_jsonMap.TryGetValue(key, out value) ? value : null);
        }
    }
}