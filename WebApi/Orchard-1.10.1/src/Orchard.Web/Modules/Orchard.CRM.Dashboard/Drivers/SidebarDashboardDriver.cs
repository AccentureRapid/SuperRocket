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
using Orchard.Core.Title.Models;
using System.Web.Mvc;
using System.Globalization;
using System.Dynamic;
using Orchard.CRM.Dashboard.ViewModels;
using System.Collections.Generic;
using Orchard.CRM.Core.Services;

namespace Orchard.CRM.Dashboard.Drivers
{
    public class SidebarDashboardDriver : BaseGenericDashboardDriver<SidebarDashboardPart>
    {
        public SidebarDashboardDriver(
            ICRMContentOwnershipService crmContentOwnershipService,
            IContentDefinitionManager contentDefinitionManager,
            IOrchardServices orchardServices,
            ISiteService siteService,
            IFeedManager feedManager, IContainerService containerService)
            : base(crmContentOwnershipService, contentDefinitionManager, orchardServices, siteService, feedManager, containerService)
        {
        }

        protected override DriverResult Editor(SidebarDashboardPart part, dynamic shapeHelper)
        {
            if (!_crmContentOwnershipService.IsCurrentUserAdvanceOperator())
            {
                return null;
            }

            List<string> currentPortlets =
                string.IsNullOrEmpty(part.SidebarPortletList) ?
                    new List<string>() :
                    part.SidebarPortletList.Split(',').Select(c => c.ToUpper(CultureInfo.InvariantCulture).Trim()).ToList();

            var portlets = _orchardServices
                .ContentManager
                .HqlQuery()
                .ForType(Consts.SidebarProjectionPortletTemplateType, Consts.SidebarStaticPortletType)
                .List()
                .Select(c => c.As<TitlePart>());

            List<EditDashboardViewModel> model = new List<EditDashboardViewModel>();


            // we assume all portlets have TitleParts
            foreach (var item in portlets.Where(c => c.Is<TitlePart>()))
            {
                string title = string.IsNullOrEmpty(item.Title) ? string.Empty : item.Title.ToUpper(CultureInfo.InvariantCulture).Trim();
                EditDashboardViewModel modelMember = new EditDashboardViewModel();
                modelMember.PortletId = item.Id;
                modelMember.Title = item.As<TitlePart>().Title;
                modelMember.IsChecked = currentPortlets.Contains(title);
                modelMember.Order = modelMember.IsChecked ? currentPortlets.IndexOf(item.Title) : -int.MaxValue;

                model.Add(modelMember);
            }

            model = model.OrderByDescending(c => c.Order).ToList();

            return ContentShape("Parts_SidebarDashboard_Edit",
                        () => shapeHelper.EditorTemplate(
                            TemplateName: "Parts/SidebarDashboard",
                            Model: model,
                            Prefix: Prefix));
        }

        protected override DriverResult Editor(SidebarDashboardPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            if (!_crmContentOwnershipService.IsCurrentUserAdvanceOperator())
            {
                return null;
            }

            List<EditDashboardViewModel> model = new List<EditDashboardViewModel>();
            updater.TryUpdateModel(model, "Portlets", null, null);

            var selectedPortlets = model.Where(c => c.IsChecked).OrderBy(c => c.Order).Select(c => c.PortletId).ToList();
            var portlets = _orchardServices.ContentManager.GetMany<TitlePart>(selectedPortlets, VersionOptions.Published, QueryHints.Empty);
            part.SidebarPortletList = string.Join(",", portlets.Select(c => c.Title));

            return Editor(part, shapeHelper);
        }
    }
}