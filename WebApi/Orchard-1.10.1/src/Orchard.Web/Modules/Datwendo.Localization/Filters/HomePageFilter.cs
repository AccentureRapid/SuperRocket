using System.Collections.Specialized;
using System.Web.Mvc;
using Orchard.Localization;
using Orchard.Mvc.Filters;
using Orchard.Localization.Services;
using Orchard.UI.Notify;
using Datwendo.Localization.Services;
using Orchard;

namespace Datwendo.Localization.Filters
{
    public class HomePageFilter : FilterProvider, IActionFilter {
        private readonly IHomePageService _homePageService;
        private readonly IOrchardServices _orchardServices;

        public HomePageFilter(IHomePageService homePageService, IOrchardServices orchardServices)
        {
            _homePageService = homePageService;
            _orchardServices = orchardServices;
        }
        public Localizer T { get; set; }

        public void OnActionExecuted(ActionExecutedContext filterContext) {
            var settings = _homePageService.GetSettings();

            if (!settings.Enabled) {
                _orchardServices.Notifier.Warning(T("You need to configure the Homepage settings."));
            }
        }

        public void OnActionExecuting(ActionExecutingContext filterContext) 
        {
            var settings = _homePageService.GetSettings();

            if (!settings.Enabled) 
            {
                return;
            }
            string newUrl = null;
            if (_homePageService.ShouldBeTranslated(filterContext,out newUrl) )
            {
                var request             = filterContext.HttpContext.Request;
                var TranslatedActionUrl = AppendQueryString(request.QueryString,newUrl);
                filterContext.Result    = new RedirectResult(TranslatedActionUrl);
                return;
            }
        }

        private static string AppendQueryString(NameValueCollection queryString, string url) {
            if (queryString.Count > 0) {
                url += '?' + queryString.ToString();
            }
            return url;
        }
    }
}