using System;
using NUnit.Framework;

namespace TS.Pisa
{
    [TestFixture]
    public class DefaultSessionTest
    {
        [Test]
        public void TestGetDefaultSession()
        {
            var defaultSession = DefaultSession.GetSession();
            Assert.NotNull(defaultSession);
            Assert.AreSame(defaultSession, DefaultSession.GetSession());
        }
    }
}