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
    public interface IBrowserCultureService : IDependency 
    {
        CultureItemModel GetBrowserCulture(HttpContextBase context = null);
        string GetCulture(HttpContextBase context = null );
        BrowserCultureSettings GetSettings();
    }

    [OrchardFeature("Datwendo.Localization")]
    public class BrowserCultureService : IBrowserCultureService
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ISignals _signals;
        private readonly ICacheManager _cacheManager;
        private readonly Lazy<ICultureService> _cultureService;

        public BrowserCultureService(IWorkContextAccessor workContextAccessor,
                                        ICacheManager cacheManager,
                                        Lazy<ICultureService> cultureService,
                                        ISignals signals)
        {
            _workContextAccessor    = workContextAccessor;
            _cacheManager           = cacheManager;
            _signals                = signals;
            _cultureService         = cultureService;
        }

        public CultureItemModel GetBrowserCulture(HttpContextBase context)
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
            if ( context == null || context.Request == null || context.Request.UserLanguages == null) 
                return null;
            
            var browserCultures =  context.Request.UserLanguages.Select(x => x.Split(new char[]{';'},StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            if (browserCultures.Length == 0) 
                return null;

            var cultureName     = SelectSuitableCulture(_cultureService.Value.SiteCultureNames(), browserCultures);
            return cultureName;
        }

        private string SelectSuitableCulture(IEnumerable<string> supportedCultures, IEnumerable<string> browserCultures)
        {
            var supportedCultureInfos = supportedCultures.Select(x => x.ParseCultureInfo()).Where(x => x != null).ToArray();
            Tuple<CultureInfo, int> best = null;
            foreach (var browserCultureInfo in browserCultures.Select(x => x.ParseCultureInfo()).Where(x => x != null))
            {
                var localBest = supportedCultureInfos.Select(x => new Tuple<CultureInfo, int>(x, GetRank(x, browserCultureInfo))).Where(x => x.Item2 > 0).OrderByDescending(x => x.Item2).FirstOrDefault();
                if (localBest != null && (best == null || localBest.Item2 > best.Item2)) best = localBest;
            }
            return best != null ? best.Item1.Name : null;
        }

        private int GetRank(CultureInfo supportedCulture, CultureInfo browserCulture)
        {
            // Certain match has highest rank
            return (supportedCulture.Name == browserCulture.Name ? 8 : 0) +
                // if supported culture is neutral 'en' and browser culture has same parent: 'en-US' or 'en-GB' have 'en' as parent (neutral) THEN select 'en'
                    (supportedCulture.IsNeutralCulture && !browserCulture.IsNeutralCulture && supportedCulture.Name == browserCulture.Parent.Name ? 4 : 0) +
                // if supported culture is 'en-US' or 'en-GB' that has 'en' as parent (neutral) and browser culture is neutral 'en' THEN select 'en-US' as possibly matched
                    (browserCulture.IsNeutralCulture && !supportedCulture.IsNeutralCulture && supportedCulture.Parent.Name == browserCulture.Name ? 2 : 0) +
                // if supported culture is 'en-US' that has 'en' as neutral and browser culture is 'en-GB' that has 'en' as neutral THEN select 'en-US' as possibly matched
                    (!browserCulture.IsNeutralCulture && !supportedCulture.IsNeutralCulture && supportedCulture.Parent.Name == browserCulture.Parent.Name ? 1 : 0);
        }
 
        public BrowserCultureSettings GetSettings()
        {
            return _cacheManager.Get("BrowserCultureSettings",
                ctx =>
                {
                    ctx.Monitor(_signals.When(BrowserCultureSettingsPart.CacheKey));
                    var settingsPart = _workContextAccessor.GetContext().CurrentSite.As<BrowserCultureSettingsPart>();
                    return new BrowserCultureSettings
                    {
                        Enabled = settingsPart.Enabled,
                        Priority = settingsPart.Priority
                    };
                });
        }
    }
}

