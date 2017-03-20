using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.Settings;
using Orchard.Themes;
using Orchard.UI.Navigation;
using Orchard.Logging;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Helpers;
using Newtonsoft.Json.Linq;
using Orchard.DoTNetX.Core;
using Orchard.DoTNetX.Models;
using Orchard.DoTNetX.Interfaces;
using Orchard.DoTNetX.ServiceModels;
using Orchard.DoTNetX.ViewModels;

namespace Orchard.DoTNetX.Controllers
{
    [Themed]
    public class BlogPostController : Controller
    {
        private readonly IBlogPostService _BlogPostService;
        private readonly IContentManager _contentManager;
        private readonly ITransactionManager _transactionManager;
        private readonly ISiteService _siteService;

        public BlogPostController(
            IOrchardServices services,
            IBlogPostService BlogPostService,
            IContentManager contentManager,
            ITransactionManager transactionManager,
            ISiteService siteService,
            IShapeFactory shapeFactory)
        {
            Services = services;
            _BlogPostService = BlogPostService;
            _contentManager = contentManager;
            _transactionManager = transactionManager;
            _siteService = siteService;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;
        }


        dynamic Shape { get; set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }
        public IOrchardServices Services { get; set; }

        protected ActionResult ShapePartialResult(dynamic shape)
        {
            return new ShapePartialResult(this, shape);
        }

        protected JsonResult Jsonp(object data, JsonRequestBehavior behavior = JsonRequestBehavior.AllowGet)
        {
            if (this.IsIE() && this.AcceptText())
            {
                return Json(data, "text/html", behavior);
            }
            else
            {
                return Json(data, behavior);
            }
        }

        private ActionResult SuccessResult()
        {
            string p = Request.Unvalidated()["p"];
            if (p == "json")
            {
                return Jsonp("OK");
            }
            else
            {
                string returnUrl = Request.Unvalidated()["returnUrl"];
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return Content("OK");
                }
            }
        }

        private BlogPostViewModel GetViewModel(BlogPostViewModel viewModel, PagerParameters pagerParameters)
        {
            var entities = _BlogPostService.GetBlogPostList(viewModel.Search, viewModel.Sort, true);
            int totalItemCount = 0;
            if (entities != null)
            {
                totalItemCount = entities.Count();
            }

            Pager pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);

            var model = new BlogPostViewModel
            {
                Search = viewModel.Search,
                Sort = viewModel.Sort ?? new BlogPostSort(),
                Entities = entities != null ? entities.Skip(pager.GetStartIndex()).Take(pager.PageSize) : null,
                Pager = Shape.Pager(pager)
                    .TotalItemCount(totalItemCount)
                    .RouteData(new RouteValueDictionary  
                        {
                            {"action", "Table"}
                        }),
            };
            model.Table = Shape.DisplayTemplate(TemplateName: "BlogPost/Table", Model: model);
            model.SearchDialog = Shape.EditorTemplate(TemplateName: "BlogPost/Search", Model: model);

            return model;
        }

        private dynamic GetEditor(BlogPostModel model)
        {
            return Shape.EditorTemplate(TemplateName: "BlogPost/Edit", Model: model);
        }

        public ActionResult Table(BlogPostViewModel viewModel, PagerParameters pagerParameters)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageBlogPost, T("Not allowed to manage BlogPost")))
                return new HttpUnauthorizedResult();

            var model = GetViewModel(viewModel, pagerParameters);
            return ShapePartialResult(model.Table);
        }

        public ActionResult List(BlogPostViewModel viewModel, PagerParameters pagerParameters)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageBlogPost, T("Not allowed to manage BlogPost")))
                return new HttpUnauthorizedResult();

            var model = GetViewModel(viewModel, pagerParameters);
            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageBlogPost, T("Not allowed to manage BlogPost")))
                return new HttpUnauthorizedResult();

            BlogPostModel model = new BlogPostModel();
            return ShapePartialResult(GetEditor(model));
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageBlogPost, T("Not allowed to manage BlogPost")))
                return new HttpUnauthorizedResult();

            BlogPostModel model = _BlogPostService.GetBlogPost(id);
            return ShapePartialResult(GetEditor(model));
        }


        [HttpPost]
        public ActionResult Update(BlogPostModel model, HttpPostedFileBase Photo1File)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageBlogPost, T("Not allowed to manage BlogPost")))
                return new HttpUnauthorizedResult();

            if (!ModelState.IsValid)
            {
                return ShapePartialResult(GetEditor(model));
            }
            else
            {
                try
                {
                    int result = 0;

                    if (model.Id <= 0)
                    {
                        result = _BlogPostService.Create(model);
                    }
                    else
                    {
                        result = _BlogPostService.Update(model);
                    }

                    if (result > 0)
                    {
                        return SuccessResult();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "BlogPost Update failed.");
                }
            }

            return ShapePartialResult(GetEditor(model));
        }

        public ActionResult Delete(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageBlogPost, T("Not allowed to manage BlogPost")))
                return new HttpUnauthorizedResult();

            int result = _BlogPostService.Delete(id);
            if (result > 0)
            {
                return SuccessResult();
            }
            else
            {
                return new EmptyResult();
            }
        }
    }
}