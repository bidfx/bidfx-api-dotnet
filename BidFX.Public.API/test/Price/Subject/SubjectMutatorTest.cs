using NUnit.Framework;

namespace BidFX.Public.API.Price.Subject
{
    public class SubjectMutatorTest
    {
        [Test]
        public void TestSpotRfsLevel1Mutator()
        {
            SubjectBuilder subjectBuilder = new SubjectBuilder();
            subjectBuilder
                .SetComponent(SubjectComponentName.BuySideAccount, "FX_ACCT")
                .SetComponent(SubjectComponentName.AssetClass, "Fx")
                .SetComponent(SubjectComponentName.Currency, "EUR")
                .SetComponent(SubjectComponentName.Quantity, "1000000.00")
                .SetComponent(SubjectComponentName.RequestFor, "Stream")
                .SetComponent(SubjectComponentName.LiquidityProvider, "RBCFX")
                .SetComponent(SubjectComponentName.DealType, "Spot")
                .SetComponent(SubjectComponentName.Symbol, "EURUSD")
                .SetComponent(SubjectComponentName.Level, "1")
                .SetComponent(SubjectComponentName.User, "pmacdona");
            Subject oldVersion = SubjectMutator.ToOldVersion(subjectBuilder.CreateSubject());
            Assert.AreEqual(
                "Account=TRADINGSCREEN,AssetClass=Fx,Currency=EUR,Customer=0001,Exchange=OTC,Level=1,Quantity=1000000.00,QuoteStyle=RFS,Source=RBCFX,SubClass=Spot,Symbol=EURUSD,User=pmacdona",
                oldVersion.ToString());
        }

        [Test]
        public void TestForwardRfsLevel1Mutator()
        {
            SubjectBuilder subjectBuilder = new SubjectBuilder();
            subjectBuilder
                .SetComponent(SubjectComponentName.BuySideAccount, "FX_ACCT")
                .SetComponent(SubjectComponentName.AssetClass, "Fx")
                .SetComponent(SubjectComponentName.Currency, "EUR")
                .SetComponent(SubjectComponentName.Quantity, "1000000.00")
                .SetComponent(SubjectComponentName.RequestFor, "Stream")
                .SetComponent(SubjectComponentName.LiquidityProvider, "BNPFX")
                .SetComponent(SubjectComponentName.DealType, "Forward")
                .SetComponent(SubjectComponentName.Symbol, "EURUSD")
                .SetComponent(SubjectComponentName.Level, "1")
                .SetComponent(SubjectComponentName.User, "pmacdona")
                .SetComponent(SubjectComponentName.SettlementDate, "20170909");
            Subject oldVersion = SubjectMutator.ToOldVersion(subjectBuilder.CreateSubject());
            Assert.AreEqual(
                "Account=TSCREENTEST,AssetClass=Fx,Currency=EUR,Customer=0001,Exchange=OTC,Level=1,Quantity=1000000.00,QuoteStyle=RFS,Source=BNPFX,SubClass=Forward,Symbol=EURUSD,User=pmacdona,ValueDate=20170909",
                oldVersion.ToString());
        }

        [Test]
        public void TestNdfRfsLevel1Mutator()
        {
            SubjectBuilder subjectBuilder = new SubjectBuilder();
            subjectBuilder
                .SetComponent(SubjectComponentName.BuySideAccount, "FX_ACCT")
                .SetComponent(SubjectComponentName.AssetClass, "Fx")
                .SetComponent(SubjectComponentName.Currency, "EUR")
                .SetComponent(SubjectComponentName.Quantity, "1000000.00")
                .SetComponent(SubjectComponentName.RequestFor, "Stream")
                .SetComponent(SubjectComponentName.LiquidityProvider, "BNPFX")
                .SetComponent(SubjectComponentName.DealType, "NDF")
                .SetComponent(SubjectComponentName.Symbol, "EURUSD")
                .SetComponent(SubjectComponentName.Level, "1")
                .SetComponent(SubjectComponentName.User, "pmacdona")
                .SetComponent(SubjectComponentName.SettlementDate, "20170909")
                .SubjectComponent(SubjectComponentName.FixingDate, "20170910");
            Subject oldVersion = SubjectMutator.ToOldVersion(subjectBuilder.CreateSubject());
            Assert.AreEqual(
                "Account=TSCREENTEST,AssetClass=Fx,Currency=EUR,Customer=0001,Exchange=OTC,FixingDate=20170910,Level=1,Quantity=1000000.00,QuoteStyle=RFS,Source=BNPFX,SubClass=NDF,Symbol=EURUSD,User=pmacdona,ValueDate=20170909",
                oldVersion.ToString());
        }

