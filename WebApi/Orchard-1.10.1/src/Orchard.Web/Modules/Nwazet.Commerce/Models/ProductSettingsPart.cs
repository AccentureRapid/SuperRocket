﻿using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using System.Collections.Generic;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.TieredPricing")]
    public class ProductSettingsPart : ContentPart {

        public bool DefineSiteDefaults {
            get { return this.Retrieve(p => p.DefineSiteDefaults); }
            set { this.Store(p => p.DefineSiteDefaults, value); }
        }

        public bool AllowProductOverrides {
            get { return this.Retrieve(p => p.AllowProductOverrides); }
            set { this.Store(p => p.AllowProductOverrides, value); }
        }

        public IEnumerable<PriceTier> PriceTiers {
            get {
                var rawTiers = Retrieve<string>("PriceTiers");
                return PriceTier.DeserializePriceTiers(rawTiers);
            }
            set {
                var serializedTiers = PriceTier.SerializePriceTiers(value);
                Store("PriceTiers", serializedTiers ?? "");
            }
        }
    }
}
