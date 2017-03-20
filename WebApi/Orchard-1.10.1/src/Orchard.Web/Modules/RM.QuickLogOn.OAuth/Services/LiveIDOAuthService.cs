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

namespace RM.QuickLogOn.OAuth.Services
{
    public interface ILiveIDOAuthService : IDependency
    {
        QuickLogOnResponse Auth(WorkContext wc, string code, string error, string returnUrl);
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.LiveID")]
    public class LiveIDOAuthService : ILiveIDOAuthService
    {
        public const string TokenRequestUrl = "https://login.live.com/oauth20_token.srf?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}&grant_type=authorization_code";
        public const string EmailRequestUrl = "https://apis.live.net/v5.0/me?access_token={0}";

        private readonly IQuickLogOnService _quickLogOnService;
        private readonly IEncryptionService _oauthHelper;

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public LiveIDOAuthService(IEncryptionService oauthHelper, IQuickLogOnService quickLogOnService)
        {
            _quickLogOnService = quickLogOnService;
            _oauthHelper = oauthHelper;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        private string GetAccessToken(WorkContext wc, string code, string returnUrl)
        {
            try
            {
                var part = wc.CurrentSite.As<LiveIDSettingsPart>();
                var clientId = part.ClientId;
                var clientSecret = _oauthHelper.Decrypt(part.Record.EncryptedClientSecret);

                var urlHelper = new UrlHelper(wc.HttpContext.Request.RequestContext);
                var redirectUrl =
                    new Uri(wc.HttpContext.Request.Url,
                            urlHelper.Action("Auth", "LiveIDOAuth", new { Area = "RM.QuickLogOn.OAuth", ReturnUrl = returnUrl })).ToString();

                var url = string.Format(TokenRequestUrl, 
                                        urlHelper.Encode(clientId), 
                                        urlHelper.Encode(redirectUrl), 
                                        urlHelper.Encode(clientSecret), 
                                        urlHelper.Encode(code));

                var wr = WebRequest.Create(url);
                wr.Proxy = OAuthHelper.GetProxy();

                //if (ServicePointManager.ServerCertificateValidationCallback == null) ServicePointManager.ServerCertificateValidationCallback = ((sender, cert, chain, errors) => true);

                var wres = wr.GetResponse();
                using (var stream = wres.GetResponseStream())
                {
                    var result = OAuthHelper.FromJson<LiveIDAccessTokenJsonModel>(stream);
                    return result.access_token;
                }
            }
            catch (Exception ex)
            {
                var wex = ex as WebException;
                string error = null;
                if (wex != null && wex.Response != null)
                {
                    using (var stream = wex.Response.GetResponseStream())
                    {
                        if (stream != null) using (var sr = new StreamReader(stream)) error = sr.ReadToEnd();
                    }
                }
                Logger.Error(ex, error ?? ex.Message);
            }
            return null;
        }

        private string GetEmailAddress(string token)
        {
            try
            {
                var wr = WebRequest.Create(string.Format(EmailRequestUrl, token));
                wr.Method = "GET";
                wr.Proxy = OAuthHelper.GetProxy();
                var wres = wr.GetResponse();
                using (var stream = wres.GetResponseStream())
                {
                    var result = OAuthHelper.FromJson<LiveIDEmailAddressJsonViewModel>(stream);
                    return result != null && result.emails != null ? result.emails.preferred ?? (result.emails.account ?? (result.emails.personal ?? result.emails.personal)) : null;
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
            if (string.IsNullOrEmpty(code) || !string.IsNullOrEmpty(error))
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
            return new QuickLogOnResponse { Error = T("LogOn through LiveID failed: {0}", error), ReturnUrl = returnUrl };
        }
    }
}
