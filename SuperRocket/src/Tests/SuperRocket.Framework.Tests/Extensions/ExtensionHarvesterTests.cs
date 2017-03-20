using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperRocket.Framework.Extensions.Folders;
using System.Linq;

namespace SuperRocket.Framework.Tests.Extensions
{
    [TestClass]
    public sealed class ExtensionHarvesterTests : TestBase
    {
        public IExtensionHarvester ExtensionHarvester { get; set; }

        //        public ILifetimeScope LifetimeScope { get; set; }

        [TestMethod]
        public void HarvestExtensionsTest()
        {
            var extensions = ExtensionHarvester.HarvestExtensions(new[] { "~/Modules" }, "Module", "Module.txt", false);

            Assert.IsTrue(extensions.Any());
        }
    }
}