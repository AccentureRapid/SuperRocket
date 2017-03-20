using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using RM.QuickLogOn.OAuth.Models;
using RM.QuickLogOn.Providers;

namespace RM.QuickLogOn.OAuth.Providers
{
    [OrchardFeature("RM.QuickLogOn.OAuth.Facebook")]
    public class FacebookOAuthProvider : IQuickLogOnProvider
    {
        public const string Url = "https://www.facebook.com/dialog/oauth?client_id={0}&response_type=code&scope=email&redirect_uri={1}&state={2}";

        public string Name
        {
            get { return "Facebook"; }
        }

        public string Description
        {
            get { return "LogOn with Your Facebook account"; }
        }

        public string GetLogOnUrl(WorkContext context)
        {
            var urlHelper = new UrlHelper(context.HttpContext.Request.RequestContext);
            var part = context.CurrentSite.As<FacebookSettingsPart>();
            var clientId = part.ClientId;
            var returnUrl = context.HttpContext.Request.Url;
            var redirectUrl = new Uri(returnUrl, urlHelper.Action("Auth", "FacebookOAuth", new { Area = "RM.QuickLogOn.OAuth" })).ToString();//, returnUrl = returnUrl
            return string.Format(Url, clientId, urlHelper.Encode(redirectUrl), urlHelper.Encode(returnUrl.ToString()));
        }
    }
}
