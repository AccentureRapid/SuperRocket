using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Security;
using Orchard;
using Orchard.Logging;
using Orchard.Localization;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Core.Common.Models;
using Orchard.Core.Common.Fields;

using Newtonsoft.Json.Linq;
using System.Text;

namespace Orchard.SuperRocket.Services
{
    public class ContentTypesService : IContentTypesService
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipService _membershipService;
        private readonly IOrchardServices _orchardServices;
        private readonly IContentManager _contentManager;

        public ContentTypesService(
            IAuthenticationService authenticationService, 
            IMembershipService membershipService,
            IOrchardServices orchardServices,
            IContentManager contentManager) {
            _authenticationService = authenticationService;
            _membershipService = membershipService;
            _orchardServices = orchardServices;
            _contentManager = contentManager;

            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }


        public IEnumerable<string> GetContentTypes()
        {
            try
            {
                var contentTypes = _contentManager.GetContentTypeDefinitions();
                return from c in contentTypes
                       select c.Name;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "An error occurred while get content types.");
                throw;
            }
        }

        public dynamic Publish(string name)
        {
            try
            {
                var contentItems = _contentManager.Query(name);
                int count = 0;
                foreach (var item in contentItems.List())
                {
                    _contentManager.Publish(item);
                    count++;
                }
                return count;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "An error occurred while Publish content items for ContentType named: [{0}].", name);
                throw;
            }

        }
    }
}