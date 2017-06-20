using System.Collections.Generic;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public class SubscriptionSyncTest
    {
        [Test]
        public void Test()
        {
            SubscriptionSync s = new SubscriptionSync(1, new List<Subject.Subject>());
            s.Encode(3);
        }
    }
}