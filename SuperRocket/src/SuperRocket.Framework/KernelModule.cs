using Autofac;
using SuperRocket.Framework.Caching;
using SuperRocket.Framework.Environment;
using SuperRocket.Framework.Environment.Assemblies;
using SuperRocket.Framework.Environment.Assemblies.Impl;
using SuperRocket.Framework.Environment.Configuration;
using SuperRocket.Framework.Environment.Configuration.Impl;
using SuperRocket.Framework.Environment.Descriptor;
using SuperRocket.Framework.Environment.Descriptor.Impl;
using SuperRocket.Framework.Environment.Impl;
using SuperRocket.Framework.Environment.ShellBuilders;
using SuperRocket.Framework.Environment.ShellBuilders.Impl;
using SuperRocket.Framework.Events;
using SuperRocket.Framework.Extensions;
using SuperRocket.Framework.Extensions.Folders;
using SuperRocket.Framework.Extensions.Folders.Impl;
using SuperRocket.Framework.Extensions.Impl;
using SuperRocket.Framework.Extensions.Loaders;
using SuperRocket.Framework.Extensions.Loaders.Impl;
using SuperRocket.Framework.FileSystems.AppData;
using SuperRocket.Framework.FileSystems.AppData.Impl;
using SuperRocket.Framework.FileSystems.Application;
using SuperRocket.Framework.FileSystems.Application.Impl;
using SuperRocket.Framework.FileSystems.Dependencies;
using SuperRocket.Framework.FileSystems.Dependencies.Impl;
using SuperRocket.Framework.FileSystems.VirtualPath;
using SuperRocket.Framework.FileSystems.VirtualPath.Impl;
using SuperRocket.Framework.Services;
using SuperRocket.Framework.Services.Impl;
using System.IO;
using System.Web.Hosting;

namespace SuperRocket.Framework
{
    /// <summary>
    /// 内核模块。
    /// </summary>
    [SuppressDependency("SuperRocket.Framework.KernelModule")]
    internal sealed class KernelModule : Module
    {
        #region Overrides of Module

        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <remarks>
        /// Note that the ContainerBuilder parameter is unique to this module.
        /// </remarks>
        /// <param name="builder">The builder through which components can be
        ///             registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new CollectionOrderModule());
            builder.RegisterModule(new CacheModule());

            //HostingEnvironment
            {
                if (!HostingEnvironment.IsHosted)
                {
                    builder.RegisterType<DefaultHostEnvironment>().As<IHostEnvironment>().SingleInstance();
                    builder.RegisterType<DefaultBuildManager>().As<IBuildManager>().SingleInstance();
                }
                else
                {
                    builder.RegisterType<WebHostEnvironment>().As<IHostEnvironment>().SingleInstance();
                    builder.RegisterType<WebBuildManager>().As<IBuildManager>().SingleInstance();
                }
            }
            builder.RegisterType<DefaultHostLocalRestart>().As<IHostLocalRestart>().SingleInstance();

            //Assembly Loader
            {
                builder.RegisterType<DefaultAssemblyLoader>().As<IAssemblyLoader>().SingleInstance();
                builder.RegisterType<AppDomainAssemblyNameResolver>().As<IAssemblyNameResolver>().SingleInstance();
                builder.RegisterType<FrameworkAssemblyNameResolver>().As<IAssemblyNameResolver>().SingleInstance();
                builder.RegisterType<GacAssemblyNameResolver>().As<IAssemblyNameResolver>().SingleInstance();
            }

            //FileSystems
            {
                RegisterVolatileProvider<DefaultVirtualPathProvider, IVirtualPathProvider>(builder);
                RegisterVolatileProvider<DefaultVirtualPathMonitor, IVirtualPathMonitor>(builder);

                RegisterVolatileProvider<DefaultAppDataFolder, IAppDataFolder>(builder);
                RegisterVolatileProvider<DefaultApplicationFolder, IApplicationFolder>(builder);

                /*RegisterVolatileProvider<DefaultExtensionDependenciesManager, IExtensionDependenciesManager>(builder);
                RegisterVolatileProvider<DefaultDependenciesFolder, IDependenciesFolder>(builder);
                RegisterVolatileProvider<DefaultAssemblyProbingFolder, IAssemblyProbingFolder>(builder);*/
            }

            RegisterVolatileProvider<DefaultClock, IClock>(builder);
            RegisterVolatileProvider<DefaultDependenciesFolder, IDependenciesFolder>(builder);
            RegisterVolatileProvider<DefaultExtensionDependenciesManager, IExtensionDependenciesManager>(builder);
            RegisterVolatileProvider<DefaultAssemblyProbingFolder, IAssemblyProbingFolder>(builder);

            builder.RegisterType<DefaultServiceTypeHarvester>().As<IServiceTypeHarvester>().SingleInstance();

            builder.RegisterType<DefaultHost>().As<IHost>().As<IEventHandler>()
                .Named<IEventHandler>(typeof(IShellSettingsManagerEventHandler).Name)
                .Named<IEventHandler>(typeof(IShellDescriptorManagerEventHandler).Name)
                .SingleInstance();
            {
                builder.RegisterType<DefaultShellSettingsManager>().As<IShellSettingsManager>().SingleInstance();
                builder.RegisterType<DefaultShellContextFactory>().As<IShellContextFactory>().SingleInstance();
                {
                    builder.RegisterType<DefaultShellDescriptorCache>().As<IShellDescriptorCache>().SingleInstance();
                    builder.RegisterType<DefaultCompositionStrategy>().As<ICompositionStrategy>().SingleInstance();
                    {
                        builder.RegisterType<DefaultExtensionLoaderCoordinator>()
                            .As<IExtensionLoaderCoordinator>()
                            .SingleInstance();
                        builder.RegisterType<DefaultExtensionMonitoringCoordinator>().As<IExtensionMonitoringCoordinator>().SingleInstance();
                        builder.RegisterType<DefaultExtensionManager>().As<IExtensionManager>().SingleInstance();
                        {
                            builder.RegisterType<DefaultExtensionHarvester>().As<IExtensionHarvester>().SingleInstance();
                            builder.RegisterType<ModuleFolders>().As<IExtensionFolders>().SingleInstance()
                                .WithParameter(new NamedParameter("paths", new[] { "~/Modules" }));

                            builder.RegisterType<ReferencedExtensionLoader>().As<IExtensionLoader>().SingleInstance();
                            builder.RegisterType<PrecompiledExtensionLoader>().As<IExtensionLoader>().SingleInstance();
                        }
                        builder.RegisterType<DefaultShell>().As<IShell>().InstancePerMatchingLifetimeScope("shell");
                    }
                }
                builder.RegisterType<DefaultShellContainerFactory>().As<IShellContainerFactory>().SingleInstance();
                builder.RegisterType<DefaultHostContainer>().As<IHostContainer>().InstancePerDependency();
            }

            builder.RegisterType<KernelMinimumShellDescriptorProvider>()
                .As<IMinimumShellDescriptorProvider>()
                .SingleInstance();

            var optionalComponentsConfig = HostingEnvironment.MapPath("~/Config/HostComponents.config");
            if (File.Exists(optionalComponentsConfig))
                builder.RegisterModule(new HostComponentsConfigModule(optionalComponentsConfig));
        }

        #endregion Overrides of Module

        #region Private Method

        private static void RegisterVolatileProvider<TRegister, TService>(ContainerBuilder builder) where TService : IVolatileProvider
        {
            builder.RegisterType<TRegister>()
                .As<TService>()
                .As<IVolatileProvider>()
                .SingleInstance();
        }

        #endregion Private Method
    }
}