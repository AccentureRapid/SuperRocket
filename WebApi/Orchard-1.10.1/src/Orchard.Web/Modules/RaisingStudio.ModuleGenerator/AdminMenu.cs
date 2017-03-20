using System.Linq;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Navigation;
using Orchard;

namespace RaisingStudio.ModuleGenerator
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

        public void GetNavigation(NavigationBuilder builder) {
            builder.AddImageSet("modulegenerator")
                .Add(T("Module Generator"), "1.0", BuildMenu);
        }

        private void BuildMenu(NavigationItemBuilder menu) {
            menu.Add(T("Module Generator"), "1.0",
                item => item.Action("Index", "Admin", new { area = "RaisingStudio.ModuleGenerator" }).Permission(Permissions.GenerateModule));
        }
    }
}