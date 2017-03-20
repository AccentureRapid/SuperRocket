using Newtonsoft.Json;
using System.Collections.Generic;

namespace OShop.PayPal.Models.Api {
    public class ItemList {
        [JsonProperty("items", NullValueHandling = NullValueHandling.Ignore)]
        public List<Item> Items { get; set; }
    }
}