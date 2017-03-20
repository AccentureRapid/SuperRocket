using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Navigation;

namespace Laughlin.ErrorLog
{
    public class AdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.AddImageSet("modules")
              .Add(T("Error Log"), "12",
                          menu => menu.Add(T("Error Log"), "12",
                              item => item.Action("Index", "Admin", new { area = "Laughlin.ErrorLog" })
                                                                                .Permission(StandardPermissions.SiteOwner)));
        }
    }
}