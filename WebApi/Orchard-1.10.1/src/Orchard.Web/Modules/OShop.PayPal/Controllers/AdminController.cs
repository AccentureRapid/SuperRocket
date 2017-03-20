using Orchard;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using OShop.PayPal.Models;
using OShop.PayPal.Services;
using OShop.Permissions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OShop.PayPal.Controllers
{
    [Admin]
    public class AdminController : Controller
    {
        private readonly IPaypalSettingsService _settingsService;
        private readonly IPaypalConnectionManager _connectionManager;

        public AdminController(
            IPaypalSettingsService settingsService,
            IPaypalConnectionManager connectionManager,
            IOrchardServices services) {
            _settingsService = settingsService;
            _connectionManager = connectionManager;
            Services = services;
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ActionResult Settings() {
            if (!Services.Authorizer.Authorize(OShopPermissions.ManageShopSettings, T("Not allowed to manage Shop Settings")))
                return new HttpUnauthorizedResult();

            return View(_settingsService.GetSettings());
        }

        [HttpPost, FormValueRequired("submit.Save")]
        [ActionName("Settings")]
        public ActionResult SettingsSave(PaypalSettings model) {
            if (!Services.Authorizer.Authorize(OShopPermissions.ManageShopSettings, T("Not allowed to manage Shop Settings")))
                return new HttpUnauthorizedResult();

            if (TryUpdateModel(model)) {
                _settingsService.SetSettings(model);
                Services.Notifier.Information(T("PayPal Settings saved successfully."));
            }
            else {
                Services.Notifier.Error(T("Could not save PayPal Settings."));
            }

            return View(model);
        }

        [HttpPost, FormValueRequired("submit.Validate")]
        [ActionName("Settings")]
        public async Task<ActionResult> SettingsValidate(PaypalSettings model) {
            if (!Services.Authorizer.Authorize(OShopPermissions.ManageShopSettings, T("Not allowed to manage Shop Settings")))
                return new HttpUnauthorizedResult();

            if (TryUpdateModel(model)) {
                if (await _connectionManager.ValidateCredentialsAsync(model.ClientId, model.ClientSecret, model.UseSandbox)) {
                    Services.Notifier.Information(T("Valid credentials."));
                }
                else {
                    Services.Notifier.Warning(T("Invalid credentials."));
                }
            }
            else {
                Services.Notifier.Error(T("Could not validate credentials."));
            }

            return View(model);
        }

    }
}