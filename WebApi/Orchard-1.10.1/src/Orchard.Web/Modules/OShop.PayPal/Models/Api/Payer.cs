using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OShop.PayPal.Models.Api {
    public class Payer {
        [JsonProperty("payment_method", Required = Required.Always)]
        [JsonConverter(typeof(StringEnumConverter))]
        public PaymentMethod PaymentMethod { get; set; }
    }

    public enum PaymentMethod {
        [JsonProperty("paypal")]
        Paypal,
        [JsonProperty("credit_card")]
        CreditCard,
    }
}