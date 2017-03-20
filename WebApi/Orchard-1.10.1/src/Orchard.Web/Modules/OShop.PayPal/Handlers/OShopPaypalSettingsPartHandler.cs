using Orchard.ContentManagement.Handlers;
using OShop.PayPal.Models;

namespace OShop.PayPal.Handlers {
    public class OShopPaypalSettingsPartHandler : ContentHandler {
        public OShopPaypalSettingsPartHandler() {
            Filters.Add(new ActivatingFilter<OShopPaypalSettingsPart>("Site"));
        }
    }
}