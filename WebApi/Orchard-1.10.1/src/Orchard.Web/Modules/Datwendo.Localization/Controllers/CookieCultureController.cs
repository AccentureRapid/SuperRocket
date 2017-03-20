using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Extensions;
using Orchard.Themes;
using Datwendo.Localization.Services;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Web;
using Orchard.Services;

namespace Datwendo.Localization.Controllers
{
    [HandleError, Themed]
    [OrchardFeature("Datwendo.Localization.CookieCultureSelector")]
    public class CookieCultureController : Controller
    {
        private readonly ICookieCultureService _cookieCultureService;
        private readonly IClock _clock;
        private static string RefreshKey    = "__r";
        private static long Epoch           = new DateTime(2014, DateTimeKind.Utc).Ticks;

        public CookieCultureController(ICookieCultureService cookieCultureService,IClock clock)
        {
            _cookieCultureService   = cookieCultureService;
            _clock                  = clock;
        }

        [HttpGet]
        public ActionResult SetCulture(string culture, string returnUrl) {
            _cookieCultureService.SetCulture(culture);
            return this.RedirectLocal(MakeUniqueUrl(returnUrl));
        }

        private string MakeUniqueUrl(string url)
        {
            var uri                 = new UriBuilder(new Uri(Request.Url, url));
            var redirectUrl         = uri.ToString();
            if (_cookieCultureService.GetSettings().EnforceCookieUrl)
            {
                
                var epIndex         = redirectUrl.IndexOf('?');
                var qs              = new NameValueCollection();
                if (epIndex > 0)
                {
                    qs = HttpUtility.ParseQueryString(redirectUrl.Substring(epIndex));
                }

                // substract Epoch to get a smaller number
                var refresh         = _clock.UtcNow.Ticks - Epoch;
                qs.Remove(RefreshKey);

                qs.Add(RefreshKey, refresh.ToString("x"));
                var querystring = "?" + string.Join("&", Array.ConvertAll(qs.AllKeys, k => string.Format("{0}={1}", HttpUtility.UrlEncode(k), HttpUtility.UrlEncode(qs[k]))));

                if (epIndex > 0)
                {
                    redirectUrl = redirectUrl.Substring(0, epIndex) + querystring;
                }
                else
                {
                    redirectUrl = redirectUrl + querystring;
                }
            }
            return redirectUrl;
        }
    }
}
