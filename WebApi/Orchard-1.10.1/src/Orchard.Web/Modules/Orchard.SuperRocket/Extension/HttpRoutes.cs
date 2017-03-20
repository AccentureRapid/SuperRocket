using System.Collections.Generic;
using Orchard.Mvc.Routes;
using Orchard.WebApi.Routes;
using System.Web.Http;

namespace Orchard.SuperRocket.Extension
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
                Name = "Orchard.SuperRocket.Api",
                Priority = -10,
                RouteTemplate = "api/{controller}/{action}/{id}",
                Defaults = new {
                    area = "Orchard.SuperRocket",
                    id = RouteParameter.Optional
                },
            }
        };
        }
    }
}
