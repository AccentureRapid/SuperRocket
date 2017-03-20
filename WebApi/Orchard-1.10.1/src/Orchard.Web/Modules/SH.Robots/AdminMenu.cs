using Orchard.Localization;
using Orchard.UI.Navigation;

namespace SH.Robots {
	public class AdminMenu : INavigationProvider {
		public Localizer T { get; set; }
		public string MenuName { get { return "admin"; } }

		public void GetNavigation(NavigationBuilder builder) {
			builder.Add(T("Robots.txt"), "50",
				menu => menu.Add(T("Robots.txt"), "20", item => item.Action("Index", "Admin", new { area = "SH.Robots" })
					.Permission(Permissions.ConfigureRobotsTextFile)));
		}
	}
}
