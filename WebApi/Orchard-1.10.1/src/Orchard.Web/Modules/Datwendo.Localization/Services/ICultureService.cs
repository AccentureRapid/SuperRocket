using Datwendo.Localization.Models;
using Datwendo.Localization.ViewModels;
using Orchard;
using Orchard.Autoroute.Models;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization.Models;
using Orchard.Localization.Records;
using Orchard.Localization.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace Datwendo.Localization.Services
{
    public interface ICultureService : IDependency 
    {
        List<CookieCultureItemViewModel> BuildViewModel(bool ShowBrowser, bool isBrowserCurrent,out string currentCulture);
        IEnumerable<CultureItemModel> ListCultures();
        IEnumerable<string> SiteCultureNames();
        string GetCurrentCulture();
        string GetSiteCulture();
        IContent GetCurrentContentItem();
        IEnumerable<LocalizationPart> GetLocalizations(LocalizationPart part, VersionOptions versionOptions);
        IEnumerable<Tuple<string, int>> GetLocalizationsIds(ContentItem item);
        bool TryGetRouteForUrl(string url, out AutoroutePart route);
        bool IsOk4Culture(ContentItem routableContent, string cultureName);
        string CultureFromRegEx(string cultureName, string RegExRules);
        IEnumerable<LocalizationPart> GetDisplayLocalizations(LocalizationPart part, VersionOptions versionOptions);
        IEnumerable<LocalizationPart> GetEditorLocalizations(LocalizationPart part);
    }
        
    [OrchardFeature("Datwendo.Localization")]
    public class CultureService : ICultureService
    {
        private readonly Lazy<ICultureManager> _lazyCultureManager;
        private readonly ILocalizationService _localizationService;
        private readonly IOrchardServices _orchardServices;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;
        private readonly Lazy<IHomePageService> _homePageService;
        private readonly IBrowserCultureService _browserCultureService;

        public CultureService(IWorkContextAccessor workContextAccessor
            , ICacheManager cacheManager
            , ISignals signals
            , IOrchardServices orchardServices
            , IBrowserCultureService browserCultureService
            , Lazy<IHomePageService> homePageService
            , Lazy<ICultureManager> lazyCultureManager
            , ILocalizationService localizationService)
        {
            _orchardServices        = orchardServices;
            _lazyCultureManager     = lazyCultureManager;
            _localizationService    = localizationService;
            _homePageService        = homePageService;
            _browserCultureService  = browserCultureService;
            _cacheManager           = cacheManager;
            _signals                = signals;
        }

        public List<CookieCultureItemViewModel> BuildViewModel(bool ShowBrowser, bool isBrowserCurrent,out string currentCulture)
        {
            RequestContext requestContext   = _orchardServices.WorkContext.HttpContext.Request.RequestContext;
            var urlHelper                   = new UrlHelper(requestContext);
            var contentItem                 = GetCurrentContentItem();
            currentCulture                  = GetCurrentCulture();
            var returnUrl                   = _orchardServices.WorkContext.HttpContext.Request.Url.LocalPath;
            var localizations               = (contentItem != null) ? GetLocalizations(contentItem.As<LocalizationPart>(), VersionOptions.Latest).ToList() : null;
            var cookieCultureItems          = new List<CookieCultureItemViewModel>();
            // Add browser CultureItem
            var browserCultureItem          = new CookieCultureItemViewModel
            {
                CultureItem                 = _browserCultureService.GetBrowserCulture(requestContext.HttpContext),
                Current                     = isBrowserCurrent,
                ReturnUrl                   = returnUrl,
                IsBrowser                   = true,
                Rel                         = (contentItem == null) ? "nofollow" : null
            };
            // Add available cutures
            foreach (var cultureItem in ListCultures())
            {
                var cookieCultureItem       = new CookieCultureItemViewModel
                {
                    CultureItem             = cultureItem,
                    Current                 = (!isBrowserCurrent) && string.Equals(currentCulture, cultureItem.Culture, StringComparison.OrdinalIgnoreCase),
                    ReturnUrl               = returnUrl,
                    IsBrowser               = false,
                    Rel                     = (contentItem == null) ? "nofollow" : null
                };
                if (localizations != null)
                {
                    var localizedContentItem        = localizations.Where(p => string.Equals(p.Culture.Culture, cultureItem.Culture, StringComparison.OrdinalIgnoreCase))
                                                                    .Select(p => p.ContentItem).FirstOrDefault();
                    var metadata                    = localizedContentItem != null ? localizedContentItem.ContentManager.GetItemMetadata(localizedContentItem) : null;
                    if (metadata != null && metadata.DisplayRouteValues != null)
                    {
                        cookieCultureItem.ReturnUrl = urlHelper.Content(urlHelper.RouteUrl(metadata.DisplayRouteValues));
                    }
                    else
                    {
                        // Apply Fallback on culture
                        string newUrl                   = null;
                        if ( (contentItem != null )&& _homePageService.Value.FindUrlFromFallbackCulture(contentItem.ContentItem, cultureItem.Culture,requestContext, out newUrl))
                        {
                            if (newUrl.StartsWith("~"))
                                newUrl                  = newUrl.Substring(1);
                            cookieCultureItem.ReturnUrl = newUrl;
                        }
                        else
                        {
                            cookieCultureItem.ReturnUrl = urlHelper.RouteUrl(new { Area = "Datwendo.Localization", Action = "NotTranslated", Controller = "LocalizedHome", Culture = cultureItem.Culture, Id = contentItem.Id });
                            cookieCultureItem.Rel       = "nofollow";
                        }
                    }
                }
                cookieCultureItems.Add(cookieCultureItem);
            }
            if ( ShowBrowser)
                cookieCultureItems.Add(browserCultureItem);
            return cookieCultureItems;
        }


        public IEnumerable<CultureItemModel> ListCultures()
        {
            return _lazyCultureManager.Value.ListCultures().Where(s => s!= null ).Select(x => new CultureInfo(x)).Select(x => new CultureItemModel { Culture = x.Name, LocalizedName = x.NativeName.UpperFirst(), ShortName = x.TwoLetterISOLanguageName, FullName = x.DisplayName });
        }

        public string GetCurrentCulture()
        {
            return _lazyCultureManager.Value.GetCurrentCulture(_orchardServices.WorkContext.HttpContext);
        }

        public string GetSiteCulture()
        {
            return _lazyCultureManager.Value.GetSiteCulture();
        }

        public IContent GetCurrentContentItem()
        {
            var values = _orchardServices.WorkContext.HttpContext.Request.RequestContext.RouteData.Values;

            object v = values.TryGetValue("area", out v) && string.Equals("Contents", v as string, StringComparison.OrdinalIgnoreCase) ? v : null;
            v = v != null && values.TryGetValue("controller", out v) && string.Equals("Item", v as string, StringComparison.OrdinalIgnoreCase) ? v : null;
            v = v != null && values.TryGetValue("action", out v) && string.Equals("Display", v as string, StringComparison.OrdinalIgnoreCase) ? v : null;

            int id = v != null && values.TryGetValue("id", out v) && int.TryParse(v as string, out id) ? id : 0;
            if (id > 0) return _orchardServices.ContentManager.Get(id);
            return null;
        }

        public IEnumerable<LocalizationPart> GetLocalizations(LocalizationPart part, VersionOptions versionOptions)
        {
            if (part == null)
                return new LocalizationPart[0];
            CultureRecord siteCulture = null;
            return new[] { (part.MasterContentItem ?? part.ContentItem).As<LocalizationPart>() }
                .Union(part.Id > 0 ? _localizationService.GetLocalizations(part.MasterContentItem ?? part.ContentItem, versionOptions) : new LocalizationPart[0])
                .Select(c =>
                {
                    var localized = c.ContentItem.As<LocalizationPart>();
                    if (localized.Culture == null)
                        localized.Culture = siteCulture ?? (siteCulture = _lazyCultureManager.Value.GetCultureByName(GetSiteCulture()));
                    return c;
                });
        }
        public IEnumerable<Tuple<string,int>> GetLocalizationsIds(ContentItem item)
        {
            CultureRecord siteCulture   = null;
            LocalizationPart part       = item.As<LocalizationPart>();
            if (part != null)
            {
                return new[] { (part.MasterContentItem ?? item).As<LocalizationPart>() }
                    .Union(part.Id > 0 ? _localizationService.GetLocalizations(part.MasterContentItem ?? item, VersionOptions.Published) : new LocalizationPart[0])
                    .Select(c =>
                    {
                        var localized = c.ContentItem.As<LocalizationPart>();
                        if (localized.Culture == null)
                            localized.Culture = siteCulture ?? (siteCulture = _lazyCultureManager.Value.GetCultureByName(GetSiteCulture()));
                        return new Tuple<string, int>(localized.Culture.Culture, localized.Id);
                    });
            }
            return new[] { new Tuple<string, int>(GetSiteCulture(), item.Id) }; //  new Tuple<string, int>[0]
        }

        public IEnumerable<string> SiteCultureNames()
        {
            return _cacheManager.Get("ListCultures", ctx =>
            {
                ctx.Monitor(_signals.When("culturesChanged"));

                return _lazyCultureManager.Value.ListCultures();
            });
        }

        public bool TryGetRouteForUrl(string url, out AutoroutePart route)
        {
            //first check for route (fast, case sensitive, not precise)
            route = _orchardServices.ContentManager.Query<AutoroutePart, AutoroutePartRecord>()
                .ForVersion(VersionOptions.Published)
                .Where(r => r.DisplayAlias == url)
                .List()
                .FirstOrDefault();

            return route != null;
        }

        public bool IsOk4Culture(ContentItem routableContent, string cultureName)
        {
            string contentCulture = _localizationService.GetContentCulture(routableContent);
            if (string.Equals(cultureName, contentCulture, System.StringComparison.InvariantCultureIgnoreCase))
                return true;
            return false;
        }
        //ru-RU:^uk-UA|be-BY$
        public string CultureFromRegEx(string cultureName,string RegExRules)
        {
            var newCulture = RegExRules.Split(new char[]{'\r', '\n'},StringSplitOptions.RemoveEmptyEntries)
                .Where(s => s.IndexOf(":") > 0)
                .Select(s => new KeyValuePair<string, string>(s.Substring(0, s.IndexOf(":")), s.Substring(s.IndexOf(":") + 1)))
                .Where(p => !string.IsNullOrWhiteSpace(p.Value) && new Regex(p.Value, RegexOptions.IgnoreCase).IsMatch(cultureName))
                .Select(p => p.Key).FirstOrDefault();
            return newCulture;
        }

        public IEnumerable<LocalizationPart> GetDisplayLocalizations(LocalizationPart part, VersionOptions versionOptions)
        {
            return _localizationService.GetLocalizations(part.ContentItem, versionOptions)
                .Select(c =>
                {
                    var localized = c.ContentItem.As<LocalizationPart>();
                    if (localized.Culture == null)
                        localized.Culture = _lazyCultureManager.Value.GetCultureByName(GetSiteCulture());
                    return c;
                }).ToList();
        }

        public IEnumerable<LocalizationPart> GetEditorLocalizations(LocalizationPart part)
        {
            return _localizationService.GetLocalizations(part.ContentItem, VersionOptions.Latest)
                .Select(c =>
                {
                    var localized = c.ContentItem.As<LocalizationPart>();
                    if (localized.Culture == null)
                        localized.Culture = _lazyCultureManager.Value.GetCultureByName(GetSiteCulture());
                    return c;
                }).ToList();
        }
    }
}

