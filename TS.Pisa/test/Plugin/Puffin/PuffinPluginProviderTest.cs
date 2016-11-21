using NUnit.Framework;

namespace TS.Pisa.Plugin.Puffin
{
    [TestFixture]
    public class PuffinPluginProviderTest
    {
        [Test]
        public void valid_subject_is_subject_compatible()
        {
            IProviderPlugin providerPlugin = new PuffinProviderPlugin();
            Assert.IsTrue(providerPlugin.IsSubjectCompatible(
                "AssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=FR0010870956"));
        }
        [Test]
        public void invalid_subject_is_subject_compatible()
        {
            IProviderPlugin providerPlugin = new PuffinProviderPlugin();
            Assert.IsFalse(providerPlugin.IsSubjectCompatible(
                "AssetClass=Equity,Exchange=OSL,Level=1,Source=ComStock,Symbol=E:SUBC"));
        }
    }
}