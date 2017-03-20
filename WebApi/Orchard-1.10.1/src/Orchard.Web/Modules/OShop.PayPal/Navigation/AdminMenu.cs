using Orchard.Localization;
using Orchard.UI.Navigation;
using OShop.Permissions;

namespace OShop.PayPal.Navigation {
    public class AdminMenu : INavigationProvider {
        public Localizer T { get; set; }

        public AdminMenu() {
            T = NullLocalizer.Instance;
        }

        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder) {
            builder
                .AddImageSet("oshop")
                .Add(menu => menu
                    .Caption(T("OShop"))
                    .Add(subMenu => subMenu
                        .Caption(T("Settings"))
                        .Add(tab => tab
                            .Caption(T("PayPal"))
                            .Position("7.1")
                            .Action("Settings", "Admin", new { area = "OShop.PayPal" })
                            .Permission(OShopPermissions.ManageShopSettings)
                            .LocalNav()
                        )
                    )
                );
        }
    }
}