using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Navigation;

namespace Amba.AmazonS3
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
            builder
              .Add(T("Settings"), menu => menu
                    .Add(T("Amazon S3"), "1.0", x => x
                        .Add(T("Settings"), "1.0", a => a.Action("Settings", "Admin", new { area = "Amba.AmazonS3" }).Permission(StandardPermissions.SiteOwner).LocalNav())
                        
                    ));;
             

        }
    }
}
