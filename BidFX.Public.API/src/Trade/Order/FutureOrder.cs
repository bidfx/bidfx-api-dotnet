using System.Collections.Generic;

namespace BidFX.Public.API.Trade.Order
{
    public class FutureOrder : Order
    {
        // Future specific for Instruments
        internal const string ContractDate = "contract_date";

        // Securities Order
        // Instrument
        internal const string InstrumentCode = "instrument_code";
        internal const string InstrumentCodeType = "instrument_code_type";
        internal const string VenueCode = "venue_code";
        internal const string VenueCodeType = "venue_code_type";
        // Order
        internal const string AggregationLevelOne = "aggregation_level_1";
        internal const string AggregationLevelTwo = "aggregation_level_2";
        internal const string AggregationLevelThree = "aggregation_level_3";
        internal const string AlgoParent = "algo_parent";
        internal const string ClearingAccount = "clearing_account";
        internal const string ClearingBroker = "clearing_broker";
        internal const string CreationDate = "creation_date";
        internal const string Description = "description";
        internal const string ExecutingBroker = "executing_broker";
        internal const string GoodTillDate = "expiry_date";
        internal const string GroupingOrderId = "grouping_order_id";
        internal const string HandlingComment = "handling_comment";
        internal const string HandlingExecutingInstruction = "handling_exec_inst";
        internal const string HedgeCurrency = "hedge_currecny";
        internal const string LocateBroker = "locate_broker";
        internal const string LocateId = "locate_id";
        internal const string LocateType = "locate_type";
        internal const string QuantityLimit = "quantity_limit";
        internal const string SettlementComment = "settlement_comment";
        internal const string SettlementCurrency = "settlement_currency";
        internal const string SettlementFxRate = "settlement_fx_rate";
        internal const string SettlementValue = "settlement_value";
        internal const string StrategyParentOrderId = "strategy_parent_order_id";
        internal const string StrategyState = "strategy_state";
        internal const string SystemOrderId = "system_order_id";
        internal const string TimeInForceType = "time_in_force_type";
        
        // Read-only fields
        private const string AllocatedQuantity = "allocated_quantity";
        private const string AverageExecutedPrice = "average_executed_price";
        private const string AverageSettlementPrice = "average_settlement_price";
        private const string CaptureDate = "capture_date";
        private const string CashOrderQuantity = "cash_order_quantity";
        private const string CounterOwner = "counter_owner";
        private const string CounterParty = "counter_party";
        private const string DeactivationDate = "deactivation_date";
        private const string DeactivationDateModifier = "deactivation_date_modifier";
        private const string DoneForDay = "done_for_day";
        private const string Ecn = "ecn";
        private const string EosOrderId = "eod_order_id";
        private const string ErrantQuantity = "errant_quantity";
        private const string Eti = "eti";
        private const string ExecutedQuantity = "executed_quantity";
        private const string ExecutedValue = "executed_value";
        private const string FullyExecuted = "fully_executed";
        private const string InstrumentTsId = "instrument_ts_id";
        private const string LeavesQuantity = "leaves_quantity";
        private const string OutstandingQuantity = "outstanding_quantity";
        private const string ReleasedQuantity = "released_quantity";
        private const string ShownQuantity = "shown_quantity";
        private const string UnexecutedQuantity = "unexecuted_quantity";
        
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
         * Securities order properties
         */
        /**
         * Instruments
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
        
        public string GetVenueCode()
        {
            return GetComponent<string>(VenueCode);
        }

        public string GetVenueCodeType()
        {
            return GetComponent<string>(VenueCodeType);
        }

        /**
         * Order
         */
        public string GetAggregationLevelOne()
        {
            return GetComponent<string>(AggregationLevelOne);
        }

        public string GetAggregationLevelTwo()
        {
            return GetComponent<string>(AggregationLevelTwo);
        }

        public string GetAggregationLevelThree()
        {
            return GetComponent<string>(AggregationLevelThree);
        }

        public string GetAlgoParent()
        {
            return GetComponent<string>(AlgoParent);
        }

        public string GetClearingAccount()
        {
            return GetComponent<string>(ClearingAccount);
        }

        public string GetCreationDate()
        {
            return GetComponent<string>(CreationDate);
        }

        public string GetDescription()
        {
            return GetComponent<string>(Description);
        }

        public string GetExecutingBroker()
        {
            return GetComponent<string>(ExecutingBroker);
        }

