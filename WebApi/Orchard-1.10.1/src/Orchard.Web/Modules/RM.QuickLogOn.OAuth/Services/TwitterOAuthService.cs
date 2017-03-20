using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Logging;
using RM.QuickLogOn.OAuth.Models;
using RM.QuickLogOn.Providers;
using RM.QuickLogOn.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.Security;

namespace RM.QuickLogOn.OAuth.Services
{
    public interface ITwitterOAuthService : IDependency
    {
        RequestTokenModel RequestToken(WorkContext wc);
        QuickLogOnResponse Auth(WorkContext wc, string oauthToken, string oauthVerifier);
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.Twitter")]
    public class TwitterOAuthService : ITwitterOAuthService
    {
        private const string RequestTokenUrl = "https://api.twitter.com/oauth/request_token";
        private const string AuthorizationHeaderValue = "OAuth oauth_consumer_key=\"{1}\", oauth_nonce=\"{2}\", oauth_signature=\"{3}\", oauth_signature_method=\"HMAC-SHA1\", oauth_timestamp=\"{4}\", oauth_token=\"{5}\", oauth_version=\"1.0\"";
        private const string ParametersSignature = "oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method=HMAC-SHA1&oauth_timestamp={2}&oauth_token={3}&oauth_version=1.0";

        private const string RequestAccessTokenUrl = "https://api.twitter.com/oauth/access_token";
        private const string AccessAuthorizationHeaderValue = "OAuth oauth_consumer_key=\"{0}\", oauth_nonce=\"{1}\", oauth_signature_method=\"HMAC-SHA1\", oauth_timestamp=\"{2}\", oauth_signature=\"{3}\", oauth_token=\"{4}\", oauth_version=\"1.0\"";
        private const string AccessParametersSignature = "oauth_consumer_key={0}&oauth_signature_method=HMAC-SHA1&oauth_timestamp={1}&oauth_token={2}&oauth_verifier={3}&oauth_version=1.0";

        private readonly IQuickLogOnService _quickLogOnService;
        private readonly IEncryptionService _oauthHelper;

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public TwitterOAuthService(IEncryptionService oauthHelper, IQuickLogOnService quickLogOnService)
        {
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            _quickLogOnService = quickLogOnService;
            _oauthHelper = oauthHelper;
        }

        public RequestTokenModel RequestToken(WorkContext wc)
        {
            var part = wc.CurrentSite.As<TwitterSettingsPart>();
            var consumerKey = part.ConsumerKey;
            var accessToken = part.AccessToken;
            var consumerSecret = _oauthHelper.Decrypt(part.Record.EncryptedConsumerSecret);

            var urlHelper = new UrlHelper(wc.HttpContext.Request.RequestContext);
            var redirectUrl =
                new Uri(wc.HttpContext.Request.Url,
                        urlHelper.Action("Auth", "TwitterOAuth", new { Area = "RM.QuickLogOn.OAuth" })).ToString();

            var wr = WebRequest.Create(RequestTokenUrl);
            wr.Method = "POST";

            var ts = GetTimeStamp();
            var nonce = GenerateNonce();

            var sig = string.Format("{0}&{1}&{2}", wr.Method, Uri.EscapeDataString(RequestTokenUrl), Uri.EscapeDataString(string.Format(ParametersSignature, consumerKey, nonce, ts, accessToken)));

            sig = OAuthHelper.HMACSHA1(sig, consumerSecret + "&");

            var header = string.Format(AuthorizationHeaderValue, Uri.EscapeDataString(redirectUrl), consumerKey, nonce, Uri.EscapeDataString(sig), ts, accessToken);

            wr.Headers[HttpRequestHeader.Authorization] = header;
            try
            {
                var wres = wr.GetResponse();
                using (var stream = wres.GetResponseStream())
                using (var sr = new StreamReader(stream))
                {
                    var result = HttpUtility.ParseQueryString(sr.ReadToEnd());
                    bool callbackConfirmed = bool.TryParse(result["oauth_callback_confirmed"], out callbackConfirmed) && callbackConfirmed;
                    if (!callbackConfirmed) throw new AccessViolationException("Callback isn't confirmed");
                    return new RequestTokenModel{
                        Token = result["oauth_token"],
                        TokenSecret = result["oauth_token_secret"]
                    };
                }
            }
            catch (Exception ex)
            {
                var error = OAuthHelper.ReadWebExceptionMessage(ex);
                Logger.Error(ex, error ?? ex.Message);
                return new RequestTokenModel { Error = error ?? ex.Message };
            }
        }

