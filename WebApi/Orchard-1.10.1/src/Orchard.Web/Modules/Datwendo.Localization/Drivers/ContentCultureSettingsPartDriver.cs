using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using Datwendo.Localization.Models;

namespace Datwendo.Localization.Drivers
{
    public class ContentCultureSettingsPartDriver : ContentPartDriver<ContentCultureSettingsPart> {
        private readonly ISignals _signals;
        private const string TemplateName = "Parts/ContentCultureSettings";

        public ContentCultureSettingsPartDriver(ISignals signals) {
            _signals = signals;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        protected override string Prefix {
            get { return "ContentCultureSettings"; }
        }

        protected override DriverResult Editor(ContentCultureSettingsPart part, dynamic shapeHelper) {
            return ContentShape("Parts_ContentCultureSettings_Edit",
                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: part, Prefix: Prefix))
                .OnGroup("Localization");
        }

        protected override DriverResult Editor(ContentCultureSettingsPart part, IUpdateModel updater, dynamic shapeHelper) {
            if (updater.TryUpdateModel(part, Prefix, null, null)) {
                _signals.Trigger(ContentCultureSettingsPart.CacheKey);
            }
            return Editor(part, shapeHelper);
        }

        protected override void Importing(ContentCultureSettingsPart part, ImportContentContext context) 
        {
            var elementName = part.PartDefinition.Name;
            part.Enabled = bool.Parse(context.Attribute(elementName, "Enabled") ?? "false");
            part.Priority = int.Parse(context.Attribute(elementName, "Priority") ?? "0");
            _signals.Trigger(ContentCultureSettingsPart.CacheKey);
        }

        protected override void Exporting(ContentCultureSettingsPart part, ExportContentContext context) 
        {
            var el = context.Element(part.PartDefinition.Name);
            el.SetAttributeValue("Enabled", part.Enabled);
            el.SetAttributeValue("Priority", part.Priority.ToString());
        }
    }
}