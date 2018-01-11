namespace BidFX.Public.API.Price
{
    internal class Subscription
    {
        public SubscriptionStatus SubscriptionStatus { get; set; }
        public PriceMap AllPriceFields { get; private set; }
        public Subject.Subject Subject { get; private set; }
        public bool AutoRefresh { get; private set; }

        public Subscription(Subject.Subject subject, bool autoRefresh)
        {
            SubscriptionStatus = SubscriptionStatus.PENDING;
            AllPriceFields = new PriceMap();
            Subject = subject;
            AutoRefresh = autoRefresh;
        }
    }
}