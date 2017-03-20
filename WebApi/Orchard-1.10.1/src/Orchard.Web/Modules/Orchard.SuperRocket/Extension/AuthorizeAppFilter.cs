using System.Linq;
using Orchard.Logging;
using System.Web.Mvc;
using Orchard.Mvc.Filters;
//using JetBrains.Annotations;
using Orchard.WebApi.Filters;

namespace Orchard.SuperRocket.Extension
{
    //[UsedImplicitly]
    public class AuthorizeAppFilter : FilterProvider, IAuthorizationFilter, IApiFilterProvider
    {

        public AuthorizeAppFilter(
            )
        {
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        private static AuthorizeAppAttribute GetAuthorizeAppAttribute(ActionDescriptor descriptor)
        {

            //return descriptor.GetCustomAttributes(typeof(AuthorizeAppAttribute), true).FirstOrDefault()  as AuthorizeAppAttribute;

            return descriptor.GetCustomAttributes(typeof(AuthorizeAppAttribute), true)
                .Concat(descriptor.ControllerDescriptor.GetCustomAttributes(typeof(AuthorizeAppAttribute), true))
                .OfType<AuthorizeAppAttribute>()
                .FirstOrDefault();
        }

        public void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {
            var attribute = GetAuthorizeAppAttribute(filterContext.ActionDescriptor);
            if (attribute != null)
            {
                var context = filterContext.HttpContext;

                string[] values = context.Request.Headers.GetValues("ClientId");

                if (values != null && values.Count() > 0)
                {
                    string clientId = values.FirstOrDefault();

                    //var app = _service.GetApp(clientId);
                    //if (app == null)
                    //{
                        filterContext.Result = new HttpUnauthorizedResult();
                    //}
                } 
            }
        }

    }

}