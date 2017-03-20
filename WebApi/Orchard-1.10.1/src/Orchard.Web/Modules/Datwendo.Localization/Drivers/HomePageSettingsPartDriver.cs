using Datwendo.Localization.Models;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace Datwendo.Localization.Drivers
{
    [OrchardFeature("Datwendo.Localization")]
    public class HomePageSettingsPartDriver : ContentPartDriver<HomePageSettingsPart> {
        private readonly ISignals _signals;
        private const string TemplateName = "Parts/HomePageSettings";

        public HomePageSettingsPartDriver(ISignals signals) {
            _signals = signals;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        protected override string Prefix {
            get { return "HomePageSettings"; }
        }

        protected override DriverResult Editor(HomePageSettingsPart part, dynamic shapeHelper) {
            return ContentShape("Parts_HomePageSettings_Edit",
                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: part, Prefix: Prefix))
                .OnGroup("Localization");
        }

        protected override DriverResult Editor(HomePageSettingsPart part, IUpdateModel updater, dynamic shapeHelper) {
            if (updater.TryUpdateModel(part, Prefix, null, null)) {
                _signals.Trigger(HomePageSettingsPart.CacheKey);
            }
            return Editor(part, shapeHelper);
        }

        protected override void Importing(HomePageSettingsPart part, ImportContentContext context) {
            var elementName = part.PartDefinition.Name;
            part.Enabled = bool.Parse(context.Attribute(elementName, "Enabled") ?? "false");
            part.FallBackRegex = context.Attribute(elementName, "FallBackRegex");
            part.FallBackMode = (CultureFallbackMode)int.Parse(context.Attribute(elementName, "FallBackMode") ?? "0");
            _signals.Trigger(HomePageSettingsPart.CacheKey);
        }

        protected override void Exporting(HomePageSettingsPart part, ExportContentContext context) {
            var el = context.Element(part.PartDefinition.Name);
            el.SetAttributeValue("Enabled", part.Enabled);
            el.SetAttributeValue("FallBackRegex", part.FallBackRegex);
            el.SetAttributeValue("FallBackMode", ((int)part.FallBackMode).ToString());
        }
    }
}