namespace BidFX.Public.API.Price.Subject
{
    public class CommonSubjects
    {
        // ******************LEVEL 1 SPOT******************

        private static SubjectBuilder CreateLevelOneSpotSubject(string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity, string requestFor)
        {
            SubjectBuilder subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, ccyPair, CommonComponents.Spot, liquidityProvider, currency,
                quantity, CommonComponents.Fx, "1", requestFor);
            subjectBuilder.SetComponent(SubjectComponentName.Tenor, CommonComponents.Spot);
            return subjectBuilder;
        }

        public static SubjectBuilder CreateLevelOneSpotStreamingSubject(string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity)
        {
            return CreateLevelOneSpotSubject(account, ccyPair, liquidityProvider, currency, quantity,
                CommonComponents.Stream);
        }

        public static SubjectBuilder CreateLevelOneSpotQuoteSubject(string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity)
        {
            SubjectBuilder levelOneSpotRfqSubject =
                CreateLevelOneSpotSubject(account, ccyPair, liquidityProvider, currency, quantity,
                    CommonComponents.Quote);
            return levelOneSpotRfqSubject;
        }

        // ******************LEVEL 1 FORWARD******************

        private static SubjectBuilder CreateLevelOneForwardSubject(string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity, string tenor, string settlementDate, string requestFor)
        {
            SubjectBuilder subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, ccyPair,
                CommonComponents.Forward, liquidityProvider, currency, quantity, CommonComponents.Fx, "1", requestFor);
            if (!string.IsNullOrWhiteSpace(tenor))
            {
                subjectBuilder.SetComponent(SubjectComponentName.Tenor, tenor);
            }

            if (!string.IsNullOrWhiteSpace(settlementDate))
            {
                subjectBuilder.SetComponent(SubjectComponentName.SettlementDate, settlementDate);
            }

