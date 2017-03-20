using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Common.Models;
using Orchard.Core.Containers.Models;
using Orchard.Core.Containers.Services;
using Orchard.Core.Feeds;
using Orchard.Localization;
using Orchard.Settings;
using System.Linq;
using System.Web.Routing;
using Orchard.CRM.Core.Models;
using Orchard.CRM.Dashboard.Models;
using Orchard.CRM.Core.Services;

namespace Orchard.CRM.Dashboard.Drivers
{
    public abstract class BaseGenericDashboardDriver<TContentPart> : ContentPartDriver<TContentPart>
        where TContentPart : ContentPart, new()
    {
        protected readonly IContentDefinitionManager _contentDefinitionManager;
        protected readonly IOrchardServices _orchardServices;
        protected readonly IContentManager _contentManager;
        protected readonly ISiteService _siteService;
        protected readonly IFeedManager _feedManager;
        protected readonly IContainerService _containerService;
        protected readonly ICRMContentOwnershipService _crmContentOwnershipService;

        public BaseGenericDashboardDriver(
            ICRMContentOwnershipService crmContentOwnershipService,
            IContentDefinitionManager contentDefinitionManager,
            IOrchardServices orchardServices,
            ISiteService siteService,
            IFeedManager feedManager, IContainerService containerService)
        {
            _crmContentOwnershipService = crmContentOwnershipService;
            _contentDefinitionManager = contentDefinitionManager;
            _orchardServices = orchardServices;
            _contentManager = orchardServices.ContentManager;
            _siteService = siteService;
            _feedManager = feedManager;
            _containerService = containerService;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        protected override DriverResult Display(TContentPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_GenericDashboard_Detail", () =>
            {
                var container = part.ContentItem;
                var query = _contentManager
                .Query(VersionOptions.Published)
                .Join<CommonPartRecord>().Where(x => x.Container.Id == container.Id)
                .Join<ContainablePartRecord>().OrderBy(x => x.Position);

                var startIndex = 0;
                var pageOfItems = query.Slice(startIndex, 10).ToList();

                var listShape = shapeHelper.List();
                listShape.AddRange(pageOfItems.Select(item => _contentManager.BuildDisplay(item, "Summary")));
                listShape.Classes.Add("content-items");
                listShape.Classes.Add("list-items");
                bool isAdmin = _crmContentOwnershipService.IsCurrentUserAdvanceOperator();

                return shapeHelper.Parts_GenericDashboard_Detail(
                    List: listShape,
                    isAdmin: isAdmin,
                        Pager: null
                    );
            });
        }
    }
}