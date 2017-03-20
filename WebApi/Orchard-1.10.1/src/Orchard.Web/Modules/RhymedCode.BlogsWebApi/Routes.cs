using Orchard.WebApi.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Mvc.Routes;

namespace RhymedCode.BlogsWebApi
{
    public class Routes : IHttpRouteProvider
    {
        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            var blogsRouteDescriptor = new HttpRouteDescriptor
            {
                Priority = -100,
                RouteTemplate = "api/blogs",
                Defaults = new
                {
                    area = "RhymedCode.BlogsWebApi",
                    controller = "Blogs",
                    action = "Get"
                }
            };

            var blogRouteDescriptor = new HttpRouteDescriptor
            {
                Priority = -100,
                RouteTemplate = "api/blogs/{blogId}",
                Defaults = new
                {
                    area = "RhymedCode.BlogsWebApi",
                    controller = "Blogs",
                    action = "Get"
                }
            };

            var blogPostsRouteDescriptor = new HttpRouteDescriptor
            {
                Priority = -100,
                RouteTemplate = "api/blogs/{blogId}/posts",
                Defaults = new
                {
                    area = "RhymedCode.BlogsWebApi",
                    controller = "Blogs",
                    action = "Posts"
                }
            };

            var blogPostRouteDescriptor = new HttpRouteDescriptor
            {
                Priority = -100,
                RouteTemplate = "api/posts/{id}",
                Defaults = new
                {
                    area = "RhymedCode.BlogsWebApi",
                    controller = "Posts",
                    action = "GetPost"
                }
            };

            var blogCommentsRouteDescriptor = new HttpRouteDescriptor
            {
                Priority = -100,
                RouteTemplate = "api/posts/{postId}/comments",
                Defaults = new
                {
                    area = "RhymedCode.BlogsWebApi",
                    controller = "Posts",
                    action = "Comments"
                }
            };

            var commentRouteDescriptor = new HttpRouteDescriptor
            {
                Priority = -100,
                RouteTemplate = "api/comments/{id}",
                Defaults = new
                {
                    area = "RhymedCode.BlogsWebApi",
                    controller = "Comments",
                    action = "Get"
                }
            };

            var latestCommentsRouteDescriptor = new HttpRouteDescriptor
            {
                Priority = -100,
                RouteTemplate = "api/comments/latest",
                Defaults = new
                {
                    area = "RhymedCode.BlogsWebApi",
                    controller = "Comments",
                    action = "Latest"
                }
            };

            return new[] {
                blogsRouteDescriptor,
                blogRouteDescriptor,
                blogPostsRouteDescriptor,
                blogPostRouteDescriptor,
                blogCommentsRouteDescriptor,
                latestCommentsRouteDescriptor,
                commentRouteDescriptor,
            };
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes()) { 
                routes.Add(routeDescriptor);
            }
        }
    }
}