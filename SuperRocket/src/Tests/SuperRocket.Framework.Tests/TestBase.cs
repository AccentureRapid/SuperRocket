using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Rabbit.Components.Logging.NLog;
using SuperRocket.Framework.Caching;
using SuperRocket.Framework.Caching.Impl;
using SuperRocket.Framework.Logging;

namespace SuperRocket.Framework.Tests
{
    [TestClass]
    public abstract class TestBase
    {
        protected IContainer Container { get; private set; }

        protected TestBase()
        {
            var kernelBuilder = new KernelBuilder();
            kernelBuilder.OnStarting(Register);
            kernelBuilder.UseCaching(c => c.UseMemoryCache());
            //.UseLogging(c => c.UseNLog());

            var container = Container = kernelBuilder.Build();
            var type = GetType();
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;
                if (!container.IsRegistered(propertyType))
                    continue;
                property.SetValue(this, container.Resolve(propertyType), null);
            }
        }

        protected virtual void Register(ContainerBuilder builder)
        {
        }
    }
}