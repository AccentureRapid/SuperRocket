using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperRocket.Framework.Environment.Configuration;
using SuperRocket.Framework.Environment.ShellBuilders;
using SuperRocket.Framework.Extensions;
using SuperRocket.Framework.FileSystems.Application;
using System;

namespace SuperRocket.Framework.Tests.Environment.ShellBuilders
{
    [TestClass]
    public sealed class ShellContextFactoryTests : TestBase, IDisposable
    {
        #region Property

        public IApplicationFolder ApplicationFolder { get; set; }

        public IExtensionLoaderCoordinator ExtensionLoaderCoordinator { get; set; }

        public IShellContextFactory ShellContextFactory { get; set; }

        #endregion Property

        public ShellContextFactoryTests()
        {
            ExtensionLoaderCoordinator.SetupExtensions();
        }

        #region Test Method

        [TestMethod]
        public void CreateShellContextTest()
        {
            var context = ShellContextFactory.CreateShellContext(new ShellSettings { Name = "Test" });
            Assert.IsNotNull(context);
            var service = context.Container.Resolve<IExtensionManager>();
            Assert.IsNotNull(service);
        }

        #endregion Test Method

        #region Implementation of IDisposable

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            try { ApplicationFolder.DeleteDirectory("~/App_Data"); }
            catch { }
        }

        #endregion Implementation of IDisposable
    }
}