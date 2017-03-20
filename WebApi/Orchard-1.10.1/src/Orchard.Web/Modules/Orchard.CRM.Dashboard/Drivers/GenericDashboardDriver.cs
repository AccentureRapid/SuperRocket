using Orchard.ContentManagement.MetaData;
using Orchard.Core.Containers.Services;
using Orchard.Core.Feeds;
using Orchard.Settings;
using Orchard.CRM.Dashboard.Models;
using Orchard.CRM.Core.Services;

namespace Orchard.CRM.Dashboard.Drivers
{
    public class GenericDashboardDriver : BaseGenericDashboardDriver<GenericDashboardPart>
    {

        public GenericDashboardDriver(
            IContentDefinitionManager contentDefinitionManager,
            IOrchardServices orchardServices,
            ICRMContentOwnershipService crmContentOwnershipService,
            ISiteService siteService,
            IFeedManager feedManager, IContainerService containerService)
            : base(crmContentOwnershipService, contentDefinitionManager, orchardServices, siteService, feedManager, containerService)
        {
        }
    }
}