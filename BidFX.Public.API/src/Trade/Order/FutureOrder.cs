using System.Collections.Generic;

namespace BidFX.Public.API.Trade.Order
{
    public class FutureOrder : Order
    {
        internal const string InstrumentCode = "instrument_code";
        internal const string InstrumentCodeType = "instrument_code_type";
        internal const string InstrumentTsId = "instrument_ts_id";
        internal const string ContractDate = "contract_date";
        
        public FutureOrder(IDictionary<string, object> components) : base(components)
        {
        }
        
        /**
         * Future order properties
         */

        public string GetContractDate()
        {
            return GetComponent<string>(ContractDate);
        }
        
        /**
         * Security order properties
         */
        public string GetInstrumentCode()
        {
            return GetComponent<string>(InstrumentCode);
        }

        public string GetInstrumentCodeType()
        {
            return GetComponent<string>(InstrumentCodeType);
        }

        public string GetInstrumentTsId()
        {
            return GetComponent<string>(InstrumentTsId);
        }
      
        public override string ToString()
        {
            return "FutureOrder: [" + base.ToString() + "]";
        }
    }
}