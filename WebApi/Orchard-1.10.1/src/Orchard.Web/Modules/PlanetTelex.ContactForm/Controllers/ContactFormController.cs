using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.Mvc.Extensions;
using PlanetTelex.ContactForm.Models;
using PlanetTelex.ContactForm.Services;

namespace PlanetTelex.ContactForm.Controllers
{
    /// <summary>
    /// The controller that handles all contact form requests.
    /// </summary>
    public class ContactFormController : Controller
    {
        private readonly IContactFormService _contactFormService;
        private readonly IContentManager _contentManager;

        public ContactFormController(IContactFormService contactFormService, IContentManager contenteManager)
        {
            _contactFormService = contactFormService;
            _contentManager = contenteManager;
        }

        /// <summary>
        /// Sends the contact email.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <param name="name">The name.</param>
        /// <param name="email">The bot false email.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        /// <param name="confirmEmail">The actual email string</param>
        public ActionResult SendContactEmail(int id, string returnUrl, string name, string email, string confirmEmail, string subject, string message) {
            var contactForm = _contentManager.Get<ContactFormPart>(id);

            if (contactForm != null)
            {
                // If a static subject message was specified, use that value for the email subject.
                if (contactForm.UseStaticSubject)
                {
                    if (contactForm.StaticSubjectMessage != null)
                        subject = contactForm.StaticSubjectMessage.Replace("{NAME}", name);
                    if (Request.Url != null)
                        subject = subject.Replace("{DOMAIN}", Request.Url.Host);
                }

                _contactFormService.SendContactEmail(name, confirmEmail, email, subject, message, contactForm.RecipientEmailAddress, contactForm.RequireNameField);
            }

            return this.RedirectLocal(returnUrl, "~/");
        }
    }
}