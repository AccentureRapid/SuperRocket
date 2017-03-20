using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using PlanetTelex.ContactForm.Models;

namespace PlanetTelex.ContactForm.Handlers
{
    public class ContactFormSettingsPartHandler : ContentHandler
    {
        public ContactFormSettingsPartHandler()
        {
            T = NullLocalizer.Instance;
            Filters.Add(new ActivatingFilter<ContactFormSettingsPart>("Site"));

            OnInitializing<ContactFormSettingsPart>((context, part) => { });
        }

        public Localizer T { get; set; }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) 
        {
            if (context.ContentItem.ContentType != "Site")
                return;
            base.GetItemMetadata(context);
            context.Metadata.EditorGroupInfo.Add(new GroupInfo(T("Contact Form")));
        }
    }
}