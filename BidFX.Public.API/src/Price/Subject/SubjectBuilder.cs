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

        private static SubjectBuilder CreateLevelOneSpotSubject(string source, string symbol, string currency,
            string quantity,
            string account, string quoteStyle)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, currency, quantity, quoteStyle, source, "Spot", symbol, "1");
            return subjectBuilder;
        }

        public static Subject CreateLevelOneSpotStreamingSubject(string source, string symbol, string currency,
            string quantity, string account)
        {
            return CreateLevelOneSpotSubject(source, symbol, currency, quantity, account, "Stream").CreateSubject();
        }

        public static Subject CreateLevelOneSpotQuoteSubject(string source, string symbol, string currency,
            string quantity, string account, bool autoRefresh = false)
        {
            var levelOneSpotRfqSubject =
                CreateLevelOneSpotSubject(source, symbol, currency, quantity, account, "Quote");
            levelOneSpotRfqSubject.AutoRefresh = autoRefresh;
            return levelOneSpotRfqSubject.CreateSubject();
        }

        // ******************LEVEL 1 FORWARD******************

        private static SubjectBuilder CreateLevelOneForwardSubject(string source, string symbol, string currency,
            string quantity, string account, string valueDate, string quoteStyle)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, currency, quantity, quoteStyle, source, "Outright", symbol,
                "1");
            return subjectBuilder
                .SetComponent(SubjectComponentName.SettlementDate, valueDate);
        }

        public static Subject CreateLevelOneForwardStreamingSubject(string source, string symbol, string currency,
            string quantity, string account, string valueDate)
        {
            return CreateLevelOneForwardSubject(source, symbol, currency, quantity, account, valueDate, "Stream")
                .CreateSubject();
        }

        public static Subject CreateLevelOneForwardQuoteSubject(string source, string symbol, string currency,
            string quantity, string account, string valueDate, bool autoRefresh = false)
        {
            var levelOneForwardRfqSubject =
                CreateLevelOneForwardSubject(source, symbol, currency, quantity, account, valueDate, "Quote");
            levelOneForwardRfqSubject.AutoRefresh = autoRefresh;
            return levelOneForwardRfqSubject.CreateSubject();
        }

        // ******************LEVEL 1 NDF******************

        private static SubjectBuilder CreateLevelOneNdfSubject(string source, string symbol, string currency,
            string quantity, string account, string valueDate, string fixingDate, string quoteStyle)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, currency, quantity, quoteStyle, source, "NDF", symbol, "1");
            return subjectBuilder
                .SetComponent(SubjectComponentName.SettlementDate, valueDate)
                .SetComponent(SubjectComponentName.FixingDate, fixingDate);
        }

        public static Subject CreateLevelOneNdfStreamingSubject(string source, string symbol, string currency,
            string quantity, string account, string valueDate, string fixingDate)
        {
            return CreateLevelOneNdfSubject(source, symbol, currency, quantity, account, valueDate, fixingDate,
                    "Stream")
                .CreateSubject();
        }

        public static Subject CreateLevelOneNdfQuoteSubject(string source, string symbol, string currency,
            string quantity, string account, string valueDate, string fixingDate, bool autoRefresh = false)
        {
            var levelOneNdfRfqSubject = CreateLevelOneNdfSubject(source, symbol, currency, quantity, account, valueDate,
                fixingDate, "Quote");
            levelOneNdfRfqSubject.AutoRefresh = autoRefresh;
            return levelOneNdfRfqSubject.CreateSubject();
        }

        // ******************LEVEL 1 SWAP******************

        private static SubjectBuilder CreateLevelOneSwapSubject(string source, string symbol, string currency,
            string quantity, string account, string quantity2, string valueDate, string valueDate2, string quoteStyle)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, currency, quantity, quoteStyle, source, "Swap", symbol, "1");
            return subjectBuilder
                .SetComponent(SubjectComponentName.FarCurrency, currency)
                .SetComponent(SubjectComponentName.FarQuantity, quantity2)
                .SetComponent(SubjectComponentName.SettlementDate, valueDate)
                .SetComponent(SubjectComponentName.FarSettlementDate, valueDate2);
        }

        public static Subject CreateLevelOneSwapStreamingSubject(string source, string symbol, string currency,
            string quantity, string account, string quantity2, string valueDate, string valueDate2)
        {
            return CreateLevelOneSwapSubject(source, symbol, currency, quantity, account, quantity2, valueDate,
                valueDate2, "Stream").CreateSubject();
        }

        public static Subject CreateLevelOneSwapQuoteSubject(string source, string symbol, string currency,
            string quantity, string account, string quantity2, string valueDate, string valueDate2,
            bool autoRefresh = false)
        {
            var levelOneSwapRfqSubject = CreateLevelOneSwapSubject(source, symbol, currency, quantity, account,
                quantity2, valueDate,
                valueDate2, "Quote");
            levelOneSwapRfqSubject.AutoRefresh = autoRefresh;
            return levelOneSwapRfqSubject.CreateSubject();
        }

        // ******************LEVEL 1 NDS******************

        private static SubjectBuilder CreateLevelOneNdsSubject(string source, string symbol, string currency,
            string quantity, string account, string fixingDate, string fixingDate2, string quantity2, string valueDate,
            string valueDate2, string quoteStyle)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, currency, quantity, quoteStyle, source, "NDS", symbol, "1");
            return subjectBuilder
                .SetComponent(SubjectComponentName.FixingDate, fixingDate)
                .SetComponent(SubjectComponentName.FarFixingDate, fixingDate2)
                .SetComponent(SubjectComponentName.FarCurrency, currency)
                .SetComponent(SubjectComponentName.FarQuantity, quantity2)
                .SetComponent(SubjectComponentName.SettlementDate, valueDate)
                .SetComponent(SubjectComponentName.FarSettlementDate, valueDate2);
        }

        public static Subject CreateLevelOneNdsStreamingSubject(string source, string symbol, string currency,
            string quantity, string account, string fixingDate, string fixingDate2, string quantity2, string valueDate,
            string valueDate2)
        {
            return CreateLevelOneNdsSubject(source, symbol, currency, quantity, account, fixingDate, fixingDate2,
                quantity2,
                valueDate, valueDate2, "Stream").CreateSubject();
        }

        public static Subject CreateLevelOneNdsQuoteSubject(string source, string symbol, string currency,
            string quantity, string account, string fixingDate, string fixingDate2, string quantity2, string valueDate,
            string valueDate2, bool autoRefresh = false)
        {
            var levelOneNdsRfqSubject = CreateLevelOneNdsSubject(source, symbol, currency, quantity, account,
                fixingDate, fixingDate2,
                quantity2,
                valueDate, valueDate2, "Quote");
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
            string account, string valueDate)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, currency, quantity, "Stream", "FXTS", "Outright", symbol, "2");
            subjectBuilder.SetComponent(SubjectComponentName.SettlementDate, valueDate);
            return subjectBuilder.CreateSubject();
        }

        // ******************LEVEL 2 NDF******************

        public static Subject CreateLevelTwoNdfStreamingSubject(string symbol, string currency, string quantity,
            string account, string valueDate, string fixingDate)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, currency, quantity, "Stream", "FXTS", "NDF", symbol, "2");
            subjectBuilder.SetComponent(SubjectComponentName.SettlementDate, valueDate)
                .SetComponent(SubjectComponentName.FixingDate, fixingDate);
            return subjectBuilder.CreateSubject();
        }

        private static void AddBasicComponents(SubjectBuilder subjectBuilder, string account, string currency,
            string quantity, string quoteStyle, string source, string subClass, string symbol, string level)
        {
            subjectBuilder
                .SetComponent(SubjectComponentName.BuySideAccount, account)
                .SetComponent(SubjectComponentName.AssetClass, "Fx")
                .SetComponent(SubjectComponentName.Currency, currency)
                .SetComponent(SubjectComponentName.Quantity, quantity)
                .SetComponent(SubjectComponentName.RequestFor, quoteStyle)
                .SetComponent(SubjectComponentName.LiquidityProvider, source)
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
                .SetComponent("Source", "Indi")
                .SetComponent(SubjectComponentName.Symbol, ccyPair)
                .CreateSubject();
        }

        public static Subject CreatePremiumFxSubject(string ccyPair, bool tiered, bool crossCurrencyRates)
        {
            var subject = new SubjectBuilder()
                .SetComponent(SubjectComponentName.AssetClass, "Fx")
                .SetComponent(SubjectComponentName.Level, tiered ? "Tiered" : "1")
                .SetComponent("Source", "PremiumFX")
                .SetComponent(SubjectComponentName.Symbol, ccyPair);
            if (crossCurrencyRates)
            {
                subject.SetComponent("SubClass", "Cross");
            }
            return subject.CreateSubject();
        }
    }
}