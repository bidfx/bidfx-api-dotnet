using System.Collections.Generic;
using BidFX.Public.API.Price;

namespace BidFX.Public.API.Trade.Order
{
    public class FxOrder
    {
        private bool _immutable = false;

        private string _currencyPair;
        private string _currency;
        private string _side;
        private string _quantity;
        private string _dealType;
        private string _tenor;
        private string _executingVenue;
        private string _handlingType;
        private string _account;
        private string _reference1;
        private string _reference2;
        private string _settlementDate;
        private string _fixingDate;
        private string _farTenor;
        private string _farCurrency;
        private string _farSettlementDate;
        private string _farFixingDate;
        private string _farQuantity;
        private string _allocationTemplate;
        private Dictionary<string, string> _strategyParameters;

        internal void SetCurrencyPair(string currencyPair)
        {
            if(_immutable)
            {
                throw new IllegalStateException("FxOrder has been made immutable");
            }
            _currencyPair = currencyPair;
        }

        internal void SetCurrency(string currency)
        {
            if(_immutable)
            {
                throw new IllegalStateException("FxOrder has been made immutable");
            }
            _currency = currency;
        }

        internal void SetSide(string side)
        {
            if(_immutable)
            {
                throw new IllegalStateException("FxOrder has been made immutable");
            }
            _side = side;
        }

        internal void SetQuantity(string quantity)
        {
            if(_immutable)
            {
                throw new IllegalStateException("FxOrder has been made immutable");
            }
            _quantity = quantity;
        }

        internal void SetDealType(string dealType)
        {
            if(_immutable)
            {
                throw new IllegalStateException("FxOrder has been made immutable");
            }
            _dealType = dealType;
        }

        internal void SetTenor(string tenor)
        {
            if(_immutable)
            {
                throw new IllegalStateException("FxOrder has been made immutable");
            }
            _tenor = tenor;
        }

        internal void SetExecutingVenue(string executingVenue) //TODO: REST API is hardcoding this to TS-SS
        {
            if(_immutable)
            {
                throw new IllegalStateException("FxOrder has been made immutable");
            }
            _executingVenue = executingVenue;
        }

        internal void SetHandlingType(string handlingType)
        {
            if(_immutable)
            {
                throw new IllegalStateException("FxOrder has been made immutable");
            }
            _handlingType = handlingType;
        }

        internal void SetAccount(string executingAccount)
        {
            if(_immutable)
            {
                throw new IllegalStateException("FxOrder has been made immutable");
            }
            _account = executingAccount;
        }

        internal void SetReference1(string reference1)
        {
            if(_immutable)
            {
                throw new IllegalStateException("FxOrder has been made immutable");
            }
            _reference1 = reference1;
        }

        internal void SetReference2(string reference2)
        {
            if(_immutable)
            {
                throw new IllegalStateException("FxOrder has been made immutable");
            }
            _reference2 = reference2;
        }

        internal void SetSettlementDate(string settlementDate)
        {
            if(_immutable)
            {
                throw new IllegalStateException("FxOrder has been made immutable");
            }
            _settlementDate = settlementDate;
        }

        internal void SetFixingDate(string fixingDate)
        {
            if(_immutable)
            {
                throw new IllegalStateException("FxOrder has been made immutable");
            }
            _fixingDate = fixingDate;
        }

        internal void SetFarTenor(string farTenor)
        {
            if(_immutable)
            {
                throw new IllegalStateException("FxOrder has been made immutable");
            }
            _farTenor = farTenor;
        }

        internal void SetFarCurrency(string farCurrency)
        {
            if(_immutable)
            {
                throw new IllegalStateException("FxOrder has been made immutable");
            }
            _farCurrency = farCurrency;
        }

        internal void SetFarSettlementDate(string farSettlementDate)
        {
            if(_immutable)
            {
                throw new IllegalStateException("FxOrder has been made immutable");
            }
            _farSettlementDate = farSettlementDate;
        }

        internal void SetFarFixingDate(string farFixingDate)
        {
            if(_immutable)
            {
                throw new IllegalStateException("FxOrder has been made immutable");
            }
            _farFixingDate = farFixingDate;
        }

        internal void SetFarQuantity(string farQuantity)
        {
            if(_immutable)
            {
                throw new IllegalStateException("FxOrder has been made immutable");
            }
            _farQuantity = farQuantity;
        }

        internal void SetAllocationTemplate(string allocationTemplate)
        {
            if(_immutable)
            {
                throw new IllegalStateException("FxOrder has been made immutable");
            }
            _allocationTemplate = allocationTemplate;
        }

        internal void SetStrategyParameter(string name, string value)
        {
            if (_immutable)
            {
                throw new IllegalStateException("FxOrder has been made immutable");
            }

            if (_strategyParameters == null)
            {
                _strategyParameters = new Dictionary<string, string>();
            }
            
            _strategyParameters[name] = value;
        }
        
        /// <summary>
        /// Make the FxOrder immutable
        /// </summary>
        public void Freeze()
        {
            _immutable = true;
        }
        
        public string GetCurrencyPair()

        {
            return _currencyPair;
        }

        public string GetCurrency()
        {
            return _currency;
        }

        public string GetSide()
        {
            return _side;
        }

        public string GetQuantity()
        {
            return _quantity;
        }

        public string GetDealType()
        {
            return _dealType;
        }

        public string GetTenor()
        {
            return _tenor;
        }

        public string GetExecutingVenue()
        {
            return _executingVenue;
        }

        public string GetHandlingType()
        {
            return _handlingType;
        }

        public string GetAccount()
        {
            return _account;
        }

        public string GetReference1()
        {
            return _reference1;
        }

        public string GetReference2()
        {
            return _reference2;
        }

        public string GetSettlementDate()
        {
            return _settlementDate;
        }

        public string GetFixingDate()
        {
            return _fixingDate;
        }

        public string GetFarTenor()
        {
            return _farTenor;
        }

        public string GetFarCurrency()
        {
            return _farCurrency;
        }

        public string GetFarSettlementDate()
        {
            return _farSettlementDate;
        }

        public string GetFarFixingDate()
        {
            return _farFixingDate;
        }

        public string GetFarQuantity()
        {
            return _farQuantity;
        }

        public string GetAllocationTemplate()
        {
            return _allocationTemplate;
        }

        public Dictionary<string, string> GetStrategyParameters()
        {
            if (_strategyParameters == null)
            {
                return null;
            }
            return new Dictionary<string, string>(_strategyParameters);
        }
    }
}