using Newtonsoft.Json;
using OShop.PayPal.Utils;

namespace OShop.PayPal.Models.Api {
    public class Amount {
        [JsonProperty("currency", Required = Required.Always)]
        public string Currency { get; set; }

        [JsonProperty("total", Required = Required.Always)]
        [JsonConverter(typeof(TwoDecimalDigitsConverter))]
        public decimal Total { get; set; }
    }
}