using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using Datwendo.Localization.Models;

namespace Datwendo.Localization.Drivers
{
    public class UserCultureSettingsPartDriver : ContentPartDriver<UserCultureSettingsPart> {
        private readonly ISignals _signals;
        private const string TemplateName = "Parts/UserCultureSettings";

        public UserCultureSettingsPartDriver(ISignals signals) {
            _signals = signals;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        protected override string Prefix {
            get { return "UserCultureSettings"; }
        }

        protected override DriverResult Editor(UserCultureSettingsPart part, dynamic shapeHelper) {
            return ContentShape("Parts_UserCultureSettings_Edit",
                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: part, Prefix: Prefix))
                .OnGroup("Localization");
        }

        protected override DriverResult Editor(UserCultureSettingsPart part, IUpdateModel updater, dynamic shapeHelper) {
            if (updater.TryUpdateModel(part, Prefix, null, null)) {
                _signals.Trigger(UserCultureSettingsPart.CacheKey);
            }
            return Editor(part, shapeHelper);
        }

        protected override void Importing(UserCultureSettingsPart part, ImportContentContext context) 
        {
            var elementName = part.PartDefinition.Name;
            part.Enabled = bool.Parse(context.Attribute(elementName, "Enabled") ?? "false");
            part.Priority = int.Parse(context.Attribute(elementName, "Priority") ?? "0");
            _signals.Trigger(UserCultureSettingsPart.CacheKey);
        }

        protected override void Exporting(UserCultureSettingsPart part, ExportContentContext context) 
        {
            var el = context.Element(part.PartDefinition.Name);
            el.SetAttributeValue("Enabled", part.Enabled);
            el.SetAttributeValue("Priority", part.Priority.ToString());
        }
    }
}