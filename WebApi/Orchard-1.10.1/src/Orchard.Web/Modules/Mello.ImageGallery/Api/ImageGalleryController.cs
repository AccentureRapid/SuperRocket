using Mello.ImageGallery.Common;
using Mello.ImageGallery.Models.Plugins;
using Mello.ImageGallery.Services;
using Mello.ImageGallery.ViewModels;
using Newtonsoft.Json;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Settings;
using Orchard.UI.Navigation;
using Orchard.UI.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Mello.ImageGallery.Api
{

    public class ImageGalleryController : ApiController
    {
        
        private static readonly char[] separator = new[] { '{', '}', ',' };
        private readonly IContentManager _contentManager;
        private readonly ISiteService _siteService;
        private readonly IImageGalleryService _imageGalleryService;
        private readonly IWorkContextAccessor _workContextAccessor;
        public ImageGalleryController(
            IContentManager contentManager,
            ISiteService siteService,
            IImageGalleryService imageGalleryService,
            IWorkContextAccessor workContextAccessor
            )
        {
            _contentManager = contentManager;
            _siteService = siteService;
            _imageGalleryService = imageGalleryService;
            _workContextAccessor = workContextAccessor;

            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        /// <summary>
        /// GET Event/GetContentTypeDefinition
        /// example: http://localhost/api/ImageGallery/GetContentTypeDefinition?type=User
        /// </summary>
        /// <param name="type">Content Type Name</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetContentTypeDefinition")]
        public ContentTypeDefinition GetContentTypeDefinition(string type)
        {
            return (from t in _contentManager.GetContentTypeDefinitions()
                    where t.Name == type
                    select t).FirstOrDefault();
        }

        /// <summary>
        /// GET GetImages
        /// example: http://localhost/api/ImageGallery/ImageGallery/GetImages?galleryName=SuperGallery&page=1 
        /// </summary>
        /// <param name="galleryName"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetImages")]
        public HttpResponseMessage GetImages(string galleryName,int page = 1)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                PagerParameters pagerParameters = new PagerParameters();
                pagerParameters.PageSize = _siteService.GetSiteSettings().PageSize;
                pagerParameters.Page = page;
                Pager pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);

                Models.ImageGallery imageGallery = _imageGalleryService.GetImageGalleryPaged(galleryName, pagerParameters);

                if (imageGallery == null)
                {
                    return null;
                }

                PluginFactory pluginFactory = PluginFactory.GetFactory(Plugin.PrettyPhoto);

                RegisterStaticContent(pluginFactory.PluginResourceDescriptor);

                ImageGalleryViewModel viewModel = new ImageGalleryViewModel { ImageGalleryPlugin = pluginFactory.Plugin };
                viewModel.ImageGalleryName = imageGallery.Name;
                viewModel.Images = imageGallery.Images;
                viewModel.Pager = pager;
                viewModel.TotalItemsCount = imageGallery.TotalItemsCount;

                response.Content = Serialize(viewModel, response);
            }
            catch (Exception ex)
            {
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                Logger.Error(string.Format("Error occurs when GetImages : {0}", ex.StackTrace));
            }

            return response;
        }

        private void RegisterStaticContent(PluginResourceDescriptor pluginResourceDescriptor)
        {
            IResourceManager resourceManager = _workContextAccessor.GetContext().Resolve<IResourceManager>();

            var links = resourceManager.GetRegisteredLinks();
            bool isIncluded = links.Any(link => link.Href.Contains("imagegallery")); // not yet added scripts and styles

            if (!isIncluded)
            { // if not added any styles or scripts, then add          
                foreach (string script in pluginResourceDescriptor.Scripts)
                {
                    resourceManager.RegisterHeadScript(script);
                }

                foreach (LinkEntry style in pluginResourceDescriptor.Styles)
                {
                    resourceManager.RegisterLink(style);
                }
            }

            resourceManager.Require("script", "jQuery").AtHead();
        }
        private StringContent Serialize(dynamic source, HttpResponseMessage response)
        {
            if (source == null)
            {
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
            }
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new NullToEmptyStringResolver(),
                PreserveReferencesHandling = PreserveReferencesHandling.None
            };

            var stringcontent = JsonConvert.SerializeObject(source, Newtonsoft.Json.Formatting.Indented, settings);
            return new StringContent(stringcontent, Encoding.GetEncoding("UTF-8"), "application/json");
        }
    }
}
