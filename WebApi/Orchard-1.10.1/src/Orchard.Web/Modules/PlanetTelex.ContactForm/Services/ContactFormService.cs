using System;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Email.Models;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Notify;
using PlanetTelex.ContactForm.Models;
using System.Collections.Generic;
using System.Linq;

namespace PlanetTelex.ContactForm.Services
{
    public class ContactFormService : IContactFormService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public ContactFormService(IOrchardServices orchardServices, INotifier notifier)
        {
            _notifier = notifier;
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
        }
        
        #region Implementation of IContactFormService
        /// <summary>
        /// Sends a contact email.
        /// </summary>
        /// <param name="name">The name of the sender.</param>
        /// <param name="email">The email address of the sender.</param>
        /// <param name="spamBotEmail">The email address entered in by spam bot</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="message">The email message.</param>
        /// <param name="sendTo">The email address to send the message to.</param>
        /// <param name="requiredName">Bool of Name is required</param>
        public void SendContactEmail(string name, string email, string spamBotEmail, string subject, string message, string sendTo, bool requiredName)
        {
            if (ValidateContactFields(name, email, message, requiredName))
            {
                if (string.IsNullOrEmpty(name))
                    name = email;

                var smtpSettings = _orchardServices.WorkContext.CurrentSite.As<SmtpSettingsPart>();
                if (smtpSettings != null && smtpSettings.IsValid())
                {
                    var mailClient = BuildSmtpClient(smtpSettings);
                    var contactFormSettings = _orchardServices.WorkContext.CurrentSite.As<ContactFormSettingsPart>();

                    if (!string.IsNullOrEmpty(spamBotEmail))
                    {
                        // This option allows spam to be sent to a separate email address.
                        try
                        {
                            if (contactFormSettings.EnableSpamEmail && ParseRecipients(contactFormSettings.SpamEmail).Any())
                            {
                                var mailMessage = new MailMessage(smtpSettings.Address, ParseRecipients(contactFormSettings.SpamEmail).FirstOrDefault(), subject, name + " writes:\r\n\r\n" + message) { IsBodyHtml = false };

                                if (!String.IsNullOrWhiteSpace(email))
                                {
                                    foreach (var recipient in ParseRecipients(email))
                                    {
                                        mailMessage.ReplyToList.Add(new MailAddress(recipient));
                                    }
                                }
                                mailClient.Send(mailMessage);
                                string spamMessage = string.Format("Your message was flagged as spam. If you feel this was in error contact us directly at: {0}", sendTo);
                                _notifier.Information(T(spamMessage)); 
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e, "An unexpected error while sending a contact form message flagged as spam to {0} at {1}", contactFormSettings.SpamEmail, DateTime.Now.ToLongDateString());
                            string errorMessage = string.Format("Your message was flagged as spam. If you feel this was in error contact us directly at: {0}", sendTo);
                            _notifier.Error(T(errorMessage));
                        }

                    }
                    else
                    {
                        try
                        {
                            var mailMessage = new MailMessage(smtpSettings.Address, sendTo, subject, name + " writes:\r\n\r\n" + message) { IsBodyHtml = false };

                            if (!String.IsNullOrWhiteSpace(email))
                            {
                                foreach (var recipient in ParseRecipients(email))
                                {
                                    mailMessage.ReplyToList.Add(new MailAddress(recipient));
                                }
                            }
                            mailClient.Send(mailMessage);
                            Logger.Debug("Contact form message sent to {0} at {1}", sendTo, DateTime.Now.ToLongDateString());
                            _notifier.Information(T("Thank you for your inquiry, we will respond to you shortly."));
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e, "An unexpected error while sending a contact form message to {0} at {1}", sendTo, DateTime.Now.ToLongDateString());
                            var errorMessage = string.Format("An unexpected error occured when sending your message. You may email us directly at: {0}", sendTo);
                            _notifier.Error(T(errorMessage));
                        }
                    }
                }
                else
                {
                    string errorMessage = string.Format("Our email server isn't configured. You may email us directly at: {0}", sendTo);
                    _notifier.Error(T(errorMessage));
                }
            }
        }

        #endregion
        private IEnumerable<string> ParseRecipients(string recipients)
        {
            return recipients.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
        }
        private SmtpClient BuildSmtpClient(SmtpSettingsPart smtpSettings)
        {
            var mailClient = new SmtpClient {
                Host = smtpSettings.Host, 
                Port = smtpSettings.Port, 
                EnableSsl = smtpSettings.EnableSsl, 
                DeliveryMethod = SmtpDeliveryMethod.Network, 
                UseDefaultCredentials = !smtpSettings.RequireCredentials
            };
            if (!mailClient.UseDefaultCredentials && !String.IsNullOrWhiteSpace(smtpSettings.UserName))
            {
                mailClient.Credentials = new NetworkCredential(smtpSettings.UserName, smtpSettings.Password);
            }
            return mailClient;
        }

        private bool ValidateContactFields(string name, string email, string message, bool nameRequired)
        {
            bool isValid = true;
            const string emailAddressRegex = @"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$";

            if ((nameRequired && String.IsNullOrEmpty(name)) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(message)) {
                _notifier.Error(T("All contact fields are required."));
                isValid = false;
            }
            else
            {
                Match emailMatch = Regex.Match(email, emailAddressRegex);
                if (!emailMatch.Success)
                {
                    _notifier.Error(T("Invalid email address."));
                    isValid = false;
                }
            }

            return isValid;
        }

    }
}