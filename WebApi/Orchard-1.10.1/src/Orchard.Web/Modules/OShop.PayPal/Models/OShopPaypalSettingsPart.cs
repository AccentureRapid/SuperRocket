using Orchard.ContentManagement;

namespace OShop.PayPal.Models {
    public class OShopPaypalSettingsPart : ContentPart {
        public bool UseSandbox {
            get { return this.Retrieve(x => x.UseSandbox, false); }
            set { this.Store(x => x.UseSandbox, value); }
        }

        public string ClientId {
            get { return this.Retrieve(x => x.ClientId); }
            set { this.Store(x => x.ClientId, value); }
        }

        public string ClientSecret {
            get { return this.Retrieve(x => x.ClientSecret); }
            set { this.Store(x => x.ClientSecret, value); }
        }
    }
}