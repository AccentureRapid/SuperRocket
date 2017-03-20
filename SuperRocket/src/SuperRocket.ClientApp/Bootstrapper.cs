using System;
using System.Windows;
using System.Reflection;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using FirstFloor.ModernUI.Presentation;
using SuperRocket.Core.Interfaces;
using SuperRocket.ClientApp;
using log4net;
using Prism.Logging;
using log4net.Config;
using SuperRocket.Core;
using SuperRocket.Framework;
using SuperRocket.Framework.Logger;
using Prism.Autofac;
using Autofac;
using SuperRocket.Framework.Caching;
using SuperRocket.Framework.Caching.Impl;
using SuperRocket.Framework.Environment;
using SuperRocket.Framework.Environment.Configuration;

namespace SuperRocket.CientApp
{
    class Bootstrapper : AutofacBootstrapper
    {
        private const string MODULES_PATH = @".\modules";
        private LinkGroupCollection linkGroupCollection = null;
        private ICacheManager _cacheManager;
        protected override DependencyObject CreateShell()
        {
            Shell shell = Container.Resolve<Shell>();

            if (linkGroupCollection != null)
            {
                shell.AddLinkGroups(linkGroupCollection);
            }

            var cb = new ContainerBuilder();
            cb.RegisterType<KernelBuilder>().As<IKernelBuilder>().SingleInstance();
            cb.Update(Container);

            

            return shell;
        }
        public ILifetimeScope LifetimeScope { get; set; }
        protected override void InitializeShell()
        {
            base.InitializeShell();

            IKernelBuilder builder = Container.Resolve<IKernelBuilder>();
            //This is where to register additional lib
            //builder.OnStarting(b => b.RegisterType<Shell>().AsSelf().As<ICacheManager>().InstancePerLifetimeScope());
            builder.UseCaching(c => c.UseMemoryCache());

            var hostContainer = builder.Build();

            //To make sure every property has its injected value
            var type = GetType();
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;
                if (!hostContainer.IsRegistered(propertyType))
                    continue;
                property.SetValue(this, hostContainer.Resolve(propertyType), null);
            }
            //To load the cache manager to manage cache
            _cacheManager = LifetimeScope.Resolve<ICacheManager>(new TypedParameter(typeof(Type), typeof(Bootstrapper)));

            var host = hostContainer.Resolve<IHost>();
            host.Initialize();

          
            

            var work = host.CreateStandaloneEnvironment(new ShellSettings { Name = "Default" });

            //var form = work.Resolve<Shell>();


            App.Current.MainWindow = (Window)Shell;
            App.Current.MainWindow.Show();
        }

        //protected override void ConfigureContainer()
        //{
        //    base.ConfigureContainer();
        //    //Container.RegisterType<InterfaceName, ClassName>();
        //}

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new DirectoryModuleCatalog() { ModulePath = MODULES_PATH };
        }

        protected override void ConfigureModuleCatalog()
        {
            // Dynamic Modules are copied to a directory as part of a post-build step.
            // These modules are not referenced in the startup project and are discovered 
            // by examining the assemblies in a directory. The module projects have the 
            // following post-build step in order to copy themselves into that directory:
            //
            // xcopy "$(TargetDir)$(TargetFileName)" "$(TargetDir)modules\" /y
            //
            // WARNING! Do not forget to explicitly compile the solution before each running
            // so the modules are copied into the modules folder.
            var directoryCatalog = (DirectoryModuleCatalog)ModuleCatalog;
            directoryCatalog.Initialize();

            Logger.Log("directoryCatalog Initialized",Category.Info,Priority.Low);

            linkGroupCollection = new LinkGroupCollection();
            var typeFilter = new TypeFilter(InterfaceFilter);

            foreach (var module in directoryCatalog.Items)
            {
                var mi = (ModuleInfo)module;
                var asm = Assembly.LoadFrom(mi.Ref);

                foreach (Type t in asm.GetTypes())
                {
                    var myInterfaces = t.FindInterfaces(typeFilter, typeof(ILinkGroupService).ToString());

                    if (myInterfaces.Length > 0)
                    {
                        // We found the type that implements the ILinkGroupService interface
                        var linkGroupService = (ILinkGroupService)asm.CreateInstance(t.FullName);
                        var linkGroup = linkGroupService.GetLinkGroup();
                        linkGroupCollection.Add(linkGroup);
                    }
                }
            }

            // Module CoreModule is defined in the code.
            var moduleCatalog = (ModuleCatalog)ModuleCatalog;
            moduleCatalog.AddModule(typeof(FrameworkModule));
            moduleCatalog.AddModule(typeof(CoreModule));
        }

        private bool InterfaceFilter(Type typeObj, Object criteriaObj)
        {
            return typeObj.ToString() == criteriaObj.ToString();
        }

        protected override ILoggerFacade CreateLogger()
        {
            var logger = new Log4NetLogger();
            XmlConfigurator.Configure();
            return logger;
        }
    }
}