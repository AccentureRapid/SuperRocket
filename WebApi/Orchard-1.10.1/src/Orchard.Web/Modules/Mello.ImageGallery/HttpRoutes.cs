using System.Collections.Generic;
using Orchard.Mvc.Routes;
using Orchard.WebApi.Routes;
using System.Web.Http;

namespace Mello.ImageGallery
{
    public class HttpRoutes : IHttpRouteProvider
    {

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (RouteDescriptor routeDescriptor in GetRoutes())
            {
                routes.Add(routeDescriptor);
            }
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
            new HttpRouteDescriptor {
                Name = "Mello.ImageGallery.Api",
                Priority = -10,
                RouteTemplate = "api/{controller}/{action}/{id}",
                Defaults = new {
                    area = "Mello.ImageGallery",
                    id = RouteParameter.Optional
                },
            }
        };
        }
    }
}
