/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace BidFX.Public.API.Trade.Order
{
    /// <summary>
    /// An immutable representation of an FXOrder. An FXOrderBuilder should be used to create one.
    /// </summary>
    public class FxOrder : Order
    {
        public const string CurrencyPair = "ccy_pair";
        public const string DealType = "deal_type";
        public const string Ccy = "dealt_ccy";
        public const string Tenor = "tenor";
        public const string FixingDate = "fixing_date";
        public const string FarTenor = "far_tenor";
        public const string FarCcy = "far_dealt_ccy";
        public const string FarSettlementDate = "far_settlement_date";
        public const string FarFixingDate = "far_fixing_date";
        public const string FarQuantity = "far_quantity";
        public const string FarSide = "far_side";

        internal FxOrder(Dictionary<string, object> components) : base(components)
        {
        }
        
        public string GetCurrencyPair()
        {
            return GetComponent<string>(CurrencyPair);
        }

        public string GetDealType()
        {
            return GetComponent<string>(DealType);
        }

        public string GetCurrency()
        {
            return GetComponent<string>(Ccy);
        }

        public string GetFarCurrency()
        {
            return GetComponent<string>(FarCcy);
        }

        public string GetFixingDate()
        {
            return GetComponent<string>(FixingDate);
        }

        public string GetFarFixingDate()
        {
            return GetComponent<string>(FarFixingDate);
        }

        public decimal? GetFarQuantity()
        {
            return GetComponent<decimal?>(FarQuantity);
        }

        public string GetFarSettlementDate()
        {
            return GetComponent<string>(FarSettlementDate);
        }

        public string GetFarTenor()
        {
            return GetComponent<string>(FarTenor);
        }

        public string GetTenor()
        {
            return GetComponent<string>(Tenor);
        }
        
        public string GetStrategyParameter(string parameterName)
        {
            return GetComponent<string>(parameterName);
        }

        public string GetFarSide()
        {
            return GetComponent<string>(FarSide);
        }

        public override string ToString()
        {
            return "[FxOrder: " + base.ToString() + "]";
        }
    }
}