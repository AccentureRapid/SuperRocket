using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OShop.PayPal.Models.Api {
    public class Transaction {
        [JsonProperty("amount", Required = Required.Always)]
        public Amount Amount { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("item_list", NullValueHandling = NullValueHandling.Ignore)]
        public ItemList ItemList { get; set; }

        [JsonProperty("invoice_number", NullValueHandling = NullValueHandling.Ignore)]
        public string InvoiceNumber { get; set; }

        [JsonProperty("custom", NullValueHandling = NullValueHandling.Ignore)]
        public string Custom { get; set; }

        [JsonProperty("soft_descriptor", NullValueHandling = NullValueHandling.Ignore)]
        public string SoftDescriptor { get; set; }
    }
}