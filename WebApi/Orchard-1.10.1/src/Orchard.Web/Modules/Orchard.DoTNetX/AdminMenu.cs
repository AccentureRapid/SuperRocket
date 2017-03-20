using System.Linq;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Navigation;
using Orchard;

namespace Orchard.DoTNetX
{
    public class AdminMenu : INavigationProvider {
        private readonly IAuthorizationService _authorizationService;
        private readonly IWorkContextAccessor _workContextAccessor;

        public AdminMenu(IAuthorizationService authorizationService, IWorkContextAccessor workContextAccessor) {
            _authorizationService = authorizationService;
            _workContextAccessor = workContextAccessor;
        }

        public Localizer T { get; set; }

        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("List"), "0", menu =>
            {
                menu.LinkToFirstChild(false);
                menu.Add(T("BlogPost"), "5", item => item.Action("List", "BlogPost", new { area = "Orchard.DoTNetX" }).Permission(Permissions.ManageBlogPost));
            });
        }
    }
}