        public string GetGoodTillDate()
        {
            return GetComponent<string>(GoodTillDate);
        }

        public string GetGroupingOrderId()
        {
            return GetComponent<string>(GroupingOrderId);
        }

        public string GetHandlingComment()
        {
            return GetComponent<string>(HandlingComment);
        }

        public string GetHandlingExecutingInstruction()
        {
            return GetComponent<string>(HandlingExecutingInstruction);
        }

        public string GetHedgeCurrency()
        {
            return GetComponent<string>(HedgeCurrency);
        }

        public string GetLocateBroker()
        {
            return GetComponent<string>(LocateBroker);
        }

        public string GetLocateId()
        {
            return GetComponent<string>(LocateId);
        }

        public string GetLocateType()
        {
            return GetComponent<string>(LocateType);
        }

        public decimal? GetQuantityLimit()
        {
            return GetComponent<decimal?>(QuantityLimit);
        }

        public string GetSettlementComment()
        {
            return GetComponent<string>(SettlementComment);
        }

        public string GetSettlementCurrency()
        {
            return GetComponent<string>(SettlementCurrency);
        }

        public decimal? GetSettlementFxRate()
        {
            return GetComponent<decimal?>(SettlementFxRate);
        }

        public decimal? GetSettlementValue()
        {
            return GetComponent<decimal?>(SettlementValue);
        }

        public string GetStrategyParentOrderId()
        {
            return GetComponent<string>(StrategyParentOrderId);
        }

        public string GetStrategyState()
        {
            return GetComponent<string>(StrategyState);
        }

        public string GetSystemOrderId()
        {
            return GetComponent<string>(SystemOrderId);
        }

        public string GetTimeInForceType()
        {
            return GetComponent<string>(TimeInForceType);
        }

        public decimal? GetAllocatedQuantity()
        {
            return GetComponent<decimal?>(AllocatedQuantity);
        }

        public decimal? GetAverageExecutedPrice()
        {
            return GetComponent<decimal?>(AverageExecutedPrice);
        }

        public decimal? GetAverageSettlementprice()
        {
            return GetComponent<decimal?>(AverageSettlementPrice);
        }

        public string GetCaptureDate()
        {
            return GetComponent<string>(CaptureDate);
        }

        public decimal? GetCashOrderQuantity()
        {
            return GetComponent<decimal?>(CashOrderQuantity);
        }

        public string GetCounterOwner()
        {
            return GetComponent<string>(CounterOwner);
        }

        public string GetCounterParty()
        {
            return GetComponent<string>(CounterParty);
        }

        public string GetDeactivationDate()
        {
            return GetComponent<string>(DeactivationDate);
        }

        public string GetDeactivationDateModifier()
        {
            return GetComponent<string>(DeactivationDateModifier);
        }
        
        public bool? GetDoneForDay()
        {
            return GetComponent<bool?>(DoneForDay);
        }

        public string GetEcn()
        {
            return GetComponent<string>(Ecn);
        }

        public string GetEosOrderId()
        {
            return GetComponent<string>(EosOrderId);
        }

        public string GetEti()
        {
            return GetComponent<string>(Eti);
        }

        public decimal? GetErrantQuantity()
        {
            return GetComponent<decimal?>(ErrantQuantity);
        }

        public decimal? GetExecutedQuantity()
        {
            return GetComponent<decimal?>(ExecutedQuantity);
        }

        public decimal? GetExecutedValue()
        {
            return GetComponent<decimal?>(ExecutedValue);
        }

        public bool? GetFullyExecuted()
        {
            return GetComponent<bool?>(FullyExecuted);
        }

        public decimal? GetLeavesQuantity()
        {
            return GetComponent<decimal?>(LeavesQuantity);
        }

        public decimal? GetOutstandingQuantity()
        {
            return GetComponent<decimal?>(OutstandingQuantity);
        }

        public decimal? GetReleasedQuantity()
        {
            return GetComponent<decimal?>(ReleasedQuantity);
        }

        public decimal? GetShownQuantity()
        {
            return GetComponent<decimal?>(ShownQuantity);
        }

        public decimal? GetUnexecutedQuantity()
        {
            return GetComponent<decimal?>(UnexecutedQuantity);
        }

        public override string ToString()
        {
            return "FutureOrder: [" + base.ToString() + "]";
        }
    }
}