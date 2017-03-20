
namespace Orchard.OData
{
    using Mvc.Routes;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel.Activation;
    using System.Web.Mvc;
    using System.Web.Routing;

    public sealed class RouteProvider : IRouteProvider
    {
        private static readonly RouteBase ODataRoute = new ServiceRoute("odata", new ODataServiceFactory(), typeof(IODataService));

        private readonly IPivotRouteHandler _pivotRouteHandler;
        public RouteProvider(IPivotRouteHandler pivotRouteHandler)
        {
            this._pivotRouteHandler = pivotRouteHandler;
        }

        IEnumerable<RouteDescriptor> IRouteProvider.GetRoutes()
        {
            var namespaces = typeof(RouteProvider).FullName.Split('.').AsEnumerable();
            namespaces = namespaces.Except(new string[] { typeof(RouteProvider).Name });

            var odata = namespaces.Last();
            var route = string.Join("-", namespaces);
            var area = string.Join(".", namespaces);

            return new List<RouteDescriptor>() {
                new RouteDescriptor { Priority = 9999, Route = new Route(odata + "/$collection/{collection}.cxml", this._pivotRouteHandler) },
                new RouteDescriptor { Priority = 9998, Route = new Route(odata + "/$pivot/$collection/{collection}.cxml", this._pivotRouteHandler) },
                new RouteDescriptor { Priority = 9997, Route = new Route(odata + "/$collection/{deepZoomCollection}.dzc", this._pivotRouteHandler) },
                new RouteDescriptor { Priority = 9996, Route = new Route(odata + "/$pivot/$collection/{deepZoomCollection}.dzc", this._pivotRouteHandler) },
                new RouteDescriptor { Priority = 9995, Route = new Route(odata + "/$collection/{deepZoomCollection}/{deepZoomIdentifier}_files/{deepZoomPacket}/{deepZoomImage}.jpg", this._pivotRouteHandler) },
                new RouteDescriptor { Priority = 9994, Route = new Route(odata + "/$pivot/$collection/{deepZoomCollection}/{deepZoomIdentifier}_files/{deepZoomPacket}/{deepZoomImage}.jpg", this._pivotRouteHandler) },
                new RouteDescriptor { Priority = 9993, Route = new Route(odata + "/$collection/{imageRootIdentifier}/dzi/{deepZoomIdentifier}_files/{deepZoomPacket}/{deepZoomImage}.jpg", this._pivotRouteHandler) },
                new RouteDescriptor { Priority = 9992, Route = new Route(odata + "/$pivot/$collection/{imageRootIdentifier}/dzi/{deepZoomIdentifier}_files/{deepZoomPacket}/{deepZoomImage}.jpg", this._pivotRouteHandler) },
                new RouteDescriptor { Priority = 9991, Route = new Route(odata + "/$collection/{imageRootIdentifier}/dzi/{deepZoomIdentifier}.dzi", this._pivotRouteHandler) },
                new RouteDescriptor { Priority = 9990, Route = new Route(odata + "/$pivot/$collection/{imageRootIdentifier}/dzi/{deepZoomIdentifier}.dzi", this._pivotRouteHandler) },

                new RouteDescriptor {
                    Priority = 9989,
                    Route = new Route(
                        odata + "/$pivot/{contentType}",
                        new RouteValueDictionary {
                            {"area", area},
                            {"controller", "Pivot"},
                            {"action", "Index"},
                            {"contentType", "page"},
                        },
                        null,
                        new RouteValueDictionary {
                            {"area", area}
                        },
                        new MvcRouteHandler())
                },

                new RouteDescriptor {
                    Priority = 9988,
                    Route = new Route(
                        odata + "/images/{fileName}",
                        new RouteValueDictionary {
                            {"area", area},
                            {"controller", "Pivot"},
                            {"action", "Resource"},
                        },
                        null,
                        new RouteValueDictionary {
                            {"area", area}
                        },
                        new MvcRouteHandler())
                },

                new RouteDescriptor {
                    Priority = 9987,
                    Route = new Route(
                        odata + "/$pivot/images/{fileName}",
                        new RouteValueDictionary {
                            {"area", area},
                            {"controller", "Pivot"},
                            {"action", "Resource"},
                        },
                        null,
                        new RouteValueDictionary {
                            {"area", area}
                        },
                        new MvcRouteHandler())
                },

                new RouteDescriptor {
                    Priority = 9986,
                    Route = RouteProvider.ODataRoute,
                }
            };
        }

        void IRouteProvider.GetRoutes(ICollection<RouteDescriptor> routes)
        {
            (this as IRouteProvider).GetRoutes().ToList().ForEach(routes.Add);
        }
    }
}