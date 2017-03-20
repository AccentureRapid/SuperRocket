using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;

namespace OShop.Downloads.Models {
    public class DownloadableProductPart : ContentPart<DownloadableProductPartRecord> {
        internal readonly LazyField<ContentItem> _media = new LazyField<ContentItem>();
        public int? MediaId {
            get { return this.Retrieve(x => x.MediaId); }
            set { this.Store(x => x.MediaId, value); }
        }

        public ContentItem Media {
            get { return _media.Value; }
            set { MediaId = (value != null ? (int?)value.Id : null); }
        }
    }
}