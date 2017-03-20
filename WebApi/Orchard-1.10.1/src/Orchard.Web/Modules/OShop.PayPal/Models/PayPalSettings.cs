using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OShop.PayPal.Models {
    public class PaypalSettings {
        public bool UseSandbox { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}