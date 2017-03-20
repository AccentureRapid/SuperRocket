using Orchard.ContentManagement;

namespace PlanetTelex.ContactForm.Models
{
    /// <summary>
    /// The content part model, uses the record class for storage.
    /// </summary>
    public class ContactFormPart : ContentPart
    {
        /// <summary>
        /// Gets or sets the recipient email address.
        /// </summary>
        public string RecipientEmailAddress 
        {
            get { return this.Retrieve(r => r.RecipientEmailAddress); }
            set { this.Store(r => r.RecipientEmailAddress, value); }
        }

        /// <summary>
        /// Gets or sets the static subject message.
        /// </summary>
        public string StaticSubjectMessage
        {
            get { return this.Retrieve(r => r.StaticSubjectMessage); }
            set { this.Store(r => r.StaticSubjectMessage, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use the static subject message.
        /// </summary>
        public bool UseStaticSubject
        {
            get { return this.Retrieve(r => r.UseStaticSubject); }
            set { this.Store(r => r.UseStaticSubject, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to display the name field.
        /// </summary>
        public bool DisplayNameField
        {
            get { return this.Retrieve(r => r.DisplayNameField); }
            set { this.Store(r => r.DisplayNameField, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to require the name field.
        /// </summary>
        public bool RequireNameField
        {
            get { return this.Retrieve(r => r.RequireNameField); }
            set { this.Store(r => r.RequireNameField, value); }
        }
    }
}