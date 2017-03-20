using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using NGM.Forum.Models;
using NGM.Forum.Extensions;
using NGM.Forum.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;

namespace NGM.Forum.Controllers {

    [ValidateInput(false), Admin]
    public class ForumAdminController : Controller, IUpdateModel {
        private readonly IOrchardServices _orchardServices;
        private readonly IForumService _forumService;
        private readonly IThreadService _threadService;
        private readonly ISiteService _siteService;
        private readonly IContentManager _contentManager;

        public ForumAdminController(IOrchardServices orchardServices, 
            IForumService forumService, 
            IThreadService threadService,
            ISiteService siteService,
            IContentManager contentManager,
            IShapeFactory shapeFactory) {
            _orchardServices = orchardServices;
            _forumService = forumService;
            _threadService = threadService;
            _siteService = siteService;
            _contentManager = contentManager;

            T = NullLocalizer.Instance;
            Shape = shapeFactory;
        }

        dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public ActionResult Create(string type) {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ManageForums, T("Not allowed to create forums")))
                return new HttpUnauthorizedResult();

            if (string.IsNullOrWhiteSpace(type)) {
                var forumTypes = _forumService.GetForumTypes();
                if (forumTypes.Count > 1)
                    return Redirect(Url.ForumSelectTypeForAdmin());

                if (forumTypes.Count == 0) {
                    _orchardServices.Notifier.Warning(T("You have no forum types available. Add one to create a forum."));
                    return Redirect(Url.DashboardForAdmin());
                }

                type = forumTypes.Single().Name;
            }

            var forum = _contentManager.New<ForumPart>(type);
            if (forum == null)
                return HttpNotFound();

            var model = _contentManager.BuildEditor(forum);
            return View((object)model);
        }

        public ActionResult SelectType() {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ManageForums, T("Not allowed to create forums")))
                return new HttpUnauthorizedResult();

            var forumTypes = _forumService.GetForumTypes();
            var model = Shape.ViewModel(ForumTypes: forumTypes);
            return View(model);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST(string type) {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ManageForums, T("Not allowed to create forums")))
                return new HttpUnauthorizedResult();

            if (string.IsNullOrWhiteSpace(type)) {
                var forumTypes = _forumService.GetForumTypes();
                if (forumTypes.Count > 1)
                    return Redirect(Url.ForumSelectTypeForAdmin());

                if (forumTypes.Count == 0) {
                    _orchardServices.Notifier.Warning(T("You have no forum types available. Add one to create a forum."));
                    return Redirect(Url.DashboardForAdmin());
                }

                type = forumTypes.Single().Name;
            }

            var forum = _contentManager.New<ForumPart>(type);

            _contentManager.Create(forum, VersionOptions.Draft);
            var model = _contentManager.UpdateEditor(forum, this);

            if (!ModelState.IsValid) {
                _orchardServices.TransactionManager.Cancel();
                return View((object)model);
            }

            _contentManager.Publish(forum.ContentItem);

            return Redirect(Url.ForumForAdmin(forum));
        }

        public ActionResult Edit(int forumId) {
            var forum = _forumService.Get(forumId, VersionOptions.Latest);

            if (forum == null)
                return HttpNotFound();

            if (!_orchardServices.Authorizer.Authorize(Permissions.ManageForums, forum, T("Not allowed to edit forum")))
                return new HttpUnauthorizedResult();

            dynamic model = _contentManager.BuildEditor(forum);
            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)model);
        }

        [HttpPost, ActionName("Edit")]
        [InternalFormValueRequired("submit.Save")]
        public ActionResult EditPOST(int forumId) {
            var forum = _forumService.Get(forumId, VersionOptions.DraftRequired);

            if (forum == null)
                return HttpNotFound();

            if (!_orchardServices.Authorizer.Authorize(Permissions.ManageForums, forum, T("Not allowed to edit forum")))
                return new HttpUnauthorizedResult();

            dynamic model = _contentManager.UpdateEditor(forum, this);
            if (!ModelState.IsValid) {
                _orchardServices.TransactionManager.Cancel();
                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return View((object)model);
            }

            _contentManager.Publish(forum.ContentItem);
            _orchardServices.Notifier.Information(T("Forum information updated"));

            return Redirect(Url.ForumsForAdmin());
        }

        [HttpPost, ActionName("Edit")]
        [InternalFormValueRequired("submit.Delete")]
        public ActionResult EditDeletePOST(int forumId) {
            return Remove(forumId);
        }

        [HttpPost]
        public ActionResult Remove(int forumId) {
            var forum = _forumService.Get(forumId, VersionOptions.Latest);

            if (forum == null)
                return HttpNotFound();

            if (!_orchardServices.Authorizer.Authorize(Permissions.ManageForums, forum, T("Not allowed to edit forum")))
                return new HttpUnauthorizedResult();

            _forumService.Delete(forum);

            _orchardServices.Notifier.Information(T("Forum was successfully deleted"));
            return Redirect(Url.ForumsForAdmin());
        }

        public ActionResult List() {
            var list = _orchardServices.New.List();
            list.AddRange(_forumService.Get(VersionOptions.Latest)
                              .Select(b => {
                                  var forum = _contentManager.BuildDisplay(b, "SummaryAdmin");
                                  forum.TotalPostCount = _threadService.Get(b, VersionOptions.Latest).Count();
                                  return forum;
                              }));

            dynamic viewModel = _orchardServices.New.ViewModel()
                .ContentItems(list);
            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)viewModel);
        }

        public ActionResult Item(int forumId, PagerParameters pagerParameters) {
            Pager pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);

            ForumPart forum = _forumService.Get(forumId, VersionOptions.Latest);

            if (forum == null)
                return HttpNotFound();

            if (!_orchardServices.Authorizer.Authorize(Permissions.ManageForums, forum, T("Not allowed to view forum")))
                return new HttpUnauthorizedResult();

            var threads = _threadService.Get(forum, pager.GetStartIndex(), pager.PageSize, VersionOptions.Latest).ToArray();
            var threadsShapes = threads.Select(bp => _contentManager.BuildDisplay(bp, "SummaryAdmin")).ToArray();

            dynamic forumShape = _contentManager.BuildDisplay(forum, "DetailAdmin");

            var list = Shape.List();
            list.AddRange(threadsShapes);
            forumShape.Content.Add(Shape.Parts_Forums_Thread_ListAdmin(ContentItems: list), "5");

            var totalItemCount = _threadService.Count(forum, VersionOptions.Latest);
            forumShape.Content.Add(Shape.Pager(pager).TotalItemCount(totalItemCount), "Content:after");

            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)forumShape);
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }

    public class InternalFormValueRequiredAttribute : ActionMethodSelectorAttribute
    {
        private readonly string _submitButtonName;

        public InternalFormValueRequiredAttribute(string submitButtonName)
        {
            _submitButtonName = submitButtonName;
        }

        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            var value = controllerContext.HttpContext.Request.Form[_submitButtonName];
            return !string.IsNullOrEmpty(value);
        }
    }
}