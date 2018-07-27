using BidFX.Public.API.Enums;
using BidFX.Public.API.Price.Plugin.Puffin;

namespace BidFX.Public.API.Price.Subject
{
    public class CommonSubjects
    {
        // ******************LEVEL 1 SPOT******************

        private static SubjectBuilder CreateLevelOneSpotSubject(string user, string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity, string requestFor)
        {
            SubjectBuilder subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, user, account, ccyPair, CommonComponents.Spot, liquidityProvider, currency,
                quantity, CommonComponents.Fx, "1", requestFor);
            subjectBuilder.SetComponent(SubjectComponentName.Tenor, CommonComponents.Spot);
            return subjectBuilder;
        }

        public static SubjectBuilder CreateLevelOneSpotStreamingSubject(string user, string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity)
        {
            return CreateLevelOneSpotSubject(user, account, ccyPair, liquidityProvider, currency, quantity,
                CommonComponents.Stream);
        }

        public static SubjectBuilder CreateLevelOneSpotQuoteSubject(string user, string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity)
        {
            SubjectBuilder levelOneSpotRfqSubject =
                CreateLevelOneSpotSubject(user, account, ccyPair, liquidityProvider, currency, quantity,
                    CommonComponents.Quote);
            return levelOneSpotRfqSubject;
        }

        // ******************LEVEL 1 FORWARD******************

        private static SubjectBuilder CreateLevelOneForwardSubject(string user, string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity, string tenor, string settlementDate, string requestFor)
        {
            SubjectBuilder subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, user,account, ccyPair,
                CommonComponents.Forward, liquidityProvider, currency, quantity, CommonComponents.Fx, "1", requestFor);
            if (!string.IsNullOrWhiteSpace(tenor))
            {
                FxTenor fxTenor = FxTenor.GetTenor(tenor);
                subjectBuilder.SetComponent(SubjectComponentName.Tenor, fxTenor != null ? fxTenor.GetBizString() : tenor);
            }

            if (!string.IsNullOrWhiteSpace(settlementDate))
            {
                subjectBuilder.SetComponent(SubjectComponentName.SettlementDate, settlementDate);
            }

