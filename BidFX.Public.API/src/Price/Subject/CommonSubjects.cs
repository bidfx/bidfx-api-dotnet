namespace BidFX.Public.API.Price.Subject
{
    public class CommonSubjects
    {
        // ******************LEVEL 1 SPOT******************

        private static SubjectBuilder CreateLevelOneSpotSubject(string account, string liquidityProvider, string symbol, string currency,
            string quantity, string requestFor)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, "Fx", currency, quantity, requestFor, liquidityProvider, "Spot", symbol, "1");
            return subjectBuilder;
        }

        public static SubjectBuilder CreateLevelOneSpotStreamingSubject(string account, string liquidityProvider, string symbol, string currency,
            string quantity)
        {
            return CreateLevelOneSpotSubject(account, liquidityProvider, symbol, currency, quantity, "Stream");
        }

        public static SubjectBuilder CreateLevelOneSpotQuoteSubject(string account, string liquidityProvider, string symbol, string currency,
            string quantity, bool autoRefresh = false)
        {
            var levelOneSpotRfqSubject =
                CreateLevelOneSpotSubject(account, liquidityProvider, symbol, currency, quantity, "Quote");
            levelOneSpotRfqSubject.AutoRefresh = autoRefresh;
            return levelOneSpotRfqSubject;
        }

        // ******************LEVEL 1 FORWARD******************

        private static SubjectBuilder CreateLevelOneForwardSubject(string account, string liquidityProvider, string symbol, string currency,
            string quantity, string settlementDate, string requestFor)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, "Fx", currency, quantity, requestFor, liquidityProvider, "Outright", symbol,
                "1");
            return subjectBuilder
                .SetComponent(SubjectComponentName.SettlementDate, settlementDate);
        }

        public static SubjectBuilder CreateLevelOneForwardStreamingSubject(string account, string liquidityProvider, string symbol, string currency,
            string quantity, string settlementDate)
        {
            return CreateLevelOneForwardSubject(account, liquidityProvider, symbol, currency, quantity, settlementDate, "Stream");
        }

        public static SubjectBuilder CreateLevelOneForwardQuoteSubject(string account, string liquidityProvider, string symbol, string currency,
            string quantity, string settlementDate, bool autoRefresh = false)
        {
            var levelOneForwardRfqSubject =
                CreateLevelOneForwardSubject(account, liquidityProvider, symbol, currency, quantity, settlementDate, "Quote");
            levelOneForwardRfqSubject.AutoRefresh = autoRefresh;
            return levelOneForwardRfqSubject;
        }

        // ******************LEVEL 1 NDF******************

        private static SubjectBuilder CreateLevelOneNdfSubject(string account, string liquidityProvider, string symbol, string currency,
            string quantity, string fixingDate, string settlementDate, string requestFor)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, "Fx", currency, quantity, requestFor, liquidityProvider, "NDF", symbol, "1");
            return subjectBuilder
                .SetComponent(SubjectComponentName.SettlementDate, settlementDate)
                .SetComponent(SubjectComponentName.FixingDate, fixingDate);
        }

        public static SubjectBuilder CreateLevelOneNdfStreamingSubject(string account, string liquidityProvider, string symbol, string currency,
            string quantity, string fixingDate, string settlementDate)
        {
            return CreateLevelOneNdfSubject(account, liquidityProvider, symbol, currency, quantity, fixingDate, settlementDate, 
                    "Stream");
        }

        public static SubjectBuilder CreateLevelOneNdfQuoteSubject(string account, string liquidityProvider, string symbol, string currency,
            string quantity, string fixingDate, string settlementDate, bool autoRefresh = false)
        {
            var levelOneNdfRfqSubject = CreateLevelOneNdfSubject(account, liquidityProvider, symbol, currency, quantity, fixingDate, settlementDate,
                "Quote");
            levelOneNdfRfqSubject.AutoRefresh = autoRefresh;
            return levelOneNdfRfqSubject;
        }

        // ******************LEVEL 1 SWAP******************

        private static SubjectBuilder CreateLevelOneSwapSubject(string account, string liquidityProvider, string symbol, string currency,
            string quantity, string farQuantity, string settlementDate, string farSettlementDate, string requestFor)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, "Fx", currency, quantity, requestFor, liquidityProvider, "Swap", symbol, "1");
            return subjectBuilder
                .SetComponent(SubjectComponentName.FarCurrency, currency)
                .SetComponent(SubjectComponentName.FarQuantity, farQuantity)
                .SetComponent(SubjectComponentName.SettlementDate, settlementDate)
                .SetComponent(SubjectComponentName.FarSettlementDate, farSettlementDate);
        }

        public static SubjectBuilder CreateLevelOneSwapStreamingSubject(string account, string liquidityProvider, string symbol, string currency,
            string quantity, string farQuantity, string settlementDate, string farSettlementDate)
        {
            return CreateLevelOneSwapSubject(account, liquidityProvider, symbol, currency, quantity, farQuantity, settlementDate,
                farSettlementDate, "Stream");
        }

        public static SubjectBuilder CreateLevelOneSwapQuoteSubject(string account, string liquidityProvider, string symbol, string currency,
            string quantity, string farQuantity, string settlementDate, string farSettlementDate,
            bool autoRefresh = false)
        {
            var levelOneSwapRfqSubject = CreateLevelOneSwapSubject(account, liquidityProvider, symbol, currency, quantity, 
                farQuantity, settlementDate, farSettlementDate, "Quote");
            levelOneSwapRfqSubject.AutoRefresh = autoRefresh;
            return levelOneSwapRfqSubject;
        }

        // ******************LEVEL 1 NDS******************

        private static SubjectBuilder CreateLevelOneNdsSubject(string account, string liquidityProvider, string symbol, string currency,
            string quantity, string farQuantity, string fixingDate, string farFixingDate, string settlementDate,
            string farSettlementDate, string requestFor)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, "Fx", currency, quantity, requestFor, liquidityProvider, "NDS", symbol, "1");
            return subjectBuilder
                .SetComponent(SubjectComponentName.FixingDate, fixingDate)
                .SetComponent(SubjectComponentName.FarFixingDate, farFixingDate)
                .SetComponent(SubjectComponentName.FarCurrency, currency)
                .SetComponent(SubjectComponentName.FarQuantity, farQuantity)
                .SetComponent(SubjectComponentName.SettlementDate, settlementDate)
                .SetComponent(SubjectComponentName.FarSettlementDate, farSettlementDate);
        }

        public static SubjectBuilder CreateLevelOneNdsStreamingSubject(string account, string liquidityProvider, string symbol, string currency,
            string quantity, string farQuantity, string fixingDate, string farFixingDate, string settlementDate,
            string farSettlementDate)
        {
            return CreateLevelOneNdsSubject(account, liquidityProvider, symbol, currency, quantity, farQuantity, fixingDate, farFixingDate,
                settlementDate, farSettlementDate, "Stream");
        }

        public static SubjectBuilder CreateLevelOneNdsQuoteSubject(string account, string liquidityProvider, string symbol, string currency,
            string quantity, string farQuantity, string fixingDate, string farFixingDate, string settlementDate,
            string farSettlementDate, bool autoRefresh = false)
        {
            var levelOneNdsRfqSubject = CreateLevelOneNdsSubject(account, liquidityProvider, symbol, currency, quantity,
                farQuantity, fixingDate, farFixingDate, settlementDate, farSettlementDate, "Quote");
            levelOneNdsRfqSubject.AutoRefresh = autoRefresh;
            return levelOneNdsRfqSubject;
        }

        // ******************LEVEL 2 SPOT******************

        public static SubjectBuilder CreateLevelTwoSpotStreamingSubject(string account, string symbol, string currency, string quantity)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, "Fx", currency, quantity, "Stream", "FXTS", "Spot", symbol, "2");
            return subjectBuilder;
        }

        // ******************LEVEL 2 FORWARD******************

        public static SubjectBuilder CreateLevelTwoForwardStreamingSubject(string account, string symbol, string currency, string quantity,
            string settlementDate)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, "Fx", currency, quantity, "Stream", "FXTS", "Outright", symbol, "2");
            subjectBuilder.SetComponent(SubjectComponentName.SettlementDate, settlementDate);
            return subjectBuilder;
        }

        // ******************LEVEL 2 NDF******************

        public static SubjectBuilder CreateLevelTwoNdfStreamingSubject(string account, string symbol, string currency, string quantity, string fixingDate,
            string settlementDate)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, "Fx", currency, quantity, "Stream", "FXTS", "NDF", symbol, "2");
            subjectBuilder.SetComponent(SubjectComponentName.SettlementDate, settlementDate)
                .SetComponent(SubjectComponentName.FixingDate, fixingDate);
            return subjectBuilder;
        }
        
        // ******************PUFFIN******************
        
        public static SubjectBuilder CreateIndicativePriceSubject(string ccyPair)
        {
            return new SubjectBuilder()
                .SetComponent(SubjectComponentName.AssetClass, "Fx")
                .SetComponent(SubjectComponentName.Level, "1")
                .SetComponent(SubjectComponentName.Source, "Indi")
                .SetComponent(SubjectComponentName.Symbol, ccyPair);
        }

        public static SubjectBuilder CreatePremiumFxSubject(string ccyPair, bool tiered, bool crossCurrencyRates)
        {
            var subjectBuilder = new SubjectBuilder()
                .SetComponent(SubjectComponentName.AssetClass, "Fx")
                .SetComponent(SubjectComponentName.Level, tiered ? "Tiered" : "1")
                .SetComponent(SubjectComponentName.Source, "PremiumFX")
                .SetComponent(SubjectComponentName.Symbol, ccyPair);
            if (crossCurrencyRates)
            {
                subjectBuilder.SetComponent("SubClass", "Cross");
            }
            return subjectBuilder;
        }
        

        private static void AddBasicComponents(SubjectBuilder subjectBuilder, string account, string assetClass, string currency,
            string quantity, string requestFor, string liquidityProvider, string dealType, string symbol, string level)
        {
            subjectBuilder
                .SetComponent(SubjectComponentName.BuySideAccount, account)
                .SetComponent(SubjectComponentName.AssetClass, assetClass)
                .SetComponent(SubjectComponentName.Currency, currency)
                .SetComponent(SubjectComponentName.Quantity, quantity)
                .SetComponent(SubjectComponentName.RequestFor, requestFor)
                .SetComponent(SubjectComponentName.LiquidityProvider, liquidityProvider)
                .SetComponent(SubjectComponentName.DealType, dealType)
                .SetComponent(SubjectComponentName.Symbol, symbol)
                .SetComponent(SubjectComponentName.Level, level);
        }
    }
}