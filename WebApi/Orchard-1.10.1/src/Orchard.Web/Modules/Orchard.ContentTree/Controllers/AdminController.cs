using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Orchard.Autoroute.Models;
using Orchard.ContentManagement;
using Orchard.ContentTree.Models;
using Orchard.ContentTree.Services;
using Orchard.ContentTree.ViewModels;
using Orchard.Core.Title.Models;
using Orchard.DisplayManagement;
using Orchard.Mvc;

namespace Orchard.ContentTree.Controllers {
    public class AdminController : Controller {
        private readonly IOrchardServices _services;
        private readonly IContentTreeService _contentTreeService;
        private readonly IShapeFactory _shapeFactory;

        public AdminController(
            IOrchardServices services, 
            IContentTreeService contentTreeService, 
            IShapeFactory shapeFactory) 
        {
            _services = services;
            _contentTreeService = contentTreeService;
            _shapeFactory = shapeFactory;
        }

        public ActionResult ContentTree() {
            return View();
        }

        public ActionResult ContentTreeAsync() {
            var treeModel = _contentTreeService.BuildTree();

            var viewModel = new ContentTreeViewModel
            {
                Tree = _contentTreeService.Display(treeModel)
            };

            return PartialView("ContentTreePartial", viewModel);
        }

        [HttpGet]
        public ActionResult SaveActions(int id, ContentTreeAction actions) {
            var contentManager = _services.ContentManager;

            var item = contentManager.Get(id, VersionOptions.Latest);

            if (item != null) {
                switch (actions)
                {
                    case ContentTreeAction.Delete: {
                            contentManager.Remove(item);

                            break;
                        }
                    case ContentTreeAction.Unpublish: {
                            contentManager.Unpublish(item);

                            break;
                        }
                    case ContentTreeAction.Publish:
                    case ContentTreeAction.PublishDraft:
                        {
                            contentManager.Publish(item);

                            break;
                        }
                }

                if (Request.IsAjaxRequest()) {
                    var shape = _contentTreeService.Display(_contentTreeService.GetTreeItem(item));
                    var html = _contentTreeService.Render(ControllerContext, TempData, "TreeItem", shape);

                    return Json(new { Id = id, Action = actions.ToString(), ItemHtml = html }, JsonRequestBehavior.AllowGet);
                }
            }

            return RedirectToAction("ContentTree");
        }

        [HttpGet]
        public ActionResult Settings() {
            var settings = _contentTreeService.GetSettings();

            var model = new ContentTreeSettingsViewModel();

            model.SelectedTypes = _contentTreeService.ContentTypeOptions(settings.IncludedTypes).ToArray();

            return View(model);
        }

        [HttpPost, ActionName("Settings")]
        public ActionResult SettingsPost(ContentTreeSettingsViewModel model) {
            var settings = _contentTreeService.GetSettings();

            settings.IncludedTypes = model.SelectedTypes.Where(t => t.Selected).Select(t => t.Value).ToArray();

            model.SelectedTypes = _contentTreeService.ContentTypeOptions(settings.IncludedTypes).ToArray();

            return View("Settings", model);
        }
    }
}