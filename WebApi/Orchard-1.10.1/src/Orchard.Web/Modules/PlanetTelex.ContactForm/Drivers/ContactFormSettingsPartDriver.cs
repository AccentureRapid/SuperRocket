using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Localization;
using PlanetTelex.ContactForm.Models;

namespace PlanetTelex.ContactForm.Drivers
{
    public class ContactFormSettingsPartDriver : ContentPartDriver<ContactFormSettingsPart> 
    {
        public ContactFormSettingsPartDriver() 
        {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        protected override string Prefix { get { return "ContactFormSettings"; } }

        protected override DriverResult Editor(ContactFormSettingsPart part, dynamic shapeHelper)
        {
            return Editor(part, null, shapeHelper);
        }

        protected override DriverResult Editor(ContactFormSettingsPart part, IUpdateModel updater, dynamic shapeHelper)
        {

            return ContentShape("Parts_ContactForm_SiteSettings", () =>
            {
                if (updater != null)
                {
                    updater.TryUpdateModel(part, Prefix, null, null);
                }
                return shapeHelper.EditorTemplate(TemplateName: "Parts.ContactForm.SiteSettings", Model: part, Prefix: Prefix);
            }).OnGroup("Contact Form");
        }
    }
}