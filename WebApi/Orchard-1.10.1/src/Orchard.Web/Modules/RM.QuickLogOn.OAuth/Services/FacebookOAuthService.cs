using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Security;
using RM.QuickLogOn.OAuth.Models;
using System.Net;
using RM.QuickLogOn.OAuth.ViewModels;
using RM.QuickLogOn.Providers;
using RM.QuickLogOn.Services;
using System.Web;
using System.Text.RegularExpressions;

namespace RM.QuickLogOn.OAuth.Services
{
    public interface IFacebookOAuthService : IDependency
    {
        string GetAccessTokenUrl(WorkContext wc, string code, string error, string returnUrl);
        QuickLogOnResponse Auth(WorkContext wc, string code, string error, string returnUrl);
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.Facebook")]
    public class FacebookOAuthService : IFacebookOAuthService
    {
        public const string TokenRequestUrl = "https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}";
        public const string EmailRequestUrl = "https://graph.facebook.com/me?access_token={0}";

        private readonly IQuickLogOnService _quickLogOnService;
        private readonly IEncryptionService _oauthHelper;

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public FacebookOAuthService(IEncryptionService oauthHelper, IQuickLogOnService quickLogOnService)
        {
            _quickLogOnService = quickLogOnService;
            _oauthHelper = oauthHelper;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public string GetAccessToken(WorkContext wc, string code, string returnUrl)
        {
            try
            {
                var part = wc.CurrentSite.As<FacebookSettingsPart>();
                var clientId = part.ClientId;
                var clientSecret = _oauthHelper.Decrypt(part.Record.EncryptedClientSecret);

                var urlHelper = new UrlHelper(wc.HttpContext.Request.RequestContext);
                var redirectUrl =
                    new Uri(wc.HttpContext.Request.Url,
                            urlHelper.Action("Auth", "FacebookOAuth", new { Area = "RM.QuickLogOn.OAuth" })).ToString();//, returnUrl = returnUrl
                var url = string.Format(TokenRequestUrl, urlHelper.Encode(clientId), urlHelper.Encode(redirectUrl), urlHelper.Encode(clientSecret), urlHelper.Encode(code));
                var wr = WebRequest.Create(url);
                wr.Proxy = OAuthHelper.GetProxy();
                wr.Method = "GET";
                var wres = wr.GetResponse();
                using (var stream = wres.GetResponseStream())
                using (var sr = new StreamReader(stream))
                {
                    var result = HttpUtility.ParseQueryString(sr.ReadToEnd());
                    return result["access_token"];
                }
                }
            catch (Exception ex)
            {
                var wex = ex as WebException;
                string error = null;
                if (wex != null && wex.Response != null)
                {
                    using(var stream = wex.Response.GetResponseStream())
                    {
                        if (stream != null) using(var sr = new StreamReader(stream)) error = sr.ReadToEnd();
                    }
                }
                Logger.Error(ex, string.IsNullOrEmpty(error) ? ex.Message : error);
            }
            
            return null;
        }

        public string GetEmailAddress(string token)
        {
            try
            {
                var wr = WebRequest.Create(string.Format(EmailRequestUrl, token));
                wr.Method = "GET";
                wr.Proxy = OAuthHelper.GetProxy();
                var wres = wr.GetResponse();
                using (var stream = wres.GetResponseStream())
                {
                    var result = OAuthHelper.FromJson<FacebookEmailAddressJsonViewModel>(stream);
                    return result != null && result.verified ? result.email : null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
            }
            return null;
        }

        public QuickLogOnResponse Auth(WorkContext wc, string code, string error, string returnUrl)
        {
            if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(error))
            {
                error = "invalid code";
            }
            else
            {
                var token = GetAccessToken(wc, code, returnUrl);
                if (!string.IsNullOrEmpty(token))
                {
                    var email = GetEmailAddress(token);
                    if (!string.IsNullOrEmpty(email))
                    {
                        return _quickLogOnService.LogOn(new QuickLogOnRequest
                        {
                            UserName = email,
                            Email = email,
                            RememberMe = false,
                            ReturnUrl = returnUrl
                        });
                    }
                    error = "invalid email";
                }
                else
                {
                    error = "invalid token";
                }
            }
            return new QuickLogOnResponse { Error = T("LogOn through Google failed: {0}", error), ReturnUrl = returnUrl };
        }

        public string GetAccessTokenUrl(WorkContext wc, string code, string error, string returnUrl)
        {
            var part = wc.CurrentSite.As<FacebookSettingsPart>();
            var clientId = part.ClientId;
            var clientSecret = _oauthHelper.Decrypt(part.Record.EncryptedClientSecret);

            var urlHelper = new UrlHelper(wc.HttpContext.Request.RequestContext);
            var redirectUrl =
                new Uri(wc.HttpContext.Request.Url,
                        urlHelper.Action("Auth", "FacebookOAuth", new { Area = "RM.QuickLogOn.OAuth", returnUrl = returnUrl })).ToString();
            return string.Format(TokenRequestUrl, clientId, urlHelper.Encode(redirectUrl), clientSecret, code);
        }
    }
}
