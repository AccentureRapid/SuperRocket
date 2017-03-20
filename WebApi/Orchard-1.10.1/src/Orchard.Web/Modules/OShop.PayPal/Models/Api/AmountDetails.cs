using Newtonsoft.Json;
using OShop.PayPal.Utils;

namespace OShop.PayPal.Models.Api {
    public class AmountDetails {
        [JsonProperty("subtotal", Required = Required.Always)]
        [JsonConverter(typeof(TwoDecimalDigitsConverter))]
        public decimal SubTotal { get; set; }

        [JsonProperty("shipping", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(TwoDecimalDigitsConverter))]
        public decimal? Shipping { get; set; }

        [JsonProperty("tax", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(TwoDecimalDigitsConverter))]
        public decimal? Tax { get; set; }

        [JsonProperty("handling_fee", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(TwoDecimalDigitsConverter))]
        public decimal? HandlingFee { get; set; }

        [JsonProperty("insurance", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(TwoDecimalDigitsConverter))]
        public decimal? Insurance { get; set; }

        [JsonProperty("shipping_discount", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(TwoDecimalDigitsConverter))]
        public decimal? ShippingDiscount { get; set; }

    }
}