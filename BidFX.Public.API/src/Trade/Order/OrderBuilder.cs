using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Trade.Order
{
    public abstract class OrderBuilder<T, TO> where T : OrderBuilder<T, TO>
        where TO : Order
    {
        protected readonly Dictionary<string, object> Components = new Dictionary<string, object>();

        protected OrderBuilder(string assetClass)
        {
            Components[Order.AssetClass] = assetClass;
        }

        public abstract TO Build();

        public T SetAccount(string account)
        {
            return SetStringField(account, Order.Account);
        }

        public T SetAllocationTemplate(string templateName)
        {
            return SetStringField(templateName, Order.AllocationTemplate);
        }

        public T SetAllocations(List<AllocationTemplateEntry> allocations)
        {
            if (allocations == null || !allocations.Any())
            {
                Components.Remove(Order.Allocations);
                return this as T;
            }

            Components[Order.Allocations] = allocations;
            return this as T;
        }

        public T SetAlternateOwner(string alternateOwner)
        {
            return SetStringField(alternateOwner, Order.AlternateOwner);
        }

        public T SetHandlingType(string handlingType)
        {
            if (Params.IsNullOrEmpty(handlingType))
            {
                Components.Remove(Order.HandlingType);
                return this as T;
            }

            handlingType = handlingType.Trim();
            switch (handlingType.ToUpper())
            {
                case "STREAM":
                case "RFS":
                    handlingType = "RFS";
                    break;
                case "QUOTE":
                case "RFQ":
                    handlingType = "RFQ";
                    break;
                case "AUTOMATIC":
                    handlingType = "AUTOMATIC";
                    break;
                default:
                    throw new ArgumentException("Unsupported handling type: " + handlingType);
            }

            Components[Order.HandlingType] = handlingType;
            return this as T;
        }

        public T SetOrderInstruction(string orderInstruction)
        {
            if (Params.IsNullOrEmpty(orderInstruction))
            {
                Components.Remove(Order.OrderInstruction);
                return this as T;
            }

            orderInstruction = orderInstruction.Trim();
            switch (orderInstruction.ToUpper())
            {
                case "REGISTER":
                    orderInstruction = "REGISTER";
                    break;
                case "SUBMIT":
                    orderInstruction = "SUBMIT";
                    break;
                case "TRADE_ENTRY":
                    orderInstruction = "TRADE_ENTRY";
                    break;
                default:
                    throw new ArgumentException("Unsupported order instruction: " + orderInstruction);
            }

            Components[Order.OrderInstruction] = orderInstruction;
            return this as T;
        }
        
        public T SetOrderType(string orderType)
        {
            return SetStringField(orderType == null ? null : orderType.ToUpper(), Order.OrderType);
        }

        public T SetPrice(decimal? price)
        {
            if (!price.HasValue)
            {
                Components.Remove(Order.Price);
                return this as T;
            }

            if (price < 0)
            {
                throw new ArgumentException("Price can not be negative: " + price);
            }
            
            Components[Order.Price] = price.Value;
            return this as T;
        }

        public T SetQuantity(decimal? quantity)
        {
            if (!quantity.HasValue)
            {
                Components.Remove(Order.Quantity);
                return this as T;
            }

            if (quantity < 0)
            {
                throw new ArgumentException("Quantity can not be negative: " + quantity);
            }
            
            Components[Order.Quantity] = quantity.Value;
            return this as T;
        }

        public T SetReferenceOne(string reference)
        {
            if (Params.IsNullOrEmpty(reference))
            {
                Components.Remove(Order.Reference1);
                return this as T;
            }
            
            if (reference.Contains("|"))
            {
                throw new ArgumentException("References can not contain pipes (|)");
            }

            Components[Order.Reference1] = reference;
            return this as T;
        }

        public T SetReferenceTwo(string reference)
        {
            if (Params.IsNullOrEmpty(reference))
            {
                Components.Remove(Order.Reference2);
                return this as T;
            }

            if (reference.Contains("|"))
            {
                throw new ArgumentException("References can not contain pipes (|)");
            }
            
            Components[Order.Reference2] = reference;
            return this as T;
        }

        public T SetSide(string side)
        {
            if (Params.IsNullOrEmpty(side))
            {
                Components.Remove(Order.Side);
                return this as T;
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
            
            Components[Order.Side] = side;
            return this as T;
        }

        private static readonly Regex FullDateRegex =
            new Regex(@"^\s*(\d\d\d\d)(?:(?:-(1[0-2]|0?[1-9])(?:-([1-2]\d|3[0-1]|0?[1-9]))?)|(?:(1[0-2]|0?[1-9])(3[0-1]|0[1-9]|[1-2]\d)?))\s*$", RegexOptions.Compiled);

       protected static string FormatDate(string fieldName, string date, bool requireDay)
        {
            Match match = FullDateRegex.Match(date);
            if (!match.Success)
            {
                throw new ArgumentException(fieldName + " was not in valid format " +
                                            (requireDay ? "(YYYY-MM-DD)" : "(YYYY-MM)") +
                                            ": " + date);
            }

            GroupCollection groups = match.Groups;
            string year = groups[1].ToString();

            string month = groups[2].ToString();
            if ("".Equals(month))
            {
                month = groups[4].ToString();
            }

            string day = groups[3].ToString();
            if ("".Equals(day))
            {
                day = groups[5].ToString();
            }

            if (requireDay && "".Equals(day))
            {
                throw new ArgumentException(fieldName + " was not in valid format (YYYY-MM-DD): " + date);
            }

            return year + "-"
                        + (month.Length == 2 ? month : "0" + month)
                        + (!"".Equals(day) ? "-" + (day.Length == 2 ? day : "0" + day) : "");
        }

        protected T SetStringField(string field, string fieldName)
        {
            if (Params.IsNullOrEmpty(field))
            {
                Components.Remove(fieldName);
                return this as T;
            }

            Components[fieldName] = field.Trim();
            return this as T;
        }
    }
}