using System.Collections.Generic;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public class SubscriptionSyncTest
    {
        private const int Edition = 123;

        public static readonly Subject.Subject[] SortedSubjects =
        {
            new Subject.Subject(
                "Account=CITILONDYACS,AllocAccount=CITILONDYACS,AllocQty=1000000,AssetClass=Fx,Currency=AUD,Customer=1A78668,Dealer=103050,Exchange=OTC,Level=1,NumAllocs=1,Quantity=1000000.00,QuoteStyle=RFS,Source=BNPFX,SubClass=Spot,Symbol=AUDNZD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMCITI,AllocAccount=DYMCITI,AllocQty=1000000,AssetClass=Fx,Currency=AUD,Customer=1A78668,Dealer=1A06455,Exchange=OTC,Level=1,NumAllocs=1,Quantity=1000000.00,QuoteStyle=RFS,Source=GSFX,SubClass=Spot,Symbol=AUDNZD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_MACRO_GUCITI,AllocAccount=DYMON_MACRO_GUCITI,AllocQty=1000000,AssetClass=Fx,Currency=AUD,Customer=1A78668,Dealer=100921,Exchange=OTC,Level=1,NumAllocs=1,Quantity=1000000.00,QuoteStyle=RFS,Source=UBSFX,SubClass=Spot,Symbol=AUDNZD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_PBCITI,AllocAccount=DYMON_PBCITI,AllocQty=1000000,AssetClass=Fx,Currency=AUD,Customer=1A78668,Dealer=102325,Exchange=OTC,Level=1,NumAllocs=1,Quantity=1000000.00,QuoteStyle=RFS,Source=NATIXISFX,SubClass=Spot,Symbol=AUDNZD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=EU0326531,AllocAccount=EU0326531,AllocQty=1000000,AssetClass=Fx,Currency=AUD,Customer=1A78668,Dealer=101727,Exchange=OTC,Level=1,NumAllocs=1,Quantity=1000000.00,QuoteStyle=RFS,Source=DBFX,SubClass=Spot,Symbol=AUDNZD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=GUABAXCITI,AllocAccount=GUABAXCITI,AllocQty=1000000,AssetClass=Fx,Currency=AUD,Customer=1A78668,Dealer=101822,Exchange=OTC,Level=1,NumAllocs=1,Quantity=1000000.00,QuoteStyle=RFS,Source=MSFX,SubClass=Spot,Symbol=AUDNZD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=1000000,AssetClass=Fx,Currency=AUD,Customer=1A78668,Dealer=101167,Exchange=OTC,Level=1,NumAllocs=1,Quantity=1300000.00,QuoteStyle=RFS,Source=BOFAFX,SubClass=Spot,Symbol=AUDNZD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=1000000,AssetClass=Fx,Currency=AUD,Customer=1A78668,Dealer=1B56446,Exchange=OTC,Level=1,NumAllocs=1,Quantity=1300000.00,QuoteStyle=RFS,Source=JEFFX,SubClass=Spot,Symbol=AUDNZD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=1000000,AssetClass=Fx,Currency=AUD,Customer=1A78668,Dealer=1A25271,Exchange=OTC,Level=1,NumAllocs=1,Quantity=1500000.00,QuoteStyle=RFS,Source=SSFX,SubClass=Spot,Symbol=AUDNZD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_CITI,AllocAccount=DYMON_CITI,AllocQty=1000000,AssetClass=Fx,Currency=AUD,Customer=1A78668,Dealer=1B55735,Exchange=OTC,Level=1,NumAllocs=1,Quantity=1500000.00,QuoteStyle=RFS,Source=RBCFX,SubClass=Spot,Symbol=AUDNZD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=105104340,AllocAccount=105104340,AllocQty=1000000,AssetClass=Fx,Currency=AUD,Customer=1A78668,Dealer=102536,Exchange=OTC,Level=1,NumAllocs=1,Quantity=1600000.00,QuoteStyle=RFS,Source=CITIFX,SubClass=Spot,Symbol=AUDNZD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=105104340,AllocAccount=105104340,AllocQty=2000000,AssetClass=Fx,Currency=AUD,Customer=1A78668,Dealer=102536,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=CITIFX,SubClass=Spot,Symbol=AUDUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=CITILONDYACS,AllocAccount=CITILONDYACS,AllocQty=2000000,AssetClass=Fx,Currency=AUD,Customer=1A78668,Dealer=103050,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=BNPFX,SubClass=Spot,Symbol=AUDUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMCITI,AllocAccount=DYMCITI,AllocQty=2000000,AssetClass=Fx,Currency=AUD,Customer=1A78668,Dealer=1A06455,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=GSFX,SubClass=Spot,Symbol=AUDUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=2000000,AssetClass=Fx,Currency=AUD,Customer=1A78668,Dealer=1A25271,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=SSFX,SubClass=Spot,Symbol=AUDUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=2000000,AssetClass=Fx,Currency=AUD,Customer=1A78668,Dealer=1B56446,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=JEFFX,SubClass=Spot,Symbol=AUDUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_CITI,AllocAccount=DYMON_CITI,AllocQty=2000000,AssetClass=Fx,Currency=AUD,Customer=1A78668,Dealer=1B55735,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=RBCFX,SubClass=Spot,Symbol=AUDUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_PBCITI,AllocAccount=DYMON_PBCITI,AllocQty=2000000,AssetClass=Fx,Currency=AUD,Customer=1A78668,Dealer=102325,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=NATIXISFX,SubClass=Spot,Symbol=AUDUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=EU0326531,AllocAccount=EU0326531,AllocQty=2000000,AssetClass=Fx,Currency=AUD,Customer=1A78668,Dealer=101727,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=DBFX,SubClass=Spot,Symbol=AUDUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=GUABAXCITI,AllocAccount=GUABAXCITI,AllocQty=2000000,AssetClass=Fx,Currency=AUD,Customer=1A78668,Dealer=101822,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=MSFX,SubClass=Spot,Symbol=AUDUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_MACRO_GUCITI,AllocAccount=DYMON_MACRO_GUCITI,AllocQty=2000000,AssetClass=Fx,Currency=AUD,Customer=1A78668,Dealer=100921,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.03,QuoteStyle=RFS,Source=UBSFX,SubClass=Spot,Symbol=AUDUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=2000000,AssetClass=Fx,Currency=AUD,Customer=1A78668,Dealer=101167,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2500000.00,QuoteStyle=RFS,Source=BOFAFX,SubClass=Spot,Symbol=AUDUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=2000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=101167,Exchange=OTC,Level=1,NumAllocs=1,Quantity=1900000.00,QuoteStyle=RFS,Source=BOFAFX,SubClass=Spot,Symbol=EURCHF,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=CITILONDYACS,AllocAccount=CITILONDYACS,AllocQty=2000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=103050,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=BNPFX,SubClass=Spot,Symbol=EURCHF,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=2000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=1A25271,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=SSFX,SubClass=Spot,Symbol=EURCHF,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=2000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=1B56446,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=JEFFX,SubClass=Spot,Symbol=EURCHF,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_CITI,AllocAccount=DYMON_CITI,AllocQty=2000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=1B55735,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=RBCFX,SubClass=Spot,Symbol=EURCHF,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_MACRO_GUCITI,AllocAccount=DYMON_MACRO_GUCITI,AllocQty=2000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=100921,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=UBSFX,SubClass=Spot,Symbol=EURCHF,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_PBCITI,AllocAccount=DYMON_PBCITI,AllocQty=2000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=102325,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=NATIXISFX,SubClass=Spot,Symbol=EURCHF,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=EU0326531,AllocAccount=EU0326531,AllocQty=2000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=101727,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=DBFX,SubClass=Spot,Symbol=EURCHF,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=GUABAXCITI,AllocAccount=GUABAXCITI,AllocQty=2000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=101822,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=MSFX,SubClass=Spot,Symbol=EURCHF,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMCITI,AllocAccount=DYMCITI,AllocQty=2000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=1A06455,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.01,QuoteStyle=RFS,Source=GSFX,SubClass=Spot,Symbol=EURCHF,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=105104340,AllocAccount=105104340,AllocQty=2000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=102536,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2200000.00,QuoteStyle=RFS,Source=CITIFX,SubClass=Spot,Symbol=EURCHF,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=105104340,AllocAccount=105104340,AllocQty=1000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=102536,Exchange=OTC,Level=1,NumAllocs=1,Quantity=1000000.00,QuoteStyle=RFS,Source=CITIFX,SubClass=Spot,Symbol=EURGBP,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=CITILONDYACS,AllocAccount=CITILONDYACS,AllocQty=1000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=103050,Exchange=OTC,Level=1,NumAllocs=1,Quantity=1000000.00,QuoteStyle=RFS,Source=BNPFX,SubClass=Spot,Symbol=EURGBP,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMCITI,AllocAccount=DYMCITI,AllocQty=1000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=1A06455,Exchange=OTC,Level=1,NumAllocs=1,Quantity=1000000.00,QuoteStyle=RFS,Source=GSFX,SubClass=Spot,Symbol=EURGBP,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=1000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=101167,Exchange=OTC,Level=1,NumAllocs=1,Quantity=1000000.00,QuoteStyle=RFS,Source=BOFAFX,SubClass=Spot,Symbol=EURGBP,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=1000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=1A25271,Exchange=OTC,Level=1,NumAllocs=1,Quantity=1000000.00,QuoteStyle=RFS,Source=SSFX,SubClass=Spot,Symbol=EURGBP,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=1000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=1B56446,Exchange=OTC,Level=1,NumAllocs=1,Quantity=1000000.00,QuoteStyle=RFS,Source=JEFFX,SubClass=Spot,Symbol=EURGBP,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_CITI,AllocAccount=DYMON_CITI,AllocQty=1000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=1B55735,Exchange=OTC,Level=1,NumAllocs=1,Quantity=1000000.00,QuoteStyle=RFS,Source=RBCFX,SubClass=Spot,Symbol=EURGBP,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=EU0326531,AllocAccount=EU0326531,AllocQty=1000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=101727,Exchange=OTC,Level=1,NumAllocs=1,Quantity=1400000.00,QuoteStyle=RFS,Source=DBFX,SubClass=Spot,Symbol=EURGBP,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=GUABAXCITI,AllocAccount=GUABAXCITI,AllocQty=1000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=101822,Exchange=OTC,Level=1,NumAllocs=1,Quantity=1500000.00,QuoteStyle=RFS,Source=MSFX,SubClass=Spot,Symbol=EURGBP,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_MACRO_GUCITI,AllocAccount=DYMON_MACRO_GUCITI,AllocQty=1000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=100921,Exchange=OTC,Level=1,NumAllocs=1,Quantity=1600000.00,QuoteStyle=RFS,Source=UBSFX,SubClass=Spot,Symbol=EURGBP,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_PBCITI,AllocAccount=DYMON_PBCITI,AllocQty=1000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=102325,Exchange=OTC,Level=1,NumAllocs=1,Quantity=1800000.00,QuoteStyle=RFS,Source=NATIXISFX,SubClass=Spot,Symbol=EURGBP,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=105104340,AllocAccount=105104340,AllocQty=2000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=102536,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=CITIFX,SubClass=Spot,Symbol=EURUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=CITILONDYACS,AllocAccount=CITILONDYACS,AllocQty=2000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=103050,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=BNPFX,SubClass=Spot,Symbol=EURUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMCITI,AllocAccount=DYMCITI,AllocQty=2000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=1A06455,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=GSFX,SubClass=Spot,Symbol=EURUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=2000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=101167,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=BOFAFX,SubClass=Spot,Symbol=EURUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=2000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=1A25271,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=SSFX,SubClass=Spot,Symbol=EURUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=2000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=1B56446,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=JEFFX,SubClass=Spot,Symbol=EURUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_CITI,AllocAccount=DYMON_CITI,AllocQty=2000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=1B55735,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=RBCFX,SubClass=Spot,Symbol=EURUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_MACRO_GUCITI,AllocAccount=DYMON_MACRO_GUCITI,AllocQty=2000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=100921,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=UBSFX,SubClass=Spot,Symbol=EURUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_PBCITI,AllocAccount=DYMON_PBCITI,AllocQty=2000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=102325,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=NATIXISFX,SubClass=Spot,Symbol=EURUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=EU0326531,AllocAccount=EU0326531,AllocQty=2000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=101727,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=DBFX,SubClass=Spot,Symbol=EURUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=GUABAXCITI,AllocAccount=GUABAXCITI,AllocQty=2000000,AssetClass=Fx,Currency=EUR,Customer=1A78668,Dealer=101822,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=MSFX,SubClass=Spot,Symbol=EURUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=7220,AssetClass=Fx,Currency=GBP,Customer=1A78668,Dealer=1B56446,Exchange=OTC,Level=1,NumAllocs=1,Quantity=7219.00,QuoteStyle=RFS,Source=JEFFX,SubClass=Spot,Symbol=GBPUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=7220,AssetClass=Fx,Currency=GBP,Customer=1A78668,Dealer=101167,Exchange=OTC,Level=1,NumAllocs=1,Quantity=7220.00,QuoteStyle=RFS,Source=BOFAFX,SubClass=Spot,Symbol=GBPUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=7220,AssetClass=Fx,Currency=GBP,Customer=1A78668,Dealer=1A25271,Exchange=OTC,Level=1,NumAllocs=1,Quantity=7220.00,QuoteStyle=RFS,Source=SSFX,SubClass=Spot,Symbol=GBPUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_CITI,AllocAccount=DYMON_CITI,AllocQty=7220,AssetClass=Fx,Currency=GBP,Customer=1A78668,Dealer=1B55735,Exchange=OTC,Level=1,NumAllocs=1,Quantity=7220.00,QuoteStyle=RFS,Source=RBCFX,SubClass=Spot,Symbol=GBPUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=GUABAXCITI,AllocAccount=GUABAXCITI,AllocQty=7220,AssetClass=Fx,Currency=GBP,Customer=1A78668,Dealer=101822,Exchange=OTC,Level=1,NumAllocs=1,Quantity=7220.00,QuoteStyle=RFS,Source=MSFX,SubClass=Spot,Symbol=GBPUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_MACRO_GUCITI,AllocAccount=DYMON_MACRO_GUCITI,AllocQty=7220,AssetClass=Fx,Currency=GBP,Customer=1A78668,Dealer=100921,Exchange=OTC,Level=1,NumAllocs=1,Quantity=7220.50,QuoteStyle=RFS,Source=UBSFX,SubClass=Spot,Symbol=GBPUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMCITI,AllocAccount=DYMCITI,AllocQty=7220,AssetClass=Fx,Currency=GBP,Customer=1A78668,Dealer=1A06455,Exchange=OTC,Level=1,NumAllocs=1,Quantity=7221.00,QuoteStyle=RFS,Source=GSFX,SubClass=Spot,Symbol=GBPUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_PBCITI,AllocAccount=DYMON_PBCITI,AllocQty=7220,AssetClass=Fx,Currency=GBP,Customer=1A78668,Dealer=102325,Exchange=OTC,Level=1,NumAllocs=1,Quantity=7221.55,QuoteStyle=RFS,Source=NATIXISFX,SubClass=Spot,Symbol=GBPUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=105104340,AllocAccount=105104340,AllocQty=7220,AssetClass=Fx,Currency=GBP,Customer=1A78668,Dealer=102536,Exchange=OTC,Level=1,NumAllocs=1,Quantity=7222.50,QuoteStyle=RFS,Source=CITIFX,SubClass=Spot,Symbol=GBPUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=EU0326531,AllocAccount=EU0326531,AllocQty=7220,AssetClass=Fx,Currency=GBP,Customer=1A78668,Dealer=101727,Exchange=OTC,Level=1,NumAllocs=1,Quantity=7223.40,QuoteStyle=RFS,Source=DBFX,SubClass=Spot,Symbol=GBPUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=CITILONDYACS,AllocAccount=CITILONDYACS,AllocQty=7220,AssetClass=Fx,Currency=GBP,Customer=1A78668,Dealer=103050,Exchange=OTC,Level=1,NumAllocs=1,Quantity=7223.50,QuoteStyle=RFS,Source=BNPFX,SubClass=Spot,Symbol=GBPUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=105104340,AllocAccount=105104340,AllocQty=3000000,AssetClass=Fx,Currency=NZD,Customer=1A78668,Dealer=102536,Exchange=OTC,Level=1,NumAllocs=1,Quantity=3000000.00,QuoteStyle=RFS,Source=CITIFX,SubClass=Spot,Symbol=NZDUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=CITILONDYACS,AllocAccount=CITILONDYACS,AllocQty=3000000,AssetClass=Fx,Currency=NZD,Customer=1A78668,Dealer=103050,Exchange=OTC,Level=1,NumAllocs=1,Quantity=3000000.00,QuoteStyle=RFS,Source=BNPFX,SubClass=Spot,Symbol=NZDUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMCITI,AllocAccount=DYMCITI,AllocQty=3000000,AssetClass=Fx,Currency=NZD,Customer=1A78668,Dealer=1A06455,Exchange=OTC,Level=1,NumAllocs=1,Quantity=3000000.00,QuoteStyle=RFS,Source=GSFX,SubClass=Spot,Symbol=NZDUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=3000000,AssetClass=Fx,Currency=NZD,Customer=1A78668,Dealer=101167,Exchange=OTC,Level=1,NumAllocs=1,Quantity=3000000.00,QuoteStyle=RFS,Source=BOFAFX,SubClass=Spot,Symbol=NZDUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=3000000,AssetClass=Fx,Currency=NZD,Customer=1A78668,Dealer=1A25271,Exchange=OTC,Level=1,NumAllocs=1,Quantity=3000000.00,QuoteStyle=RFS,Source=SSFX,SubClass=Spot,Symbol=NZDUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=3000000,AssetClass=Fx,Currency=NZD,Customer=1A78668,Dealer=1B56446,Exchange=OTC,Level=1,NumAllocs=1,Quantity=3000000.00,QuoteStyle=RFS,Source=JEFFX,SubClass=Spot,Symbol=NZDUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_CITI,AllocAccount=DYMON_CITI,AllocQty=3000000,AssetClass=Fx,Currency=NZD,Customer=1A78668,Dealer=1B55735,Exchange=OTC,Level=1,NumAllocs=1,Quantity=3000000.00,QuoteStyle=RFS,Source=RBCFX,SubClass=Spot,Symbol=NZDUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_MACRO_GUCITI,AllocAccount=DYMON_MACRO_GUCITI,AllocQty=3000000,AssetClass=Fx,Currency=NZD,Customer=1A78668,Dealer=100921,Exchange=OTC,Level=1,NumAllocs=1,Quantity=3000000.00,QuoteStyle=RFS,Source=UBSFX,SubClass=Spot,Symbol=NZDUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_PBCITI,AllocAccount=DYMON_PBCITI,AllocQty=3000000,AssetClass=Fx,Currency=NZD,Customer=1A78668,Dealer=102325,Exchange=OTC,Level=1,NumAllocs=1,Quantity=3000000.00,QuoteStyle=RFS,Source=NATIXISFX,SubClass=Spot,Symbol=NZDUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=EU0326531,AllocAccount=EU0326531,AllocQty=3000000,AssetClass=Fx,Currency=NZD,Customer=1A78668,Dealer=101727,Exchange=OTC,Level=1,NumAllocs=1,Quantity=3000000.00,QuoteStyle=RFS,Source=DBFX,SubClass=Spot,Symbol=NZDUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=GUABAXCITI,AllocAccount=GUABAXCITI,AllocQty=3000000,AssetClass=Fx,Currency=NZD,Customer=1A78668,Dealer=101822,Exchange=OTC,Level=1,NumAllocs=1,Quantity=3000000.00,QuoteStyle=RFS,Source=MSFX,SubClass=Spot,Symbol=NZDUSD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=105104340,AllocAccount=105104340,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=102536,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=CITIFX,SubClass=Spot,Symbol=USDCAD,Tenor=Spot,User=cheechungli,ValueDate=20141103"),
            new Subject.Subject(
                "Account=CITILONDYACS,AllocAccount=CITILONDYACS,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=103050,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=BNPFX,SubClass=Spot,Symbol=USDCAD,Tenor=Spot,User=cheechungli,ValueDate=20141103"),
            new Subject.Subject(
                "Account=DYMON_MACRO_GUCITI,AllocAccount=DYMON_MACRO_GUCITI,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=100921,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=UBSFX,SubClass=Spot,Symbol=USDCAD,Tenor=Spot,User=cheechungli,ValueDate=20141103"),
            new Subject.Subject(
                "Account=DYMON_PBCITI,AllocAccount=DYMON_PBCITI,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=102325,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=NATIXISFX,SubClass=Spot,Symbol=USDCAD,Tenor=Spot,User=cheechungli,ValueDate=20141103"),
            new Subject.Subject(
                "Account=GUABAXCITI,AllocAccount=GUABAXCITI,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101822,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=MSFX,SubClass=Spot,Symbol=USDCAD,Tenor=Spot,User=cheechungli,ValueDate=20141103"),
            new Subject.Subject(
                "Account=EU0326531,AllocAccount=EU0326531,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101727,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000001.00,QuoteStyle=RFS,Source=DBFX,SubClass=Spot,Symbol=USDCAD,Tenor=Spot,User=cheechungli,ValueDate=20141103"),
            new Subject.Subject(
                "Account=DYMON_CITI,AllocAccount=DYMON_CITI,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=1B55735,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000001.50,QuoteStyle=RFS,Source=RBCFX,SubClass=Spot,Symbol=USDCAD,Tenor=Spot,User=cheechungli,ValueDate=20141103"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=1A25271,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000002.00,QuoteStyle=RFS,Source=SSFX,SubClass=Spot,Symbol=USDCAD,Tenor=Spot,User=cheechungli,ValueDate=20141103"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101167,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2100000.00,QuoteStyle=RFS,Source=BOFAFX,SubClass=Spot,Symbol=USDCAD,Tenor=Spot,User=cheechungli,ValueDate=20141103"),
            new Subject.Subject(
                "Account=DYMCITI,AllocAccount=DYMCITI,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=1A06455,Exchange=OTC,Level=1,NumAllocs=1,Quantity=3000000.00,QuoteStyle=RFS,Source=GSFX,SubClass=Spot,Symbol=USDCAD,Tenor=Spot,User=cheechungli,ValueDate=20141103"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=1B56446,Exchange=OTC,Level=1,NumAllocs=1,Quantity=3000000.00,QuoteStyle=RFS,Source=JEFFX,SubClass=Spot,Symbol=USDCAD,Tenor=Spot,User=cheechungli,ValueDate=20141103"),
            new Subject.Subject(
                "Account=105104340,AllocAccount=105104340,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=102536,Exchange=OTC,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=CITIFX,SubClass=Spot,Symbol=USDCNH,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMCITI,AllocAccount=DYMCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=1A06455,Exchange=OTC,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=GSFX,SubClass=Spot,Symbol=USDCNH,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101167,Exchange=OTC,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=BOFAFX,SubClass=Spot,Symbol=USDCNH,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=1A25271,Exchange=OTC,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=SSFX,SubClass=Spot,Symbol=USDCNH,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_CITI,AllocAccount=DYMON_CITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=1B55735,Exchange=OTC,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=RBCFX,SubClass=Spot,Symbol=USDCNH,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_MACRO_GUCITI,AllocAccount=DYMON_MACRO_GUCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=100921,Exchange=OTC,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=UBSFX,SubClass=Spot,Symbol=USDCNH,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_PBCITI,AllocAccount=DYMON_PBCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=102325,Exchange=OTC,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=NATIXISFX,SubClass=Spot,Symbol=USDCNH,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=EU0326531,AllocAccount=EU0326531,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101727,Exchange=OTC,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=DBFX,SubClass=Spot,Symbol=USDCNH,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=GUABAXCITI,AllocAccount=GUABAXCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101822,Exchange=OTC,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=MSFX,SubClass=Spot,Symbol=USDCNH,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=CITILONDYACS,AllocAccount=CITILONDYACS,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=103050,Exchange=OTC,Level=1,NumAllocs=1,Quantity=6000000.00,QuoteStyle=RFS,Source=BNPFX,SubClass=Spot,Symbol=USDCNH,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_MACRO_GUCITI,AllocAccount=DYMON_MACRO_GUCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=100921,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141230,Level=1,NumAllocs=1,Quantity=3000000.00,QuoteStyle=RFS,Source=UBSFX,SubClass=NDF,Symbol=USDCNY,Tenor=2M,User=cheechungli,ValueDate=20150105"),
            new Subject.Subject(
                "Account=EU0326531,AllocAccount=EU0326531,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101727,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20150429,Level=1,NumAllocs=1,Quantity=4000000.00,QuoteStyle=RFS,Source=DBFX,SubClass=NDF,Symbol=USDCNY,Tenor=6M,User=cheechungli,ValueDate=20150508"),
            new Subject.Subject(
                "Account=GUABAXCITI,AllocAccount=GUABAXCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101822,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=4000000.00,QuoteStyle=RFS,Source=MSFX,SubClass=NDF,Symbol=USDCNY,Tenor=1M,User=cheechungli,ValueDate=20141204"),
            new Subject.Subject(
                "Account=CITILONDYACS,AllocAccount=CITILONDYACS,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=103050,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=BNPFX,SubClass=NDF,Symbol=USDCNY,Tenor=1M,User=cheechungli,ValueDate=20141204"),
            new Subject.Subject(
                "Account=CITILONDYACS,AllocAccount=CITILONDYACS,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=103050,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141230,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=BNPFX,SubClass=NDF,Symbol=USDCNY,Tenor=2M,User=cheechungli,ValueDate=20150105"),
            new Subject.Subject(
                "Account=CITILONDYACS,AllocAccount=CITILONDYACS,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=103050,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20150202,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=BNPFX,SubClass=NDF,Symbol=USDCNY,Tenor=3M,User=cheechungli,ValueDate=20150204"),
            new Subject.Subject(
                "Account=CITILONDYACS,AllocAccount=CITILONDYACS,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=103050,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20150429,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=BNPFX,SubClass=NDF,Symbol=USDCNY,Tenor=6M,User=cheechungli,ValueDate=20150508"),
            new Subject.Subject(
                "Account=CITILONDYACS,AllocAccount=CITILONDYACS,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=103050,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20150731,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=BNPFX,SubClass=NDF,Symbol=USDCNY,Tenor=9M,User=cheechungli,ValueDate=20150804"),
            new Subject.Subject(
                "Account=CITILONDYACS,AllocAccount=CITILONDYACS,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=103050,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20151102,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=BNPFX,SubClass=NDF,Symbol=USDCNY,Tenor=1Y,User=cheechungli,ValueDate=20151104"),
            new Subject.Subject(
                "Account=DYMCITI,AllocAccount=DYMCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=1A06455,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=GSFX,SubClass=NDF,Symbol=USDCNY,Tenor=1M,User=cheechungli,ValueDate=20141204"),
            new Subject.Subject(
                "Account=DYMCITI,AllocAccount=DYMCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=1A06455,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141230,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=GSFX,SubClass=NDF,Symbol=USDCNY,Tenor=2M,User=cheechungli,ValueDate=20150105"),
            new Subject.Subject(
                "Account=DYMCITI,AllocAccount=DYMCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=1A06455,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20150202,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=GSFX,SubClass=NDF,Symbol=USDCNY,Tenor=3M,User=cheechungli,ValueDate=20150204"),
            new Subject.Subject(
                "Account=DYMCITI,AllocAccount=DYMCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=1A06455,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20150731,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=GSFX,SubClass=NDF,Symbol=USDCNY,Tenor=9M,User=cheechungli,ValueDate=20150804"),
            new Subject.Subject(
                "Account=DYMCITI,AllocAccount=DYMCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=1A06455,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20151102,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=GSFX,SubClass=NDF,Symbol=USDCNY,Tenor=1Y,User=cheechungli,ValueDate=20151104"),
            new Subject.Subject(
                "Account=DYMON_MACRO_GUCITI,AllocAccount=DYMON_MACRO_GUCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=100921,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=UBSFX,SubClass=NDF,Symbol=USDCNY,Tenor=1M,User=cheechungli,ValueDate=20141204"),
            new Subject.Subject(
                "Account=EU0326531,AllocAccount=EU0326531,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101727,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=DBFX,SubClass=NDF,Symbol=USDCNY,Tenor=1M,User=cheechungli,ValueDate=20141204"),
            new Subject.Subject(
                "Account=EU0326531,AllocAccount=EU0326531,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101727,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141230,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=DBFX,SubClass=NDF,Symbol=USDCNY,Tenor=2M,User=cheechungli,ValueDate=20150105"),
            new Subject.Subject(
                "Account=EU0326531,AllocAccount=EU0326531,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101727,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20150731,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=DBFX,SubClass=NDF,Symbol=USDCNY,Tenor=9M,User=cheechungli,ValueDate=20150804"),
            new Subject.Subject(
                "Account=EU0326531,AllocAccount=EU0326531,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101727,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20151102,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=DBFX,SubClass=NDF,Symbol=USDCNY,Tenor=1Y,User=cheechungli,ValueDate=20151104"),
            new Subject.Subject(
                "Account=GUABAXCITI,AllocAccount=GUABAXCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101822,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141230,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=MSFX,SubClass=NDF,Symbol=USDCNY,Tenor=2M,User=cheechungli,ValueDate=20150105"),
            new Subject.Subject(
                "Account=GUABAXCITI,AllocAccount=GUABAXCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101822,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20150202,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=MSFX,SubClass=NDF,Symbol=USDCNY,Tenor=3M,User=cheechungli,ValueDate=20150204"),
            new Subject.Subject(
                "Account=GUABAXCITI,AllocAccount=GUABAXCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101822,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20150731,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=MSFX,SubClass=NDF,Symbol=USDCNY,Tenor=9M,User=cheechungli,ValueDate=20150804"),
            new Subject.Subject(
                "Account=GUABAXCITI,AllocAccount=GUABAXCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101822,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20151102,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=MSFX,SubClass=NDF,Symbol=USDCNY,Tenor=1Y,User=cheechungli,ValueDate=20151104"),
            new Subject.Subject(
                "Account=EU0326531,AllocAccount=EU0326531,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101727,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20150202,Level=1,NumAllocs=1,Quantity=5500000.00,QuoteStyle=RFS,Source=DBFX,SubClass=NDF,Symbol=USDCNY,Tenor=3M,User=cheechungli,ValueDate=20150204"),
            new Subject.Subject(
                "Account=DYMCITI,AllocAccount=DYMCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=1A06455,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20150429,Level=1,NumAllocs=1,Quantity=6000000.00,QuoteStyle=RFS,Source=GSFX,SubClass=NDF,Symbol=USDCNY,Tenor=6M,User=cheechungli,ValueDate=20150508"),
            new Subject.Subject(
                "Account=GUABAXCITI,AllocAccount=GUABAXCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101822,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20150429,Level=1,NumAllocs=1,Quantity=6000000.00,QuoteStyle=RFS,Source=MSFX,SubClass=NDF,Symbol=USDCNY,Tenor=6M,User=cheechungli,ValueDate=20150508"),
            new Subject.Subject(
                "Account=CITILONDYACS,AllocAccount=CITILONDYACS,AllocQty=1000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=103050,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=100000.00,QuoteStyle=RFS,Source=BNPFX,SubClass=NDF,Symbol=USDIDR,Tenor=1M,User=cheechungli,ValueDate=20141204"),
            new Subject.Subject(
                "Account=DYMCITI,AllocAccount=DYMCITI,AllocQty=1000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=1A06455,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=1000000.00,QuoteStyle=RFS,Source=GSFX,SubClass=NDF,Symbol=USDIDR,Tenor=1M,User=cheechungli,ValueDate=20141204"),
            new Subject.Subject(
                "Account=EU0326531,AllocAccount=EU0326531,AllocQty=1000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101727,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=1000000.00,QuoteStyle=RFS,Source=DBFX,SubClass=NDF,Symbol=USDIDR,Tenor=1M,User=cheechungli,ValueDate=20141204"),
            new Subject.Subject(
                "Account=GUABAXCITI,AllocAccount=GUABAXCITI,AllocQty=1000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101822,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=1100000.00,QuoteStyle=RFS,Source=MSFX,SubClass=NDF,Symbol=USDIDR,Tenor=1M,User=cheechungli,ValueDate=20141204"),
            new Subject.Subject(
                "Account=DYMON_MACRO_GUCITI,AllocAccount=DYMON_MACRO_GUCITI,AllocQty=1000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=100921,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=1500000.00,QuoteStyle=RFS,Source=UBSFX,SubClass=NDF,Symbol=USDIDR,Tenor=1M,User=cheechungli,ValueDate=20141204"),
            new Subject.Subject(
                "Account=CITILONDYACS,AllocAccount=CITILONDYACS,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=103050,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141203,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=BNPFX,SubClass=NDF,Symbol=USDINR,Tenor=1M,User=cheechungli,ValueDate=20141205"),
            new Subject.Subject(
                "Account=DYMCITI,AllocAccount=DYMCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=1A06455,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141203,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=GSFX,SubClass=NDF,Symbol=USDINR,Tenor=1M,User=cheechungli,ValueDate=20141205"),
            new Subject.Subject(
                "Account=DYMON_MACRO_GUCITI,AllocAccount=DYMON_MACRO_GUCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=100921,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141203,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=UBSFX,SubClass=NDF,Symbol=USDINR,Tenor=1M,User=cheechungli,ValueDate=20141205"),
            new Subject.Subject(
                "Account=EU0326531,AllocAccount=EU0326531,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101727,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141203,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=DBFX,SubClass=NDF,Symbol=USDINR,Tenor=1M,User=cheechungli,ValueDate=20141205"),
            new Subject.Subject(
                "Account=GUABAXCITI,AllocAccount=GUABAXCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101822,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141203,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=MSFX,SubClass=NDF,Symbol=USDINR,Tenor=1M,User=cheechungli,ValueDate=20141205"),
            new Subject.Subject(
                "Account=DYMON_MACRO_GUCITI,AllocAccount=DYMON_MACRO_GUCITI,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=100921,Exchange=OTC,Level=1,NumAllocs=1,Quantity=1800000.00,QuoteStyle=RFS,Source=UBSFX,SubClass=Spot,Symbol=USDJPY,Tenor=Spot,User=cheechungli,ValueDate=20141105"),
            new Subject.Subject(
                "Account=105104340,AllocAccount=105104340,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=102536,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=CITIFX,SubClass=Spot,Symbol=USDJPY,Tenor=Spot,User=cheechungli,ValueDate=20141105"),
            new Subject.Subject(
                "Account=DYMCITI,AllocAccount=DYMCITI,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=1A06455,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=GSFX,SubClass=Spot,Symbol=USDJPY,Tenor=Spot,User=cheechungli,ValueDate=20141105"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101167,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=BOFAFX,SubClass=Spot,Symbol=USDJPY,Tenor=Spot,User=cheechungli,ValueDate=20141105"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=1A25271,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=SSFX,SubClass=Spot,Symbol=USDJPY,Tenor=Spot,User=cheechungli,ValueDate=20141105"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=1B56446,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=JEFFX,SubClass=Spot,Symbol=USDJPY,Tenor=Spot,User=cheechungli,ValueDate=20141105"),
            new Subject.Subject(
                "Account=DYMON_CITI,AllocAccount=DYMON_CITI,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=1B55735,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=RBCFX,SubClass=Spot,Symbol=USDJPY,Tenor=Spot,User=cheechungli,ValueDate=20141105"),
            new Subject.Subject(
                "Account=DYMON_PBCITI,AllocAccount=DYMON_PBCITI,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=102325,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=NATIXISFX,SubClass=Spot,Symbol=USDJPY,Tenor=Spot,User=cheechungli,ValueDate=20141105"),
            new Subject.Subject(
                "Account=EU0326531,AllocAccount=EU0326531,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101727,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=DBFX,SubClass=Spot,Symbol=USDJPY,Tenor=Spot,User=cheechungli,ValueDate=20141105"),
            new Subject.Subject(
                "Account=GUABAXCITI,AllocAccount=GUABAXCITI,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101822,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=MSFX,SubClass=Spot,Symbol=USDJPY,Tenor=Spot,User=cheechungli,ValueDate=20141105"),
            new Subject.Subject(
                "Account=CITILONDYACS,AllocAccount=CITILONDYACS,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=103050,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.02,QuoteStyle=RFS,Source=BNPFX,SubClass=Spot,Symbol=USDJPY,Tenor=Spot,User=cheechungli,ValueDate=20141105"),
            new Subject.Subject(
                "Account=DYMCITI,AllocAccount=DYMCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=1A06455,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=GSFX,SubClass=NDF,Symbol=USDKRW,Tenor=1M,User=cheechungli,ValueDate=20141204"),
            new Subject.Subject(
                "Account=DYMON_MACRO_GUCITI,AllocAccount=DYMON_MACRO_GUCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=100921,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=UBSFX,SubClass=NDF,Symbol=USDKRW,Tenor=1M,User=cheechungli,ValueDate=20141204"),
            new Subject.Subject(
                "Account=EU0326531,AllocAccount=EU0326531,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101727,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=DBFX,SubClass=NDF,Symbol=USDKRW,Tenor=1M,User=cheechungli,ValueDate=20141204"),
            new Subject.Subject(
                "Account=GUABAXCITI,AllocAccount=GUABAXCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101822,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=MSFX,SubClass=NDF,Symbol=USDKRW,Tenor=1M,User=cheechungli,ValueDate=20141204"),
            new Subject.Subject(
                "Account=CITILONDYACS,AllocAccount=CITILONDYACS,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=103050,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=5500000.00,QuoteStyle=RFS,Source=BNPFX,SubClass=NDF,Symbol=USDKRW,Tenor=1M,User=cheechungli,ValueDate=20141204"),
            new Subject.Subject(
                "Account=CITILONDYACS,AllocAccount=CITILONDYACS,AllocQty=3000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=103050,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=3000000.00,QuoteStyle=RFS,Source=BNPFX,SubClass=NDF,Symbol=USDMYR,Tenor=1M,User=cheechungli,ValueDate=20141204"),
            new Subject.Subject(
                "Account=DYMCITI,AllocAccount=DYMCITI,AllocQty=3000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=1A06455,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=3000000.00,QuoteStyle=RFS,Source=GSFX,SubClass=NDF,Symbol=USDMYR,Tenor=1M,User=cheechungli,ValueDate=20141204"),
            new Subject.Subject(
                "Account=DYMON_MACRO_GUCITI,AllocAccount=DYMON_MACRO_GUCITI,AllocQty=3000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=100921,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=3000000.00,QuoteStyle=RFS,Source=UBSFX,SubClass=NDF,Symbol=USDMYR,Tenor=1M,User=cheechungli,ValueDate=20141204"),
            new Subject.Subject(
                "Account=EU0326531,AllocAccount=EU0326531,AllocQty=3000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101727,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=3000000.00,QuoteStyle=RFS,Source=DBFX,SubClass=NDF,Symbol=USDMYR,Tenor=1M,User=cheechungli,ValueDate=20141204"),
            new Subject.Subject(
                "Account=GUABAXCITI,AllocAccount=GUABAXCITI,AllocQty=3000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101822,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=3000000.00,QuoteStyle=RFS,Source=MSFX,SubClass=NDF,Symbol=USDMYR,Tenor=1M,User=cheechungli,ValueDate=20141204"),
            new Subject.Subject(
                "Account=CITILONDYACS,AllocAccount=CITILONDYACS,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=103050,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=BNPFX,SubClass=NDF,Symbol=USDPHP,Tenor=1M,User=cheechungli,ValueDate=20141203"),
            new Subject.Subject(
                "Account=DYMCITI,AllocAccount=DYMCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=1A06455,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=GSFX,SubClass=NDF,Symbol=USDPHP,Tenor=1M,User=cheechungli,ValueDate=20141203"),
            new Subject.Subject(
                "Account=DYMON_MACRO_GUCITI,AllocAccount=DYMON_MACRO_GUCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=100921,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=UBSFX,SubClass=NDF,Symbol=USDPHP,Tenor=1M,User=cheechungli,ValueDate=20141203"),
            new Subject.Subject(
                "Account=EU0326531,AllocAccount=EU0326531,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101727,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=DBFX,SubClass=NDF,Symbol=USDPHP,Tenor=1M,User=cheechungli,ValueDate=20141203"),
            new Subject.Subject(
                "Account=GUABAXCITI,AllocAccount=GUABAXCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101822,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=5500000.00,QuoteStyle=RFS,Source=MSFX,SubClass=NDF,Symbol=USDPHP,Tenor=1M,User=cheechungli,ValueDate=20141203"),
            new Subject.Subject(
                "Account=105104340,AllocAccount=105104340,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=102536,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=CITIFX,SubClass=Spot,Symbol=USDSGD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=CITILONDYACS,AllocAccount=CITILONDYACS,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=103050,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=BNPFX,SubClass=Spot,Symbol=USDSGD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMCITI,AllocAccount=DYMCITI,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=1A06455,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=GSFX,SubClass=Spot,Symbol=USDSGD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101167,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=BOFAFX,SubClass=Spot,Symbol=USDSGD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=1A25271,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=SSFX,SubClass=Spot,Symbol=USDSGD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=1B56446,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=JEFFX,SubClass=Spot,Symbol=USDSGD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_CITI,AllocAccount=DYMON_CITI,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=1B55735,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=RBCFX,SubClass=Spot,Symbol=USDSGD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_MACRO_GUCITI,AllocAccount=DYMON_MACRO_GUCITI,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=100921,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=UBSFX,SubClass=Spot,Symbol=USDSGD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_PBCITI,AllocAccount=DYMON_PBCITI,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=102325,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=NATIXISFX,SubClass=Spot,Symbol=USDSGD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=EU0326531,AllocAccount=EU0326531,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101727,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=DBFX,SubClass=Spot,Symbol=USDSGD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=GUABAXCITI,AllocAccount=GUABAXCITI,AllocQty=2000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101822,Exchange=OTC,Level=1,NumAllocs=1,Quantity=2000000.00,QuoteStyle=RFS,Source=MSFX,SubClass=Spot,Symbol=USDSGD,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=CITILONDYACS,AllocAccount=CITILONDYACS,AllocQty=874000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=103050,Exchange=OTC,Level=1,NumAllocs=1,Quantity=874000.00,QuoteStyle=RFS,Source=BNPFX,SubClass=Spot,Symbol=USDTHB,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMCITI,AllocAccount=DYMCITI,AllocQty=874000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=1A06455,Exchange=OTC,Level=1,NumAllocs=1,Quantity=874000.00,QuoteStyle=RFS,Source=GSFX,SubClass=Spot,Symbol=USDTHB,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMONCITI,AllocAccount=DYMONCITI,AllocQty=874000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101167,Exchange=OTC,Level=1,NumAllocs=1,Quantity=874000.00,QuoteStyle=RFS,Source=BOFAFX,SubClass=Spot,Symbol=USDTHB,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_MACRO_GUCITI,AllocAccount=DYMON_MACRO_GUCITI,AllocQty=874000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=100921,Exchange=OTC,Level=1,NumAllocs=1,Quantity=874000.00,QuoteStyle=RFS,Source=UBSFX,SubClass=Spot,Symbol=USDTHB,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMON_PBCITI,AllocAccount=DYMON_PBCITI,AllocQty=874000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=102325,Exchange=OTC,Level=1,NumAllocs=1,Quantity=874000.00,QuoteStyle=RFS,Source=NATIXISFX,SubClass=Spot,Symbol=USDTHB,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=EU0326531,AllocAccount=EU0326531,AllocQty=874000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101727,Exchange=OTC,Level=1,NumAllocs=1,Quantity=874000.00,QuoteStyle=RFS,Source=DBFX,SubClass=Spot,Symbol=USDTHB,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=GUABAXCITI,AllocAccount=GUABAXCITI,AllocQty=874000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101822,Exchange=OTC,Level=1,NumAllocs=1,Quantity=874000.00,QuoteStyle=RFS,Source=MSFX,SubClass=Spot,Symbol=USDTHB,Tenor=Spot,User=cheechungli,ValueDate=20141104"),
            new Subject.Subject(
                "Account=DYMCITI,AllocAccount=DYMCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=1A06455,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=GSFX,SubClass=NDF,Symbol=USDTWD,Tenor=1M,User=cheechungli,ValueDate=20141204"),
            new Subject.Subject(
                "Account=DYMON_MACRO_GUCITI,AllocAccount=DYMON_MACRO_GUCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=100921,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=UBSFX,SubClass=NDF,Symbol=USDTWD,Tenor=1M,User=cheechungli,ValueDate=20141204"),
            new Subject.Subject(
                "Account=EU0326531,AllocAccount=EU0326531,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101727,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=DBFX,SubClass=NDF,Symbol=USDTWD,Tenor=1M,User=cheechungli,ValueDate=20141204"),
            new Subject.Subject(
                "Account=GUABAXCITI,AllocAccount=GUABAXCITI,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=101822,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=5000000.00,QuoteStyle=RFS,Source=MSFX,SubClass=NDF,Symbol=USDTWD,Tenor=1M,User=cheechungli,ValueDate=20141204"),
            new Subject.Subject(
                "Account=CITILONDYACS,AllocAccount=CITILONDYACS,AllocQty=5000000,AssetClass=Fx,Currency=USD,Customer=1A78668,Dealer=103050,Exchange=OTC,FixingCcy=USD,FixingCenter=New&#32;York,FixingDate=20141202,Level=1,NumAllocs=1,Quantity=6000000.00,QuoteStyle=RFS,Source=BNPFX,SubClass=NDF,Symbol=USDTWD,Tenor=1M,User=cheechungli,ValueDate=20141204")
        };

        private readonly List<Subject.Subject> _subjectList = new List<Subject.Subject>()
        {
            new Subject.Subject("Symbol=EURGBP,Quantity=2500000"),
            new Subject.Subject("Symbol=USDJPY,Quantity=5600000")
        };

        [Test]
        public void TestIsChangedIfTrueByDefault()
        {
            SubscriptionSync subscriptionSync = new SubscriptionSync(Edition, _subjectList);
            Assert.AreEqual(true, subscriptionSync.IsChangedEdition());
        }

        [Test]
        public void TestSizeIsPopulatedFromTheSubjectListSize()
        {
            SubscriptionSync subscriptionSync = new SubscriptionSync(Edition, _subjectList);
            Assert.AreEqual(_subjectList.Count, subscriptionSync.Size);
        }

        [Test]
        public void TestIsCompressedIfFalseByDefault()
        {
            SubscriptionSync subscriptionSync = new SubscriptionSync(Edition, _subjectList);
            Assert.IsFalse(subscriptionSync.IsCompressed());
        }

        [Test]
        public void TestHasNoControlsByDefault()
        {
            SubscriptionSync subscriptionSync = new SubscriptionSync(Edition, _subjectList);
            Assert.IsFalse(subscriptionSync.HasControls());
        }

        [Test]
        public void testSettingValidControls_hasControlsIsTrue()
        {
            SubscriptionSync subscriptionSync = new SubscriptionSync(Edition, _subjectList);
            subscriptionSync.AddControl(0, ControlOperation.Toggle);
            Assert.IsTrue(subscriptionSync.HasControls());
        }

        [Test]
        public void testBugFix_hasControlsIsRemainsTrueAfterSettingCompression()
        {
            SubscriptionSync subscriptionSync = new SubscriptionSync(Edition, _subjectList);
            subscriptionSync.AddControl(0, ControlOperation.Toggle);
            Assert.IsTrue(subscriptionSync.HasControls());
            subscriptionSync.SetCompressed(true);
            Assert.IsTrue(subscriptionSync.HasControls());
        }

        [Test]
        public void TestEncodingWithCompressionResultsInASmallerMessageSize()
        {
            SubscriptionSync subscriptionSync = new SubscriptionSync(Edition, _subjectList);
            subscriptionSync.SetCompressed(false);
            long uncompressedSize = subscriptionSync.Encode(PixieVersion.CurrentVersion).Position;
            subscriptionSync.SetCompressed(true);
            long compressedSize = subscriptionSync.Encode(PixieVersion.CurrentVersion).Position;
            Assert.IsTrue(compressedSize < uncompressedSize);
        }

        [Test]
        public void testEncodingWithCompressionResultsInTypicalSubjectsUsingLessThan_16_Bytes()
        {
            SubscriptionSync subscriptionSync =
                new SubscriptionSync(Edition, new List<Subject.Subject>(SortedSubjects));
            subscriptionSync.SetCompressed(true);
            long compressedSize = subscriptionSync.Encode(PixieVersion.CurrentVersion).Position;
            Assert.IsTrue(compressedSize / SortedSubjects.Length <= 16);
        }

        [Test]
        public void TestSummarize()
        {
            SubscriptionSync subscriptionSync = new SubscriptionSync(Edition, _subjectList);
            Assert.AreEqual("SubscriptionSync(edition=123, compressed=False, controls=0, changed=True, subjects=2)",
                subscriptionSync.Summarize());

            subscriptionSync.AddControl(0, ControlOperation.Toggle);
            subscriptionSync.AddControl(1, ControlOperation.Refresh);
            Assert.AreEqual("SubscriptionSync(edition=123, compressed=False, controls=2, changed=True, subjects=2)",
                subscriptionSync.Summarize());

            subscriptionSync.SetCompressed(true);
            Assert.AreEqual("SubscriptionSync(edition=123, compressed=True, controls=2, changed=True, subjects=2)",
                subscriptionSync.Summarize());
        }
    }
}