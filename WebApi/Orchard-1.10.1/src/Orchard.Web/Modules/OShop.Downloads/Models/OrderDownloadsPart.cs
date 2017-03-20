using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;
using System.Collections.Generic;

namespace OShop.Downloads.Models {
    public class OrderDownloadsPart : ContentPart {
        internal readonly LazyField<IDictionary<int, DownloadableProductPart>> _downloads = new LazyField<IDictionary<int, DownloadableProductPart>>();

        public IDictionary<int, DownloadableProductPart> Downloads {
            get { return _downloads.Value; }
        }
    }
}