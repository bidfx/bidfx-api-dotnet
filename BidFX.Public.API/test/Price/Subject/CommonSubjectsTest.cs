using NUnit.Framework;

namespace BidFX.Public.API.Price.Subject
{
    public class SubjectHelperTest
    {
        [Test]
        public void CreateLevelOneSpotStreamingSubjectTest()
        {
            Subject subj = CommonSubjects
                .CreateLevelOneSpotStreamingSubject("lasman", "FX_ACCT", "GBPUSD", "BARC", "GBP", "5000000.00").CreateSubject();
            Assert.AreEqual(
                "AssetClass=Fx,BuySideAccount=FX_ACCT,Currency=GBP,DealType=Spot,Level=1,LiquidityProvider=BARC,Quantity=5000000.00,RequestFor=Stream,Symbol=GBPUSD,User=lasman",
                subj.ToString());
        }

        [Test]
        public void CreateLevelOneSpotQuoteSubjectTest()
        {
            Subject subj = CommonSubjects.CreateLevelOneSpotQuoteSubject("lasman", "FX_ACCT", "EURUSD", "JPMX", "EUR", "10000000.00")
                .CreateSubject();
            Assert.AreEqual(
                "AssetClass=Fx,BuySideAccount=FX_ACCT,Currency=EUR,DealType=Spot,Level=1,LiquidityProvider=JPMX,Quantity=10000000.00,RequestFor=Quote,Symbol=EURUSD,User=lasman",
                subj.ToString());
        }

        [Test]
        public void CreateLevelOneForwardStreamingSubjectTest()
        {
            Subject subj = CommonSubjects.CreateLevelOneForwardStreamingSubject("lasman", "FX_ACCT", "GBPUSD", "BARC",
                "GBP", "1000000.00", null, "20180101").CreateSubject();
            Assert.AreEqual(
                "AssetClass=Fx,BuySideAccount=FX_ACCT,Currency=GBP,DealType=Outright,Level=1,LiquidityProvider=BARC,Quantity=1000000.00,RequestFor=Stream,SettlementDate=20180101,Symbol=GBPUSD,User=lasman",
                subj.ToString());
        }

        [Test]
        public void CreateLevelOneForwardStreamingSubjectWithTenorTest()
        {
            Subject subj = CommonSubjects.CreateLevelOneForwardStreamingSubject("lasman","FX_ACCT", "GBPUSD", "BARC",
                "GBP", "1000000.00", "2M", null).CreateSubject();
            Assert.AreEqual(
                "AssetClass=Fx,BuySideAccount=FX_ACCT,Currency=GBP,DealType=Outright,Level=1,LiquidityProvider=BARC,Quantity=1000000.00,RequestFor=Stream,Symbol=GBPUSD,Tenor=2M,User=lasman",
                subj.ToString());
        }

        [Test]
        public void CreateLevelOneForwardQuoteSubjectTest()
        {
            Subject subj = CommonSubjects.CreateLevelOneForwardQuoteSubject("lasman","FX_ACCT", "GBPUSD", "BARC", "GBP",
                "1000000.00", null, "20180101").CreateSubject();
            Assert.AreEqual(
                "AssetClass=Fx,BuySideAccount=FX_ACCT,Currency=GBP,DealType=Outright,Level=1,LiquidityProvider=BARC,Quantity=1000000.00,RequestFor=Quote,SettlementDate=20180101,Symbol=GBPUSD,User=lasman",
                subj.ToString());
        }

        [Test]
        public void CreateLevelOneForwardQuoteSubjectWithTenorTest()
        {
            Subject subj = CommonSubjects.CreateLevelOneForwardQuoteSubject("lasman", "FX_ACCT", "GBPUSD", "BARC", "GBP",
                "1000000.00", "1W", null).CreateSubject();
            Assert.AreEqual(
                "AssetClass=Fx,BuySideAccount=FX_ACCT,Currency=GBP,DealType=Outright,Level=1,LiquidityProvider=BARC,Quantity=1000000.00,RequestFor=Quote,Symbol=GBPUSD,Tenor=1W,User=lasman",
                subj.ToString());
        }

