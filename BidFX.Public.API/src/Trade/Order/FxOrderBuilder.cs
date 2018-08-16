/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BidFX.Public.API.Enums;
using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Trade.Order
{
    public class FxOrderBuilder : OrderBuilder<FxOrderBuilder, FxOrder>
    {
        public FxOrderBuilder SetCurrencyPair(string currencyPair)
        {
            if (Params.IsNullOrEmpty(currencyPair))
            {
                Components.Remove(FxOrder.CurrencyPair);
                return this;
            }

            currencyPair = Params.ExactLength(currencyPair, 6, "CurrencyPair must be in format 'XXXYYY': " + currencyPair);
            Components[FxOrder.CurrencyPair] = currencyPair;
            return this;
        }

        public FxOrderBuilder SetCurrency(string currency)
        {
            if (Params.IsNullOrEmpty(currency))
            {
                Components.Remove(FxOrder.Currency);
                return this;
            }

            currency = Params.ExactLength(currency, 3, "Currency must be in format 'XXX': " + currency);
            Components[FxOrder.Currency] = currency;
            return this;
        }

        public FxOrderBuilder SetDealType(string dealType)
        {
            if (Params.IsNullOrEmpty(dealType))
            {
                Components.Remove(FxOrder.DealType);
                return this;
            }

            dealType = dealType.Trim();
            switch (dealType.ToLower())
            {
                case "spot":
                    dealType = "SPOT";
                    break;
                case "outright":
                case "forward":
                case "fwd":
                    dealType = "FWD";
                    break;
                case "ndf":
                    dealType = "NDF";
                    break;
                case "swap":
                    dealType = "SWAP";
                    break;
                case "nds":
                    dealType = "NDS";
                    break;
                default:
                    throw new ArgumentException("Unsupported DealType: " + dealType);
            }

            Components[FxOrder.DealType] = dealType;
            return this;
        }

        public FxOrderBuilder SetTenor(string tenor)
        {
            if (Params.IsNullOrEmpty(tenor))
            {
                Components.Remove(FxOrder.Tenor);
                return this;
            }

            FxTenor fxTenor = FxTenor.GetTenor(tenor);
            if (fxTenor != null)
            {
                tenor = fxTenor.GetBizString();
            }
            else
            {
                tenor = tenor.Trim().ToUpper();
            }
            Components[FxOrder.Tenor] = tenor;
            return this;
        }

        public FxOrderBuilder SetSettlementDate(string settlementDate)
        {
            if (Params.IsNullOrEmpty(settlementDate))
            {
                Components.Remove(FxOrder.SettlementDate);
                return this;
            }

            settlementDate = settlementDate.Trim();
            settlementDate = FormatDate("SettlementDate", settlementDate, true);
            Components[FxOrder.SettlementDate] = settlementDate;
            return this;
        }

        public FxOrderBuilder SetFixingDate(string fixingDate)
        {
            if (Params.IsNullOrEmpty(fixingDate))
            {
                Components.Remove(FxOrder.FixingDate);
                return this;
            }

            fixingDate = fixingDate.Trim();
            fixingDate = FormatDate("FixingDate", fixingDate, true);
            Components[FxOrder.FixingDate] = fixingDate;
            return this;
        }

        public FxOrderBuilder SetFarTenor(string farTenor)
        {
            if (Params.IsNullOrEmpty(farTenor))
            {
                Components.Remove(FxOrder.FarTenor);
                return this;
            }

            FxTenor fxTenor = FxTenor.GetTenor(farTenor);
            if (fxTenor != null)
            {
                farTenor = fxTenor.GetRestString();
            }
            else
            {
                farTenor = farTenor.Trim().ToUpper();
            }
            Components[FxOrder.FarTenor] = farTenor;
            return this;
        }

        public FxOrderBuilder SetFarCurrency(string farCurrency)
        {
            if (Params.IsNullOrEmpty(farCurrency))
            {
                Components.Remove(FxOrder.FarCcy);
                return this;
            }

            farCurrency = Params.ExactLength(farCurrency, 3, "FarCurrency must be in format 'AAA': " + farCurrency);
            Components[FxOrder.FarCcy] = farCurrency;
            return this;
        }

        public FxOrderBuilder SetFarSettlementDate(string farSettlementDate)
        {
            if (Params.IsNullOrEmpty(farSettlementDate))
            {
                Components.Remove(FxOrder.FarSettlementDate);
                return this;
            }

            farSettlementDate = farSettlementDate.Trim();
            farSettlementDate = FormatDate("FarSettlementDate", farSettlementDate, true);
            Components[FxOrder.FarSettlementDate] = farSettlementDate;
            return this;
        }

        public FxOrderBuilder SetFarFixingDate(string farFixingDate)
        {
            if (Params.IsNullOrEmpty(farFixingDate))
            {
                Components.Remove(FxOrder.FarFixingDate);
                return this;
            }

            farFixingDate = farFixingDate.Trim();
            farFixingDate = FormatDate("FarFixingDate", farFixingDate, true);
            Components[FxOrder.FarFixingDate] = farFixingDate;
            return this;
        }

        public FxOrderBuilder SetFarQuantity(decimal? farQuantity)
        {
            if (!farQuantity.HasValue)
            {
                Components.Remove(FxOrder.FarQuantity);
                return this;
            }

            if (farQuantity < 0)
            {
                throw new ArgumentException("FarQuantity can not be negative: " + farQuantity);
            }
            
            Components[FxOrder.FarQuantity] = farQuantity.Value;
            return this;
        }

        public FxOrderBuilder SetStrategyParameter(string name, string value)
        {
            Params.NotBlank(name);
            name = name.Trim(); //TODO: Checking on names vs existing properties?
            if (Params.IsNullOrEmpty(value))
            {
                Components.Remove(name);
                return this;
            }

            value = value.Trim();
            Components[name] = value;
            return this;
        }

        public FxOrderBuilder SetFarSide(string side)
        {
            if (Params.IsNullOrEmpty(side))
            {
                Components.Remove(FxOrder.FarSide);
                return this;
            }

            side = side.Trim();
            switch (side.ToUpper())
            {
                case "BUY":
                    side = "BUY";
                    break;
                case "SELL":
                    side = "SELL";
                    break;
                default:
                    throw new ArgumentException("Side must be either 'BUY' or 'SELL': " + side);
            }
            
            Components[FxOrder.FarSide] = side;
            return this;
        }

        public override FxOrder Build()
        {
            return new FxOrder(Components);
        }

        public FxOrderBuilder() : base("FX")
        {
        }
    }
}