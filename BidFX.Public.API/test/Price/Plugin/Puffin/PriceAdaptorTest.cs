using System;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Plugin.Puffin
{
    [TestFixture]
    public class PriceAdaptorTest
    {
        [Test]
        public void LongValuesAreNotConvertedIfTheyDoNotHaveDateFieldNames()
        {
            var priceField = LongField(1480808590928L);
            Assert.AreEqual(1480808590928L, PriceAdaptor.AdaptPriceField(FieldName.BidSize, priceField).Value);
        }

        [Test]
        public void LongDateTimeFieldsAreConvertedToDataTime()
        {
            var dateTime = new DateTime(2016, 12, 3, 23, 43, 10, 928);
            var priceField = LongField(1480808590928L);
            Assert.AreEqual(dateTime, PriceAdaptor.AdaptPriceField(FieldName.SystemTime, priceField).Value);
            Assert.AreEqual(dateTime, PriceAdaptor.AdaptPriceField(FieldName.BidTime, priceField).Value);
            Assert.AreEqual(dateTime, PriceAdaptor.AdaptPriceField(FieldName.LastTime, priceField).Value);
            Assert.AreEqual(dateTime, PriceAdaptor.AdaptPriceField(FieldName.AskTime, priceField).Value);
            Assert.AreEqual(dateTime, PriceAdaptor.AdaptPriceField(FieldName.TimeOfUpdate, priceField).Value);
        }

        [Test]
        public void LongDateTimeFieldsAreConvertedToIsoStringFormat()
        {
            const string isoDate = "2016-12-03T23:43:10.928Z";
            var priceField = LongField(1480808590928L);
            Assert.AreEqual(isoDate, PriceAdaptor.AdaptPriceField(FieldName.SystemTime, priceField).Text);
            Assert.AreEqual(isoDate, PriceAdaptor.AdaptPriceField(FieldName.BidTime, priceField).Text);
        }

        private static IPriceField LongField(long javaDate)
        {
            return new PriceField(javaDate.ToString(), javaDate);
        }
        
        [Test]
        public void IntValuesAreNotConvertedIfTheyDoNotHaveDateFieldNames()
        {
            var priceField = IntField(20161203);
            Assert.AreEqual(20161203, PriceAdaptor.AdaptPriceField(FieldName.NumBids, priceField).Value);
        }

        [Test]
        public void IntDateTimeFieldsAreConvertedToDataTime()
        {
            var dateTime = new DateTime(2016, 12, 3);
            var priceField = IntField(20161203);
            Assert.AreEqual(dateTime, PriceAdaptor.AdaptPriceField(FieldName.ExMarkerDate, priceField).Value);
            Assert.AreEqual(dateTime, PriceAdaptor.AdaptPriceField(FieldName.DividendDate, priceField).Value);
        }

        [Test]
        public void IntDateTimeFieldsAreConvertedToIsoStringFormat()
        {
            const string isoDate = "2016-12-01";
            var priceField = IntField(20161201);
            Assert.AreEqual(isoDate, PriceAdaptor.AdaptPriceField(FieldName.ExMarkerDate, priceField).Text);
            Assert.AreEqual(isoDate, PriceAdaptor.AdaptPriceField(FieldName.DividendDate, priceField).Text);
        }

        private static IPriceField IntField(int yyyymmdd)
        {
            return new PriceField(yyyymmdd.ToString(), yyyymmdd);
        }

        [Test]
        public void PuffinDateTimeCanHaveSecondsMissing()
        {
            var dateTime = new DateTime(2016, 11, 11, 15, 19, 0);
            const string dateText = "2016/11/11 15:19";
            var priceField = new PriceField(dateText, dateText);
            Assert.AreEqual(dateTime, PriceAdaptor.AdaptPriceField(FieldName.SystemTime, priceField).Value);
            Assert.AreEqual(dateTime, PriceAdaptor.AdaptPriceField(FieldName.BidTime, priceField).Value);
        }

        [Test]
        public void PuffinDateTimeCanHaveTimeMissing()
        {
            var dateTime = new DateTime(2016, 11, 11);
            const string dateText = "2016/11/11";
            var priceField = new PriceField(dateText, dateText);
            Assert.AreEqual(dateTime, PriceAdaptor.AdaptPriceField(FieldName.SystemTime, priceField).Value);
            Assert.AreEqual(dateTime, PriceAdaptor.AdaptPriceField(FieldName.BidTime, priceField).Value);
        }

        [Test]
        public void PuffinDateTimeCanHaveDateMissing()
        {
            const string dateText = "20:30:56";
            var priceField = new PriceField(dateText, dateText);
            var dateTime = (DateTime)PriceAdaptor.AdaptPriceField(FieldName.SystemTime, priceField).Value;
            Assert.AreEqual(20, dateTime.Hour);
            Assert.AreEqual(30, dateTime.Minute);
            Assert.AreEqual(56, dateTime.Second);
            Assert.AreEqual(-1, dateTime.CompareTo(DateTime.UtcNow));
            Assert.AreEqual(1, dateTime.Add(TimeSpan.FromDays(1)).CompareTo(DateTime.UtcNow));
        }

        [Test]
        public void PuffinDateTimeCanHaveSingleDigitDayAndMonth()
        {
            var dateTime = new DateTime(2016, 1, 2, 15, 19, 0);
            var priceField = StringField("2016/1/2 15:19");
            Assert.AreEqual(dateTime, PriceAdaptor.AdaptPriceField(FieldName.SystemTime, priceField).Value);
            Assert.AreEqual(dateTime, PriceAdaptor.AdaptPriceField(FieldName.BidTime, priceField).Value);
        }


        private static IPriceField StringField(string s)
        {
            return new PriceField(s, s);
        }

        [Test]
        public void TicksAreConvertedToTickTrend()
        {
            Assert.AreEqual(Tick.Up, PriceAdaptor.AdaptPriceField(FieldName.BidTick, StringField("^")).Value);
            Assert.AreEqual(Tick.Down, PriceAdaptor.AdaptPriceField(FieldName.AskTick, StringField("v")).Value);
            Assert.AreEqual(Tick.Flat, PriceAdaptor.AdaptPriceField(FieldName.LastTick, StringField("=")).Value);
            Assert.AreEqual(Tick.Up, PriceAdaptor.AdaptPriceField(FieldName.BidTick, StringField("Up")).Value);
            Assert.AreEqual(Tick.Up, PriceAdaptor.AdaptPriceField(FieldName.BidTick, StringField("WasUp")).Value);
            Assert.AreEqual(Tick.Down, PriceAdaptor.AdaptPriceField(FieldName.BidTick, StringField("Down")).Value);
            Assert.AreEqual(Tick.Down, PriceAdaptor.AdaptPriceField(FieldName.BidTick, StringField("WasDown")).Value);
            Assert.AreEqual(Tick.Flat, PriceAdaptor.AdaptPriceField(FieldName.BidTick, StringField("Steady")).Value);
            Assert.AreEqual(Tick.Flat, PriceAdaptor.AdaptPriceField(FieldName.BidTick, StringField("Balls")).Value);
        }

    }
}