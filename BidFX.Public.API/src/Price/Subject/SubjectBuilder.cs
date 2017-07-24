using System;
using System.Collections;
using System.Collections.Generic;

namespace BidFX.Public.API.Price.Subject
{
    /// <summary>
    /// This class provides a mutable subject builder.
    /// </summary>
    public class SubjectBuilder : IComponentHandler, IEnumerable<SubjectComponent>
    {
        private string[] _components = new string[16];
        private int _size = 0;
        
        /// <summary>
        /// Whether the subscription should refresh if it can expire
        /// </summary>
        public bool AutoRefresh { get; set; }

        /// <summary>
        /// Creates a new subject from the components that have been set.
        /// </summary>
        /// <returns></returns>
        public Subject CreateSubject()
        {
            return new Subject(GetComponents(), AutoRefresh);
        }

        /// <summary>
        /// Sets a component of the subject.
        /// </summary>
        /// <param name="key">the components key</param>
        /// <param name="value">the components value</param>
        /// <returns>the builder so that calls can be chained</returns>
        public SubjectBuilder SetComponent(string key, string value)
        {
            SubjectComponent(key, value);
            return this;
        }

        public void SubjectComponent(string key, string value)
        {
            AddComponent(InternaliseKey(key), InternaliseValue(value));
        }

        private void AddComponent(string key, string value)
        {
            if (IsKeyInOrder(key))
            {
                AddComponentAt(_size, key, value);
            }
            else
            {
                var index = SubjectUtils.BinarySearch(_components, _size, key);
                if (index < 0)
                {
                    AddComponentAt(-1 - index, key, value);
                }
                else
                {
                    _components[index + 1] = value;
                }
            }
        }

        private void AddComponentAt(int index, string key, string value)
        {
            if (_size == _components.Length) ExtendCapacity();
            Array.Copy(_components, index, _components, index + 2, _size - index);
            _components[index] = key;
            _components[index + 1] = value;
            _size += 2;
        }

        private void ExtendCapacity()
        {
            var newCapacity = _components.Length << 1;
            var oldComponents = _components;
            _components = new string[newCapacity];
            Array.Copy(oldComponents, 0, _components, 0, _size);
        }

        private bool IsKeyInOrder(string key)
        {
            return _size == 0 || string.Compare(_components[_size - 2], key, StringComparison.Ordinal) < 0;
        }

        private static string InternaliseKey(string key)
        {
            var alt = CommonComponents.CommonKey(key);
            if (alt != null) return alt;
            SubjectValidator.ValidatePart(key, SubjectPart.Key);
            return key;
        }

        private static string InternaliseValue(string value)
        {
            var alt = CommonComponents.CommonKey(value);
            if (alt != null) return alt;
            SubjectValidator.ValidatePart(value, SubjectPart.Value);
            return value;
        }

        internal string[] GetComponents()
        {
            var components = new string[_size];
            Array.Copy(_components, 0, components, 0, _size);
            return components;
        }

        /// <summary>
        /// Looks up the value of a variable.
        /// </summary>
        /// <param name="key">the name of the variable to look up</param>
        /// <returns>the variable value of null if the variable is undefined</returns>
        public string LookupValue(string key)
        {
            var index = SubjectUtils.BinarySearch(_components, _size, key);
            return index >= 0 ? _components[index + 1] : null;
        }

        public IEnumerator<SubjectComponent> GetEnumerator()
        {
            return new SubjectIterator(GetComponents());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            _size = 0;
        }

        public override string ToString()
        {
            return new SubjectFormatter().FormatToString(this);
        }

        // ******************LEVEL 1 SPOT******************

