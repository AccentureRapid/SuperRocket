using Datwendo.Localization.Events;
using Datwendo.Localization.Models;
using Orchard;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Security;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Linq;


namespace Datwendo.Localization.Services
{
    public interface IUserCultureService : IDependency 
    {
        CultureItemModel GetUserCulture(HttpContextBase context = null);
        string GetCulture(HttpContextBase context = null, bool fallback2Browser = true); 
        UserCultureSettings GetSettings();
    }

    [OrchardFeature("Datwendo.Localization.UserCultureSelector")]
    public class UserCultureService : IUserCultureService
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ISignals _signals;
        private readonly ICacheManager _cacheManager;
        private readonly IBrowserCultureService _browserCultureService;
        private readonly IProfileProviderEventHandler _profileHandlers;
        private readonly IAuthenticationService _authenticationService;

        public UserCultureService(IWorkContextAccessor workContextAccessor
                                        , ICacheManager cacheManager
                                        , IBrowserCultureService browserCultureService
                                        , IProfileProviderEventHandler profileHandlers
                                        , IAuthenticationService authenticationService
                                        , ISignals signals)
        {
            _workContextAccessor    = workContextAccessor;
            _cacheManager           = cacheManager;
            _signals                = signals;
            _authenticationService  = authenticationService;
            _profileHandlers        = profileHandlers;
            _browserCultureService  = browserCultureService;
        }

        public CultureItemModel GetUserCulture(HttpContextBase context)
        {
            if (context == null)
            {
                var workContext = _workContextAccessor.GetContext();
                if (workContext != null)
                    context = workContext.HttpContext;
            }
            var culture         = GetCulture(context);
            if (culture == null)
                return null;
            var ci              = new CultureInfo(culture);
            return new CultureItemModel { Culture = ci.Name, LocalizedName = ci.NativeName.UpperFirst(), ShortName = ci.TwoLetterISOLanguageName, FullName = ci.DisplayName };
        }

        public string GetCulture(HttpContextBase context =null, bool fallback2Browser=true) 
        {
            if ( context == null )
            {
                var workContext     = _workContextAccessor.GetContext();
                if (workContext != null )
                    context = workContext.HttpContext;
            }
            if ( context == null || context.Request == null || context.Request.UserLanguages == null) 
                return null;
            
            var settings = GetSettings();
            if (settings == null || !settings.Enabled)
            {
                return null;
            }

            if ( !context.Request.IsAuthenticated)
            {
                return null;
            }
            var currentUser         = _authenticationService.GetAuthenticatedUser();

            PreferedCulture pre     = new PreferedCulture { User=currentUser, Culture=null, PreferredList = null};
            _profileHandlers.ProvidePreferredCultureRequest(pre);
            if ( !string.IsNullOrEmpty(pre.Culture) )
                    return pre.Culture;

            if ( pre.PreferredList != null )
                    return pre.PreferredList.OrderBy(t => t.Item2).Select(t => t.Item1).FirstOrDefault();

            if ( !fallback2Browser )
                return null;
            // We should have a Profile Provider to bring back the user profile with its prefered culture
            // Defaults to browser
            return _browserCultureService.GetCulture(context);
        }

        public UserCultureSettings GetSettings()
        {
            return _cacheManager.Get("UserCultureSettings",
                ctx =>
                {
                    ctx.Monitor(_signals.When(UserCultureSettingsPart.CacheKey));
                    var settingsPart = _workContextAccessor.GetContext().CurrentSite.As<UserCultureSettingsPart>();
                    return new UserCultureSettings
                    {
                        Enabled         = settingsPart.Enabled,
                        Priority        = settingsPart.Priority
                    };
                });
        }
    }
}

