using Datwendo.Localization.Services;
using Orchard.Environment.Extensions;
using Orchard.Localization.Services;
using System;
using System.Web;

namespace Datwendo.Localization.Providers
{
    [OrchardFeature("Datwendo.Localization.CookieCultureSelector")]
    public class CookieCultureSelector : ICultureSelector 
    {
        private readonly ICookieCultureService _cookieCultureService;

        public CookieCultureSelector(ICookieCultureService cookieCultureService)
        {
            _cookieCultureService = cookieCultureService;
        }

        public CultureSelectorResult GetCulture(HttpContextBase context)
        {
            var cultureCookie   = _cookieCultureService.GetCulture();
            if (cultureCookie == null || string.Equals(cultureCookie,"Browser",StringComparison.InvariantCultureIgnoreCase) ) 
                return null;

            var cultureName     = _cookieCultureService.GetSpecificOrNeutralCulture(cultureCookie);

            return cultureName == null ? null : new CultureSelectorResult { Priority = _cookieCultureService.GetSettings().Priority, CultureName = cultureName };
        }
    }
}

