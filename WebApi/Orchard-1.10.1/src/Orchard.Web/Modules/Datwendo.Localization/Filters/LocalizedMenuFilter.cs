using System.Collections.Generic;
//using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Navigation.Models;
using Orchard.Environment.Extensions;
using Orchard.Localization.Services;
using Orchard.UI.Navigation;
using Orchard.Settings;
using Orchard.Core.Settings.Models;
using Orchard;
using Datwendo.Localization.Services;

namespace Datwendo.Localization.Filters {

    //[UsedImplicitly]
    [OrchardFeature("Datwendo.Localization.LocalizedMenuFilter")]
    public class LocalizedMenuFilter : INavigationFilter {
        /*
         * Adds a filter to the menus to remove untranslated menu entries.
         * Orchard by defaul will show menu its from the default culture when other languages are selected if the menu
         * has not been translate to at least one other language.
         */
        private readonly ICultureManager _cultureManager;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ISiteService _siteService;
        private readonly IHomePageService _homePageService;

        public LocalizedMenuFilter(IHomePageService homePageService,
            ICultureManager cultureManager, 
            IWorkContextAccessor workContextAccessor,
            ISiteService siteService
         ) 
        {
            _cultureManager         = cultureManager;
            _workContextAccessor    = workContextAccessor;
            _siteService            = siteService;
            _homePageService        = homePageService;
        }

        #region INavigationFilter Members

        public IEnumerable<MenuItem> Filter(IEnumerable<MenuItem> menuItems) 
        {

            string defaultSiteCulture = _siteService.GetSiteSettings().As<SiteSettingsPart>().SiteCulture;

            string currentCulture = _cultureManager.GetCurrentCulture(_workContextAccessor.GetContext().HttpContext);
            foreach (MenuItem menuItem in menuItems) 
            {            
                if ( (  ( menuItem.Culture == null)  &&
                    ( ( _homePageService.GetSettings().MenuMode == Models.MenuFilterMode.NonLocalizedAreAll ) ||
                      ( ( _homePageService.GetSettings().MenuMode == Models.MenuFilterMode.NonLocalizedAreSite ) && (currentCulture == defaultSiteCulture )  ) ) )
                    || menuItem.Culture == currentCulture)
                    yield return menuItem;
            }
        }

        #endregion
    }
}