        [Test]
        public void CreateLevelOneNdfStreamingSubjectTest()
        {
            Subject subj = CommonSubjects.CreateLevelOneNdfStreamingSubject("lasman", "FX_ACCT", "GBPUSD", "BARC", "GBP",
                "1000000.00", null, "20180103").CreateSubject();
            Assert.AreEqual(
                "AssetClass=Fx,BuySideAccount=FX_ACCT,Currency=GBP,DealType=NDF,Level=1,LiquidityProvider=BARC,Quantity=1000000.00,RequestFor=Stream,SettlementDate=20180103,Symbol=GBPUSD,User=lasman",
                subj.ToString());
        }

        [Test]
        public void CreateLevelOneNdfStreamingSubjectWithTenorTest()
        {
            Subject subj = CommonSubjects.CreateLevelOneNdfStreamingSubject("lasman", "FX_ACCT", "GBPUSD", "BARC", "GBP",
                "1000000.00", "4M", null).CreateSubject();
            Assert.AreEqual(
                "AssetClass=Fx,BuySideAccount=FX_ACCT,Currency=GBP,DealType=NDF,Level=1,LiquidityProvider=BARC,Quantity=1000000.00,RequestFor=Stream,Symbol=GBPUSD,Tenor=4M,User=lasman",
                subj.ToString());
        }

        [Test]
        public void CreateLevelOneNdfQuoteSubjectTest()
        {
            Subject subj = CommonSubjects.CreateLevelOneNdfQuoteSubject("lasman", "FX_ACCT", "GBPUSD", "BARC", "GBP",
                "1000000.00", null, "20180103").CreateSubject();
            Assert.AreEqual(
                "AssetClass=Fx,BuySideAccount=FX_ACCT,Currency=GBP,DealType=NDF,Level=1,LiquidityProvider=BARC,Quantity=1000000.00,RequestFor=Quote,SettlementDate=20180103,Symbol=GBPUSD,User=lasman",
                subj.ToString());
        }

        [Test]
        public void CreateLevelOneNdfQuoteSubjectWithTenorTest()
        {
            Subject subj = CommonSubjects.CreateLevelOneNdfQuoteSubject("lasman", "FX_ACCT", "GBPUSD", "BARC", "GBP",
                "1000000.00", "2W", null).CreateSubject();
            Assert.AreEqual(
                "AssetClass=Fx,BuySideAccount=FX_ACCT,Currency=GBP,DealType=NDF,Level=1,LiquidityProvider=BARC,Quantity=1000000.00,RequestFor=Quote,Symbol=GBPUSD,Tenor=2W,User=lasman",
                subj.ToString());
        }

        [Test]
        public void CreateLevelOneSwapStreamingSubjectTest()
        {
            Subject subj = CommonSubjects.CreateLevelOneSwapStreamingSubject("lasman", "FX_ACCT", "GBPUSD", "BARC", "GBP",
                "1000000.00",
                null, "20180101", "1000000.00", null, "20180202").CreateSubject();
            Assert.AreEqual(
                "AssetClass=Fx,BuySideAccount=FX_ACCT,Currency=GBP,DealType=Swap,FarCurrency=GBP,FarQuantity=1000000.00,FarSettlementDate=20180202,Level=1,LiquidityProvider=BARC,Quantity=1000000.00,RequestFor=Stream,SettlementDate=20180101,Symbol=GBPUSD,User=lasman",
                subj.ToString());
        }

        [Test]
        public void CreateLevelOneSwapStreamingSubjectWithTenorTest()
        {
            Subject subj = CommonSubjects.CreateLevelOneSwapStreamingSubject("lasman", "FX_ACCT", "GBPUSD", "BARC", "GBP",
                "1000000.00",
                "IMMM", null, "1000000.00", "IMMZ", null).CreateSubject();
            Assert.AreEqual(
                "AssetClass=Fx,BuySideAccount=FX_ACCT,Currency=GBP,DealType=Swap,FarCurrency=GBP,FarQuantity=1000000.00,FarTenor=IMMZ,Level=1,LiquidityProvider=BARC,Quantity=1000000.00,RequestFor=Stream,Symbol=GBPUSD,Tenor=IMMM,User=lasman",
                subj.ToString());
        }

