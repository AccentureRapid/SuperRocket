using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OShop.PayPal.Models.Api {
    public class Presentation {
        [JsonProperty("brand_name", NullValueHandling = NullValueHandling.Ignore)]
        public string BrandName { get; set; }

        [JsonProperty("logo_image", NullValueHandling = NullValueHandling.Ignore)]
        public string LogoImage { get; set; }

        [JsonProperty("locale_code", NullValueHandling = NullValueHandling.Ignore)]
        public string LocaleCode { get; set; }
    }
}