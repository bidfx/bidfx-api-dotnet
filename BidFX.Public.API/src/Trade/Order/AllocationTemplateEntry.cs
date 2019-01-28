using System.Collections.Generic;

namespace BidFX.Public.API.Trade.Order
{
    public class AllocationTemplateEntry
    {
        public double GetRatio()
        {
            return 0;
        }

        public string GetClearingAccount()
        {
            return "";
        }

        public string GetClearingBroker()
        {
            return "";
        }

        public double GetQuantity()
        {
            return 0;
        }

        public double GetFarQuantity()
        {
            return 0;
        }

        public AllocationTemplateEntry(IDictionary<string, object> jsonMap)
        {
        }

        public IDictionary<string, object> GetJsonMap()
        {
            return null;
        }
    }
}