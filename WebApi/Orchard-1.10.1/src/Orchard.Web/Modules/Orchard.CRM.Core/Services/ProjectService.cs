using Newtonsoft.Json.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Navigation.Models;
using Orchard.Core.Navigation.Services;
using Orchard.Core.Title.Models;
using Orchard.CRM.Core;
using Orchard.CRM.Core.Models;
using Orchard.CRM.Core.Providers.Filters;
using Orchard.CRM.Core.Services;
using Orchard.CRM.Core.ViewModels;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Settings;
using Orchard.UI.Navigation;
using Orchard.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Orchard.CRM.Core.Services
{
    public class ProjectService : BaseService, IProjectService
    {
        public const string ProjectContentType = "ProjectItem";

        private readonly ICRMContentOwnershipService crmContentOwnershipService;
        private readonly ISiteService siteService;
        private readonly IMenuService menuService;
        protected readonly INavigationManager navigationManager;
        private readonly IContentDefinitionManager contentDefinitionManager;

        public ProjectService(
            IContentDefinitionManager contentDefinitionManager,
            INavigationManager navigationManager,
            IMenuService menuService,
            ISiteService siteService,
            IOrchardServices services,
            ICRMContentOwnershipService crmContentOwnershipService,
            IProjectionManagerWithDynamicSort projectionManagerWithDynamicSort)
            : base(services, projectionManagerWithDynamicSort)
        {
            this.contentDefinitionManager = contentDefinitionManager;
            this.navigationManager = navigationManager;
            this.menuService = menuService;
            this.siteService = siteService;
            this.crmContentOwnershipService = crmContentOwnershipService;
            this.Logger = NullLogger.Instance;
            this.T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public int? GetProjectIdFromQueryString()
        {
            // retrieving paging parameters
            var queryString = this.services.WorkContext.HttpContext.Request.QueryString;
            var projectKey = "projectId";
            int? project = null;

            if (queryString.AllKeys.Contains(projectKey))
            {
                int temp;
                Int32.TryParse(queryString[projectKey], out temp);
                project = temp;
            }

            return project;
        }

        public int GetProjectsCount()
        {
            var contentQuery = this.GetProjectsQuery();

            return contentQuery.Count();
        }

        public IEnumerable<ContentItem> GetProjects(Pager pager)
        {
            var contentQuery = this.GetProjectsQuery();
            dynamic state = new JObject();
            state.Sort = false;
            contentQuery = this.projectionManagerWithDynamicSort.AddSortCriterion("CommonPartRecord", "PublishedUtc", state, contentQuery);
 
            var contentItems = pager != null ?
                contentQuery.Slice((pager.Page > 0 ? pager.Page - 1 : 0) * pager.PageSize, pager.PageSize) :
                contentQuery.List();

            return contentItems.ToArray();
        }

        public ProjectPart GetProject(int id)
        {
            var contentQuery = this.services.ContentManager.HqlQuery().ForVersion(VersionOptions.Published);
            dynamic state = new JObject();

            // Project contentType
            state.ContentTypes = ProjectContentType;
            contentQuery = this.projectionManagerWithDynamicSort.ApplyFilter(contentQuery, "Content", "ContentTypes", state);

            state.Project_Id = id.ToString(CultureInfo.InvariantCulture);
            contentQuery = this.projectionManagerWithDynamicSort.ApplyFilter(contentQuery, ProjectFilter.CategoryName, ProjectFilter.IdFilterType, state);

            contentQuery = this.ApplyContentPermissionFilter(contentQuery);
            contentQuery = contentQuery.Include("ProjectPartRecord");

            var contentItem = contentQuery.Slice(0, 1).FirstOrDefault();

            return contentItem != null ? contentItem.As<ProjectPart>() : null;
        }

        public bool IsTicketsRelatedToProjects()
        {
            var contentTypeDefinition = contentDefinitionManager.GetTypeDefinition("Ticket");
            if (contentTypeDefinition == null)
            {
                return false;
            }

            return contentTypeDefinition.Parts.Any(c => c.PartDefinition.Name == "AttachToProjectPart");
        }

        private IHqlQuery GetProjectsQuery()
        {
            var contentQuery = this.services.ContentManager.HqlQuery().ForVersion(VersionOptions.Published);
            dynamic state = new JObject();

            // Project contentType
            state.ContentTypes = ProjectContentType;
            contentQuery = this.projectionManagerWithDynamicSort.ApplyFilter(contentQuery, "Content", "ContentTypes", state);

            contentQuery = this.ApplyContentPermissionFilter(contentQuery);

            contentQuery = contentQuery.Include(new[] { "ProjectPartRecord", "ContentItemPermissionPartRecord" });
            return contentQuery;
        }
    }
}