        private static SubjectBuilder CreateLevelOneSpotSubject(string liquidityProvider, string symbol, string currency,
            string quantity, string account, string requestFor)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, currency, quantity, requestFor, liquidityProvider, "Spot", symbol, "1");
            return subjectBuilder;
        }

        public static Subject CreateLevelOneSpotStreamingSubject(string liquidityProvider, string symbol, string currency,
            string quantity, string account)
        {
            return CreateLevelOneSpotSubject(liquidityProvider, symbol, currency, quantity, account, "Stream").CreateSubject();
        }

        public static Subject CreateLevelOneSpotQuoteSubject(string liquidityProvider, string symbol, string currency,
            string quantity, string account, bool autoRefresh = false)
        {
            var levelOneSpotRfqSubject =
                CreateLevelOneSpotSubject(liquidityProvider, symbol, currency, quantity, account, "Quote");
            levelOneSpotRfqSubject.AutoRefresh = autoRefresh;
            return levelOneSpotRfqSubject.CreateSubject();
        }

        // ******************LEVEL 1 FORWARD******************

        private static SubjectBuilder CreateLevelOneForwardSubject(string liquidityProvider, string symbol, string currency,
            string quantity, string account, string settlementDate, string requestFor)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, currency, quantity, requestFor, liquidityProvider, "Outright", symbol,
                "1");
            return subjectBuilder
                .SetComponent(SubjectComponentName.SettlementDate, settlementDate);
        }

        public static Subject CreateLevelOneForwardStreamingSubject(string liquidityProvider, string symbol, string currency,
            string quantity, string account, string settlementDate)
        {
            return CreateLevelOneForwardSubject(liquidityProvider, symbol, currency, quantity, account, settlementDate, "Stream")
                .CreateSubject();
        }

        public static Subject CreateLevelOneForwardQuoteSubject(string liquidityProvider, string symbol, string currency,
            string quantity, string account, string settlementDate, bool autoRefresh = false)
        {
            var levelOneForwardRfqSubject =
                CreateLevelOneForwardSubject(liquidityProvider, symbol, currency, quantity, account, settlementDate, "Quote");
            levelOneForwardRfqSubject.AutoRefresh = autoRefresh;
            return levelOneForwardRfqSubject.CreateSubject();
        }

        // ******************LEVEL 1 NDF******************

        private static SubjectBuilder CreateLevelOneNdfSubject(string liquidityProvider, string symbol, string currency,
            string quantity, string account, string settlementDate, string fixingDate, string requestFor)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, currency, quantity, requestFor, liquidityProvider, "NDF", symbol, "1");
            return subjectBuilder
                .SetComponent(SubjectComponentName.SettlementDate, settlementDate)
                .SetComponent(SubjectComponentName.FixingDate, fixingDate);
        }

        public static Subject CreateLevelOneNdfStreamingSubject(string liquidityProvider, string symbol, string currency,
            string quantity, string account, string settlementDate, string fixingDate)
        {
            return CreateLevelOneNdfSubject(liquidityProvider, symbol, currency, quantity, account, settlementDate, fixingDate,
                    "Stream")
                .CreateSubject();
        }

        public static Subject CreateLevelOneNdfQuoteSubject(string liquidityProvider, string symbol, string currency,
            string quantity, string account, string settlementDate, string fixingDate, bool autoRefresh = false)
        {
            var levelOneNdfRfqSubject = CreateLevelOneNdfSubject(liquidityProvider, symbol, currency, quantity, account, settlementDate,
                fixingDate, "Quote");
            levelOneNdfRfqSubject.AutoRefresh = autoRefresh;
            return levelOneNdfRfqSubject.CreateSubject();
        }

        // ******************LEVEL 1 SWAP******************

        private static SubjectBuilder CreateLevelOneSwapSubject(string liquidityProvider, string symbol, string currency,
            string quantity, string account, string farQuantity, string settlementDate, string farSettlementDate, string requestFor)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, currency, quantity, requestFor, liquidityProvider, "Swap", symbol, "1");
            return subjectBuilder
                .SetComponent(SubjectComponentName.FarCurrency, currency)
                .SetComponent(SubjectComponentName.FarQuantity, farQuantity)
                .SetComponent(SubjectComponentName.SettlementDate, settlementDate)
                .SetComponent(SubjectComponentName.FarSettlementDate, farSettlementDate);
        }

        public static Subject CreateLevelOneSwapStreamingSubject(string liquidityProvider, string symbol, string currency,
            string quantity, string account, string farQuantity, string settlementDate, string farSettlementDate)
        {
            return CreateLevelOneSwapSubject(liquidityProvider, symbol, currency, quantity, account, farQuantity, settlementDate,
                farSettlementDate, "Stream").CreateSubject();
        }

        public static Subject CreateLevelOneSwapQuoteSubject(string liquidityProvider, string symbol, string currency,
            string quantity, string account, string farQuantity, string settlementDate, string farSettlementDate,
            bool autoRefresh = false)
        {
            var levelOneSwapRfqSubject = CreateLevelOneSwapSubject(liquidityProvider, symbol, currency, quantity, account,
                farQuantity, settlementDate,
                farSettlementDate, "Quote");
            levelOneSwapRfqSubject.AutoRefresh = autoRefresh;
            return levelOneSwapRfqSubject.CreateSubject();
        }

        // ******************LEVEL 1 NDS******************

        private static SubjectBuilder CreateLevelOneNdsSubject(string liquidityProvider, string symbol, string currency,
            string quantity, string account, string fixingDate, string farFixingDate, string farQuantity, string settlementDate,
            string farSettlementDate, string requestFor)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, currency, quantity, requestFor, liquidityProvider, "NDS", symbol, "1");
            return subjectBuilder
                .SetComponent(SubjectComponentName.FixingDate, fixingDate)
                .SetComponent(SubjectComponentName.FarFixingDate, farFixingDate)
                .SetComponent(SubjectComponentName.FarCurrency, currency)
                .SetComponent(SubjectComponentName.FarQuantity, farQuantity)
                .SetComponent(SubjectComponentName.SettlementDate, settlementDate)
                .SetComponent(SubjectComponentName.FarSettlementDate, farSettlementDate);
        }

        public static Subject CreateLevelOneNdsStreamingSubject(string liquidityProvider, string symbol, string currency,
            string quantity, string account, string fixingDate, string farFixingDate, string farQuantity, string settlementDate,
            string farSettlementDate)
        {
            return CreateLevelOneNdsSubject(liquidityProvider, symbol, currency, quantity, account, fixingDate, farFixingDate,
                farQuantity,
                settlementDate, farSettlementDate, "Stream").CreateSubject();
        }

        public static Subject CreateLevelOneNdsQuoteSubject(string liquidityProvider, string symbol, string currency,
            string quantity, string account, string fixingDate, string farFixingDate, string farQuantity, string settlementDate,
            string farSettlementDate, bool autoRefresh = false)
        {
            var levelOneNdsRfqSubject = CreateLevelOneNdsSubject(liquidityProvider, symbol, currency, quantity, account,
                fixingDate, farFixingDate,
                farQuantity,
                settlementDate, farSettlementDate, "Quote");
            levelOneNdsRfqSubject.AutoRefresh = autoRefresh;
            return levelOneNdsRfqSubject.CreateSubject();
        }

        // ******************LEVEL 2 SPOT******************

        public static Subject CreateLevelTwoSpotStreamingSubject(string symbol, string currency, string quantity,
            string account)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, currency, quantity, "Stream", "FXTS", "Spot", symbol, "2");
            return subjectBuilder.CreateSubject();
        }

        // ******************LEVEL 2 FORWARD******************

        public static Subject CreateLevelTwoForwardStreamingSubject(string symbol, string currency, string quantity,
            string account, string settlementDate)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, currency, quantity, "Stream", "FXTS", "Outright", symbol, "2");
            subjectBuilder.SetComponent(SubjectComponentName.SettlementDate, settlementDate);
            return subjectBuilder.CreateSubject();
        }

        // ******************LEVEL 2 NDF******************

        public static Subject CreateLevelTwoNdfStreamingSubject(string symbol, string currency, string quantity,
            string account, string settlementDate, string fixingDate)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, currency, quantity, "Stream", "FXTS", "NDF", symbol, "2");
            subjectBuilder.SetComponent(SubjectComponentName.SettlementDate, settlementDate)
                .SetComponent(SubjectComponentName.FixingDate, fixingDate);
            return subjectBuilder.CreateSubject();
        }

        private static void AddBasicComponents(SubjectBuilder subjectBuilder, string account, string currency,
            string quantity, string requestFor, string liquidityProvider, string subClass, string symbol, string level)
        {
            subjectBuilder
                .SetComponent(SubjectComponentName.BuySideAccount, account)
                .SetComponent(SubjectComponentName.AssetClass, "Fx")
                .SetComponent(SubjectComponentName.Currency, currency)
                .SetComponent(SubjectComponentName.Quantity, quantity)
                .SetComponent(SubjectComponentName.RequestFor, requestFor)
                .SetComponent(SubjectComponentName.LiquidityProvider, liquidityProvider)
                .SetComponent(SubjectComponentName.DealType, subClass)
                .SetComponent(SubjectComponentName.Symbol, symbol)
                .SetComponent(SubjectComponentName.Level, level);
        }
        
        // ******************PUFFIN******************
        
        public static Subject CreateIndicativePriceSubject(string ccyPair)
        {
            return new SubjectBuilder()
                .SetComponent(SubjectComponentName.AssetClass, "Fx")
                .SetComponent(SubjectComponentName.Level, "1")
                .SetComponent(SubjectComponentName.Source, "Indi")
                .SetComponent(SubjectComponentName.Symbol, ccyPair)
                .CreateSubject();
        }

        public static Subject CreatePremiumFxSubject(string ccyPair, bool tiered, bool crossCurrencyRates)
        {
            var subject = new SubjectBuilder()
                .SetComponent(SubjectComponentName.AssetClass, "Fx")
                .SetComponent(SubjectComponentName.Level, tiered ? "Tiered" : "1")
                .SetComponent(SubjectComponentName.Source, "PremiumFX")
                .SetComponent(SubjectComponentName.Symbol, ccyPair);
            if (crossCurrencyRates)
            {
                subject.SetComponent("SubClass", "Cross");
            }
            return subject.CreateSubject();
        }
    }
}