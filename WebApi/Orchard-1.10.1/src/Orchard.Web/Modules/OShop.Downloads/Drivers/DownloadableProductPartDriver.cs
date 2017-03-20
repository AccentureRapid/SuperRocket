using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using OShop.Downloads.Models;
using OShop.Models;

namespace OShop.Downloads.Drivers {
    public class DownloadableProductPartDriver : ContentPartDriver<DownloadableProductPart> {
        private const string TemplateName = "Parts/DownloadableProduct";

        protected override string Prefix { get { return "DownloadableProduct"; } }

        // GET
        protected override DriverResult Editor(DownloadableProductPart part, dynamic shapeHelper) {
            return ContentShape("Parts_DownloadableProduct_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: TemplateName,
                    Model: part,
                    Prefix: Prefix)
            );
        }

        // POST
        protected override DriverResult Editor(DownloadableProductPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}