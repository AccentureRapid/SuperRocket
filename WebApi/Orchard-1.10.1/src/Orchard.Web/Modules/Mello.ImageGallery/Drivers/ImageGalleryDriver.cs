using System.Linq;
using Mello.ImageGallery.Models;
using Mello.ImageGallery.Models.Plugins;
using Mello.ImageGallery.Services;
using Mello.ImageGallery.ViewModels;
using Orchard;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement;
using Orchard.UI.Resources;
using System.Web.Mvc;
using System;
using Orchard.UI.Navigation;
using Orchard.Settings;
using System.Web;
using Orchard.DisplayManagement;

namespace Mello.ImageGallery.Drivers {
    public class ImageGalleryDriver : ContentPartDriver<ImageGalleryPart> {
        private readonly IImageGalleryService _imageGalleryService;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IThumbnailService _thumbnailService;
        private readonly ISiteService _siteService;

        public ImageGalleryDriver(IImageGalleryService imageGalleryService, 
            IThumbnailService thumbnailService, 
            IWorkContextAccessor workContextAccessor,
            ISiteService siteService,
            IShapeFactory shapeFactory
            )
        {
            _thumbnailService = thumbnailService;
            _workContextAccessor = workContextAccessor;
            _imageGalleryService = imageGalleryService;
            _siteService = siteService;
            Shape = shapeFactory;
        }

        dynamic Shape { get; set; }
        private void RegisterStaticContent(PluginResourceDescriptor pluginResourceDescriptor) {
            IResourceManager resourceManager = _workContextAccessor.GetContext().Resolve<IResourceManager>();

            var links = resourceManager.GetRegisteredLinks();
            bool isIncluded = links.Any(link => link.Href.Contains("imagegallery")); // not yet added scripts and styles

            if (!isIncluded){ // if not added any styles or scripts, then add          
                foreach (string script in pluginResourceDescriptor.Scripts) {
                    resourceManager.RegisterHeadScript(script);
                }

                foreach (LinkEntry style in pluginResourceDescriptor.Styles) {
                    resourceManager.RegisterLink(style);
                }
            }

            resourceManager.Require("script", "jQuery").AtHead();
        }

        protected override DriverResult Display(ImageGalleryPart part, string displayType, dynamic shapeHelper) {
	        if (string.Equals(displayType, "SummaryAdmin", StringComparison.OrdinalIgnoreCase) &&
	            string.Equals(displayType, "Summary", StringComparison.OrdinalIgnoreCase)) {
		        // Image gallery returns nothing if in Summary Admin
		        return null;
	        }

	        if (!part.Record.DisplayImageGallery.GetValueOrDefault() || string.IsNullOrWhiteSpace(part.Record.ImageGalleryName)) {
				return null;
			}

            //TODO get image gallery by paging
            int page = HttpContext.Current.Request.QueryString["page"] == null ? 1 : Convert.ToInt32(HttpContext.Current.Request.QueryString["page"]);
            
            PagerParameters pagerParameters = new PagerParameters();
            pagerParameters.PageSize = _siteService.GetSiteSettings().PageSize;
            pagerParameters.Page = page;
            Pager pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);

            Models.ImageGallery imageGallery = _imageGalleryService.GetImageGalleryPaged(part.ImageGalleryName,pagerParameters);

			if (imageGallery == null) {
				return null;
			}

			PluginFactory pluginFactory = PluginFactory.GetFactory(part.SelectedPlugin);

            RegisterStaticContent(pluginFactory.PluginResourceDescriptor);

            ImageGalleryViewModel viewModel = new ImageGalleryViewModel {ImageGalleryPlugin = pluginFactory.Plugin};
            viewModel.ImageGalleryName = imageGallery.Name;
            viewModel.Images = imageGallery.Images;
            viewModel.Pager = pager;
            viewModel.TotalItemsCount = imageGallery.TotalItemsCount;

            // Construct a Pager shape
            var pagerShape = Shape.Pager(pager).TotalItemCount(viewModel.TotalItemsCount);
            viewModel.PagerShape = pagerShape;

            return ContentShape("Parts_ImageGallery",
                    () => shapeHelper.DisplayTemplate(
                        TemplateName: pluginFactory.Plugin.ImageGalleryTemplateName,//"Parts/ImageGallery",
                        Model: viewModel,
                        Prefix: Prefix));

            //return Combined(
            //    ContentShape("Parts_ImageGallery",
            //                    () => shapeHelper.DisplayTemplate(
            //                        TemplateName: pluginFactory.Plugin.ImageGalleryTemplateName,//"Parts/ImageGallery",
            //                        Model: viewModel,
            //                        Prefix: Prefix)),
            //    ContentShape("Pager",
            //        () => shapeHelper.Pager(Pager: pager))
            //    );
        }

        //GET
        protected override DriverResult Editor(ImageGalleryPart part, dynamic shapeHelper) {
            part.AvailableGalleries = _imageGalleryService.GetImageGalleries()
                .OrderBy(o => o.Name).Select(o => new SelectListItem
                                                    {
                                                      Text = o.Name,
                                                      Value = o.Name
                                                    });

            if (!string.IsNullOrWhiteSpace(part.ImageGalleryName)) {
                part.SelectedGallery = part.ImageGalleryName;
            }
            else {
                part.SelectedGallery = part.AvailableGalleries.FirstOrDefault() == null
                                           ? string.Empty
                                           : part.AvailableGalleries.FirstOrDefault().Value;
            }

            part.AvailablePlugins = Enum.GetNames(typeof (Plugin))
                .Select(o => new SelectListItem
                             {
                                 Text = o,
                                 Value = o
                             });

            return ContentShape("Parts_ImageGallery_Edit",
                                () => shapeHelper.EditorTemplate(
                                    TemplateName: "Parts/ImageGallery",
                                    Model: part,
                                    Prefix: Prefix));
        }

        //POST
        protected override DriverResult Editor(ImageGalleryPart part, IUpdateModel updater, dynamic shapeHelper) {
			updater.TryUpdateModel(part, Prefix, null, null);

			part.ImageGalleryName = part.SelectedGallery;

            return Editor(part, shapeHelper);
        }
    }
}