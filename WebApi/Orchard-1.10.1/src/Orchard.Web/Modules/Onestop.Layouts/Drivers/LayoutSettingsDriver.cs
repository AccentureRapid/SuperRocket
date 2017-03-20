using Onestop.Layouts.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace Onestop.Layouts.Drivers {
    [OrchardFeature("Onestop.Layouts")]
    public class CommerceSettingsDriver : ContentPartDriver<LayoutSettingsPart> {
        private const string TemplateName = "Parts/LayoutSettings";
        protected override string Prefix { get { return "LayoutSettings"; } }
        public Localizer T { get; set; }

        public CommerceSettingsDriver() {
            T = NullLocalizer.Instance;
        }

        protected override DriverResult Editor(LayoutSettingsPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_CommerceSettingsPart_Edit",
                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: part, Prefix: Prefix))
                    .OnGroup("Layouts");
        }

        protected override DriverResult Editor(LayoutSettingsPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

    }
}