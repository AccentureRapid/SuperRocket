using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace SH.Robots {
	public class Routes : IRouteProvider {
		public void GetRoutes(ICollection<RouteDescriptor> routes) {
			foreach (var routeDescriptor in GetRoutes())
				routes.Add(routeDescriptor);
		}

		public IEnumerable<RouteDescriptor> GetRoutes() {
			return new[] {
								new RouteDescriptor {   Priority = 5,
														Route = new Route(
															"robots.txt",
															new RouteValueDictionary {
																						{"area", "SH.Robots"},
																						{"controller", "Robots"},
																						{"action", "Index"}
															},
															new RouteValueDictionary(),
															new RouteValueDictionary {
																						{"area", "SH.Robots"}
															},
															new MvcRouteHandler())
								},
							};
		}
	}
}