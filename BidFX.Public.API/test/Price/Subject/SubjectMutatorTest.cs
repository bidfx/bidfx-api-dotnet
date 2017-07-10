using NUnit.Framework;

namespace BidFX.Public.API.Price.Subject
{
    public class SubjectMutatorTest
    {
        [Test]
        public void TestSpotRfsLevel1Mutator()
        {
            var subjectBuilder = new SubjectBuilder();
            subjectBuilder
                .SetComponent(SubjectComponentName.BuySideAccount, "FX_ACCT")
                .SetComponent(SubjectComponentName.AssetClass, "Fx")
                .SetComponent(SubjectComponentName.Currency, "EUR")
                .SetComponent(SubjectComponentName.Exchange, "OTC")
                .SetComponent(SubjectComponentName.Quantity, "1000000.00")
                .SetComponent(SubjectComponentName.QuoteStyle, "RFS")
                .SetComponent(SubjectComponentName.LiquidityProvider, "RBCFX")
                .SetComponent(SubjectComponentName.DealType, "Spot")
                .SetComponent(SubjectComponentName.Symbol, "EURUSD")
                .SetComponent(SubjectComponentName.Level, "1")
                .SetComponent(SubjectComponentName.User, "pmacdona");
            var oldVersion = SubjectMutator.ToOldVersion(subjectBuilder.CreateSubject());
            Assert.AreEqual(
                "Account=TRADINGSCREEN,AssetClass=Fx,Currency=EUR,Customer=0001,Exchange=OTC,Level=1,Quantity=1000000.00,QuoteStyle=RFS,Source=RBCFX,SubClass=Spot,Symbol=EURUSD,User=pmacdona",
                oldVersion.ToString());
            Assert.AreEqual(subjectBuilder.CreateSubject(), SubjectMutator.ToNewVersion(oldVersion));
        }
        
        [Test]
        public void TestForwardRfsLevel1Mutator()
        {
            var subjectBuilder = new SubjectBuilder();
            subjectBuilder
                .SetComponent(SubjectComponentName.BuySideAccount, "FX_ACCT")
                .SetComponent(SubjectComponentName.AssetClass, "Fx")
                .SetComponent(SubjectComponentName.Currency, "EUR")
                .SetComponent(SubjectComponentName.Exchange, "OTC")
                .SetComponent(SubjectComponentName.Quantity, "1000000.00")
                .SetComponent(SubjectComponentName.QuoteStyle, "RFS")
                .SetComponent(SubjectComponentName.LiquidityProvider, "BNPFX")
                .SetComponent(SubjectComponentName.DealType, "Forward")
                .SetComponent(SubjectComponentName.Symbol, "EURUSD")
                .SetComponent(SubjectComponentName.Level, "1")
                .SetComponent(SubjectComponentName.User, "pmacdona")
                .SetComponent(SubjectComponentName.SettlementDate, "20170909");
            var oldVersion = SubjectMutator.ToOldVersion(subjectBuilder.CreateSubject());
            Assert.AreEqual(
                "Account=TSCREENTEST,AssetClass=Fx,Currency=EUR,Customer=0001,Exchange=OTC,Level=1,Quantity=1000000.00,QuoteStyle=RFS,Source=BNPFX,SubClass=Forward,Symbol=EURUSD,User=pmacdona,ValueDate=20170909",
                oldVersion.ToString());
            Assert.AreEqual(subjectBuilder.CreateSubject(), SubjectMutator.ToNewVersion(oldVersion));
        }
        
        [Test]
        public void TestNdfRfsLevel1Mutator()
        {
            var subjectBuilder = new SubjectBuilder();
            subjectBuilder
                .SetComponent(SubjectComponentName.BuySideAccount, "FX_ACCT")
                .SetComponent(SubjectComponentName.AssetClass, "Fx")
                .SetComponent(SubjectComponentName.Currency, "EUR")
                .SetComponent(SubjectComponentName.Exchange, "OTC")
                .SetComponent(SubjectComponentName.Quantity, "1000000.00")
                .SetComponent(SubjectComponentName.QuoteStyle, "RFS")
                .SetComponent(SubjectComponentName.LiquidityProvider, "BNPFX")
                .SetComponent(SubjectComponentName.DealType, "NDF")
                .SetComponent(SubjectComponentName.Symbol, "EURUSD")
                .SetComponent(SubjectComponentName.Level, "1")
                .SetComponent(SubjectComponentName.User, "pmacdona")
                .SetComponent(SubjectComponentName.SettlementDate, "20170909")
                .SetComponent(SubjectComponentName.FixingDate, "20170910");
            var oldVersion = SubjectMutator.ToOldVersion(subjectBuilder.CreateSubject());
            Assert.AreEqual(
                "Account=TSCREENTEST,AssetClass=Fx,Currency=EUR,Customer=0001,Exchange=OTC,FixingDate=20170910,Level=1,Quantity=1000000.00,QuoteStyle=RFS,Source=BNPFX,SubClass=NDF,Symbol=EURUSD,User=pmacdona,ValueDate=20170909",
                oldVersion.ToString());
            Assert.AreEqual(subjectBuilder.CreateSubject(), SubjectMutator.ToNewVersion(oldVersion));
        }
        
