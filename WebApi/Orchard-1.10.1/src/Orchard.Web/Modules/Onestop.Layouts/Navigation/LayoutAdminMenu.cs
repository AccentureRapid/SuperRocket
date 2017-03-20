using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Onestop.Layouts.Navigation {
    [OrchardFeature("Onestop.Layouts")]
    public class LayoutAdminMenu : INavigationProvider {
        public Localizer T { get; set; }

        public string MenuName {
            get { return "admin"; }
        }

        public void GetNavigation(NavigationBuilder builder) {
            builder
                .AddImageSet("layout")
                .Add(
                    T("OS Layouts"), "1.4.5", menu => menu
                        .Add(
                            T("Layouts"), "2.0", li => li
                                .Action("List", "LayoutAdmin", new {area = "Onestop.Layouts"})
                                .Permission(Permissions.ManageLayouts)
                        ));
        }
    }
}