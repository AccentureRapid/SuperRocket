using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using OShop.Downloads.Models;

namespace OShop.Downloads.Handlers {
    public class DownloadableProductPartHandler : ContentHandler {
        public DownloadableProductPartHandler(
            IRepository<DownloadableProductPartRecord> repository,
            IContentManager contentManager) {
            Filters.Add(StorageFilter.For(repository));

            OnActivated<DownloadableProductPart>((context, part) => {
                part._media.Loader(media => part.MediaId.HasValue ? contentManager.Get(part.MediaId.Value) : null);
            });
        }
    }
}