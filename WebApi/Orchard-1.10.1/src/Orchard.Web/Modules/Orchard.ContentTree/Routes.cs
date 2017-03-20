using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Mvc.Routes;
using System.Web.Routing;
using System.Web.Mvc;

namespace Orchard.ContentTree
{
    public class Routes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
            {
                routes.Add(routeDescriptor);
            }
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            string areaName = "Orchard.ContentTree";

            var emptyConstraints = new RouteValueDictionary();
            var sliderRouteValueDictionary = new RouteValueDictionary { { "area", areaName } };
            var mvcRouteHandler = new MvcRouteHandler();

            return new[] {
                new RouteDescriptor {
                    Priority = 0,
                    Route = new Route(
                        "Admin/ContentTree",
                        new RouteValueDictionary {
                            {"area", areaName},
                            {"action", "ContentTree"},
                            {"controller", "Admin"},
                        },
                        emptyConstraints, sliderRouteValueDictionary, mvcRouteHandler)
                },
                new RouteDescriptor {
                    Priority = 0,
                    Route = new Route(
                        "Admin/ContentTreeAsync",
                        new RouteValueDictionary {
                            {"area", areaName},
                            {"action", "ContentTreeAsync"},
                            {"controller", "Admin"},
                        },
                        emptyConstraints, sliderRouteValueDictionary, mvcRouteHandler)
                },
                new RouteDescriptor {
                    Route = new Route(
                        "Admin/ContentTree/SaveActions",
                        new RouteValueDictionary {
                            {"area", areaName},
                            {"action", "SaveActions"},
                            {"controller", "Admin"},
                        },
                        emptyConstraints, sliderRouteValueDictionary, mvcRouteHandler)
                }
            };
        }
    }
}