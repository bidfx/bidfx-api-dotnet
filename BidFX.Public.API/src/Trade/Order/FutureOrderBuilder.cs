using System;
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
            return SetStringField(instrumentCode, FutureOrder.InstrumentCode);
        }

        public FutureOrderBuilder SetInstrumentCodeType(string instrumentCodeType)
        {
            if (Params.IsNullOrEmpty(instrumentCodeType))
            {
                Components.Remove(FutureOrder.InstrumentCodeType);
                return this;
            }

            instrumentCodeType = instrumentCodeType.Trim();
            switch (instrumentCodeType.ToUpper())
            {
                    case "RIC":
                        instrumentCodeType = "RIC";
                        break;
                    case "BLOOMBERG":
                        instrumentCodeType = "BLOOMBERG";
                        break;
                    case "ISIN":
                        instrumentCodeType = "ISIN";
                        break;
                    case "SEDOL":
                        instrumentCodeType = "SEDOL";
                        break;
                    case "CUSIP":
                        instrumentCodeType = "CUSIP";
                        break;
                    case "VALOREN":
                        instrumentCodeType = "VALOREN";
                        break;
                    case "TSPRODUCTID":
                        instrumentCodeType = "TSPRODUCTID";
                        break;
                    case "TICKER":
                        instrumentCodeType = "TICKER";
                        break;
                    default:
                        throw new ArgumentException("Unsupported Instrument Code Type: " + instrumentCodeType);
            }

            Components[FutureOrder.InstrumentCodeType] = instrumentCodeType;
            return this;
        }

        public FutureOrderBuilder SetVenueCode(string venueCode)
        {
            return SetStringField(venueCode, FutureOrder.VenueCode);
        }

        public FutureOrderBuilder SetVenueCodeType(string venueCodeType)
        {
            return SetStringField(venueCodeType, FutureOrder.VenueCodeType);
        }

        public FutureOrderBuilder SetAggregationLevelOne(string aggregationLevel)
        {
            return SetStringField(aggregationLevel, FutureOrder.AggregationLevelOne);
        }

        public FutureOrderBuilder SetAggregationLevelTwo(string aggregationLevel)
        {
            return SetStringField(aggregationLevel, FutureOrder.AggregationLevelTwo);
        }

        public FutureOrderBuilder SetAggregationLevelThree(string aggregationLevel)
        {
            return SetStringField(aggregationLevel, FutureOrder.AggregationLevelThree);
        }

        public FutureOrderBuilder SetAlgoParent(string algoParent)
        {
            return SetStringField(algoParent, FutureOrder.AlgoParent);
        }

        public FutureOrderBuilder SetClearingAccount(string clearingAccount)
        {
            return SetStringField(clearingAccount, FutureOrder.ClearingAccount);
        }

        public FutureOrderBuilder SetClearingBroker(string clearingBroker)
        {
            return SetStringField(clearingBroker, FutureOrder.ClearingBroker);
        }

        public FutureOrderBuilder SetCreationDate(string creationDate)
        {
            if (Params.IsNullOrEmpty(creationDate))
            {
                Components.Remove(FutureOrder.CreationDate);
                return this;
            }

            Components[FutureOrder.CreationDate] = FormatDate(FutureOrder.CreationDate, creationDate, true);
            return this;
        }

        public FutureOrderBuilder SetDescription(string description)
        {
            return SetStringField(description, FutureOrder.Description);
        }

        public FutureOrderBuilder SetExecutingBroker(string executingBroker)
        {
            return SetStringField(executingBroker, FutureOrder.ExecutingBroker);
        }

        public FutureOrderBuilder SetGoodTillDate(string goodTillDate)
        {
            
            if (Params.IsNullOrEmpty(goodTillDate))
            {
                Components.Remove(FutureOrder.GoodTillDate);
                return this;
            }

            Components[FutureOrder.GoodTillDate] = FormatDate(FutureOrder.GoodTillDate, goodTillDate, true);
            return this;
        }

        public FutureOrderBuilder SetGroupingOrderId(string groupingOrderId)
        {
            return SetStringField(groupingOrderId, FutureOrder.GroupingOrderId);
        }

        public FutureOrderBuilder SetHandlingComment(string handlingComment)
        {
            return SetStringField(handlingComment, FutureOrder.HandlingComment);
        }

        public FutureOrderBuilder SetHandlingExecutingInstruction(string handlingExecutingInstruction)
        {
            return SetStringField(handlingExecutingInstruction, FutureOrder.HandlingExecutingInstruction);
        }

        public FutureOrderBuilder SetHedgeCurrency(string hedgeCurrency)
        {
            return SetStringField(hedgeCurrency, FutureOrder.HedgeCurrency);
        }

        public FutureOrderBuilder SetLocateBroker(string locateBroker)
        {
            return SetStringField(locateBroker, FutureOrder.LocateBroker);
        }

        public FutureOrderBuilder SetLocateId(string locateId)
        {
            return SetStringField(locateId, FutureOrder.LocateId);
        }

        public FutureOrderBuilder SetLocateType(string locateType)
        {
            return SetStringField(locateType, FutureOrder.LocateType);
        }

        public FutureOrderBuilder SetQuantityLimit(decimal? quantityLimit)
        {
            if (!quantityLimit.HasValue)
            {
                Components.Remove(FutureOrder.QuantityLimit);
                return this;
            }

            Components[FutureOrder.QuantityLimit] = quantityLimit.Value;
            return this;
        }

        public FutureOrderBuilder SetSettlementComment(string settlementComment)
        {
            return SetStringField(settlementComment, FutureOrder.SettlementComment);
        }

        public FutureOrderBuilder SetSettlementCurrency(string settlementCurrency)
        {
            return SetStringField(settlementCurrency, FutureOrder.SettlementCurrency);
        }

        public FutureOrderBuilder SetSettlementFxRate(decimal? settlementFxRate)
        {
            if (!settlementFxRate.HasValue)
            {
                Components.Remove(FutureOrder.SettlementFxRate);
                return this;
            }

            Components[FutureOrder.SettlementFxRate] = settlementFxRate.Value;
            return this;
        }

        public FutureOrderBuilder SetSettlementValue(decimal? settlementValue)
        {
            if (!settlementValue.HasValue)
            {
                Components.Remove(FutureOrder.SettlementValue);
                return this;
            }

            Components[FutureOrder.SettlementValue] = settlementValue.Value;
            return this;
        }

        public FutureOrderBuilder SetStrategyParentOrderId(string parentOrderId)
        {
            return SetStringField(parentOrderId, FutureOrder.StrategyParentOrderId);
        }
        
        public FutureOrderBuilder SetStrategyState(string strategyState)
        {
            return SetStringField(strategyState, FutureOrder.StrategyState);
        }

        public FutureOrderBuilder SetSystemOrderId(string systemOrderId)
        {
            return SetStringField(systemOrderId, FutureOrder.SystemOrderId);
        }

        public FutureOrderBuilder SetTimeInForceType(string timeInForceType)
        {
            return SetStringField(timeInForceType, FutureOrder.TimeInForceType);
        }
    }
}