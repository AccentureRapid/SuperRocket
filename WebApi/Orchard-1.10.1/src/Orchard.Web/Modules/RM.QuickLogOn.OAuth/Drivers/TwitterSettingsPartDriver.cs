using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security;
using RM.QuickLogOn.OAuth.Models;
using RM.QuickLogOn.OAuth.Services;

namespace RM.QuickLogOn.OAuth.Drivers
{
    [OrchardFeature("RM.QuickLogOn.OAuth.Twitter")]
    public class TwitterSettingsPartDriver : ContentPartDriver<TwitterSettingsPart>
    {
        private readonly IEncryptionService _service;

        public Localizer T { get; set; }

        public TwitterSettingsPartDriver(IEncryptionService service)
        {
            _service = service;
            T = NullLocalizer.Instance;
        }

        protected override string Prefix { get { return "TwitterSettings"; } }

        protected override DriverResult Editor(TwitterSettingsPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_Twitter_SiteSettings",
                               () => shapeHelper.EditorTemplate(TemplateName: "Parts.Twitter.SiteSettings", Model: part, Prefix: Prefix)).OnGroup("QuickLogOn");
        }

        protected override DriverResult Editor(TwitterSettingsPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            if(updater.TryUpdateModel(part, Prefix, null, null))
            {
                if(!string.IsNullOrWhiteSpace(part.ConsumerSecret))
                {
                    part.Record.EncryptedConsumerSecret = _service.Encrypt(part.ConsumerSecret);
                }
            }
            return Editor(part, shapeHelper);
        }
    }
}
