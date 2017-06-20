using System;
using BidFX.Public.API.Price.Plugin.Puffin;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Plugin.Pixie
{
    [TestFixture]
    public class SubjectSetRegisterTest
    {
        private static readonly Subject.Subject EURCHR_2MM = new Subject.Subject(
            "Account=DYMON,AllocAccount=DYMONCITI,AllocQty=2000000,AssetClass=Fx,Currency=EUR" +
            ",Customer=1A78668,Dealer=1B56446,Exchange=OTC,Level=1,NumAllocs=1" +
            ",Symbol=EURCHF,Quantity=2000000.00" +
            ",QuoteStyle=RFS,Source=JEFFX,SubClass=Spot,Tenor=Spot,User=cheechungli,ValueDate=20141114");


        private static readonly Subject.Subject EURGBP_1MM = new Subject.Subject(
            "Account=CIC,AllocAccount=CIC,AllocQty=1000000,AssetClass=Fx,Currency=EUR" +
            ",Customer=1A78668,Dealer=1B56446,Exchange=OTC,Level=1,NumAllocs=1" +
            ",Symbol=EURGBP,Quantity=1000000.00" +
            ",QuoteStyle=RFS,Source=MSFX,SubClass=Spot,Tenor=Spot,User=brucelee,ValueDate=20141109");


        private static readonly Subject.Subject EURGBP_5MM = new Subject.Subject(
            "Account=CIC,AllocAccount=CIC,AllocQty=5000000,AssetClass=Fx,Currency=EUR" +
            ",Customer=1A78668,Dealer=1B56446,Exchange=OTC,Level=1,NumAllocs=1" +
            ",Symbol=EURGBP,Quantity=5000000.00" +
            ",QuoteStyle=RFS,Source=MSFX,SubClass=Spot,Tenor=Spot,User=brucelee,ValueDate=20141107");


        private static readonly Subject.Subject GBPUSD_1MM = new Subject.Subject(
            "Account=AAA,AllocAccount=AAA,AllocQty=1000000,AssetClass=Fx,Currency=USD" +
            ",Customer=1A78668,Dealer=1B56446,Exchange=OTC,Level=1,NumAllocs=1" +
            ",Symbol=GBPUSD,Quantity=1000000.00" +
            ",QuoteStyle=RFQ,Source=MSFX,SubClass=Spot,Tenor=Spot,User=wchurchill,ValueDate=20141105");

        private readonly SubjectSetRegister _register = new SubjectSetRegister();

        [Test]
        public void SubjectSetByEditionMayNotBeNegativeEditionNumber()
        {
            try
            {
                _register.SubjectSetByEdition(-1);
                Assert.Fail("should throw exception");
            }
            catch (ArgumentException e)
            {
                
            }
        }

        [Test]
        public void SubjectSetByEditionReturnsNullInitially()
        {
            Assert.IsNull(_register.SubjectSetByEdition(1));
            Assert.IsNull(_register.SubjectSetByEdition(2));
        }

        [Test]
        public void SubjectSetByEditionReturnsAfterSubscriptionSyncUpdates()
        {
            _register.Register(EURCHR_2MM, false);
            _register.Register(EURGBP_1MM, false);
        }
    }
}