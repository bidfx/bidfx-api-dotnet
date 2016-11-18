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
            var defaultSession = DefaultSession.GetDefault();
            Assert.NotNull(defaultSession);
            Assert.AreSame(defaultSession, DefaultSession.GetDefault());
        }
    }
}