using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Trade.Order
{
    public class FutureOrderBuilder : OrderBuilder<FutureOrderBuilder, FutureOrder>
    {
        public FutureOrderBuilder() : base("FUTURE")
        {
        }

        public override FutureOrder Build()
        {
            return new FutureOrder(Components);
        }
        
        /**
         * Future Order Properties
         */

        public FutureOrderBuilder SetContractDate(string contractDate)
        {
            if (Params.IsNullOrEmpty(contractDate))
            {
                Components.Remove(FutureOrder.ContractDate);
                return this;
            }

            Components[FutureOrder.ContractDate] =  FormatDate("ContractDate", contractDate, false);
            return this;
        }
        
        /**
         * Security Order Properties
         */

        public FutureOrderBuilder SetInstrumentCode(string instrumentCode)
        {
            if (Params.IsNullOrEmpty(instrumentCode))
            {
                Components.Remove(FutureOrder.InstrumentCode);
                return this;
            }

            Components[FutureOrder.InstrumentCode] = instrumentCode.Trim();
            return this;
        }

        public FutureOrderBuilder SetInstrumentCodeType(string instrumentCodeType)
        {
            if (Params.IsNullOrEmpty(instrumentCodeType))
            {
                Components.Remove(FutureOrder.InstrumentCodeType);
                return this;
            }

            Components[FutureOrder.InstrumentCodeType] = instrumentCodeType.Trim();
            return this;
        }

        public FutureOrderBuilder SetVenueCode(string venueCode)
        {
            return this;
        }

        public FutureOrderBuilder SetVenueCodeType(string venueCodeType)
        {
            return this;
        }

        public FutureOrderBuilder SetAggregationLevelOne(string aggregationLevel)
        {
            return this;
        }

        public FutureOrderBuilder SetAggregationLevelTwo(string aggregationLevel)
        {
            return this;
        }

        public FutureOrderBuilder SetAggregationLevelThree(string aggregationLevel)
        {
            return this;
        }

        public FutureOrderBuilder SetAlgoParent(string algoParent)
        {
            return this;
        }

        public FutureOrderBuilder SetClearingAccount(string clearingAccount)
        {
            return this;
        }

        public FutureOrderBuilder SetClearingBroker(string clearingBroker)
        {
            return this;
        }

        public FutureOrderBuilder SetCreationDate(string creationDate)
        {
            return this;
        }

        public FutureOrderBuilder SetDescription(string description)
        {
            return this;
        }

        public FutureOrderBuilder SetExecutingBroker(string executingBroker)
        {
            return this;
        }

        public FutureOrderBuilder SetExpiryDate(string expiryDate)
        {
            return this;
        }

        public FutureOrderBuilder SetGroupingOrderId(string groupingOrderId)
        {
            return this;
        }

        public FutureOrderBuilder SetHandlingComment(string handlingComment)
        {
            return this;
        }

        public FutureOrderBuilder SetHandlingExecInst(string handlingExecInst)
        {
            return this;
        }

        public FutureOrderBuilder SetHedgeCurrency(string hedgeCurrency)
        {
            return this;
        }

        public FutureOrderBuilder SetLocateBroker(string locateBroker)
        {
            return this;
        }

        public FutureOrderBuilder SetLocateId(string locateId)
        {
            return this;
        }

        public FutureOrderBuilder SetLocateType(string locateType)
        {
            return this;
        }

        public FutureOrderBuilder SetPositionType(string positionType)
        {
            return this;
        }

        public FutureOrderBuilder SetQuantityLimit(string quantityLimit)
        {
            return this;
        }

        public FutureOrderBuilder SetSettlementComment(string settlementComment)
        {
            return this;
        }

        public FutureOrderBuilder SetSettlementCurrency(string settlementCurrency)
        {
            return this;
        }

        public FutureOrderBuilder SetSettlementFxRate(string settlementFxRate)
        {
            return this;
        }

        public FutureOrderBuilder SetSettlementValue(string settlementValue)
        {
            return this;
        }

        public FutureOrderBuilder SetStrategyState(string strategyState)
        {
            return this;
        }

        public FutureOrderBuilder SetSystemOrderId(string systemOrderId)
        {
            return this;
        }

        public FutureOrderBuilder SetTimeInForceType(string timeInForceType)
        {
            return this;
        }
    }
}