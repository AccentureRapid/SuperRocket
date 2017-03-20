using System;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using SuperRocket.Core.Services;
using Autofac;

namespace SuperRocket.Core
{
    public class CoreModule : IModule
    {
        private readonly IContainer _container;

        public CoreModule(IContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException($"{nameof(container)}");
            }

            _container = container;
        }

        public void Initialize()
        {
            var cb = new ContainerBuilder();
            cb.RegisterType<CustomerService>().As<ICustomerService>().InstancePerDependency();
            cb.RegisterType<DownloadService>().As<IDownloadService>().InstancePerDependency();
            cb.RegisterType<ZipService>().As<IZipService>().InstancePerDependency();
            cb.Update(_container);
        }
    }
}