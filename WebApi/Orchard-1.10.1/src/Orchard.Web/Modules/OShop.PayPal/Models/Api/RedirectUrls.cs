using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OShop.PayPal.Models.Api {
    public class RedirectUrls {
        [JsonProperty("return_url", Required = Required.Always)]
        public string ReturnUrl { get; set; }

        [JsonProperty("cancel_url", Required = Required.Always)]
        public string CancelUrl { get; set; }
    }
}