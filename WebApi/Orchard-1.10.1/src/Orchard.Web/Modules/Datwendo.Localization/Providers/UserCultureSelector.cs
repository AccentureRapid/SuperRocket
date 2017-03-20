using Datwendo.Localization.Services;
using Orchard.Environment.Extensions;
using Orchard.Localization.Services;
using System.Web;

namespace Datwendo.Localization.Providers
{
    [OrchardFeature("Datwendo.Localization.UserCultureSelector")]
    public class UserCultureSelector : ICultureSelector
    {
        private readonly IUserCultureService _UserCultureService;

        public UserCultureSelector(IUserCultureService UserCultureService)
        {
            _UserCultureService    = UserCultureService;
        }

        public CultureSelectorResult GetCulture(HttpContextBase context)
        {
            var culture = _UserCultureService.GetCulture(context);
            if (culture == null)
                return null;
            return new CultureSelectorResult { Priority = _UserCultureService.GetSettings().Priority, CultureName = culture };
        }
    }
}
