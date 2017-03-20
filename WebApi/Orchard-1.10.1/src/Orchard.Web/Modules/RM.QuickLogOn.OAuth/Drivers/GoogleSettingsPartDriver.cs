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
    [OrchardFeature("RM.QuickLogOn.OAuth")]
    public class GoogleSettingsPartDriver : ContentPartDriver<GoogleSettingsPart>
    {
        private readonly IEncryptionService _service;

        public Localizer T { get; set; }

        public GoogleSettingsPartDriver(IEncryptionService service)
        {
            _service = service;
            T = NullLocalizer.Instance;
        }

        protected override string Prefix { get { return "GoogleSettings"; } }

        protected override DriverResult Editor(GoogleSettingsPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_Google_SiteSettings",
                               () => shapeHelper.EditorTemplate(TemplateName: "Parts.Google.SiteSettings", Model: part, Prefix: Prefix)).OnGroup("QuickLogOn");
        }

        protected override DriverResult Editor(GoogleSettingsPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            if(updater.TryUpdateModel(part, Prefix, null, null))
            {
                if(!string.IsNullOrWhiteSpace(part.ClientSecret))
                {
                    part.Record.EncryptedClientSecret = _service.Encrypt(part.ClientSecret);
                }
            }
            return Editor(part, shapeHelper);
        }
    }
}
