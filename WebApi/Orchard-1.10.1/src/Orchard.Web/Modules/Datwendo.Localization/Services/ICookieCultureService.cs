using Datwendo.Localization.Models;
using Orchard;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Datwendo.Localization.Services
{
    public interface ICookieCultureService : IDependency 
    {
        bool isBrowserCurrent();
        string GetCulture();
        void SetCulture(string culture);
        void ResetCulture();
        string GetSpecificOrNeutralCulture(string cultureName);
        CookieCultureSettings GetSettings();
    }

    [OrchardFeature("Datwendo.Localization.CookieCultureSelector")]
    public class CookieCultureService : ICookieCultureService
    {
        public const string browserTag          = "browser";
        private const string CookieNameTemplate = "{0}_CurrentCulture";

        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ISignals _signals;
        private readonly ICacheManager _cacheManager;
        private readonly Lazy<ICultureService> _cultureService;
        public CookieCultureService(IWorkContextAccessor workContextAccessor,
                                        Lazy<ICultureService> cultureService,
                                        ICacheManager cacheManager,
                                        ISignals signals)
        {
            _workContextAccessor    = workContextAccessor;
            _cacheManager           = cacheManager;
            _signals                = signals;
            _cultureService         = cultureService;
        }              
            
        private static string GetCookieName(WorkContext wc)
        {
            return string.Format(CookieNameTemplate, wc.CurrentSite != null && !string.IsNullOrWhiteSpace(wc.CurrentSite.SiteName) ? wc.CurrentSite.SiteName : "Site");
        }

        public bool isBrowserCurrent()
        {
            return string.Equals(GetCulture(), browserTag, StringComparison.OrdinalIgnoreCase);
        }
        public string GetCulture()
        {
            var wc          = _workContextAccessor.GetContext();
            var cookie      = wc.HttpContext != null ? wc.HttpContext.Request.Cookies[GetCookieName(wc)] : null;
            return cookie != null ? cookie.Value : null;
        }

        public void SetCulture(string culture)
        {
            var wc              = _workContextAccessor.GetContext();
            var coockieName     = GetCookieName(wc);
            var cookie          = new HttpCookie(coockieName) { Expires = DateTime.Now.AddYears(1), Value = culture };
            wc.HttpContext.Response.Cookies.Add(cookie);
        }

        public void ResetCulture()
        {
            var wc          = _workContextAccessor.GetContext();
            var coockieName = GetCookieName(wc);
            var cookie      = new HttpCookie(coockieName) { Expires = DateTime.MinValue, Value = null };
            wc.HttpContext.Response.Cookies.Add(cookie);
        }

        public CookieCultureSettings GetSettings()
        {
            return _cacheManager.Get("CookieCultureSettings",
                ctx =>
                {
                    ctx.Monitor(_signals.When(CookieCultureSettingsPart.CacheKey));
                    var settingsPart        = _workContextAccessor.GetContext().CurrentSite.As<CookieCultureSettingsPart>();
                    return new CookieCultureSettings
                    {
                        Enabled             = settingsPart.Enabled,
                        Priority            = settingsPart.Priority,
                        EnforceCookieUrl    = settingsPart.EnforceCookieUrl
                    };
                });
        }

        public string GetSpecificOrNeutralCulture(string cultureName)
        {
            IEnumerable<string> supportedCultures = _cultureService.Value.SiteCultureNames();
            try
            {
                var ci  = !string.IsNullOrEmpty(cultureName) ? CultureInfo.GetCultureInfo(cultureName) : null;
                var nci = (ci != null && !ci.IsNeutralCulture) ? ci.Parent : ci;

                if (ci != null && supportedCultures.Contains(ci.Name, StringComparer.InvariantCultureIgnoreCase)) return ci.Name;

                if (nci != null && ci != nci && supportedCultures.Contains(nci.Name, StringComparer.InvariantCultureIgnoreCase)) return nci.Name;

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}