            return subjectBuilder;
        }

        public static SubjectBuilder CreateLevelOneForwardStreamingSubject(string user, string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity, string tenor, string settlementDate)
        {
            return CreateLevelOneForwardSubject(user, account, ccyPair, liquidityProvider, currency, quantity, tenor,
                settlementDate, CommonComponents.Stream);
        }

        public static SubjectBuilder CreateLevelOneForwardQuoteSubject(string user, string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity, string tenor, string settlementDate)
        {
            SubjectBuilder levelOneForwardRfqSubject =
                CreateLevelOneForwardSubject(user, account, ccyPair, liquidityProvider, currency, quantity, tenor,
                    settlementDate, CommonComponents.Quote);
            return levelOneForwardRfqSubject;
        }

        // ******************LEVEL 1 NDF******************

        private static SubjectBuilder CreateLevelOneNdfSubject(string user, string account, string ccyPair,
            string liquidityProvider,
            string currency,
            string quantity, string tenor, string settlementDate, string requestFor)
        {
            SubjectBuilder subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, user, account, ccyPair, CommonComponents.NDF, liquidityProvider, currency,
                quantity, CommonComponents.Fx, "1", requestFor);

            if (!string.IsNullOrWhiteSpace(tenor))
            {
                FxTenor fxTenor = FxTenor.GetTenor(tenor);
                subjectBuilder.SetComponent(SubjectComponentName.Tenor, fxTenor != null ? fxTenor.GetBizString() : tenor);
            }

            if (!string.IsNullOrWhiteSpace(settlementDate))
            {
                subjectBuilder.SetComponent(SubjectComponentName.SettlementDate, settlementDate);
            }

            return subjectBuilder;
        }

        public static SubjectBuilder CreateLevelOneNdfStreamingSubject(string user, string account, string ccyPair,
            string liquidityProvider,
            string currency,
            string quantity, string tenor, string settlementDate)
        {
            return CreateLevelOneNdfSubject(user, account, ccyPair, liquidityProvider, currency, quantity, tenor,
                settlementDate, CommonComponents.Stream);
        }

        public static SubjectBuilder CreateLevelOneNdfQuoteSubject(string user, string account, string ccyPair,
            string liquidityProvider,
            string currency,
            string quantity, string tenor, string settlementDate)
        {
            SubjectBuilder levelOneNdfRfqSubject = CreateLevelOneNdfSubject(user, account, ccyPair, liquidityProvider, currency,
                quantity, tenor, settlementDate, CommonComponents.Quote);
            return levelOneNdfRfqSubject;
        }

        // ******************LEVEL 1 SWAP******************

        private static SubjectBuilder CreateLevelOneSwapSubject(string user, string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity, string tenor, string settlementDate, string farQuantity, string farTenor,
            string farSettlementDate, string requestFor)
        {
            SubjectBuilder subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, user, account, ccyPair, CommonComponents.Swap, liquidityProvider, currency,
                quantity, CommonComponents.Fx, "1", requestFor);
            subjectBuilder
                .SetComponent(SubjectComponentName.FarCurrency, currency)
                .SetComponent(SubjectComponentName.FarQuantity, farQuantity);

            if (!string.IsNullOrWhiteSpace(tenor))
            {
                FxTenor fxTenor = FxTenor.GetTenor(tenor);
                subjectBuilder.SetComponent(SubjectComponentName.Tenor, fxTenor != null ? fxTenor.GetBizString() : tenor);
            }

            if (!string.IsNullOrWhiteSpace(settlementDate))
            {
                subjectBuilder.SetComponent(SubjectComponentName.SettlementDate, settlementDate);
            }

            if (!string.IsNullOrWhiteSpace(farTenor))
            {
                FxTenor fxTenor = FxTenor.GetTenor(farTenor);
                subjectBuilder.SetComponent(SubjectComponentName.FarTenor, fxTenor != null ? fxTenor.GetBizString() : farTenor);
            }

            if (!string.IsNullOrWhiteSpace(farSettlementDate))
            {
                subjectBuilder.SetComponent(SubjectComponentName.FarSettlementDate, farSettlementDate);
            }

            return subjectBuilder;
        }

        public static SubjectBuilder CreateLevelOneSwapStreamingSubject(string user, string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity, string tenor, string settlementDate, string farQuantity, string farTenor,
            string farSettlementDate)
        {
            return CreateLevelOneSwapSubject(user, account, ccyPair, liquidityProvider, currency, quantity, tenor,
                settlementDate,
                farQuantity, farTenor, farSettlementDate, CommonComponents.Stream);
        }

        public static SubjectBuilder CreateLevelOneSwapQuoteSubject(string user, string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity, string tenor, string settlementDate, string farQuantity, string farTenor,
            string farSettlementDate)
        {
            SubjectBuilder levelOneSwapRfqSubject = CreateLevelOneSwapSubject(user, account, ccyPair, liquidityProvider, currency,
                quantity, tenor, settlementDate, farQuantity, farTenor, farSettlementDate, CommonComponents.Quote);
            return levelOneSwapRfqSubject;
        }

        // ******************LEVEL 1 NDS******************

        private static SubjectBuilder CreateLevelOneNdsSubject(string user, string account, string ccyPair, string liquidityProvider,
            string currency,
            string quantity, string tenor, string settlementDate,
            string farQuantity, string farTenor,
            string farSettlementDate, string requestFor)
        {
            SubjectBuilder subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, user, account, ccyPair, CommonComponents.NDS, liquidityProvider, currency,
                quantity, CommonComponents.Fx, "1", requestFor);
            subjectBuilder
                .SetComponent(SubjectComponentName.FarCurrency, currency)
                .SetComponent(SubjectComponentName.FarQuantity, farQuantity);
            if (!string.IsNullOrWhiteSpace(tenor))
            {
                FxTenor fxTenor = FxTenor.GetTenor(tenor);
                subjectBuilder.SetComponent(SubjectComponentName.Tenor, fxTenor != null ? fxTenor.GetBizString() : tenor);
            }

            if (!string.IsNullOrWhiteSpace(settlementDate))
            {
                subjectBuilder.SetComponent(SubjectComponentName.SettlementDate, settlementDate);
            }

            if (!string.IsNullOrWhiteSpace(farTenor))
            {
                FxTenor fxTenor = FxTenor.GetTenor(farTenor);
                subjectBuilder.SetComponent(SubjectComponentName.FarTenor, fxTenor != null ? fxTenor.GetBizString() : farTenor);
            }

            if (!string.IsNullOrWhiteSpace(farSettlementDate))
            {
                subjectBuilder.SetComponent(SubjectComponentName.FarSettlementDate, farSettlementDate);
            }

            return subjectBuilder;
        }

        public static SubjectBuilder CreateLevelOneNdsStreamingSubject(string user, string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity, string tenor, string settlementDate,
            string farQuantity, string farTenor,
            string farSettlementDate)
        {
            return CreateLevelOneNdsSubject(user, account, ccyPair, liquidityProvider, currency, quantity, tenor,
                settlementDate, farQuantity, farTenor, farSettlementDate, CommonComponents.Stream);
        }

        public static SubjectBuilder CreateLevelOneNdsQuoteSubject(string user, string account, string ccyPair,
            string liquidityProvider, string currency,
            string quantity, string tenor, string settlementDate,
            string farQuantity, string farTenor,
            string farSettlementDate)
        {
            SubjectBuilder levelOneNdsRfqSubject = CreateLevelOneNdsSubject(user, account, ccyPair, liquidityProvider, currency,
                quantity, tenor, settlementDate, farQuantity, farTenor, farSettlementDate, CommonComponents.Quote);
            return levelOneNdsRfqSubject;
        }

        // ******************LEVEL 2 SPOT******************

        public static SubjectBuilder CreateLevelTwoSpotStreamingSubject(string user, string account, string ccyPair, string currency,
            string quantity)
        {
            SubjectBuilder subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, user, account, ccyPair, CommonComponents.Spot, CommonComponents.FXTS, currency,
                quantity, CommonComponents.Fx, "2", CommonComponents.Stream);
            subjectBuilder.SetComponent(SubjectComponentName.Tenor, CommonComponents.Spot);
            return subjectBuilder;
        }

        // ******************LEVEL 2 FORWARD******************

        public static SubjectBuilder CreateLevelTwoForwardStreamingSubject(string user, string account, string ccyPair,
            string currency, string quantity, string tenor,
            string settlementDate)
        {
            SubjectBuilder subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, user, account, ccyPair, CommonComponents.Forward, CommonComponents.FXTS,
                currency, quantity, CommonComponents.Fx, "2", CommonComponents.Stream);
            if (!string.IsNullOrWhiteSpace(tenor))
            {
                FxTenor fxTenor = FxTenor.GetTenor(tenor);
                subjectBuilder.SetComponent(SubjectComponentName.Tenor, fxTenor != null ? fxTenor.GetBizString() : tenor);
            }

            if (!string.IsNullOrWhiteSpace(settlementDate))
            {
                subjectBuilder.SetComponent(SubjectComponentName.SettlementDate, settlementDate);
            }

            return subjectBuilder;
        }

        // ******************LEVEL 2 NDF******************

        public static SubjectBuilder CreateLevelTwoNdfStreamingSubject(string user, string account, string ccyPair, string currency,
            string quantity, string tenor,
            string settlementDate)
        {
            SubjectBuilder subjectBuilder = new SubjectBuilder();
            AddBasicComponents(subjectBuilder, user, account, ccyPair, CommonComponents.NDF, CommonComponents.FXTS, currency,
                quantity, CommonComponents.Fx, "2", CommonComponents.Stream);
            if (!string.IsNullOrWhiteSpace(tenor))
            {
                FxTenor fxTenor = FxTenor.GetTenor(tenor);
                subjectBuilder.SetComponent(SubjectComponentName.Tenor, fxTenor != null ? fxTenor.GetBizString() : tenor);
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


        private static void AddBasicComponents(SubjectBuilder subjectBuilder, string user, string account, string ccyPair,
            string dealType, string liquidityProvider, string currency,
            string quantity, string assetClass, string level, string requestFor)
        {
            subjectBuilder
                .SetIfNotNullOrEmpty(SubjectComponentName.User, user)
                .SetIfNotNullOrEmpty(SubjectComponentName.BuySideAccount, account)
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