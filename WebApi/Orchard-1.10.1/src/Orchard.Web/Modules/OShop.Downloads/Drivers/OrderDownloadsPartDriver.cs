using Orchard.ContentManagement.Drivers;
using OShop.Downloads.Models;

namespace OShop.Downloads.Drivers {
    public class OrderDownloadsPartDriver : ContentPartDriver<OrderDownloadsPart> {
        protected override DriverResult Display(OrderDownloadsPart part, string displayType, dynamic shapeHelper) {
            return ContentShape("Parts_Order_Downloads", () => shapeHelper.Parts_Order_Downloads(
                    ContentPart: part));
        }
    }
}