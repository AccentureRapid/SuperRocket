//using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Orchard.Caching;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Services;
using Orchard.Settings;
using Orchard.WebApi.Filters;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Orchard.SuperRocket.Extension
{
    //[UsedImplicitly]
    public class AuthorizeAppApiFilter : IAuthorizationFilter, IApiFilterProvider
    {
        private readonly IOrchardServices _orchardServices;

        private readonly ISiteService _siteService;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;
        private IClock _clock;

        public const string TokenSignal = "Orchard.Accenture.Event.Token.Signal";

        public AuthorizeAppApiFilter(
            ISiteService siteService,
            ICacheManager cacheManager,
            ISignals signals,
            IClock clock,
            IOrchardServices orchardServices)
        {
            _siteService = siteService;
            _cacheManager = cacheManager;
            _signals = signals;
            _clock = clock;
            _orchardServices = orchardServices;

            Logger = NullLogger.Instance;
        }

        public bool AllowMultiple
        {
            get
            {
                return true;
            }
        }

        public ILogger Logger { get; set; }
                
        /// <summary>
        /// https://federation-sts.accenture.com/services/jwt/issue/adfs Production
        /// https://federation-sts-stage.accenture.com/services/jwt/issue/adfs Staging
        /// Skips authorization for local orchard user
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="continuation"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            //var attribute = GetAuthorizeAppAttribute(actionContext);

            //if (attribute == null)
            //    return continuation();

            //var request = actionContext.Request;
            //HttpResponseMessage response = new HttpResponseMessage();

            //if (request.Headers.Authorization == null || string.IsNullOrEmpty(request.Headers.Authorization.Parameter)) { 
            //    // Check if local orchard user is null
            //    IUser user = _orchardServices.WorkContext.CurrentUser;
            //    if (user == null)
            //    {
            //        response.StatusCode = HttpStatusCode.Forbidden;
            //    }
            //}
            //else
            //{
            //    string jwt = request.Headers.Authorization.Parameter;

            //var part = _siteService.GetSiteSettings().As<PeopleServiceSettingsPart>();
            //string endPoint = string.IsNullOrEmpty(part.Endpoint) ? "https://federation-sts.accenture.com/services/jwt/issue/adfs" : part.Endpoint;
            //string scope = string.IsNullOrEmpty(part.Scope) ? "https://federation-sts.accenture.com/demo/api/" : part.Scope;

            //    try
            //    {
            //        var result = _cacheManager.Get(jwt, ctx =>
            //        {
            //            ctx.Monitor(_clock.When(TimeSpan.FromMinutes(59)));
            //            return GetESOResponse(endPoint, jwt, scope);
            //        });

            //        response.StatusCode = string.IsNullOrEmpty(result) ? HttpStatusCode.Forbidden : HttpStatusCode.OK;
            //    }
            //    catch (Exception ex)
            //    {
            //        Logger.Error("Error when receive response from ESO end point :" + ex.Message + "\r\n Token :" + jwt);
            //        response.StatusCode = HttpStatusCode.Forbidden;
            //    }
            //}

            //if (response.StatusCode == HttpStatusCode.OK)
            //    return continuation();

            //return Task.FromResult<HttpResponseMessage>(response);            
            return continuation();
        }

        private string GetEid(HttpRequestMessage request)
        {
            string eid = string.Empty;
            var jsob = getObjectFromJWT(request);

            if (jsob != null)
            {
                eid = jsob.GetValue("https://federation-sts.accenture.com/schemas/claims/1/enterpriseid").ToString();
            }
            else 
            {
                // Get eid from current orchard user
                IUser user = _orchardServices.WorkContext.CurrentUser;
                eid = user.UserName;
            }
            return eid;
        }

        private JObject getObjectFromJWT(HttpRequestMessage request)
        {
            if (request.Headers.Authorization != null)
            {
                string jwt = request.Headers.Authorization.Parameter;

                if (!string.IsNullOrWhiteSpace(jwt))
                {
                    string[] tokens = jwt.Split('.');

                    if (tokens != null && tokens.Length == 3)
                    {
                        string claims = tokens[1];
                        int mod4 = claims.Length % 4;

                        if (mod4 > 0)
                        {
                            claims += new string('=', 4 - mod4);
                        }

                        return (JObject)JsonConvert.DeserializeObject(Encoding.ASCII.GetString(Convert.FromBase64String(claims)));
                    }
                }
            }

            return null;

        }

        private static AuthorizeAppApiAttribute GetAuthorizeAppAttribute(HttpActionContext context)
        {
            var attribute = context.ActionDescriptor.GetCustomAttributes<AuthorizeAppApiAttribute>(true)
                .Concat(context.ControllerContext.ControllerDescriptor.GetCustomAttributes<AuthorizeAppApiAttribute>(true)
                .OfType<AuthorizeAppApiAttribute>()).FirstOrDefault();

            return attribute;
        }

        private string GetESOResponse(string endPoint,string jwt,string scope)
        {
            // Create a request using a URL that can receive a post. 
            WebRequest esoRequest = WebRequest.Create(endPoint);
            // Set the Method property of the request to POST.
            esoRequest.Method = "POST";
            // Create POST data and convert it to a byte array.
            string postData = string.Format("grant_type={0}&assertion={1}&scope={2}", "urn:ietf:params:oauth:grant-type:jwt-bearer", jwt, scope);
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            esoRequest.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.
            esoRequest.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = esoRequest.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();

            // Get the response.
            WebResponse esoResponse = esoRequest.GetResponse();
            // Display the status.
            Console.WriteLine(((HttpWebResponse)esoResponse).StatusDescription);
            // Get the stream containing content returned by the server.
            dataStream = esoResponse.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();

            JObject jsob = (JObject)JsonConvert.DeserializeObject(responseFromServer);

            var accessToken = jsob["access_token"].ToString();
           
            reader.Close();
            dataStream.Close();
            esoResponse.Close();

            return accessToken;
        }

        private class PeopleServiceSettingsPart
        {
        }

        #region sample
        //public Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        //{
        //    var attribute = GetAuthorizeAppAttribute(actionContext);

        //    var request = actionContext.Request;

        //    if (request.RequestUri.Scheme != Uri.UriSchemeHttps && attribute != null)
        //    {
        //        HttpResponseMessage response = request.CreateResponse(HttpStatusCode.Forbidden);
        //        response.Content = new StringContent("<h1>HTTPS Required</h1>", Encoding.UTF8, "text/html");
        //        actionContext.Response = response;

        //        return Task.FromResult<HttpResponseMessage>(response);
        //    }
        //    else
        //        return continuation();

        //} 
        #endregion
    }

}