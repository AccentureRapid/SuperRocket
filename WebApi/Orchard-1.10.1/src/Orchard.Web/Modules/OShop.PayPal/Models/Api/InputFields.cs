using Newtonsoft.Json;

namespace OShop.PayPal.Models.Api {
    public class InputFields {
        [JsonProperty("allow_note", NullValueHandling = NullValueHandling.Ignore)]
        public bool? allow_note { get; set; }

        [JsonProperty("no_shipping", NullValueHandling = NullValueHandling.Ignore)]
        public int NoShipping { get; set; }

        [JsonProperty("address_override", NullValueHandling = NullValueHandling.Ignore)]
        public int AddressOverride { get; set; }
    }
}