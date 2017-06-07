using BidFX.Public.NAPI.Price;
using NUnit.Framework;

namespace BidFX.Public.NAPI.test
{
    [TestFixture]
    public class DefaultSessionTest
    {
        [Test]
        public void TestDefaultSession()
        {
            var defaultSession = DefaultSession.Session;
            Assert.NotNull(defaultSession);
            Assert.AreSame(defaultSession, DefaultSession.Session);
        }

        [Test]
        public void TestDefaultSubscriber()
        {
            var defaultSubscriber = DefaultSession.Subscriber;
            Assert.NotNull(defaultSubscriber);
            Assert.AreSame(defaultSubscriber, DefaultSession.Subscriber);
        }
    }
}