﻿using Nwazet.Commerce.Permissions;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Navigation;

namespace Nwazet.Commerce.Menus {
    [OrchardFeature("Nwazet.TieredPricing")]
    public class ProductSettingsAdminMenu : INavigationProvider {
         public string MenuName {
            get { return "admin"; }
        }

         public ProductSettingsAdminMenu() {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void GetNavigation(NavigationBuilder builder) {
            builder
                .AddImageSet("nwazet-commerce")
                .Add(item => item
                    .Caption(T("Commerce"))
                    .Position("2")
                    .LinkToFirstChild(false)

                    .Add(subItem => subItem
                        .Caption(T("Tiered Pricing"))
                        .Position("2.4.5")
                        .Action("Index", "ProductSettingsAdmin", new { area = "Nwazet.Commerce" })
                        .Permission(CommercePermissions.ManageCommerce)
                    )
                );
        }
    }
}
