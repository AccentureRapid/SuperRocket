using Orchard;
using Orchard.Blogs.Models;
using Orchard.Blogs.Services;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Settings;
using Orchard.UI.Navigation;
using RhymedCode.BlogsWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RhymedCode.BlogsWebApi.Controllers
{
    [JsonCamelCasePropertyNames]
    public class BlogsController : ApiController
    {
        private readonly IOrchardServices _services;
        private readonly ISiteService _siteService;
        private readonly IBlogService _blogService;
        private readonly IBlogPostService _blogPostService;

        public BlogsController(
            IOrchardServices services, 
            ISiteService siteService, 
            IBlogService blogService,
            IBlogPostService blogPostService)
        {
            _services = services;
            _siteService = siteService;
            _blogService = blogService;
            _blogPostService = blogPostService;
        }

        [HttpGet]
        public IHttpActionResult Get()
        {
            var blogs = _blogService.Get(VersionOptions.Published)
                .Select(x => new Blog
                {
                    Id = x.Id,
                    Title = x.Name,
                    Description = x.Description,
                    PostCount = x.PostCount
                });

            return Ok(blogs);
        }

        [HttpGet]
        public IHttpActionResult Get(int blogId)
        {
            var blogPart = _blogService.Get(blogId, VersionOptions.Published).As<BlogPart>();
            if (blogPart == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            if (!_services.Authorizer.Authorize(Orchard.Core.Contents.Permissions.ViewContent, blogPart, NullLocalizer.Instance("Cannot view content")))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            var blog = new Blog
            {
                Id = blogPart.Id,
                Title = blogPart.Name,
                Description = blogPart.Description,
                PostCount = blogPart.PostCount
            };

            return Ok(blog);
        }


        [HttpGet]
        public IHttpActionResult Posts(int blogId, [FromUri]PagerParameters pagerParameters)
        {
            Pager pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);

            var blogPart = _blogService.Get(blogId, VersionOptions.Published).As<BlogPart>();
            if (blogPart == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            if (!_services.Authorizer.Authorize(Orchard.Core.Contents.Permissions.ViewContent, blogPart, NullLocalizer.Instance("Cannot view content")))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            var posts = _blogPostService.Get(blogPart, pager.GetStartIndex(), pager.PageSize).Select(bp => new BlogPost
            {
                Id = bp.Id,
                Title = bp.Title,
                CreatedBy = bp.Creator.UserName,
                PublishedUtc = bp.PublishedUtc
            }).ToList();

            return Ok(posts);
        }
    }
}
