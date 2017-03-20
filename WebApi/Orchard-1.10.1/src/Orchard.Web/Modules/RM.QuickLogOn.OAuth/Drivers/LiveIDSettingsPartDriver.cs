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
    [OrchardFeature("RM.QuickLogOn.OAuth.LiveID")]
    public class LiveIDSettingsPartDriver : ContentPartDriver<LiveIDSettingsPart>
    {
        private readonly IEncryptionService _service;

        public Localizer T { get; set; }

        public LiveIDSettingsPartDriver(IEncryptionService service)
        {
            _service = service;
            T = NullLocalizer.Instance;
        }

        protected override string Prefix { get { return "LiveIDSettings"; } }

        protected override DriverResult Editor(LiveIDSettingsPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_LiveID_SiteSettings",
                               () => shapeHelper.EditorTemplate(TemplateName: "Parts.LiveID.SiteSettings", Model: part, Prefix: Prefix)).OnGroup("QuickLogOn");
        }

        protected override DriverResult Editor(LiveIDSettingsPart part, IUpdateModel updater, dynamic shapeHelper)
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
