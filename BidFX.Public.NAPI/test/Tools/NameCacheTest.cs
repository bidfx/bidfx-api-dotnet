using System.Collections.Generic;
using BidFX.Public.NAPI.Price.Tools;
using NUnit.Framework;

namespace BidFX.Public.NAPI.test.Tools
{
    [TestFixture]
    public class NameCacheTest
    {
        [Test]
        public virtual void TestCreateUniqueNameForprefix()
        {
            NameCache nameCache = new NameCache();
            Assert.AreEqual("Test1", nameCache.CreateUniqueName("Test"));
            Assert.AreEqual("Test2", nameCache.CreateUniqueName("Test"));
            Assert.AreEqual("Test3", nameCache.CreateUniqueName("Test"));
            Assert.AreEqual("Client1", nameCache.CreateUniqueName("Client"));
            Assert.AreEqual("File.1", nameCache.CreateUniqueName("File."));
            Assert.AreEqual("File.2", nameCache.CreateUniqueName("File."));
            Assert.AreEqual("File.3", nameCache.CreateUniqueName("File."));
        }

        [Test]
        public virtual void TestCreateUniqueNameForType()
        {
            NameCache nameCache = new NameCache();
            Assert.AreEqual("NameCacheTest1", nameCache.CreateUniqueName(typeof(NameCacheTest)));
            Assert.AreEqual("NameCacheTest2", nameCache.CreateUniqueName(typeof(NameCacheTest)));
            Assert.AreEqual("NameCacheTest3", nameCache.CreateUniqueName(typeof(NameCacheTest)));
            Assert.AreEqual("List1", nameCache.CreateUniqueName(typeof(List)));
            Assert.AreEqual("List2", nameCache.CreateUniqueName(typeof(List)));
        }

        [Test]
        public virtual void TestNextID()
        {
            NameCache nameCache = new NameCache();
            Assert.AreEqual(1, nameCache.NextID("Test"));
            Assert.AreEqual(2, nameCache.NextID("Test"));
            Assert.AreEqual(3, nameCache.NextID("Test"));
            Assert.AreEqual(1, nameCache.NextID("Test2"));
        }

        [Test]
        public virtual void TestLastID()
        {
            NameCache nameCache = new NameCache();
            Assert.AreEqual(0, nameCache.LastID("Test"));
            Assert.AreEqual(0, nameCache.LastID("Test"));
            Assert.AreEqual(1, nameCache.NextID("Test"));
            Assert.AreEqual(2, nameCache.NextID("Test"));
            Assert.AreEqual(2, nameCache.LastID("Test"));
            Assert.AreEqual(2, nameCache.LastID("Test"));
            Assert.AreEqual(0, nameCache.LastID("Test2"));
            Assert.AreEqual(0, nameCache.LastID("Test2"));
        }

        [Test]
        public virtual void TestGetDefault()
        {
            NameCache cache = NameCache.Default();
            Assert.IsNotNull(cache);
            Assert.AreSame(cache, NameCache.Default());
        }

        [Test]
        public virtual void TestNames()
        {
            NameCache nameCache = new NameCache();
            nameCache.NextID("Test");
            nameCache.NextID("Test2");
            nameCache.NextID("Test2");
            nameCache.LastID("Test3");
            nameCache.NextID("Test");
            Assert.AreEqual(new List<string> {"Test", "Test2"}, nameCache.Names());
        }
    }
}