using Orchard.ContentManagement.Drivers;
using Orchard.CRM.Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using System.Dynamic;
using Orchard.CRM.Project.ViewModels;
using Orchard.ContentManagement.FieldStorage.InfosetStorage;
using Orchard.CRM.Project.Services;

namespace Orchard.CRM.Project.Drivers
{
    public class ProjectDashboardEditorDriver : ContentPartDriver<ProjectDashboardEditorPart>
    {
        private readonly IOrchardServices services;
        private readonly IExtendedProjectService projectService;

        public ProjectDashboardEditorDriver(IOrchardServices services, IExtendedProjectService projectService)
        {
            this.projectService = projectService;
            this.services = services;
        }

        protected override DriverResult Display(ProjectDashboardEditorPart part, string displayType, dynamic shapeHelper)
        {
            // There is nothing to show in Display Mode
            return null;
        }

        protected override DriverResult Editor(ProjectDashboardEditorPart part, ContentManagement.IUpdateModel updater, dynamic shapeHelper)
        {
            List<EditPortletViewModel> model = new List<EditPortletViewModel>();

            updater.TryUpdateModel(model, "Portlets", null, null);

            var selectedPortlets = model.Where(c => c.IsChecked).OrderBy(c => c.Order).ToList();
            part.PortletList = selectedPortlets.Select(c => c.PortletId).ToArray();

            return null;
        }

        protected override DriverResult Editor(ProjectDashboardEditorPart part, dynamic shapeHelper)
        {
            // get the portlets
            IEnumerable<ContentItem> portlets = this.projectService.GetPortletsTemplates();

            List<EditPortletViewModel> model = new List<EditPortletViewModel>();

            var currentPortletIds = part.PortletList != null ? part.PortletList.ToList() : new List<int>();

            // if the Part is in the add mode, then select the default portlets
            if (part.Id == default(int))
            {
                currentPortletIds.AddRange(this.projectService.GetDefaultPortletIds(portlets));
            }

            // we assume all portlets have TitleParts
            foreach (var item in portlets.Where(c => c.Is<TitlePart>()))
            {
                EditPortletViewModel modelMember = new EditPortletViewModel();
                modelMember.PortletId = item.Id;
                modelMember.Title = item.As<TitlePart>().Title;
                modelMember.IsChecked = currentPortletIds.Contains(item.Id);
                modelMember.Order = currentPortletIds.Contains(item.Id) ? currentPortletIds.IndexOf(item.Id) : -int.MaxValue;

                model.Add(modelMember);
            }

            model = model.OrderByDescending(c => c.Order).ToList();

            return ContentShape("Parts_ProjectDashboardEditor_Edit",
                   () => shapeHelper.EditorTemplate(
                       TemplateName: "Parts/ProjectDashboardEditor",
                       Model: model,
                       Prefix: Prefix));
        }
    }
}