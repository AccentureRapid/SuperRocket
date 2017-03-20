using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Navigation;

namespace dcp.Routing
{
    [OrchardFeature("dcp.Routing.Redirects")]
    public class RedirectsAdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder

                // Image set
                .AddImageSet("dcp.Routing")

                // "Webshop"
                .Add(item => item

                    .Caption(T("Routing"))
                    .Position("7")
                    .LinkToFirstChild(false)

                    .Add(subItem => subItem
                        .Caption(T("Redirects"))
                        .Action("List", "RedirectRule", new { area = "dcp.Routing" })
                        .Add(x => x
                            .Caption(T("List"))
                            .Action("List", "RedirectRule", new { area = "dcp.Routing" })
                            .LocalNav())
                    )
                );
        }
    }
    
    public class AdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("Aliases"), "1.4.1", menu =>
            {
                menu.Add(T("Extended"), "3", item => item.Action("List", "ExtendedAlias", new { area = "dcp.Routing" }).LocalNav().Permission(StandardPermissions.SiteOwner));
            });
        }

        public string MenuName
        {
            get { return "admin"; }
        }
    }
}