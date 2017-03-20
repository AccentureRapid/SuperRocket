using Onestop.Layouts.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace Onestop.Layouts.Handlers {
    [OrchardFeature("Onestop.Layouts")]
    public class LayoutSettingsHandler : ContentHandler {
        public Localizer T { get; set; }

        public LayoutSettingsHandler() {
            Filters.Add(new ActivatingFilter<LayoutSettingsPart>("Site"));
            T = NullLocalizer.Instance;
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            if (context.ContentItem.ContentType != "Site") return;
            context.Metadata.EditorGroupInfo.Add(new GroupInfo(T("Layouts")) {
                Position = "20"
            });
        }
    }
}