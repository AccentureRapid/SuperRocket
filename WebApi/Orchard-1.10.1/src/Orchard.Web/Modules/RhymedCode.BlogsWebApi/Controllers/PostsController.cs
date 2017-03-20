using Orchard;
using Orchard.Blogs.Models;
using Orchard.Blogs.Services;
using Orchard.Comments.Models;
using Orchard.Comments.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Localization;
using Orchard.Settings;
using Orchard.UI.Navigation;
using RhymedCode.BlogsWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace RhymedCode.BlogsWebApi.Controllers
{
    [JsonCamelCasePropertyNames]
    public class PostsController : ApiController
    {
        private readonly IOrchardServices _services;
        private readonly IBlogService _blogService;
        private readonly IBlogPostService _blogPostService;
        private readonly ISiteService _siteService;
        private readonly ICommentService _commentService;

        public PostsController(
            IOrchardServices services,
            IBlogService blogService,
            //IShapeFactory shapeFactory,
            IBlogPostService blogPostService,
            ISiteService siteService,
            ICommentService commentService)
        {
            _services = services;
            _blogService = blogService;
            //Shape = shapeFactory;
            _blogPostService = blogPostService;
            _siteService = siteService;
            _commentService = commentService;
        }

        [HttpGet]
        public IHttpActionResult Comments(int postId)
        {
            var comments = _commentService
                            .GetCommentsForCommentedContent(postId)
                            .Where(x => x.Status == CommentStatus.Approved)
                            .OrderBy(x => x.Position)
                            .List()
                            .ToList()
                            .Select(x => new Comment
                            {
                                Id = x.Id,
                                Author = x.Author,
                                CommentDateUtc = x.CommentDateUtc,
                                CommentText = x.CommentText
                            });

            return Ok(comments);
        }

        [HttpGet]
        public IHttpActionResult GetPost(int id)
        {
            var blogPostPart = _blogPostService.Get(id, VersionOptions.Published);
            var post = new BlogPost
            {
                Id = blogPostPart.Id,
                Text = ConvertRelativePathsToAbsolute(blogPostPart.Text, Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority + "/"),
                Title = blogPostPart.Title,
                CreatedBy = blogPostPart.Creator.UserName,
                PublishedUtc = blogPostPart.PublishedUtc
            };
            return Ok(post);
        }

        // POST: api/Posts
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Posts/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Posts/5
        public void Delete(int id)
        {
        }

        private string ConvertRelativePathsToAbsolute(string text, string absoluteUrl)
        {
            string value = Regex.Replace(text, "<(.*?)(src|href)=\"(?!http)(.*?)\"(.*?)>", "<$1$2=\"" + absoluteUrl + "$3\"$4>",
                                         RegexOptions.IgnoreCase | RegexOptions.Multiline);

            // Now just make sure that there isn't a // because if
            // the original relative path started with a / then the
            // replacement above would create a //.

            return value.Replace(absoluteUrl + "/", absoluteUrl);
        }
    }
}
