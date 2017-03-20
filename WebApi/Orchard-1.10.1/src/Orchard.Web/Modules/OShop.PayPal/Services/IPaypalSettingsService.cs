using Orchard;
using OShop.PayPal.Models;

namespace OShop.PayPal.Services {
    public interface IPaypalSettingsService : IDependency {
        PaypalSettings GetSettings();
        void SetSettings(PaypalSettings Settings);
    }
}
