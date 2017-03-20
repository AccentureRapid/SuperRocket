using Orchard.Localization;
using Orchard.Services;
using OShop.Models;
using OShop.PayPal.Models.Api;
using OShop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace OShop.PayPal.Services {
    public class PaypalPaymentProvider : IPaymentProvider {
        public PaypalPaymentProvider(){
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public int Priority {
            get { return 50; }
        }

        public string Name {
            get { return "Paypal"; }
        }

        public LocalizedString Label {
            get { return T("PayPal"); }
        }

        public LocalizedString Description {
            get { return T("Pay with your PayPal account or credit card"); }
        }

        public RouteValueDictionary GetPaymentRoute(PaymentPart Part) {
            if (Part == null) {
                throw new ArgumentNullException("Part", "PaymentPart cannot be null.");
            }
            return new RouteValueDictionary(new {
                Area = "OShop.PayPal",
                Controller = "Payment",
                Action = "Create",
                Id = Part.ContentItem.Id
            });
        }
    }
}