using Orchard.ContentManagement;

namespace PlanetTelex.ContactForm.Models
{
    public class ContactFormSettingsPart : ContentPart 
    {
        public bool EnableSpamProtection
        {
            get { return this.Retrieve(r => r.EnableSpamProtection); }
            set { this.Store(r => r.EnableSpamProtection, value); }
        }

        public bool EnableSpamEmail
        {
            get { return this.Retrieve(r => r.EnableSpamEmail); }
            set { this.Store(r => r.EnableSpamEmail, value); }
        }

        public string SpamEmail 
        {
            get { return this.Retrieve(r => r.SpamEmail); }
            set { this.Store(r => r.SpamEmail, value); }
        }
    }
}