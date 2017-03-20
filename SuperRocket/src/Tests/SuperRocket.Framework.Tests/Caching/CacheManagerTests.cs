using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperRocket.Framework.Caching;
using System;

namespace SuperRocket.Framework.Tests.Caching
{
    [TestClass]
    public sealed class CacheManagerTests : TestBase
    {
        private readonly ICacheManager _cacheManager;

        public CacheManagerTests()
        {
            _cacheManager = LifetimeScope.Resolve<ICacheManager>(new TypedParameter(typeof(Type), typeof(CacheManagerTests)));
        }

        public ILifetimeScope LifetimeScope { get; set; }

        [TestMethod]
        public void CacheTest()
        {
            var count = 0;
            var result = _cacheManager.Get("Test", ctx => count = count + 1);
            Assert.AreEqual(1, result);
            result = _cacheManager.Get("Test", ctx => count = count + 1);
            Assert.AreEqual(1, result);

            result = _cacheManager.Get("Test1", ctx => count = count + 1);
            Assert.AreEqual(2, result);
        }
    }
}