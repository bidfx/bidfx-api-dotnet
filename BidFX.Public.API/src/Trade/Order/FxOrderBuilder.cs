using System;
using BidFX.Public.API.Price;

namespace BidFX.Public.API.Trade.Order
{
    public class FxOrderBuilder : Builder<FxOrder>
    {
        private FxOrder _fxOrder = new FxOrder();
        
        public FxOrderBuilder SetCurrencyPair(string currencyPair)
        {
            //TODO
            return this;
        }

        public FxOrderBuilder SetCurrency(string currency)
        {
            //TODO
            return this;
        }

        public FxOrderBuilder SetSide(string side)
        {
            //TODO
            return this;
        }

        public FxOrderBuilder SetQuantity(string quantity)
        {
            //TODO
            return this;
        }

        public FxOrderBuilder SetDealType(string dealType)
        {
            //TODO
            return this;
        }

        public FxOrderBuilder SetTenor(string tenor)
        {
            //TODO
            return this;
        }

        public FxOrderBuilder SetExecutingVenue(string executingVenue)
        {
            //TODO
            return this;
        }

        public FxOrderBuilder SetHandlingType(string handlingType)
        {
            //TODO
            return this;
        }

        public FxOrderBuilder SetAccount(string account)
        {
            //TODO
            return this;
        }

        public FxOrderBuilder SetReference(string reference1, string reference2)
        {
            //TODO
            return this;
        }

        public FxOrderBuilder SetSettlementDate(string settlementDate)
        {
            //TODO
            return this;
        }

        public FxOrderBuilder SetFixingDate(string fixingDate)
        {
            //TODO
            return this;
        }

        public FxOrderBuilder SetFarTenor(string farTenor)
        {
            //TODO
            return this;
        }

        public FxOrderBuilder SetFarCurrency(string farCurrency)
        {
            //TODO
            return this;
        }

        public FxOrderBuilder SetFarSettlementDate(string farSettlementDate)
        {
            //TODO
            return this;
        }

        public FxOrderBuilder SetFarFixingDate(string farFixingDate)
        {
            //TODO
            return this;
        }

        public FxOrderBuilder SetFarQuantity(string farQuantity)
        {
            //TODO
            return this;
        }

        public FxOrderBuilder SetAllocationTemplate(string templateName)
        {
            //TODO
            return this;
        }

        public FxOrderBuilder SetStrategyParameter(string name, string value)
        {
            //TODO
            return this;
        }
        
        public FxOrder Build()
        {
            //TODO
            return null;
        }

        private bool IsNullOrEmpty(string p)
        {
            return p == null || p.Trim().Length == 0;
        }
    }
}