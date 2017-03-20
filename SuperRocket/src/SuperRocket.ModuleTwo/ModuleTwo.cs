using System;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Autofac;

namespace SuperRocket.ModuleTwo
{
    public class ModuleTwo : IModule
    {
        private readonly IContainer _container;

        public ModuleTwo(IContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException($"{nameof(container)}");
            }

            _container = container;
        }

        public void Initialize()
        {
            //_container.RegisterType<InterfaceName, ClassName>();
            //System.Windows.MessageBox.Show($"{nameof(ModuleTwo)} has been initialized ;-)");
        }
    }
}