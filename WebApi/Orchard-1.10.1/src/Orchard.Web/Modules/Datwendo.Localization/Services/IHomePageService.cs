using Datwendo.Localization.Models;
using Orchard;
using Orchard.Alias;
using Orchard.Autoroute.Models;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Datwendo.Localization.Services
{
    public interface IHomePageService : IDependency 
    {
        bool ShouldBeTranslated(ActionExecutingContext actionContext, out string newUrl);
        bool FindUrlFromFallbackCulture(ContentItem content, string culture, RequestContext requestContext, out string newUrl, int orgItem = -1, IEnumerable<Tuple<string, int>> HPLocalizations = null, string siteCulture = null,bool AddTild= false);
        HomePageSettings GetSettings();
    }
    
    [OrchardFeature("Datwendo.Localization")]
    public class HomePageService : IHomePageService 
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;
        private readonly ICultureService _cultureService;
        private readonly IAliasService _aliasService;
        private readonly ICultureService _CultureService;
        private readonly IOrchardServices _orchardServices;
        public HomePageService(IWorkContextAccessor workContextAccessor, 
                                ICultureService cultureService, 
                                ICacheManager cacheManager,
                                ISignals signals,
                                IOrchardServices orchardServices, 
                                ICultureService CultureService, 
                                IAliasService aliasService)  
        {
            _orchardServices        = orchardServices;
            _CultureService         = CultureService;
            _aliasService           = aliasService;
            Logger                  = NullLogger.Instance;
            _workContextAccessor    = workContextAccessor;
            _cacheManager           = cacheManager;
            _signals                = signals;
            _cultureService         = cultureService;
        }

        public ILogger Logger { get; set; }

        public bool ShouldBeTranslated(ActionExecutingContext actionContext, out string newUrl) 
        {
            newUrl                                              = null;
            var controllerName                                  = actionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var actionName                                      = actionContext.ActionDescriptor.ActionName;
            if (controllerName == null)
                controllerName                                  = string.Empty;
            if (actionName == null)
                actionName      = string.Empty;
            if (!string.Equals(controllerName, "Item", StringComparison.InvariantCultureIgnoreCase)
                    || !string.Equals(actionName, "Display", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
            
            var settings                                        = GetSettings();
            if (settings == null || !settings.Enabled) 
            {
                Logger.Debug("ShouldBeTranslated: Home Pages Settings disabled.");
                return false;
            }
            var currentCulture                                  = _cultureService.GetCurrentCulture();
            if (string.IsNullOrWhiteSpace(currentCulture) )
            {
                Logger.Debug("ShouldBeTranslated: Current Culture null.");
                return false;
            }

            // Look for Home Page
            RouteValueDictionary homepageRouteValueDictionary   = _aliasService.Get(string.Empty);
            int routeId                                         = Convert.ToInt32(homepageRouteValueDictionary["Id"]);
            ContentItem content                                 = _orchardServices.ContentManager.Get(routeId, VersionOptions.Published);
            if (content == null)
            {
                Logger.Debug(string.Format("ShouldBeTranslated: Object not found for Home Page Id={0}",routeId));
                return false;
            }
            IEnumerable<Tuple<string,int>> HPLocalizations      =  _cultureService.GetLocalizationsIds(content);

            //var routeData       = new RouteData();
            int ItemId                                          = -1;
            foreach (var routeValue in actionContext.RequestContext.RouteData.Values)
            {
                //routeData.Values[routeValue.Key] = routeValue.Value;
                if (string.Equals(routeValue.Key, "id", StringComparison.InvariantCultureIgnoreCase))
                {
                    string IdStr                                = routeValue.Value as string;
                    if (IdStr != null)
                    {
                        int Id                                  = -1;
                        if (int.TryParse(IdStr, out Id))
                            ItemId                              = Id;
                    }
                }
            }
            // Is the request concerning homepage ?
            bool isHomepageRequest                              = HPLocalizations.Where(t => t.Item2 == ItemId).Any();
            var context                                         = actionContext.HttpContext;
            if ( context == null )
                context                                         = _workContextAccessor.GetContext().HttpContext;
            if ( ItemId == -1 )
            {
                Logger.Debug(string.Format("ShouldBeTranslated: No contentItem found for route {0}.", (context.Request != null) ? context.Request.Path: "No Request"));
                return false;
            }

            if ( !settings.AllPages && !isHomepageRequest )
            {
                Logger.Debug("ShouldBeTranslated: Settings set for Home Page only.");
                return false;
            }
            
            var siteCulture                                     = _cultureService.GetSiteCulture();
            if (isHomepageRequest)
            {
                if (routeId == ItemId)
                {
                    if (string.Equals(currentCulture, siteCulture, StringComparison.InvariantCultureIgnoreCase))
                    {
                        // this is the correct culture for this Id
                        return false;
                    }
                }
                else
                {
                    // Does the required Id correspond to current culture                    
                    Tuple<string, int> contenTuple              = HPLocalizations.Where(t => t.Item2 == ItemId).FirstOrDefault();
                    if (string.Equals(contenTuple.Item1, currentCulture, StringComparison.InvariantCultureIgnoreCase))
                        return false;
                    //we must find the content for current culture
                }
            }
            else
            {
                // get the required content
                content                                         = _orchardServices.ContentManager.Get(ItemId, VersionOptions.Published);
                if (content == null)
                {
                    Logger.Debug(string.Format("ShouldBeTranslated: Object not found for target Page where Id={0}", ItemId));
                    return false;
                }
                HPLocalizations                                 = _cultureService.GetLocalizationsIds(content);
            }
            // is there a localization for current culture
            Tuple<string,int> contentLoc                        = HPLocalizations.Where(t => t.Item1 == currentCulture).FirstOrDefault();
            if (contentLoc != null)
            {
                if (contentLoc.Item2 == ItemId)
                    return false; // no reroute needed
                return FindRoute(HPLocalizations,ItemId, currentCulture, out newUrl);
            }
            // Invoke the fallback rules
            return FindUrlFromFallbackCulture( content,currentCulture,context.Request.RequestContext, out newUrl,ItemId,HPLocalizations,siteCulture,true);
            /*
            switch( settings.FallBackMode)
            {
                case CultureFallbackMode.FallbackToSite:
                    break;
                case CultureFallbackMode.FallbackToFirstExisting:
                    return false;
                case CultureFallbackMode.ShowExistingTranslations:
                    UrlHelper urlHelper                         = new UrlHelper(context.Request.RequestContext);
                    newUrl                                      = urlHelper.RouteUrl(new { Area = "Datwendo.Localization", Action = "NotTranslated", Controller = "LocalizedHome", Culture = currentCulture, Id = ItemId });
                    return true;
                case CultureFallbackMode.UseRegex:
                    if (!string.IsNullOrWhiteSpace(settings.FallBackRegex))
                    {
                        string regExCulture = _cultureService.CultureFromRegEx(currentCulture, settings.FallBackRegex);
                        if (!string.IsNullOrWhiteSpace(regExCulture))
                            return FindRoute(HPLocalizations, ItemId, regExCulture, out newUrl);
                    }
                    break;
            }
            return FindRoute(HPLocalizations, ItemId,siteCulture, out newUrl);
             * */
        }

        public bool FindUrlFromFallbackCulture(ContentItem content, string culture, RequestContext requestContext, out string newUrl, int orgItem = -1, IEnumerable<Tuple<string, int>> HPLocalizations = null, string siteCulture = null, bool AddTild = false)
        {
            newUrl              = null;
            var settings = GetSettings();
            if (HPLocalizations == null )
                HPLocalizations = _cultureService.GetLocalizationsIds(content);
            if ( string.IsNullOrEmpty(siteCulture) )
                siteCulture     = _cultureService.GetSiteCulture();
            switch (settings.FallBackMode)
            {
                case CultureFallbackMode.FallbackToSite:
                    break;
                case CultureFallbackMode.FallbackToFirstExisting:
                    return false;
                case CultureFallbackMode.ShowExistingTranslations:
                    UrlHelper urlHelper = new UrlHelper(requestContext);
                    newUrl              = urlHelper.RouteUrl(new { Area = "Datwendo.Localization", Action = "NotTranslated", Controller = "LocalizedHome", Culture = culture, Id = content.Id });
                    return true;
                case CultureFallbackMode.UseRegex:
                    if (!string.IsNullOrWhiteSpace(settings.FallBackRegex))
                    {
                        string regExCulture = _cultureService.CultureFromRegEx(culture, settings.FallBackRegex);
                        if (!string.IsNullOrWhiteSpace(regExCulture))
                            return FindRoute(HPLocalizations, orgItem, regExCulture, out newUrl);
                    }
                    break;
            }
            return FindRoute(HPLocalizations, orgItem, siteCulture, out newUrl,AddTild);
        }

        bool FindRoute(IEnumerable<Tuple<string, int>> HPLocalizations,int orgItemId, string culture, out string newUrl,bool AddTild= true)
        {
            newUrl                          = null;
            Tuple<string,int> contentLoc    = HPLocalizations.Where(t => t.Item1 == culture).FirstOrDefault();
            if (contentLoc != null)
            {
                ContentItem content         = _orchardServices.ContentManager.Get(contentLoc.Item2, VersionOptions.Published);
                if (content.Id == orgItemId) // no reroute needed
                    return false;
                AutoroutePart localizedRoutePart = content.Parts.Single(p => p is AutoroutePart).As<AutoroutePart>();
                if (localizedRoutePart != null)
                {
                    string returnUrl        = localizedRoutePart.Path;
                    if (!returnUrl.StartsWith("~/"))
                    {
                        returnUrl = ("~/" + returnUrl).Replace("//","/");
                    }
                    if ( !AddTild)
                    {
                        if (returnUrl.StartsWith("~"))
                        {
                            returnUrl = returnUrl.Substring(1);
                        }
                    }
                    newUrl                  = returnUrl;
                    return true;
                }
            }
            return false;
        }

        public HomePageSettings GetSettings() 
        {
            return _cacheManager.Get("HomePageSettings",
                ctx => {
                    ctx.Monitor(_signals.When(HomePageSettingsPart.CacheKey));
                    var settingsPart = _workContextAccessor.GetContext().CurrentSite.As<HomePageSettingsPart>();
                    return new HomePageSettings 
                    {
                        AllPages        = settingsPart.AllPages,
                        Enabled         = settingsPart.Enabled,
                        FallBackMode    = settingsPart.FallBackMode,
                        MenuMode        = settingsPart.MenuMode,
                        FallBackRegex   = settingsPart.FallBackRegex
                    };
                });
        }
    }
}