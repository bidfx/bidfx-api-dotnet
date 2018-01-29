using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Trade.Order
{
    public class FxOrderBuilder
    {
        
        private static readonly Regex DateRegex = new Regex(@"^(\d\d\d\d)(?:(?:-([0-1]?\d)-([0-3]?\d))|([0-1]?\d)([0-3]?\d))$", RegexOptions.Compiled);
        
        
        private readonly Dictionary<string, string> _components = new Dictionary<string, string>();
        
        public FxOrderBuilder SetCurrencyPair(string currencyPair)
        {
            Params.NotNull(currencyPair, "currency pair must be provided"); //TODO: more informative error message
            currencyPair = currencyPair.Trim();
            if (currencyPair.Length != 6)
            {
                throw new ArgumentException("currency pair must be provided"); //TODO: more informative error message
            }
            _components[FxOrder.CurrencyPair] = currencyPair;
            return this;
        }

        public FxOrderBuilder SetCurrency(string currency)
        {
            currency = Params.ExactLength(currency, 3, "currency must be provided"); //TODO: more informative error message
            _components[FxOrder.Currency] = currency;
            return this;
        }

        public FxOrderBuilder SetSide(string side)
        {
            Params.NotBlank(side); //TODO: more informative error message
            side = Params.Trim(side);
            switch (side.ToLower())
            {
                case "buy":
                    side = "Buy";
                    break;
                case "sell":
                    side = "Sell";
                    break;
                default:
                    throw new ArgumentException();
            }
            _components[FxOrder.Side] = side;
            return this;
        }

        public FxOrderBuilder SetQuantity(string quantity)
        {
            Params.NotBlank(quantity); //TODO: more informative error message
            quantity = Params.Trim(quantity);
            if (!Params.IsNumeric(quantity))
            {
                throw new ArgumentException(); //TODO: more informative error message
            }

            _components[FxOrder.Quantity] = quantity;
            return this;
        }

        public FxOrderBuilder SetDealType(string dealType)
        {
            Params.NotBlank(dealType); //TODO: more informative error message
            dealType = Params.Trim(dealType);
            switch (dealType.ToLower())
            {
                    case "spot":
                        dealType = "Spot";
                        break;
                    case "forward":
                        dealType = "Forward";
                        break;
                    case "ndf":
                        dealType = "NDF";
                        break;
                    case "swap":
                        dealType = "Swap";
                        break;
                    case "nds":
                        dealType = "NDS";
                        break;
                    default:
                        throw new ArgumentException("unsupported deal type: " + dealType);
            }
            _components[FxOrder.DealType] = dealType;
            return this;
        }

        public FxOrderBuilder SetTenor(string tenor)
        {
            Params.NotBlank(tenor); //TODO: more informative error message
            tenor = Params.Trim(tenor);
            _components[FxOrder.Tenor] = tenor;
            return this;
        }

        public FxOrderBuilder SetExecutingVenue(string executingVenue) //TODO: REST API is hardcoding this to TS-SS
        {
            Params.NotBlank(executingVenue);
            executingVenue = Params.Trim(executingVenue);
            _components[FxOrder.ExecutingVenue] = executingVenue;
            return this;
        }

        public FxOrderBuilder SetHandlingType(string handlingType)
        {
            Params.NotBlank(handlingType); //TODO: more informative error message
            handlingType = Params.Trim(handlingType);
            switch (handlingType.ToLower())
            {
                case "stream":
                    handlingType = "stream";
                    break;
                case "quote":
                    handlingType = "quote";
                    break;
                case "automatic":
                    handlingType = "automatic";
                    break;
                default:
                    throw new ArgumentException("unsupported handling type: " + handlingType);
            }
            _components[FxOrder.HandlingType] = handlingType;
            return this;
        }

        public FxOrderBuilder SetAccount(string account)
        {
            Params.NotBlank(account);
            account = Params.Trim(account);
            _components[FxOrder.Account] = account;
            return this;
        }

        public FxOrderBuilder SetReference(string reference1, string reference2)
        {
            Params.NotNull(reference1);
            Params.NotNull(reference2);
            if (reference1.Contains("|") || reference2.Contains("|"))
            {
                throw new ArgumentException("references can not contain pipes (|)");
            }
            _components[FxOrder.Reference1] = reference1;
            _components[FxOrder.Reference2] = reference2;
            return this;
        }

        public FxOrderBuilder SetSettlementDate(string settlementDate)
        {
            Params.NotNull(settlementDate); //TODO: more informative error message
            settlementDate = Params.Trim(settlementDate);
            settlementDate = FormatDates(settlementDate);
            _components[FxOrder.SettlementDate] = settlementDate;
            return this;
        }

        public FxOrderBuilder SetFixingDate(string fixingDate)
        {
            Params.NotBlank(fixingDate); //TODO: more informative error message
            fixingDate = Params.Trim(fixingDate);
            fixingDate = FormatDates(fixingDate);
            _components[FxOrder.FixingDate] = fixingDate;
            return this;
        }

        public FxOrderBuilder SetFarTenor(string farTenor)
        {
            Params.NotBlank(farTenor); //TODO: more informative error message
            farTenor = Params.Trim(farTenor);
            _components[FxOrder.FarTenor] = farTenor;
            return this;
        }

        public FxOrderBuilder SetFarCurrency(string farCurrency)
        {
            farCurrency = Params.ExactLength(farCurrency, 3, "farCurrency must be supplied"); //TODO: more informative error message
            _components[FxOrder.FarCurrency] = farCurrency;
            return this;
        }

        public FxOrderBuilder SetFarSettlementDate(string farSettlementDate)
        {
            Params.NotBlank(farSettlementDate);
            farSettlementDate = Params.Trim(farSettlementDate);
            farSettlementDate = FormatDates(farSettlementDate);
            _components[FxOrder.FarSettlementDate] = farSettlementDate;
            return this;
        }

        public FxOrderBuilder SetFarFixingDate(string farFixingDate)
        {
            Params.NotBlank(farFixingDate);
            farFixingDate = Params.Trim(farFixingDate);
            farFixingDate = FormatDates(farFixingDate);
            _components[FxOrder.FarFixingDate] = farFixingDate;
            return this;
        }

        public FxOrderBuilder SetFarQuantity(string farQuantity)
        {
            Params.NotBlank(farQuantity); //TODO: more informative error message
            farQuantity = Params.Trim(farQuantity);
            if (!Params.IsNumeric(farQuantity))
            {
                throw new ArgumentException(); //TODO: more informative error message
            }
            _components[FxOrder.FarQuantity] = farQuantity;
            return this;
        }

        public FxOrderBuilder SetAllocationTemplate(string templateName) //TODO: Is this a template name or a set of allocation accounts + quantities?
        {
            Params.NotBlank(templateName);
            templateName = Params.Trim(templateName);
            _components[FxOrder.AllocationTemplate] = templateName;
            return this;
        }

        public FxOrderBuilder SetStrategyParameter(string name, string value)
        {
            Params.NotBlank(name);
            Params.NotNull(value);
            name = Params.Trim(name); //TODO: Checking on names?
            value = Params.Trim(value);
            _components[name] = value;
            return this;
        }
        
        public FxOrder Build()
        {
            var components = new string[_components.Count * 2];
            var i = 0;
            foreach (var component in _components)
            {
                components[i++] = component.Key;
                components[i++] = component.Value;
            }
            return new FxOrder(components);
        }

        private static string FormatDates(string date)
        {
            var match = DateRegex.Match(date);
            if (!match.Success)
            {
                throw new ArgumentException();
            }
            var groups = match.Groups;
            var year = groups[1].ToString();
            
            var month = groups[2].ToString();
            if (month == "")
            {
                month = groups[4].ToString();
            }

            var day = groups[3].ToString();
            if (day == "")
            {
                day = groups[5].ToString();
            }
            
            return year + "-" 
                        + (month.Length == 2 ? month : "0" + month)
                        + "-"
                        + (day.Length == 2 ? day : "0" + day);
        }
    }
}