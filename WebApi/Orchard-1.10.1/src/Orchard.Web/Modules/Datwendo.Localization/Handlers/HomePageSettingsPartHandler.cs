using Datwendo.Localization.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace Datwendo.Localization.Handlers 
{
    [OrchardFeature("Datwendo.Localization")]
    public class HomePageSettingsPartHandler : ContentHandler {
        public HomePageSettingsPartHandler()
        {
            T = NullLocalizer.Instance;
            Filters.Add(new ActivatingFilter<HomePageSettingsPart>("Site"));
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