using System.Web.Mvc;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Filters;

namespace dcp.Routing.Services
{
    [OrchardFeature("dcp.Routing.Redirects")]
    public class RedirectFilter : FilterProvider, IActionFilter
    {
        private readonly IRoutingAppService _routingAppService;

        public RedirectFilter(IRoutingAppService routingAppService)
        {
            _routingAppService = routingAppService;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var url = filterContext.RequestContext.HttpContext.Request.Url;
            if (url == null)
                return;

            var redirect = _routingAppService.GetRedirect(url.AbsolutePath);

            if (redirect == null)
                return;
            
            filterContext.Result = new RedirectResult("/" + redirect.DestinationUrl + url.Query, redirect.IsPermanent);
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}