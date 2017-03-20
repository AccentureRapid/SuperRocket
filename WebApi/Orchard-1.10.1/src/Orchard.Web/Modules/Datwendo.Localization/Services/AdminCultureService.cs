using Datwendo.Localization.Models;
using Orchard;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.UI.Admin;
using System;
using System.Globalization;
using System.Web;


namespace Datwendo.Localization.Services
{
    public interface IAdminCultureService : IDependency 
    {
        CultureItemModel GetAdminCulture(HttpContextBase context = null);
        string GetCulture(HttpContextBase context = null );
        AdminCultureSettings GetSettings();
    }

    [OrchardFeature("Datwendo.Localization.AdminCultureSelector")]
    public class AdminCultureService : IAdminCultureService
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ISignals _signals;
        private readonly ICacheManager _cacheManager;
        private readonly IUserCultureService _userCultureService;

        public AdminCultureService(IWorkContextAccessor workContextAccessor
                                        , ICacheManager cacheManager
                                        , IUserCultureService userCultureService
                                        , ISignals signals)
        {
            _workContextAccessor    = workContextAccessor;
            _cacheManager           = cacheManager;
            _userCultureService     = userCultureService;
            _signals                = signals;
        }

        public CultureItemModel GetAdminCulture(HttpContextBase context)
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

        public string GetCulture(HttpContextBase context) 
        {
            if ( context == null )
            {
                var workContext = _workContextAccessor.GetContext();
                if (workContext != null )
                    context = workContext.HttpContext;
            }
            if (context == null || context.Request == null || context.Request.RequestContext == null) 
                return null;

            var settings        = GetSettings();
            if ( settings == null || !settings.Enabled )
                return null;
        
            var controllerName  =  (string)context.Request.RequestContext.RouteData.Values["controller"];

            // only for admin mode
            if ( ( !string.IsNullOrEmpty(controllerName) && !string.Equals(controllerName, "admin",StringComparison.InvariantCultureIgnoreCase) ) 
                || !AdminFilter.IsApplied(context.Request.RequestContext))
            {
                return null;
            }
            var culture         = _userCultureService.GetCulture(context, false);
            if (!string.IsNullOrEmpty(culture) )
            {
                return culture;
            }
            culture             = settings.AdminCulture;
            if (string.IsNullOrEmpty(culture)) 
                return null;
            return culture;
        }

        public AdminCultureSettings GetSettings()
        {
            return _cacheManager.Get("AdminCultureSettings",
                ctx =>
                {
                    ctx.Monitor(_signals.When(AdminCultureSettingsPart.CacheKey));
                    var settingsPart = _workContextAccessor.GetContext().CurrentSite.As<AdminCultureSettingsPart>();
                    return new AdminCultureSettings
                    {
                        AdminCulture    = settingsPart.AdminCulture,
                        Enabled         = settingsPart.Enabled,
                        Priority        = settingsPart.Priority
                    };
                });
        }
    }
}

