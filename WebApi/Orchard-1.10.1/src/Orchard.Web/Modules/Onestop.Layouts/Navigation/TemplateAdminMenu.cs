using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Onestop.Layouts.Navigation {
    [OrchardFeature("Onestop.Layouts")]
    public class TemplateAdminMenu : INavigationProvider {
        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder) {
            builder.AddImageSet("template").Add(T("OS Templates"), "1.4.5",
                menu => menu
                    .Add(T("Templates"), "2.0.1", ti => ti
                        .Action("List", "TemplateAdmin", new { area = "Onestop.Layouts" })
                        .Permission(Permissions.ManageTemplates)
            ));
        }
    }
}