            return subjectBuilder;
        }

        public static SubjectBuilder CreateLevelOneForwardStreamingSubject(string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity, string tenor, string settlementDate)
        {
            return CreateLevelOneForwardSubject(account, ccyPair, liquidityProvider, currency, quantity, tenor,
                settlementDate, CommonComponents.Stream);
        }

        public static SubjectBuilder CreateLevelOneForwardQuoteSubject(string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity, string tenor, string settlementDate)
        {
            SubjectBuilder levelOneForwardRfqSubject =
                CreateLevelOneForwardSubject(account, ccyPair, liquidityProvider, currency, quantity, tenor,
                    settlementDate, CommonComponents.Quote);
            return levelOneForwardRfqSubject;
        }

        // ******************LEVEL 1 NDF******************

        private static SubjectBuilder CreateLevelOneNdfSubject(string account, string ccyPair,
            string liquidityProvider,
            string currency,
            string quantity, string tenor, string settlementDate, string requestFor)
        {
            SubjectBuilder subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, ccyPair, CommonComponents.NDF, liquidityProvider, currency,
                quantity, CommonComponents.Fx, "1", requestFor);

            if (!string.IsNullOrWhiteSpace(tenor))
            {
                subjectBuilder.SetComponent(SubjectComponentName.Tenor, tenor);
            }

            if (!string.IsNullOrWhiteSpace(settlementDate))
            {
                subjectBuilder.SetComponent(SubjectComponentName.SettlementDate, settlementDate);
            }

            return subjectBuilder;
        }

        public static SubjectBuilder CreateLevelOneNdfStreamingSubject(string account, string ccyPair,
            string liquidityProvider,
            string currency,
            string quantity, string tenor, string settlementDate)
        {
            return CreateLevelOneNdfSubject(account, ccyPair, liquidityProvider, currency, quantity, tenor,
                settlementDate, CommonComponents.Stream);
        }

        public static SubjectBuilder CreateLevelOneNdfQuoteSubject(string account, string ccyPair,
            string liquidityProvider,
            string currency,
            string quantity, string tenor, string settlementDate)
        {
            SubjectBuilder levelOneNdfRfqSubject = CreateLevelOneNdfSubject(account, ccyPair, liquidityProvider, currency,
                quantity, tenor, settlementDate, CommonComponents.Quote);
            return levelOneNdfRfqSubject;
        }

        // ******************LEVEL 1 SWAP******************

        private static SubjectBuilder CreateLevelOneSwapSubject(string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity, string tenor, string settlementDate, string farQuantity, string farTenor,
            string farSettlementDate, string requestFor)
        {
            SubjectBuilder subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, ccyPair, CommonComponents.Swap, liquidityProvider, currency,
                quantity, CommonComponents.Fx, "1", requestFor);
            subjectBuilder
                .SetComponent(SubjectComponentName.FarCurrency, currency)
                .SetComponent(SubjectComponentName.FarQuantity, farQuantity);

            if (!string.IsNullOrWhiteSpace(tenor))
            {
                subjectBuilder.SetComponent(SubjectComponentName.Tenor, tenor);
            }

            if (!string.IsNullOrWhiteSpace(settlementDate))
            {
                subjectBuilder.SetComponent(SubjectComponentName.SettlementDate, settlementDate);
            }

            if (!string.IsNullOrWhiteSpace(farTenor))
            {
                subjectBuilder.SetComponent(SubjectComponentName.FarTenor, farTenor);
            }

            if (!string.IsNullOrWhiteSpace(farSettlementDate))
            {
                subjectBuilder.SetComponent(SubjectComponentName.FarSettlementDate, farSettlementDate);
            }

            return subjectBuilder;
        }

        public static SubjectBuilder CreateLevelOneSwapStreamingSubject(string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity, string tenor, string settlementDate, string farQuantity, string farTenor,
            string farSettlementDate)
        {
            return CreateLevelOneSwapSubject(account, ccyPair, liquidityProvider, currency, quantity, tenor,
                settlementDate,
                farQuantity, farTenor, farSettlementDate, CommonComponents.Stream);
        }

        public static SubjectBuilder CreateLevelOneSwapQuoteSubject(string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity, string tenor, string settlementDate, string farQuantity, string farTenor,
            string farSettlementDate)
        {
            SubjectBuilder levelOneSwapRfqSubject = CreateLevelOneSwapSubject(account, ccyPair, liquidityProvider, currency,
                quantity, tenor, settlementDate, farQuantity, farTenor, farSettlementDate, CommonComponents.Quote);
            return levelOneSwapRfqSubject;
        }

        // ******************LEVEL 1 NDS******************

        private static SubjectBuilder CreateLevelOneNdsSubject(string account, string ccyPair, string liquidityProvider,
            string currency,
            string quantity, string tenor, string settlementDate,
            string farQuantity, string farTenor,
            string farSettlementDate, string requestFor)
        {
            SubjectBuilder subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, ccyPair, CommonComponents.NDS, liquidityProvider, currency,
                quantity, CommonComponents.Fx, "1", requestFor);
            subjectBuilder
                .SetComponent(SubjectComponentName.FarCurrency, currency)
                .SetComponent(SubjectComponentName.FarQuantity, farQuantity);
            if (!string.IsNullOrWhiteSpace(tenor))
            {
                subjectBuilder.SetComponent(SubjectComponentName.Tenor, tenor);
            }

            if (!string.IsNullOrWhiteSpace(settlementDate))
            {
                subjectBuilder.SetComponent(SubjectComponentName.SettlementDate, settlementDate);
            }

            if (!string.IsNullOrWhiteSpace(farTenor))
            {
                subjectBuilder.SetComponent(SubjectComponentName.FarTenor, farTenor);
            }

            if (!string.IsNullOrWhiteSpace(farSettlementDate))
            {
                subjectBuilder.SetComponent(SubjectComponentName.FarSettlementDate, farSettlementDate);
            }

            return subjectBuilder;
        }

        public static SubjectBuilder CreateLevelOneNdsStreamingSubject(string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity, string tenor, string settlementDate,
            string farQuantity, string farTenor,
            string farSettlementDate)
        {
            return CreateLevelOneNdsSubject(account, ccyPair, liquidityProvider, currency, quantity, tenor,
                settlementDate, farQuantity, farTenor, farSettlementDate, CommonComponents.Stream);
        }

        public static SubjectBuilder CreateLevelOneNdsQuoteSubject(string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity, string tenor, string settlementDate,
            string farQuantity, string farTenor,
            string farSettlementDate)
        {
            SubjectBuilder levelOneNdsRfqSubject = CreateLevelOneNdsSubject(account, ccyPair, liquidityProvider, currency,
                quantity, tenor, settlementDate, farQuantity, farTenor, farSettlementDate, CommonComponents.Quote);
            return levelOneNdsRfqSubject;
        }

        // ******************LEVEL 2 SPOT******************

        public static SubjectBuilder CreateLevelTwoSpotStreamingSubject(string account, string ccyPair, string currency,
            string quantity)
        {
            SubjectBuilder subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, ccyPair, CommonComponents.Spot, CommonComponents.FXTS, currency,
                quantity, CommonComponents.Fx, "2", CommonComponents.Stream);
            subjectBuilder.SetComponent(SubjectComponentName.Tenor, CommonComponents.Spot);
            return subjectBuilder;
        }

        // ******************LEVEL 2 FORWARD******************

        public static SubjectBuilder CreateLevelTwoForwardStreamingSubject(string account, string ccyPair,
            string currency, string quantity, string tenor,
            string settlementDate)
        {
            SubjectBuilder subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, ccyPair, CommonComponents.Forward, CommonComponents.FXTS,
                currency, quantity, CommonComponents.Fx, "2", CommonComponents.Stream);
            if (!string.IsNullOrWhiteSpace(tenor))
            {
                subjectBuilder.SetComponent(SubjectComponentName.Tenor, tenor);
            }

            if (!string.IsNullOrWhiteSpace(settlementDate))
            {
                subjectBuilder.SetComponent(SubjectComponentName.SettlementDate, settlementDate);
            }

            return subjectBuilder;
        }

        // ******************LEVEL 2 NDF******************

        public static SubjectBuilder CreateLevelTwoNdfStreamingSubject(string account, string ccyPair, string currency,
            string quantity, string tenor,
            string settlementDate)
        {
            SubjectBuilder subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, account, ccyPair, CommonComponents.NDF, CommonComponents.FXTS, currency,
                quantity, CommonComponents.Fx, "2", CommonComponents.Stream);
            if (!string.IsNullOrWhiteSpace(tenor))
            {
                subjectBuilder.SetComponent(SubjectComponentName.Tenor, tenor);
            }

            if (!string.IsNullOrWhiteSpace(settlementDate))
            {
                subjectBuilder.SetComponent(SubjectComponentName.SettlementDate, settlementDate);
            }

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
            SubjectBuilder subjectBuilder = new SubjectBuilder()
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