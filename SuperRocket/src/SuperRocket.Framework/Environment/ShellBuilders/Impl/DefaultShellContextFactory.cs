﻿using Autofac;
using SuperRocket.Framework.Environment.Configuration;
using SuperRocket.Framework.Environment.Descriptor;
using SuperRocket.Framework.Environment.Descriptor.Models;
using SuperRocket.Framework.Logging;
using SuperRocket.Framework.Utility.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SuperRocket.Framework.Environment.ShellBuilders.Impl
{
    internal sealed class DefaultShellContextFactory : IShellContextFactory
    {
        #region Field

        private readonly IShellDescriptorCache _shellDescriptorCache;
        private readonly IEnumerable<IMinimumShellDescriptorProvider> _minimumShellDescriptorProviders;
        private readonly ICompositionStrategy _compositionStrategy;
        private readonly IShellContainerFactory _shellContainerFactory;

        #endregion Field

        #region Property

        public ILogger Logger { get; set; }

        #endregion Property

        #region Constructor

        public DefaultShellContextFactory(ICompositionStrategy compositionStrategy, IShellContainerFactory shellContainerFactory, IShellDescriptorCache shellDescriptorCache, IEnumerable<IMinimumShellDescriptorProvider> minimumShellDescriptorProviders)
        {
            _compositionStrategy = compositionStrategy;
            _shellContainerFactory = shellContainerFactory;
            _shellDescriptorCache = shellDescriptorCache;
            _minimumShellDescriptorProviders = minimumShellDescriptorProviders;
            Logger = NullLogger.Instance;
        }

        #endregion Constructor

        #region Implementation of IShellContextFactory

        /// <summary>
        /// 创建一个外壳上下文工厂。
        /// </summary>
        /// <param name="settings">外壳设置。</param>
        /// <returns>外壳上下文。</returns>
        public ShellContext CreateShellContext(ShellSettings settings)
        {
            Logger.Debug("准备为租户 {0} 创建外壳上下文", settings.Name);

            var knownDescriptor = _shellDescriptorCache.Fetch(settings.Name);
            if (knownDescriptor == null)
            {
                Logger.Information("在缓存中找不到外壳描述符信息。 以最少的组件开始。");
                var features = new List<ShellFeature>();
                _minimumShellDescriptorProviders.Invoke(i => i.GetFeatures(features), Logger);
                knownDescriptor = MinimumShellDescriptor(features);
            }

            var blueprint = _compositionStrategy.Compose(settings, knownDescriptor);
            var shellScope = _shellContainerFactory.CreateContainer(settings, blueprint);

            ShellDescriptor currentDescriptor;
            using (var standaloneEnvironment = shellScope.CreateWorkContextScope())
            {
                var shellDescriptorManager = standaloneEnvironment.Resolve<IShellDescriptorManager>();
                currentDescriptor = shellDescriptorManager.GetShellDescriptor();
            }

            if (currentDescriptor != null && knownDescriptor.SerialNumber != currentDescriptor.SerialNumber)
            {
                currentDescriptor.Features = currentDescriptor.Features.Distinct(new ShellFeatureEqualityComparer()).ToArray();
                Logger.Information("获得较新的外壳描述符。重新构建外壳容器。");

                _shellDescriptorCache.Store(settings.Name, currentDescriptor);
                blueprint = _compositionStrategy.Compose(settings, currentDescriptor);
                shellScope.Dispose();
                shellScope = _shellContainerFactory.CreateContainer(settings, blueprint);
            }

            return new ShellContext
            {
                Settings = settings,
                Descriptor = currentDescriptor,
                Blueprint = blueprint,
                Container = shellScope,
                Shell = shellScope.Resolve<IShell>(),
            };
        }

        #endregion Implementation of IShellContextFactory

        #region Private Method

        private static ShellDescriptor MinimumShellDescriptor(IEnumerable<ShellFeature> features)
        {
            return new ShellDescriptor
            {
                SerialNumber = -1,
                Features = features.Distinct(new ShellFeatureEqualityComparer()).ToArray()
            };
        }

        #endregion Private Method
    }
}