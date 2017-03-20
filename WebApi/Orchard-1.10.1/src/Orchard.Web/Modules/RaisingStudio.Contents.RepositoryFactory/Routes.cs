using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace RaisingStudio.Contents.RepositoryFactory
{
    public class Routes : IRouteProvider
    {
        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[]
            {
                new RouteDescriptor
                {
                    Route = new Route("MyRoute/{controller}/{action}",
                        new RouteValueDictionary
                        {
                            {"area", "<module name>"},
                            {"controller", "<default controller>"},
                            {"action", "<default action>"}
                        }, new RouteValueDictionary(),
                        new RouteValueDictionary {{"area", "<module name>"}},
                        new MvcRouteHandler())
                }
            };
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (RouteDescriptor route in GetRoutes())
            {
                //uncomment this line to use the routes in defined in GetRoutes()
                //routes.Add(route);
            }
        }
    }
}