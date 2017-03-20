using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace Contrib.MediaFolder {
    public class Routes : IRouteProvider {

        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                             new RouteDescriptor {
                                                     Priority = 5,
                                                     Route = new Route(
                                                         "Media/{*path}",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Contrib.MediaFolder"},
                                                                                      {"controller", "Home"},
                                                                                      {"action", "Index"}
                                                                                  },
                                                         null,
                                                         new RouteValueDictionary {
                                                                                      {"area", "Contrib.MediaFolder"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 }
                         };
        }
    }
}