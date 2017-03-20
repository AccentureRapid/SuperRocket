using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using OShop.Downloads.Models;
using OShop.Models;
using System.Collections.Generic;

namespace OShop.Downloads.Handlers {
    public class OrderDownloadsPartHandler : ContentHandler {
        public OrderDownloadsPartHandler(IContentManager contentManager) {
            OnActivated<OrderDownloadsPart>((context, part) => {
                part._downloads.Loader(productDownloads => {
                    var orderPart = context.ContentItem.As<OrderPart>();
                    var availableDownloads = new Dictionary<int, DownloadableProductPart>();
                    if (orderPart.OrderStatus >= OrderStatus.Completed) {
                        foreach(var orderDetail in orderPart.Details) {
                            var product = contentManager.Get<DownloadableProductPart>(orderDetail.ContentId);
                            if(product != null && product.Media != null) {
                                availableDownloads.Add(orderDetail.Id, product);
                            }
                        }
                    }
                    return availableDownloads;
                });
            });
        }
    }
}