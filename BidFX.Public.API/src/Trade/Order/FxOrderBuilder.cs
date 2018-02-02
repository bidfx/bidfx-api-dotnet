using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Trade.Order
{
    //TODO: Setting null should remove from the dictionary
    public class FxOrderBuilder
    {
        private readonly Dictionary<string, string> _components = new Dictionary<string, string>();

        public FxOrderBuilder SetCurrencyPair(string currencyPair)
        {
            if (Params.IsNullOrEmpty(currencyPair))
            {
                _components.Remove(FxOrder.CurrencyPair);
                return this;
            }

            currencyPair = Params.ExactLength(currencyPair, 6, "Currency Pair must be in format 'AAABBB'");
            _components[FxOrder.CurrencyPair] = currencyPair;
            return this;
        }

        public FxOrderBuilder SetCurrency(string currency)
        {
            if (Params.IsNullOrEmpty(currency))
            {
                _components.Remove(FxOrder.Currency);
                return this;
            }

            currency = Params.ExactLength(currency, 3, "Currency must be in format 'AAA'");
            _components[FxOrder.Currency] = currency;
            return this;
        }

        public FxOrderBuilder SetSide(string side)
        {
            if (Params.IsNullOrEmpty(side))
            {
                _components.Remove(FxOrder.Side);
                return this;
            }

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
            if (Params.IsNullOrEmpty(quantity))
            {
                _components.Remove(FxOrder.Quantity);
                return this;
            }

            quantity = Params.Trim(quantity);
            if (!Params.IsNumeric(quantity))
            {
                throw new ArgumentException("Quantity is not a number: " + quantity);
            }

            _components[FxOrder.Quantity] = quantity;
            return this;
        }

        public FxOrderBuilder SetDealType(string dealType)
        {
            if (Params.IsNullOrEmpty(dealType))
            {
                _components.Remove(FxOrder.DealType);
                return this;
            }

            dealType = Params.Trim(dealType);
            switch (dealType.ToLower())
            {
                case "spot":
                    dealType = "Spot";
                    break;
                case "outright":
                case "forward":
                    dealType = "Outright";
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
                    throw new ArgumentException("Unsupported deal type: " + dealType);
            }

            _components[FxOrder.DealType] = dealType;
            return this;
        }

        public FxOrderBuilder SetTenor(string tenor)
        {
            if (Params.IsNullOrEmpty(tenor))
            {
                _components.Remove(FxOrder.Tenor);
                return this;
            }

            tenor = Params.Trim(tenor);
            _components[FxOrder.Tenor] = tenor;
            return this;
        }

        public FxOrderBuilder
            SetExecutingVenue(string executingVenue) //TODO: REST API is hardcoding this to TS-SS, should we offer this?
        {
            if (Params.IsNullOrEmpty(executingVenue))
            {
                _components.Remove(FxOrder.ExecutingVenue);
                return this;
            }

            executingVenue = Params.Trim(executingVenue);
            _components[FxOrder.ExecutingVenue] = executingVenue;
            return this;
        }

        public FxOrderBuilder SetHandlingType(string handlingType)
        {
            if (Params.IsNullOrEmpty(handlingType))
            {
                _components.Remove(FxOrder.HandlingType);
                return this;
            }

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
                    throw new ArgumentException("Unsupported handling type: " + handlingType);
            }

            _components[FxOrder.HandlingType] = handlingType;
            return this;
        }

        public FxOrderBuilder SetAccount(string account)
        {
            if (Params.IsNullOrEmpty(account))
            {
                _components.Remove(FxOrder.Account);
                return this;
            }

            account = Params.Trim(account);
            _components[FxOrder.Account] = account;
            return this;
        }

        public FxOrderBuilder SetSettlementDate(string settlementDate)
        {
            if (Params.IsNullOrEmpty(settlementDate))
            {
                _components.Remove(FxOrder.SettlementDate);
                return this;
            }

            settlementDate = Params.Trim(settlementDate);
            settlementDate = FormatDates(settlementDate);
            _components[FxOrder.SettlementDate] = settlementDate;
            return this;
        }

        public FxOrderBuilder SetFixingDate(string fixingDate)
        {
            if (Params.IsNullOrEmpty(fixingDate))
            {
                _components.Remove(FxOrder.FixingDate);
                return this;
            }

            fixingDate = Params.Trim(fixingDate);
            fixingDate = FormatDates(fixingDate);
            _components[FxOrder.FixingDate] = fixingDate;
            return this;
        }

        public FxOrderBuilder SetFarTenor(string farTenor)
        {
            if (Params.IsNullOrEmpty(farTenor))
            {
                _components.Remove(FxOrder.FarTenor);
                return this;
            }

            farTenor = Params.Trim(farTenor);
            _components[FxOrder.FarTenor] = farTenor;
            return this;
        }

        public FxOrderBuilder SetFarCurrency(string farCurrency)
        {
            if (Params.IsNullOrEmpty(farCurrency))
            {
                _components.Remove(FxOrder.FarCurrency);
                return this;
            }

            farCurrency = Params.ExactLength(farCurrency, 3, "farCurrency must be in format 'AAA'");
            _components[FxOrder.FarCurrency] = farCurrency;
            return this;
        }

        public FxOrderBuilder SetFarSettlementDate(string farSettlementDate)
        {
            if (Params.IsNullOrEmpty(farSettlementDate))
            {
                _components.Remove(FxOrder.FarSettlementDate);
                return this;
            }

            farSettlementDate = Params.Trim(farSettlementDate);
            farSettlementDate = FormatDates(farSettlementDate);
            _components[FxOrder.FarSettlementDate] = farSettlementDate;
            return this;
        }

        public FxOrderBuilder SetFarFixingDate(string farFixingDate)
        {
            if (Params.IsNullOrEmpty(farFixingDate))
            {
                _components.Remove(FxOrder.FarFixingDate);
                return this;
            }

            farFixingDate = Params.Trim(farFixingDate);
            farFixingDate = FormatDates(farFixingDate);
            _components[FxOrder.FarFixingDate] = farFixingDate;
            return this;
        }

        public FxOrderBuilder SetFarQuantity(string farQuantity)
        {
            if (Params.IsNullOrEmpty(farQuantity))
            {
                _components.Remove(FxOrder.FarQuantity);
                return this;
            }

            farQuantity = Params.Trim(farQuantity);
            if (!Params.IsNumeric(farQuantity))
            {
                throw new ArgumentException("FarQuantity was not a number: " + farQuantity);
            }

            _components[FxOrder.FarQuantity] = farQuantity;
            return this;
        }

        public FxOrderBuilder
            SetAllocationTemplate(
                string templateName) //TODO: Is this a template name or a set of allocation accounts + quantities?
        {
            if (Params.IsNullOrEmpty(templateName))
            {
                _components.Remove(FxOrder.AllocationTemplate);
                return this;
            }

            templateName = Params.Trim(templateName);
            _components[FxOrder.AllocationTemplate] = templateName;
            return this;
        }

        public FxOrderBuilder SetPrice(string price)
        {
            if (Params.IsNullOrEmpty(price))
            {
                _components.Remove(FxOrder.Price);
                return this;
            }

            price = Params.Trim(price);
            if (!Params.IsNumeric(price))
            {
                throw new ArgumentException("Price is not a number: " + price);
            }

            _components[FxOrder.Price] = price;
            return this;
        }

        public FxOrderBuilder SetPriceType(string priceType)
        {
            if (Params.IsNullOrEmpty(priceType))
            {
                _components.Remove(FxOrder.PriceType);
                return this;
            }

            priceType = Params.Trim(priceType);
            _components[FxOrder.PriceType] = priceType;
            return this;
        }

        public FxOrderBuilder SetReferenceOne(string referenceOne)
        {
            if (Params.IsNullOrEmpty(referenceOne))
            {
                _components.Remove(FxOrder.Reference1);
                return this;
            }

            if (referenceOne.Contains("|"))
            {
                throw new ArgumentException("References can not contain pipes (|)");
            }

            _components[FxOrder.Reference1] = referenceOne;
            return this;
        }

        public FxOrderBuilder SetReferenceTwo(string referenceTwo)
        {
            if (Params.IsNullOrEmpty(referenceTwo))
            {
                _components.Remove(FxOrder.Reference2);
                return this;
            }

            if (referenceTwo.Contains("|"))
            {
                throw new ArgumentException("References can not contain pipes (|)");
            }

            _components[FxOrder.Reference2] = referenceTwo;
            return this;
        }

        public FxOrderBuilder SetStrategyParameter(string name, string value)
        {
            Params.NotBlank(name);
            name = Params.Trim(name); //TODO: Checking on names vs existing properties?
            if (Params.IsNullOrEmpty(value))
            {
                _components.Remove(name);
                return this;
            }

            value = Params.Trim(value);
            _components[name] = value;
            return this;
        }

        public FxOrder Build()
        {
            string[] internalComponents = new string[_components.Count * 2];
            int i = 0;
            IOrderedEnumerable<KeyValuePair<string, string>> components = _components.OrderBy(kvp => kvp.Key);
            foreach (KeyValuePair<string, string> component in components)
            {
                internalComponents[i++] = component.Key;
                internalComponents[i++] = component.Value;
            }

            return new FxOrder(internalComponents);
        }

        private static readonly Regex DateRegex =
            new Regex(@"^(\d\d\d\d)(?:(?:-([0-1]?\d)-([0-3]?\d))|([0-1]?\d)([0-3]?\d))$", RegexOptions.Compiled);

        private static string FormatDates(string date)
        {
            Match match = DateRegex.Match(date);
            if (!match.Success)
            {
                throw new ArgumentException("Date was not in valid format (YYYYY-MM-DD): " + date);
            }

            GroupCollection groups = match.Groups;
            string year = groups[1].ToString();

            string month = groups[2].ToString();
            if (month == "")
            {
                month = groups[4].ToString();
            }

            string day = groups[3].ToString();
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