        [Test]
        public void TestSwapRfsLevel1Mutator()
        {
            SubjectBuilder subjectBuilder = new SubjectBuilder();
            subjectBuilder
                .SetComponent(SubjectComponentName.BuySideAccount, "FX_ACCT")
                .SetComponent(SubjectComponentName.AssetClass, "Fx")
                .SetComponent(SubjectComponentName.Currency, "EUR")
                .SetComponent(SubjectComponentName.Quantity, "1000000.00")
                .SetComponent(SubjectComponentName.RequestFor, "Stream")
                .SetComponent(SubjectComponentName.LiquidityProvider, "BNPFX")
                .SetComponent(SubjectComponentName.DealType, "Swap")
                .SetComponent(SubjectComponentName.Symbol, "EURUSD")
                .SetComponent(SubjectComponentName.Level, "1")
                .SetComponent(SubjectComponentName.User, "pmacdona")
                .SetComponent(SubjectComponentName.SettlementDate, "20170909")
                .SetComponent(SubjectComponentName.FarSettlementDate, "20171009");
            Subject oldVersion = SubjectMutator.ToOldVersion(subjectBuilder.CreateSubject());
            Assert.AreEqual(
                "Account=TSCREENTEST,AssetClass=Fx,Currency=EUR,Customer=0001,Exchange=OTC,LegCount=2,Level=1,Quantity=1000000.00,QuoteStyle=RFS,Source=BNPFX,SubClass=Swap,Symbol=EURUSD,User=pmacdona,ValueDate=20170909,ValueDate2=20171009",
                oldVersion.ToString());
        }

        [Test]
        public void TestNdsRfsLevel1Mutator()
        {
            SubjectBuilder subjectBuilder = new SubjectBuilder();
            subjectBuilder
                .SetComponent(SubjectComponentName.BuySideAccount, "FX_ACCT")
                .SetComponent(SubjectComponentName.AssetClass, "Fx")
                .SetComponent(SubjectComponentName.Currency, "EUR")
                .SetComponent(SubjectComponentName.Quantity, "1000000.00")
                .SetComponent(SubjectComponentName.RequestFor, "Quote")
                .SetComponent(SubjectComponentName.LiquidityProvider, "BNPFX")
                .SetComponent(SubjectComponentName.DealType, "NDS")
                .SetComponent(SubjectComponentName.Symbol, "EURUSD")
                .SetComponent(SubjectComponentName.Level, "1")
                .SetComponent(SubjectComponentName.User, "pmacdona")
                .SetComponent(SubjectComponentName.SettlementDate, "20170909")
                .SetComponent(SubjectComponentName.FarSettlementDate, "20171009");
            subjectBuilder
                .SubjectComponent(SubjectComponentName.FixingDate, "20170910");
            subjectBuilder
                .SubjectComponent(SubjectComponentName.FarFixingDate, "20170911");
            Subject oldVersion = SubjectMutator.ToOldVersion(subjectBuilder.CreateSubject());
            Assert.AreEqual(
                "Account=TSCREENTEST,AssetClass=Fx,Currency=EUR,Customer=0001,Exchange=OTC,FixingDate=20170910,FixingDate2=20170911,LegCount=2,Level=1,Quantity=1000000.00,QuoteStyle=RFQ,Source=BNPFX,SubClass=NDS,Symbol=EURUSD,User=pmacdona,ValueDate=20170909,ValueDate2=20171009",
                oldVersion.ToString());
        }

        [Test]
        public void TestSpotRfsLevel2Mutator()
        {
            SubjectBuilder subjectBuilder = new SubjectBuilder();
            subjectBuilder
                .SetComponent(SubjectComponentName.BuySideAccount, "FX_ACCT")
                .SetComponent(SubjectComponentName.AssetClass, "Fx")
                .SetComponent(SubjectComponentName.Currency, "EUR")
                .SetComponent(SubjectComponentName.Quantity, "1000000.00")
                .SetComponent(SubjectComponentName.RequestFor, "Stream")
                .SetComponent(SubjectComponentName.LiquidityProvider, "RBCFX")
                .SetComponent(SubjectComponentName.DealType, "Spot")
                .SetComponent(SubjectComponentName.Symbol, "EURUSD")
                .SetComponent(SubjectComponentName.Level, "2")
                .SetComponent(SubjectComponentName.User, "pmacdona");
            Subject oldVersion = SubjectMutator.ToOldVersion(subjectBuilder.CreateSubject());
            Assert.AreEqual(
                "Account=FX_ACCT,AssetClass=Fx,Currency=EUR,Customer=0001,Exchange=OTC,Level=2,Quantity=1000000.00,QuoteStyle=RFS,Source=RBCFX,SubClass=Spot,Symbol=EURUSD,User=pmacdona",
                oldVersion.ToString());
        }
    }
}