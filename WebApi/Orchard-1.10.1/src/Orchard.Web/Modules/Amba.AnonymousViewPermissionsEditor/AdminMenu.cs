using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Navigation;

namespace Amba.AnonymousViewPermissionsEditor
{
    public class AdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }

        public string MenuName
        {
            get { return "admin"; }
        }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("Users"),
                menu => menu.Add(T("Anonymous View Permissions Editor"), "3.0", item => item.Action("AnonymousViewEditor", "Admin", new { area = "Amba.AnonymousViewPermissionsEditor" })
                    .LocalNav().Permission(StandardPermissions.SiteOwner)));
        }
    }
}