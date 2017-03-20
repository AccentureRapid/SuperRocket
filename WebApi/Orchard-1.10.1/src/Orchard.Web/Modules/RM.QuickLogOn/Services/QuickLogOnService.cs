using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Users.Models;
using RM.QuickLogOn.Providers;
using System.Web.Mvc;

namespace RM.QuickLogOn.Services
{
    public interface IQuickLogOnService : IDependency
    {
        IEnumerable<IQuickLogOnProvider> GetProviders();
        QuickLogOnResponse LogOn(QuickLogOnRequest request);
    }

    public class QuickLogOnService : IQuickLogOnService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IMembershipService _membershipService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IEnumerable<IQuickLogOnProvider> _providers = null;

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public QuickLogOnService(IEnumerable<IQuickLogOnProvider> providers,
                                 IMembershipService membershipService,
                                 IAuthenticationService authenticationService,
                                 IOrchardServices orchardServices)
        {
            _providers = providers;
            _membershipService = membershipService;
            _authenticationService = authenticationService;
            _orchardServices = orchardServices;

            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public IEnumerable<IQuickLogOnProvider> GetProviders()
        {
            return _providers;
        }

        public QuickLogOnResponse LogOn(QuickLogOnRequest request)
        {
            var currentUser = _authenticationService.GetAuthenticatedUser();
            if (currentUser != null) _authenticationService.SignOut();

            var userName = request.UserName;
            var lowerEmail = request.Email == null ? "" : request.Email.ToLowerInvariant();

            //var user = _orchardServices.ContentManager.Query<UserPart, UserPartRecord>().Where(u => u.NormalizedUserName == lowerName).List().FirstOrDefault();
            UserPart user = null;
            if (!string.IsNullOrWhiteSpace(userName))
            {
                user = _orchardServices.ContentManager.Query<UserPart, UserPartRecord>().Where(u => u.UserName == userName).List().FirstOrDefault();
            }
            else if (!string.IsNullOrWhiteSpace(lowerEmail))
            {
                user = _orchardServices.ContentManager.Query<UserPart, UserPartRecord>().Where(u => u.Email == lowerEmail).List().FirstOrDefault();
            }

            if (user == null)
            {
                user = _membershipService.CreateUser(new CreateUserParams(userName, Guid.NewGuid().ToString(), lowerEmail, null, null, true)) as UserPart;
                if (user == null)
                {
                    return new QuickLogOnResponse { User = null, Error = T("User can not be created to assign to Quick LogOn credentials") };
                }
            }

            if (user.RegistrationStatus != UserStatus.Approved)
            {
                return new QuickLogOnResponse { User = null, Error = T("User was disabled by site administrator"), ReturnUrl = request.ReturnUrl };
            }

            _authenticationService.SignIn(user, request.RememberMe);

            return new QuickLogOnResponse { User = user, ReturnUrl = request.ReturnUrl };
        }
    }
}
