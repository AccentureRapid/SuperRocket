using System.Linq;
using System.Web.Mvc;
using Onestop.Layouts.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Contents.ViewModels;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Admin;

namespace Onestop.Layouts.Controllers {
    [OrchardFeature("Onestop.Layouts")]
    [ValidateInput(false), Admin]
    public class LayoutAdminController : Controller {
        private readonly IContentManager _contentManager;
        private readonly ILayoutService _layoutService;

        public LayoutAdminController(
            IOrchardServices services,
            IContentManager contentManager,
            ILayoutService layoutService,
            IShapeFactory shapeFactory) {
            Services = services;
            _contentManager = contentManager;
            _layoutService = layoutService;
            T = NullLocalizer.Instance;
            Shape = shapeFactory;
        }

        dynamic Shape { get; set; }
        public Localizer T { get; set; }
        public IOrchardServices Services { get; set; }

        public ActionResult List(ListContentsViewModel model) {
            if (!Services.Authorizer.Authorize(Permissions.ManageLayouts, T("Not allowed to manage layouts.")))
                return new HttpUnauthorizedResult();

            var contentItems = _layoutService.GetLayouts(model.Options.OrderBy);

            var list = Shape.List();
            list.AddRange(contentItems.Select(ci => _contentManager.BuildDisplay(ci, "SummaryAdmin")));

            dynamic viewModel = Shape.ViewModel(
                ContentItems: list,
                Options: model.Options);

            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)viewModel);
        }
    }
}