        [Test]
        public void CreateLevelOneSwapQuoteSubjectTest()
        {
            Subject subj = CommonSubjects.CreateLevelOneSwapQuoteSubject("lasman", "FX_ACCT", "GBPUSD", "BARC", "GBP", "1000000.00",
                null, "20180101", "1000000.00", null, "20180202").CreateSubject();
            Assert.AreEqual(
                "AssetClass=Fx,BuySideAccount=FX_ACCT,Currency=GBP,DealType=Swap,FarCurrency=GBP,FarQuantity=1000000.00,FarSettlementDate=20180202,Level=1,LiquidityProvider=BARC,Quantity=1000000.00,RequestFor=Quote,SettlementDate=20180101,Symbol=GBPUSD,User=lasman",
                subj.ToString());
        }

        [Test]
        public void CreateLevelOneSwapQuoteSubjectWithTenorTest()
        {
            Subject subj = CommonSubjects.CreateLevelOneSwapQuoteSubject("lasman", "FX_ACCT", "GBPUSD", "BARC", "GBP", "1000000.00",
                "6M", null, "1000000.00", "2Y", null).CreateSubject();
            Assert.AreEqual(
                "AssetClass=Fx,BuySideAccount=FX_ACCT,Currency=GBP,DealType=Swap,FarCurrency=GBP,FarQuantity=1000000.00,FarTenor=2Y,Level=1,LiquidityProvider=BARC,Quantity=1000000.00,RequestFor=Quote,Symbol=GBPUSD,Tenor=6M,User=lasman",
                subj.ToString());
        }

        [Test]
        public void CreateLevelOneNdsStreamingSubjectTest()
        {
            Subject subj = CommonSubjects.CreateLevelOneNdsStreamingSubject("lasman", "FX_ACCT", "EURGBP", "JPMX",
                "EUR", "25000000.00", null, "20180204", "25000000.00", null, "20180603").CreateSubject();
            Assert.AreEqual(
                "AssetClass=Fx,BuySideAccount=FX_ACCT,Currency=EUR,DealType=NDS,FarCurrency=EUR,FarQuantity=25000000.00,FarSettlementDate=20180603,Level=1,LiquidityProvider=JPMX,Quantity=25000000.00,RequestFor=Stream,SettlementDate=20180204,Symbol=EURGBP,User=lasman",
                subj.ToString());
        }

        [Test]
        public void CreateLevelOneNdsStreamingSubjectWithTenorTest()
        {
            Subject subj = CommonSubjects.CreateLevelOneNdsStreamingSubject("lasman", "FX_ACCT", "EURGBP", "JPMX",
                "EUR", "25000000.00", "1M", null, "25000000.00", "2M", null).CreateSubject();
            Assert.AreEqual(
                "AssetClass=Fx,BuySideAccount=FX_ACCT,Currency=EUR,DealType=NDS,FarCurrency=EUR,FarQuantity=25000000.00,FarTenor=2M,Level=1,LiquidityProvider=JPMX,Quantity=25000000.00,RequestFor=Stream,Symbol=EURGBP,Tenor=1M,User=lasman",
                subj.ToString());
        }

        [Test]
        public void CreateLevelOneNdsQuoteSubjectTest()
        {
            Subject subj = CommonSubjects.CreateLevelOneNdsQuoteSubject("lasman", "FX_ACCT", "EURGBP", "JPMX",
                "EUR", "25000000.00", null, "20180204", "25000000.00", null, "20180603").CreateSubject();
            Assert.AreEqual(
                "AssetClass=Fx,BuySideAccount=FX_ACCT,Currency=EUR,DealType=NDS,FarCurrency=EUR,FarQuantity=25000000.00,FarSettlementDate=20180603,Level=1,LiquidityProvider=JPMX,Quantity=25000000.00,RequestFor=Quote,SettlementDate=20180204,Symbol=EURGBP,User=lasman",
                subj.ToString());
        }

