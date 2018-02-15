using System.Collections.Generic;

namespace BidFX.Public.API.Trade.Order
{
    /// <summary>
    /// An immutable representation of an FXOrder. An FXOrderBuilder should be used to create one.
    /// </summary>
    public class FxOrder
    {
        public const string CurrencyPair = "ccy_pair";
        public const string Currency = "dealt_ccy";
        public const string Side = "side";
        public const string Quantity = "quantity";
        public const string DealType = "deal_type";
        public const string Tenor = "tenor";
        public const string ExecutingVenue = "executing_venue";
        public const string HandlingType = "handling_type";
        public const string Account = "account";
        public const string SettlementDate = "settlement_date";
        public const string FixingDate = "fixing_date";
        public const string Reference1 = "reference";
        public const string Reference2 = "reference2";
        public const string FarTenor = "far_tenor";
        public const string FarCurrency = "far_dealt_ccy";
        public const string FarSettlementDate = "far_settlement_date";
        public const string FarFixingDate = "far_fixing_date";
        public const string FarQuantity = "far_quantity";
        public const string AllocationTemplate = "allocation_template";
        public const string Price = "price";
        public const string PriceType = "price_type";

        private readonly string[] _components;
        private Dictionary<string, string> _componentDictionary;

        internal FxOrder(string[] components)
        {
            _components = components;
        }

        /// <summary>
        /// Dangerous method: returns the internal components. Should only be used for marshalling.
        /// </summary>
        internal string[] GetInternalComponents()
        {
            return _components;
        }

        private void GenerateComponentDictionary()
        {
            if (_componentDictionary == null)
            {
                _componentDictionary = new Dictionary<string, string>();
                int i = 0;
                while (i < _components.Length)
                {
                    string key = _components[i++];
                    string value = _components[i++];
                    _componentDictionary[key] = value;
                }
            }
        }

        public string GetCurrencyPair()
        {
            GenerateComponentDictionary();
            return _componentDictionary[CurrencyPair];
        }

        public string GetCurrency()
        {
            GenerateComponentDictionary();
            return _componentDictionary[Currency];
        }

        public string GetSide()
        {
            GenerateComponentDictionary();
            return _componentDictionary[Side];
        }

        public string GetQuantity()
        {
            GenerateComponentDictionary();
            return _componentDictionary[Quantity];
        }

        public string GetDealType()
        {
            GenerateComponentDictionary();
            return _componentDictionary[DealType];
        }

        public string GetTenor()
        {
            GenerateComponentDictionary();
            return _componentDictionary[Tenor];
        }

        public string GetExecutingVenue()
        {
            GenerateComponentDictionary();
            return _componentDictionary[ExecutingVenue];
        }

        public string GetHandlingType()
        {
            GenerateComponentDictionary();
            return _componentDictionary[HandlingType];
        }

        public string GetAccount()
        {
            GenerateComponentDictionary();
            return _componentDictionary[Account];
        }

        public string GetReference1()
        {
            GenerateComponentDictionary();
            return _componentDictionary[Reference1];
        }

        public string GetReference2()
        {
            GenerateComponentDictionary();
            return _componentDictionary[Reference2];
        }

        public string GetSettlementDate()
        {
            GenerateComponentDictionary();
            return _componentDictionary[SettlementDate];
        }

        public string GetFixingDate()
        {
            GenerateComponentDictionary();
            return _componentDictionary[FixingDate];
        }

        public string GetFarTenor()
        {
            GenerateComponentDictionary();
            return _componentDictionary[FarTenor];
        }

        public string GetFarCurrency()
        {
            GenerateComponentDictionary();
            return _componentDictionary[FarCurrency];
        }

        public string GetFarSettlementDate()
        {
            GenerateComponentDictionary();
            return _componentDictionary[FarSettlementDate];
        }

        public string GetFarFixingDate()
        {
            GenerateComponentDictionary();
            return _componentDictionary[FarFixingDate];
        }

        public string GetFarQuantity()
        {
            GenerateComponentDictionary();
            return _componentDictionary[FarQuantity];
        }

        public string GetPrice()
        {
            GenerateComponentDictionary();
            return _componentDictionary[Price];
        }

        public string GetAllocationTemplate()
        {
            GenerateComponentDictionary();
            return _componentDictionary[AllocationTemplate];
        }

        public string GetStrategyParameter(string parameterName)
        {
            GenerateComponentDictionary();
            return _componentDictionary[parameterName];
        }

        public string GetPriceType()
        {
            GenerateComponentDictionary();
            return _componentDictionary[PriceType];
        }
    }
}