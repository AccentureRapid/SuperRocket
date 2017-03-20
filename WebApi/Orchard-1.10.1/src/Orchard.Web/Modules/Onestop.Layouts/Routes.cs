using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Routes;

namespace Onestop.Layouts {
    [OrchardFeature("Onestop.Layouts")]
    public class Routes : IRouteProvider {
        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes()) {
                routes.Add(routeDescriptor);
            }
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                new RouteDescriptor {
                    Route = new Route(
                        "Admin/OnestopLayout/{action}",
                        new RouteValueDictionary {
                            {"area", "Onestop.Layouts"},
                            {"controller", "LayoutAdmin"},
                            {"action", "List"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Onestop.Layouts"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "Admin/OnestopTemplate/{action}",
                        new RouteValueDictionary {
                            {"area", "Onestop.Layouts"},
                            {"controller", "TemplateAdmin"},
                            {"action", "List"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Onestop.Layouts"}
                        },
                        new MvcRouteHandler())
                }
            };
        }
    }
}
