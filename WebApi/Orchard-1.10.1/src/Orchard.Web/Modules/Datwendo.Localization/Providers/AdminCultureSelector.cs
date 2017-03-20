using Datwendo.Localization.Services;
using Orchard.Environment.Extensions;
using Orchard.Localization.Services;
using System.Web;

namespace Datwendo.Localization.Providers
{
    [OrchardFeature("Datwendo.Localization.AdminCultureSelector")]
    public class AdminCultureSelector : ICultureSelector
    {
        private readonly IAdminCultureService _adminCultureService;

        public AdminCultureSelector(IAdminCultureService adminCultureService)
        {
            _adminCultureService    = adminCultureService;
        }

        public CultureSelectorResult GetCulture(HttpContextBase context)
        {
            var culture = _adminCultureService.GetCulture(context);
            if (culture == null)
                return null;
            return new CultureSelectorResult { Priority = _adminCultureService.GetSettings().Priority, CultureName = culture };
        }
    }
}
