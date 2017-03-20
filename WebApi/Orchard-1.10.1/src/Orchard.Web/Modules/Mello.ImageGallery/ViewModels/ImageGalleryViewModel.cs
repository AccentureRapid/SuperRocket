using System.Collections.Generic;
using Mello.ImageGallery.Models;
using Mello.ImageGallery.Models.Plugins;
using Orchard.UI.Navigation;

namespace Mello.ImageGallery.ViewModels {
    public class ImageGalleryViewModel {
        public IEnumerable<ImageGalleryImage> Images { get; set; }

        public ImageGalleryPlugin ImageGalleryPlugin { get; set; }

        public int ThumbnailWidth { get; set; }

        public int ThumbnailHeight { get; set; }

        public string ImageGalleryName { get; set; }

        public Pager Pager { get; set; }

        public int TotalItemsCount { get; set; }
        public dynamic PagerShape { get; set; }
    }
}