using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Amba.HtmlBlocks
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
            builder.Add(T("Html Blocks"), "4",
                        menu => menu
                                    .Add(T("Html Blocks"), "1.0",
                                    item => item.Action("List", "Admin", new { area = "Amba.HtmlBlocks" }))
                                    );
  
        }
    }
}
