using System;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Autofac;

namespace SuperRocket.Framework
{
    public class FrameworkModule : IModule
    {
        private readonly IContainer _container;

        public FrameworkModule(IContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException($"{nameof(container)}");
            }

            _container = container;
        }

        public void Initialize()
        {

        }
    }
}