        private AccessTokenModel RequestAccessToken(WorkContext wc, string oauthToken, string oauthVerifier)
        {
            var part = wc.CurrentSite.As<TwitterSettingsPart>();
            var consumerKey = part.ConsumerKey;
            var consumerSecret = _oauthHelper.Decrypt(part.Record.EncryptedConsumerSecret);

            var urlHelper = new UrlHelper(wc.HttpContext.Request.RequestContext);
            var redirectUrl =
                new Uri(wc.HttpContext.Request.Url,
                        urlHelper.Action("Auth", "TwitterOAuth", new { Area = "RM.QuickLogOn.OAuth" })).ToString();

            var wr = WebRequest.Create(RequestAccessTokenUrl);
            wr.Method = "POST";

            var ts = GetTimeStamp();
            var nonce = GenerateNonce();

            var sig = string.Format("{0}&{1}&{2}", wr.Method, Uri.EscapeDataString(RequestAccessTokenUrl), Uri.EscapeDataString(string.Format(AccessParametersSignature, consumerKey, ts, oauthToken, oauthVerifier)));

            sig = OAuthHelper.HMACSHA1(sig, consumerSecret + "&");

            var header = string.Format(AccessAuthorizationHeaderValue, consumerKey, nonce, ts, Uri.EscapeDataString(sig), oauthToken);

            wr.Headers[HttpRequestHeader.Authorization] = header;
            try
            {
                using(var wreq = wr.GetRequestStream())
                using(var sw = new StreamWriter(wreq))
                {
                    sw.Write("oauth_verifier={0}", oauthVerifier);
                }

                var wres = wr.GetResponse();
                using (var stream = wres.GetResponseStream())
                using (var sr = new StreamReader(stream))
                {
                    var result = HttpUtility.ParseQueryString(sr.ReadToEnd());
                    return new AccessTokenModel {
                        AccessToken = result["oauth_token"],
                        AccessTokenSecret = result["oauth_token_secret"],
                        UserId = result["user_id"],
                        ScreenName = result["screen_name"]
                    };
                }
            }
            catch (Exception ex)
            {
                var error = OAuthHelper.ReadWebExceptionMessage(ex);
                Logger.Error(ex, error ?? ex.Message);
                return new AccessTokenModel { Error = error ?? ex.Message };
            }
        }

        public QuickLogOnResponse Auth(WorkContext wc, string oauthToken, string oauthVerifier)
        {
            if (!string.IsNullOrWhiteSpace(oauthToken) && !string.IsNullOrWhiteSpace(oauthVerifier))
            {
                var accessToken = RequestAccessToken(wc, oauthToken, oauthVerifier);
                if (string.IsNullOrWhiteSpace(accessToken.Error))
                {
                    return _quickLogOnService.LogOn(new QuickLogOnRequest
                    {
                        UserName = accessToken.User,
                        Email = accessToken.User,
                        RememberMe = false,
                        ReturnUrl = "~/"
                    });
                }
            }
            return new QuickLogOnResponse { Error = T("Invalid OAuth token") };
        }

        private static string GenerateNonce()
        {
            var rnd = new Random(DateTime.Now.Second);
            return new byte[16].Aggregate(string.Empty, (a, b) => a + rnd.Next(255).ToString("x2"));
        }

        private static string GetTimeStamp()
        {
            return (DateTime.UtcNow - new DateTime(1970, 01, 01)).TotalSeconds.ToString("#");
        }
    }
}
