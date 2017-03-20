using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using Datwendo.Localization.Models;

namespace Datwendo.Localization.Handlers {
    public class BrowserCultureSettingsPartHandler : ContentHandler {
        public BrowserCultureSettingsPartHandler()
        {
            T = NullLocalizer.Instance;
            Filters.Add(new ActivatingFilter<BrowserCultureSettingsPart>("Site"));
        }

        public Localizer T { get; set; }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            if (context.ContentItem.ContentType != "Site")
                return;
            base.GetItemMetadata(context);
            context.Metadata.EditorGroupInfo.Add(new GroupInfo(T("Localization")) {
                Id = "Localization",
                Position = "3"
            });
        }
    }
}