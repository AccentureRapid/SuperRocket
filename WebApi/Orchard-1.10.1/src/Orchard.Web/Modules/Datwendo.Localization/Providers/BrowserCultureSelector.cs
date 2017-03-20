using Datwendo.Localization.Services;
using Orchard.Environment.Extensions;
using Orchard.Localization.Services;
using System.Web;

namespace Datwendo.Localization.Providers 
{
    [OrchardFeature("Datwendo.Localization.BrowserCultureSelector")]
    public class BrowserCultureSelector : ICultureSelector 
    {
        private readonly IBrowserCultureService _browserCultureService;

        public BrowserCultureSelector(IBrowserCultureService browserCultureService)
        {
            _browserCultureService  = browserCultureService;
        }

        public CultureSelectorResult GetCulture(HttpContextBase context) 
        {
            var cultureName = _browserCultureService.GetCulture(context);
            return cultureName == null ? null : new CultureSelectorResult { Priority = _browserCultureService.GetSettings().Priority, CultureName = cultureName };
        }     
    }
}
