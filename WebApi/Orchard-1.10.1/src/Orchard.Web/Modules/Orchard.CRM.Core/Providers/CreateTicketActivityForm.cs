using NHibernate.Mapping;
using Orchard.ContentManagement;
using Orchard.CRM.Core.Models;
using Orchard.CRM.Core.Services;
using Orchard.CRM.Core.ViewModels;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Forms.Services;
using Orchard.Localization;
using Orchard.Users.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Orchard.CRM.Core.Providers
{
    public class CreateTicketActivityForm : IFormProvider
    {
        private readonly IBasicDataService basicDataService;
        private readonly ICRMContentOwnershipService contentOwnershipService;
        private readonly IContentOwnershipHelper contentOwnershipHelper;
        private readonly IProjectService projectService;

        public const string Name = "CreateTicketActivityForm";
        public const string ServiceId = "ServiceId";
        public const string PriorityId = "PriorityId";
        public const string ProjectId = "ProjectId";
        public const string StatusId = "StatusId";
        public const string SelectedUserId = "UserId";
        public const string DueDateId = "DueDate";
        public const string TicketTitle = "Title";
        public const string TicketDescription = "Description";

        public CreateTicketActivityForm(
              IProjectService projectService,
              IBasicDataService basicDataService,
              IContentOwnershipHelper contentOwnershipHelper,
              ICRMContentOwnershipService contentOwnershipService,
              IShapeFactory shapeFactory,
              IContentManager contentManager)
        {
            this.contentOwnershipHelper = contentOwnershipHelper;
            this.contentOwnershipService = contentOwnershipService;
            this.basicDataService = basicDataService;
            this.contentManager = contentManager;
            this.projectService = projectService;
            this.Shape = shapeFactory;
            this.T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        protected IContentManager contentManager;
        protected dynamic Shape { get; set; }

        public void Describe(DescribeContext context)
        {
            Func<IShapeFactory, dynamic> formFactory =
                shapeFactory =>
                {
                    var priorities = this.basicDataService.GetPriorities().ToList();
                    var statusRecords = this.basicDataService.GetStatusRecords().ToList().Select(c => new BasicDataRecordViewModel { Id = c.Id, Name = c.Name }).ToList();
                    var serviceRecords = this.basicDataService.GetServices().ToList();
                    var projects = this.projectService.GetProjects(null).AsPart<ProjectPart>().ToList();

                    Collection<SelectListItem> prioritySelectList = new Collection<SelectListItem>();
                    Collection<SelectListItem> statusSelectList = new Collection<SelectListItem>();
                    Collection<SelectListItem> serviceSelectList = new Collection<SelectListItem>();
                    Collection<SelectListItem> projectList = new Collection<SelectListItem>();

                    Converter.Fill(prioritySelectList, null, priorities);
                    prioritySelectList.Insert(0, new SelectListItem());

                    Converter.Fill(statusSelectList, null, statusRecords);
                    statusSelectList.Insert(0, new SelectListItem());

                    Converter.Fill(serviceSelectList, null, serviceRecords);
                    serviceSelectList.Insert(0, new SelectListItem());

                    Converter.Fill(projectList, projects);

                    Collection<SelectListItem> dueDates = CRMHelper.GetDueDateItems(this.T);

                    ContentItemSetPermissionsViewModel permissionsViewModel = this.contentOwnershipHelper.CreateModel();
                    permissionsViewModel.Users.Insert(0, new SelectListItem());
                    var t = this.Shape.Form(_main: this.Shape.CreateTicketActivity(
                        Priorities: prioritySelectList,
                        Projects: projectList,
                        StatusList: statusSelectList,
                        Services: serviceSelectList,
                        DueDates: dueDates,
                        Organization: permissionsViewModel
                        ));

                    return t;

                };

            context.Form(CreateTicketActivityForm.Name, formFactory);
        }
    }
}