        [Test]
        public void CreateLevelOneNdsQuoteSubjectWithTenorTest()
        {
            Subject subj = CommonSubjects.CreateLevelOneNdsQuoteSubject("lasman", "FX_ACCT", "EURGBP", "JPMX",
                "EUR", "25000000.00", "1Y", null, "25000000.00", "2Y", null).CreateSubject();
            Assert.AreEqual(
                "AssetClass=Fx,BuySideAccount=FX_ACCT,Currency=EUR,DealType=NDS,FarCurrency=EUR,FarQuantity=25000000.00,FarTenor=2Y,Level=1,LiquidityProvider=JPMX,Quantity=25000000.00,RequestFor=Quote,Symbol=EURGBP,Tenor=1Y,User=lasman",
                subj.ToString());
        }

        [Test]
        public void CreateLevelTwoSpotStreamingSubjectTest()
        {
            Subject subj = CommonSubjects.CreateLevelTwoSpotStreamingSubject("lasman", "FX_ACCT", "GBPUSD", "GBP", "50000000.00")
                .CreateSubject();
            Assert.AreEqual(
                "AssetClass=Fx,BuySideAccount=FX_ACCT,Currency=GBP,DealType=Spot,Level=2,LiquidityProvider=FXTS,Quantity=50000000.00,RequestFor=Stream,Symbol=GBPUSD,User=lasman",
                subj.ToString());
        }

        [Test]
        public void CreateLevelTwoForwardSubjectTest()
        {
            Subject subj = CommonSubjects
                .CreateLevelTwoForwardStreamingSubject("lasman", "FX_ACCT", "GBPUSD", "GBP", "50000000.00", null, "20180601")
                .CreateSubject();
            Assert.AreEqual(
                "AssetClass=Fx,BuySideAccount=FX_ACCT,Currency=GBP,DealType=Outright,Level=2,LiquidityProvider=FXTS,Quantity=50000000.00,RequestFor=Stream,SettlementDate=20180601,Symbol=GBPUSD,User=lasman",
                subj.ToString());
        }

        [Test]
        public void CreateLevelTwoForwardSubjectWithTenorTest()
        {
            Subject subj = CommonSubjects
                .CreateLevelTwoForwardStreamingSubject("lasman", "FX_ACCT", "GBPUSD", "GBP", "50000000.00", "2M", null)
                .CreateSubject();
            Assert.AreEqual(
                "AssetClass=Fx,BuySideAccount=FX_ACCT,Currency=GBP,DealType=Outright,Level=2,LiquidityProvider=FXTS,Quantity=50000000.00,RequestFor=Stream,Symbol=GBPUSD,Tenor=2M,User=lasman",
                subj.ToString());
        }

        [Test]
        public void CreateLevelTwoNdfSubjectTest()
        {
            Subject subj = CommonSubjects
                .CreateLevelTwoNdfStreamingSubject("lasman", "FX_ACCT", "GBPUSD", "GBP", "50000000.00", null, "20180603")
                .CreateSubject();
            Assert.AreEqual(
                "AssetClass=Fx,BuySideAccount=FX_ACCT,Currency=GBP,DealType=NDF,Level=2,LiquidityProvider=FXTS,Quantity=50000000.00,RequestFor=Stream,SettlementDate=20180603,Symbol=GBPUSD,User=lasman",
                subj.ToString());
        }

        [Test]
        public void CreateLevelTwoNdfSubjectWithTenorTest()
        {
            Subject subj = CommonSubjects
                .CreateLevelTwoNdfStreamingSubject("lasman", "FX_ACCT", "GBPUSD", "GBP", "50000000.00", "2M", null)
                .CreateSubject();
            Assert.AreEqual(
                "AssetClass=Fx,BuySideAccount=FX_ACCT,Currency=GBP,DealType=NDF,Level=2,LiquidityProvider=FXTS,Quantity=50000000.00,RequestFor=Stream,Symbol=GBPUSD,Tenor=2M,User=lasman",
                subj.ToString());
        }
    }
}