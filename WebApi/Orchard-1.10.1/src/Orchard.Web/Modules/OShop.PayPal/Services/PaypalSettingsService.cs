using Orchard;
using Orchard.ContentManagement;
using Orchard.Security;
using OShop.Models;
using OShop.PayPal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace OShop.PayPal.Services {
    public class PaypalSettingsService : IPaypalSettingsService {
        private readonly IEncryptionService _encryptionService;

        public PaypalSettingsService(
            IEncryptionService encryptionService,
            IOrchardServices services) {
            _encryptionService = encryptionService;
            Services = services;
        }

        public IOrchardServices Services { get; set; }

        public PaypalSettings GetSettings() {
            var settingsPart = Services.WorkContext.CurrentSite.As<OShopPaypalSettingsPart>();

            var settings = new PaypalSettings() {
                UseSandbox = settingsPart.UseSandbox,
                ClientId = settingsPart.ClientId,
            };

            if (!string.IsNullOrEmpty(settingsPart.ClientSecret)) {
                settings.ClientSecret = Encoding.UTF8.GetString(_encryptionService.Decode(Convert.FromBase64String(settingsPart.ClientSecret)));
            }

            return settings;
        }

        public void SetSettings(PaypalSettings Settings) {
            var settingsPart = Services.WorkContext.CurrentSite.As<OShopPaypalSettingsPart>();

            settingsPart.UseSandbox = Settings.UseSandbox;
            settingsPart.ClientId = Settings.ClientId;
            settingsPart.ClientSecret = Convert.ToBase64String(_encryptionService.Encode(Encoding.UTF8.GetBytes(Settings.ClientSecret)));
        }
    }
}