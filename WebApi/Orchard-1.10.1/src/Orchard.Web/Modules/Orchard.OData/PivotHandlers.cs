// Copyright (c) Microsoft Corporation. All rights reserved.

namespace Orchard.OData
{
    using PivotServerTools;
    using System;
    using System.Web;
    using System.Web.Routing;

    public interface IPivotRouteHandler : IRouteHandler, IDependency {
    }
    public interface ICxmlHandler : IHttpHandler, IDependency {
    }

    public interface IDzcHandler : IHttpHandler, IDependency {
    }

    public interface IDziHandler : IHttpHandler, IDependency {
    }

    public interface IImageTileHandler : IHttpHandler, IDependency {
    }

    public interface IDeepZoomImageHandler : IHttpHandler, IDependency {
    }

    public sealed class CxmlRouteHandler : IPivotRouteHandler
    {
        private readonly ICxmlHandler cxmlHandler;
        private readonly IDzcHandler dzcHandler;
        private readonly IDziHandler dziHandler;
        private readonly IImageTileHandler imageTileHandler;
        private readonly IDeepZoomImageHandler deepZoomImageHandler;
        public CxmlRouteHandler(
            ICxmlHandler cxmlHandler,
            IDzcHandler dzcHandler,
            IDziHandler dziHandler,
            IImageTileHandler imageTileHandler,
            IDeepZoomImageHandler deepZoomImageHandler)
        {
            this.cxmlHandler = cxmlHandler;
            this.dzcHandler = dzcHandler;
            this.dziHandler = dziHandler;
            this.imageTileHandler = imageTileHandler;
            this.deepZoomImageHandler = deepZoomImageHandler;
        }

        IHttpHandler IRouteHandler.GetHttpHandler(RequestContext requestContext) 
        {
            if (requestContext.HttpContext.Request.Url.AbsoluteUri.EndsWith("cxml")) {
                return this.cxmlHandler;
            }
            if (requestContext.HttpContext.Request.Url.AbsoluteUri.EndsWith("dzc")) {
                return this.dzcHandler;
            }
            if (requestContext.HttpContext.Request.Url.AbsoluteUri.EndsWith("dzi")) {
                return this.dziHandler;
            }
            if (requestContext.HttpContext.Request.Url.AbsoluteUri.Contains("dzi")){
                return this.deepZoomImageHandler;
            }
            if (requestContext.HttpContext.Request.Url.AbsoluteUri.EndsWith("jpg")) {
                return this.imageTileHandler;
            }
            throw new ArgumentException();
        }
    }

    public sealed class CxmlHandler : ICxmlHandler 
    {
        private readonly IODataPivotCollection oDataPivotCollection;
        public CxmlHandler(IODataPivotCollection oDataPivotCollection) {
            this.oDataPivotCollection = oDataPivotCollection;
        }

        void IHttpHandler.ProcessRequest(HttpContext context) {
            PivotHttpHandlers.ServeCxml(context, this.oDataPivotCollection.MakeCollection(context));
        }

        bool IHttpHandler.IsReusable {
            get { return true; }
        }
    }

    public sealed class DzcHandler : IDzcHandler
    {
        public DzcHandler() {
        }

        public void ProcessRequest(HttpContext context) {
            PivotHttpHandlers.ServeDzc(context);
        }

        public bool IsReusable {
            get { return true; }
        }
    }

    public sealed class DziHandler : IDziHandler
    {
        public DziHandler() {
        }

        public void ProcessRequest(HttpContext context) {
            PivotHttpHandlers.ServeDzi(context);
        }

        public bool IsReusable {
            get { return true; }
        }
    }

    public sealed class ImageTileHandler : IImageTileHandler
    {
        public ImageTileHandler() {
        }

        public void ProcessRequest(HttpContext context) {
            PivotHttpHandlers.ServeImageTile(context);
        }

        public bool IsReusable {
            get { return true; }
        }
    }

    public sealed class DeepZoomImageHandler : IDeepZoomImageHandler
    {
        public DeepZoomImageHandler() {
        }

        public void ProcessRequest(HttpContext context) {
            PivotHttpHandlers.ServeDeepZoomImage(context);
        }

        public bool IsReusable {
            get { return true; }
        }
    }
}
