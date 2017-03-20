using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Orchard.SuperRocket.Extension
{

    public class CustomAuthorizeAttribute : AuthorizationFilterAttribute
    {

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            //var query = actionContext.Request.RequestUri.ParseQueryString();
            //var apiKey = query["ClientId"];
            //var workContext = actionContext.ControllerContext.GetWorkContext();
            

            //if (apiKey == null)
            //{
            //    actionContext.Response = actionContext.ControllerContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            //}
        }
    }
}