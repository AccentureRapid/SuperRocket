using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Orchard.ContentTree {
    public class AdminMenu : INavigationProvider {
        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder) {
            builder.AddImageSet("content-tree")
                .Add(T("Content Tree"), "1.4.0.1", menu => menu
                    .Add(T("Content Tree"), "2", item => item.Action("ContentTree", "Admin", new { area = "Orchard.ContentTree" }).LocalNav()));

            builder.Add(T("Settings"), "99", menu => menu.
                Add(T("Content Tree"), "5", item => item.Action("Settings", "Admin", new { area = "Orchard.ContentTree" })));
        }
    }
}