using Orchard.Comments.Models;
using Orchard.Comments.Services;
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
    public class CommentsController : ApiController
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            var commentPart = _commentService.GetComment(id);
            var comment = new Comment
            {
                Id = commentPart.Id,
                Author = commentPart.Author,
                CommentDateUtc = commentPart.CommentDateUtc,
                CommentText = commentPart.CommentText
            };

            return Ok(comment);
        }

        [HttpGet]
        public IHttpActionResult Latest()
        {
            var comments = _commentService.GetComments(CommentStatus.Approved)
                .OrderByDescending(x => x.CommentDateUtc)
                .List()
                .Take(10)
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

        // POST: api/BwaComments
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/BwaComments/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/BwaComments/5
        public void Delete(int id)
        {
        }
    }
}
