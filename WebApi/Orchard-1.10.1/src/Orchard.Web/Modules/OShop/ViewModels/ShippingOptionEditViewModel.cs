﻿using OShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OShop.ViewModels {
    public class ShippingOptionEditViewModel {
        public ShippingOptionEditViewModel() {
            Contraints = new List<ShippingContraint>();
            NewContraint = new ShippingContraint();
        }

        public int OptionId { get; set; }
        public int ShippingProviderId { get; set; }
        public string ShippingProviderName { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public IEnumerable<ShippingZoneRecord> ShippingZones { get; set; }
        public int ShippingZoneId { get; set; }
        public int Priority { get; set; }
        public decimal Price { get; set; }
        public IList<ShippingContraint> Contraints { get; set; }
        public ShippingContraint NewContraint { get; set; }
        public string ReturnUrl { get; set; }
    }
}