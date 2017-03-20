using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using RM.QuickLogOn.OAuth.Models;
using RM.QuickLogOn.Providers;
using System.Web.Mvc;

namespace RM.QuickLogOn.OAuth.Providers
{
    [OrchardFeature("RM.QuickLogOn.OAuth.LiveID")]
    public class LiveIDOAuthProvider : IQuickLogOnProvider
    {
        public const string Url = "https://login.live.com/oauth20_authorize.srf?client_id={0}&scope=wl.signin,wl.emails&response_type=code&redirect_uri={1}";

        public string Name
        {
            get { return "LiveID"; }
        }

        public string Description
        {
            get { return "LogOn with Your LiveID account"; }
        }

        public string GetLogOnUrl(WorkContext context)
        {
            var urlHelper = new UrlHelper(context.HttpContext.Request.RequestContext);
            var part = context.CurrentSite.As<LiveIDSettingsPart>();
            var clientId = part.ClientId;
            var returnUrl = context.HttpContext.Request.Url;
            var redirectUrl = new Uri(returnUrl, urlHelper.Action("Auth", "LiveIDOAuth", new { Area = "RM.QuickLogOn.OAuth", ReturnUrl = returnUrl })).ToString();
            return string.Format(Url, clientId, urlHelper.Encode(redirectUrl));
        }
    }
}
