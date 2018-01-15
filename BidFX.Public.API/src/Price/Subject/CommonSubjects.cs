namespace BidFX.Public.API.Price.Subject
{
    public class CommonSubjects
    {
        // ******************LEVEL 1 SPOT******************

        private static SubjectBuilder CreateLevelOneSpotSubject(string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity, string requestFor)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, ccyPair, CommonComponents.Spot, liquidityProvider, currency, quantity, CommonComponents.Fx, "1", requestFor);
            return subjectBuilder;
        }

        public static SubjectBuilder CreateLevelOneSpotStreamingSubject(string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity)
        {
            return CreateLevelOneSpotSubject(account, liquidityProvider, ccyPair, currency, quantity, CommonComponents.Stream);
        }

        public static SubjectBuilder CreateLevelOneSpotQuoteSubject(string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity)
        {
            var levelOneSpotRfqSubject =
                CreateLevelOneSpotSubject(account, liquidityProvider, ccyPair, currency, quantity, CommonComponents.Quote);
            return levelOneSpotRfqSubject;
        }

        // ******************LEVEL 1 FORWARD******************

        private static SubjectBuilder CreateLevelOneForwardSubject(string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity, string settlementDate, string requestFor)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, ccyPair,
                CommonComponents.Forward, liquidityProvider, currency, quantity, CommonComponents.Fx, "1", requestFor);
            return subjectBuilder
                .SetComponent(SubjectComponentName.SettlementDate, settlementDate);
        }

        public static SubjectBuilder CreateLevelOneForwardStreamingSubject(string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity, string settlementDate)
        {
            return CreateLevelOneForwardSubject(account, ccyPair, liquidityProvider, currency, quantity, settlementDate, CommonComponents.Stream);
        }

        public static SubjectBuilder CreateLevelOneForwardQuoteSubject(string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity, string settlementDate)
        {
            var levelOneForwardRfqSubject =
                CreateLevelOneForwardSubject(account, ccyPair, liquidityProvider, currency, quantity, settlementDate, CommonComponents.Quote);
            return levelOneForwardRfqSubject;
        }

        // ******************LEVEL 1 NDF******************

        private static SubjectBuilder CreateLevelOneNdfSubject(string account, string ccyPair,
            string liquidityProvider,
            string currency,
            string quantity, string settlementDate, string requestFor)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, ccyPair, CommonComponents.NDF, liquidityProvider, currency, quantity, CommonComponents.Fx, "1", requestFor);
            return subjectBuilder
                .SetComponent(SubjectComponentName.SettlementDate, settlementDate);
        }

        public static SubjectBuilder CreateLevelOneNdfStreamingSubject(string account, string ccyPair,
            string liquidityProvider,
            string currency,
            string quantity, string settlementDate)
        {
            return CreateLevelOneNdfSubject(account, ccyPair, liquidityProvider, currency, quantity, settlementDate, CommonComponents.Stream);
        }

        public static SubjectBuilder CreateLevelOneNdfQuoteSubject(string account, string ccyPair,
            string liquidityProvider,
            string currency,
            string quantity, string settlementDate)
        {
            var levelOneNdfRfqSubject = CreateLevelOneNdfSubject(account, ccyPair, liquidityProvider, currency, quantity, settlementDate, CommonComponents.Quote);
            return levelOneNdfRfqSubject;
        }

        // ******************LEVEL 1 SWAP******************

        private static SubjectBuilder CreateLevelOneSwapSubject(string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity, string settlementDate, string farQuantity, string farSettlementDate, string requestFor)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, ccyPair, CommonComponents.Swap, liquidityProvider, currency, quantity, CommonComponents.Fx, "1", requestFor);
            return subjectBuilder
                .SetComponent(SubjectComponentName.FarCurrency, currency)
                .SetComponent(SubjectComponentName.FarQuantity, farQuantity)
                .SetComponent(SubjectComponentName.SettlementDate, settlementDate)
                .SetComponent(SubjectComponentName.FarSettlementDate, farSettlementDate);
        }

        public static SubjectBuilder CreateLevelOneSwapStreamingSubject(string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity, string settlementDate, string farQuantity, string farSettlementDate)
        {
            return CreateLevelOneSwapSubject(account, ccyPair, liquidityProvider, currency, quantity, settlementDate,
                farQuantity, farSettlementDate, CommonComponents.Stream);
        }

        public static SubjectBuilder CreateLevelOneSwapQuoteSubject(string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity, string settlementDate, string farQuantity, string farSettlementDate)
        {
            var levelOneSwapRfqSubject = CreateLevelOneSwapSubject(account, ccyPair, liquidityProvider, currency, 
                quantity, settlementDate, farQuantity, farSettlementDate, CommonComponents.Quote);
            return levelOneSwapRfqSubject;
        }

        // ******************LEVEL 1 NDS******************

        private static SubjectBuilder CreateLevelOneNdsSubject(string account, string ccyPair, string liquidityProvider,
            string currency,
            string quantity, string settlementDate,
            string farQuantity,
            string farSettlementDate, string requestFor)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, ccyPair, CommonComponents.NDS, liquidityProvider, currency, quantity, CommonComponents.Fx, "1", requestFor);
            return subjectBuilder
                .SetComponent(SubjectComponentName.FarCurrency, currency)
                .SetComponent(SubjectComponentName.FarQuantity, farQuantity)
                .SetComponent(SubjectComponentName.SettlementDate, settlementDate)
                .SetComponent(SubjectComponentName.FarSettlementDate, farSettlementDate);
        }

        public static SubjectBuilder CreateLevelOneNdsStreamingSubject(string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity, string settlementDate,
            string farQuantity,
            string farSettlementDate)
        {
            return CreateLevelOneNdsSubject(account, ccyPair, liquidityProvider, currency, quantity,
                settlementDate, farQuantity, farSettlementDate, CommonComponents.Stream);
        }

        public static SubjectBuilder CreateLevelOneNdsQuoteSubject(string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity, string settlementDate,
            string farQuantity,
            string farSettlementDate)
        {
            var levelOneNdsRfqSubject = CreateLevelOneNdsSubject(account, ccyPair, liquidityProvider, currency,
                quantity, settlementDate, farQuantity, farSettlementDate, CommonComponents.Quote);
            return levelOneNdsRfqSubject;
        }

        // ******************LEVEL 2 SPOT******************

        public static SubjectBuilder CreateLevelTwoSpotStreamingSubject(string account, string ccyPair, string currency,
            string quantity)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, ccyPair, CommonComponents.Spot, CommonComponents.FXTS, currency, quantity, CommonComponents.Fx, "2", CommonComponents.Stream);
            return subjectBuilder;
        }

        // ******************LEVEL 2 FORWARD******************

        public static SubjectBuilder CreateLevelTwoForwardStreamingSubject(string account, string ccyPair,
            string currency, string quantity,
            string settlementDate)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, ccyPair, CommonComponents.Forward, CommonComponents.FXTS, currency, quantity, CommonComponents.Fx, "2", CommonComponents.Stream);
            subjectBuilder.SetComponent(SubjectComponentName.SettlementDate, settlementDate);
            return subjectBuilder;
        }

        // ******************LEVEL 2 NDF******************

        public static SubjectBuilder CreateLevelTwoNdfStreamingSubject(string account, string ccyPair, string currency,
            string quantity,
            string settlementDate)
        {
            var subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, ccyPair, CommonComponents.NDF, CommonComponents.FXTS, currency, quantity, CommonComponents.Fx, "2", CommonComponents.Stream);
            subjectBuilder.SetComponent(SubjectComponentName.SettlementDate, settlementDate);
            return subjectBuilder;
        }
        
        // ******************PUFFIN******************
        
        public static SubjectBuilder CreateIndicativePriceSubject(string ccyPair)
        {
            return new SubjectBuilder()
                .SetComponent(SubjectComponentName.AssetClass, CommonComponents.Fx)
                .SetComponent(SubjectComponentName.Level, "1")
                .SetComponent(SubjectComponentName.Source, "Indi")
                .SetComponent(SubjectComponentName.Symbol, ccyPair);
        }

        public static SubjectBuilder CreatePremiumFxSubject(string ccyPair, bool tiered, bool crossCurrencyRates)
        {
            var subjectBuilder = new SubjectBuilder()
                .SetComponent(SubjectComponentName.AssetClass, CommonComponents.Fx)
                .SetComponent(SubjectComponentName.Level, tiered ? "Tiered" : "1")
                .SetComponent(SubjectComponentName.Source, "PremiumFX")
                .SetComponent(SubjectComponentName.Symbol, ccyPair);
            if (crossCurrencyRates)
            {
                subjectBuilder.SetComponent("SubClass", CommonComponents.Cross);
            }
            return subjectBuilder;
        }
        

        private static void AddBasicComponents(SubjectBuilder subjectBuilder, string account, string ccyPair,
            string dealType, string liquidityProvider, string currency,
            string quantity, string assetClass, string level, string requestFor)
        {
            subjectBuilder
                .SetComponent(SubjectComponentName.BuySideAccount, account)
                .SetComponent(SubjectComponentName.AssetClass, assetClass)
                .SetComponent(SubjectComponentName.Currency, currency)
                .SetComponent(SubjectComponentName.Quantity, quantity)
                .SetComponent(SubjectComponentName.RequestFor, requestFor)
                .SetComponent(SubjectComponentName.LiquidityProvider, liquidityProvider)
                .SetComponent(SubjectComponentName.DealType, dealType)
                .SetComponent(SubjectComponentName.Symbol, ccyPair)
                .SetComponent(SubjectComponentName.Level, level);
        }
    }
}