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
using Orchard.Settings;
using Orchard.Caching;
using Orchard.Services;
using Orchard.SuperRocket.Common;

namespace Orchard.SuperRocket.Services
{
    public class HtmlModuleService : IHtmlModuleService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IContentManager _contentManager;
        private readonly ISiteService _siteService;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;
        private IClock _clock;

        public HtmlModuleService(
            IOrchardServices orchardServices,
            IContentManager contentManager,
            ISiteService siteService,
            ICacheManager cacheManager,
            ISignals signals,
            IClock clock) {

            _orchardServices = orchardServices;
            _contentManager = contentManager;
            _siteService = siteService;
            _cacheManager = cacheManager;
            _signals = signals;
            _clock = clock;

            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public dynamic GetAvailableHtmlModules()
        {
            var result = _contentManager.Query("HtmlModule").List().Select(contentItem => new
            {
                Title = ((dynamic)contentItem).HtmlModulePart.Title.Value,
                Author = ((dynamic)contentItem).HtmlModulePart.Author.Value,
                Url = ((dynamic)contentItem).HtmlModulePart.Url.Value,
                Description = ((dynamic)contentItem).HtmlModulePart.Description.Value,
                File = GetBaseUrl() + ((dynamic)contentItem).HtmlModulePart.HtmlModuleFile.FirstMediaUrl ?? string.Empty,
                Ico = GetBaseUrl() +((dynamic)contentItem).HtmlModulePart.HtmlModuleIco.FirstMediaUrl ?? string.Empty
            });
            return result;
        }


        private string GetBaseUrl()
        {
            var result = _cacheManager.Get(CacheAndSignals.BaseUrlCache, ctx =>
            {
                ctx.Monitor(_clock.When(TimeSpan.FromMinutes(999)));
                return GetBaseUrlToCache();
            });
            return result;
        }

        private string GetBaseUrlToCache()
        {
            return _siteService.GetSiteSettings().BaseUrl;
        }
    }
}