        [Test]
        public void TestSwapRfsLevel1Mutator()
        {
            var subjectBuilder = new SubjectBuilder();
            subjectBuilder
                .SetComponent(SubjectComponentName.BuySideAccount, "FX_ACCT")
                .SetComponent(SubjectComponentName.AssetClass, "Fx")
                .SetComponent(SubjectComponentName.Currency, "EUR")
                .SetComponent(SubjectComponentName.Exchange, "OTC")
                .SetComponent(SubjectComponentName.Quantity, "1000000.00")
                .SetComponent(SubjectComponentName.QuoteStyle, "RFS")
                .SetComponent(SubjectComponentName.LiquidityProvider, "BNPFX")
                .SetComponent(SubjectComponentName.DealType, "Swap")
                .SetComponent(SubjectComponentName.Symbol, "EURUSD")
                .SetComponent(SubjectComponentName.Level, "1")
                .SetComponent(SubjectComponentName.User, "pmacdona")
                .SetComponent(SubjectComponentName.SettlementDate, "20170909")
                .SetComponent(SubjectComponentName.SettlementDate2, "20171009");
            var oldVersion = SubjectMutator.ToOldVersion(subjectBuilder.CreateSubject());
            Assert.AreEqual(
                "Account=TSCREENTEST,AssetClass=Fx,Currency=EUR,Customer=0001,Exchange=OTC,Level=1,Quantity=1000000.00,QuoteStyle=RFS,Source=BNPFX,SubClass=Swap,Symbol=EURUSD,User=pmacdona,ValueDate=20170909,ValueDate2=20171009",
                oldVersion.ToString());
            Assert.AreEqual(subjectBuilder.CreateSubject(), SubjectMutator.ToNewVersion(oldVersion));
        }
        
        [Test]
        public void TestNdsRfsLevel1Mutator()
        {
            var subjectBuilder = new SubjectBuilder();
            subjectBuilder
                .SetComponent(SubjectComponentName.BuySideAccount, "FX_ACCT")
                .SetComponent(SubjectComponentName.AssetClass, "Fx")
                .SetComponent(SubjectComponentName.Currency, "EUR")
                .SetComponent(SubjectComponentName.Exchange, "OTC")
                .SetComponent(SubjectComponentName.Quantity, "1000000.00")
                .SetComponent(SubjectComponentName.QuoteStyle, "RFS")
                .SetComponent(SubjectComponentName.LiquidityProvider, "BNPFX")
                .SetComponent(SubjectComponentName.DealType, "NDS")
                .SetComponent(SubjectComponentName.Symbol, "EURUSD")
                .SetComponent(SubjectComponentName.Level, "1")
                .SetComponent(SubjectComponentName.User, "pmacdona")
                .SetComponent(SubjectComponentName.SettlementDate, "20170909")
                .SetComponent(SubjectComponentName.SettlementDate2, "20171009")
                .SetComponent(SubjectComponentName.FixingDate, "20170910")
                .SetComponent(SubjectComponentName.FixingDate2, "20170911");
            var oldVersion = SubjectMutator.ToOldVersion(subjectBuilder.CreateSubject());
            Assert.AreEqual(
                "Account=TSCREENTEST,AssetClass=Fx,Currency=EUR,Customer=0001,Exchange=OTC,FixingDate=20170910,FixingDate2=20170911,Level=1,Quantity=1000000.00,QuoteStyle=RFS,Source=BNPFX,SubClass=NDS,Symbol=EURUSD,User=pmacdona,ValueDate=20170909,ValueDate2=20171009",
                oldVersion.ToString());
            Assert.AreEqual(subjectBuilder.CreateSubject(), SubjectMutator.ToNewVersion(oldVersion));
        }
    }
}