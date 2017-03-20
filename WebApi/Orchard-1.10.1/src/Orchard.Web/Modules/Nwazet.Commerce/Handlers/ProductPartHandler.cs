﻿using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Handlers {
    [OrchardFeature("Nwazet.Commerce")]
    public class ProductPartHandler : ContentHandler {
        public ProductPartHandler(IRepository<ProductPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }

         protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            var part = context.ContentItem.As<ProductPart>();

            if (part != null) {
                context.Metadata.Identity.Add("Sku", part.Sku);
            }
        }
    }
}
