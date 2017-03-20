using Newtonsoft.Json;

namespace OShop.PayPal.Models.Api {
    public class WebProfile {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty("input_fields", NullValueHandling = NullValueHandling.Ignore)]
        public InputFields InputFields { get; set; }

        [JsonProperty("presentation", NullValueHandling = NullValueHandling.Ignore)]
        public Presentation Presentation { get; set; }
    }

    public class CreateProfileResponse {
        [JsonProperty("id", Required = Required.Always)]
        public string Id { get; set; }
    }
}