
namespace Orchard.OData
{
    using Orchard.Wcf;
    using System;
    using System.Data.Services;
    using System.ServiceModel;

    internal class ODataServiceFactory : OrchardServiceHostFactory
    {
        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
        {
            return base.CreateServiceHost(constructorString, baseAddresses);
        }

        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return new DataServiceHost(serviceType, baseAddresses);
        }
    }
}