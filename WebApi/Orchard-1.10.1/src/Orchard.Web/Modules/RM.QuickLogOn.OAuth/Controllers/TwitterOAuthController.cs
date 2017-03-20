using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Themes;
using Orchard.UI.Notify;
using RM.QuickLogOn.OAuth.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Orchard.Mvc.Extensions;
using Orchard.Localization;

namespace RM.QuickLogOn.OAuth.Controllers
{
    [HandleError, Themed]
    [OrchardFeature("RM.QuickLogOn.OAuth.Twitter")]
    public class TwitterOAuthController : Controller
    {
        private string AuthenticateUrl = "https://api.twitter.com/oauth/authenticate?oauth_token={0}";

        public Localizer T { get; set; }
        private ITwitterOAuthService _twitterOAuthService;
        private IOrchardServices _services;

        public TwitterOAuthController(ITwitterOAuthService twitterOAuthService, IOrchardServices services)
        {
            T = NullLocalizer.Instance;
            _twitterOAuthService = twitterOAuthService;
            _services = services;
        }

        public ActionResult RequestToken(string returnUrl)
        {
            var token = _twitterOAuthService.RequestToken(_services.WorkContext);
            if (string.IsNullOrWhiteSpace(token.Error))
            {
                return Redirect(string.Format(AuthenticateUrl, token.Token));
            }

            _services.Notifier.Add(NotifyType.Error, T(token.Error));
            return this.RedirectLocal(returnUrl);
        }

        public ActionResult Auth(string oauth_token, string oauth_verifier)
        {
            var response = _twitterOAuthService.Auth(_services.WorkContext, oauth_token, oauth_verifier);
            if (response.Error != null)
            {
                _services.Notifier.Add(NotifyType.Error, response.Error);
            } 

            return this.RedirectLocal("~/");
        }
    }
}
