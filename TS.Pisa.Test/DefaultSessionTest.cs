using System;
using NUnit.Framework;

namespace TS.Pisa.Test
{
    [TestFixture]
    public class DefaultSessionTest
    {
        [Test]
        public void TestGetDefaultSession()
        {
            ISession defaultSession = DefaultSession.GetDefault();
            Assert.NotNull(defaultSession);
            Assert.AreSame(defaultSession, DefaultSession.GetDefault());
        }
    }
}


