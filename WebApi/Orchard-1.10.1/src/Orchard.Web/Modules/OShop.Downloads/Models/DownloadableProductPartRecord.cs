using Orchard.ContentManagement.Records;

namespace OShop.Downloads.Models {
    public class DownloadableProductPartRecord : ContentPartRecord {
        public virtual int? MediaId { get; set; }
    }
}