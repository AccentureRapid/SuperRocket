using Datwendo.Localization.Models;
using Orchard;
using Orchard.Alias;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Environment.Extensions;
using Orchard.UI.Admin;
using System;
using System.Web;


namespace Datwendo.Localization.Services
{
    public interface IContentCultureService : IDependency 
    {
        string GetCulture(HttpContextBase context);
        ContentCultureSettings GetSettings();
    }

    [OrchardFeature("Datwendo.Localization.ContentCultureSelector")]
    public class ContentCultureService : IContentCultureService
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ISignals _signals;
        private readonly ICacheManager _cacheManager;
        private readonly IAliasService _aliasService;
        private readonly IContentManager _contentManager;
        public ContentCultureService(IAliasService aliasService,
                                        IContentManager contentManager,
                                        IWorkContextAccessor workContextAccessor,
                                        ICacheManager cacheManager,
                                        ISignals signals)
        {
            _workContextAccessor    = workContextAccessor;
            _cacheManager           = cacheManager;
            _signals                = signals;
            _aliasService           = aliasService;
            _contentManager         = contentManager;
        }

        public string GetCulture(HttpContextBase context)
        {
            if (context == null)
            {
                var workContext = _workContextAccessor.GetContext();
                if (workContext != null)
                    context = workContext.HttpContext;
            }
            if ( context == null || context.Request == null || context.Request.Path == null)
                return null;
            var settings = GetSettings();
            if (settings == null || !settings.Enabled)
            {
                return null;
            }            
            
            var controllerName = (context.Request.RequestContext != null) ? (string)context.Request.RequestContext.RouteData.Values["controller"] : string.Empty;
            // don't detect when in admin mode
            if ((string.IsNullOrEmpty(controllerName) || string.Equals(controllerName, "admin", StringComparison.InvariantCultureIgnoreCase))
                || AdminFilter.IsApplied(context.Request.RequestContext)
                )
            {
                return null;
            }
            string cultureName  = null;
            var content         = GetByPath(context.Request.Path.TrimStart('/'));
            if (content != null)
            {
                var localizationPart = content.As<ILocalizableAspect>();
                if (localizationPart != null)
                {
                    cultureName = localizationPart.Culture;
                }
            }
            return cultureName;
        }

        public IContent GetByPath(string aliasPath)
        {
            var contentRouting = _aliasService.Get(aliasPath);

            if (contentRouting == null)
                return null;

            object id;
            if (contentRouting.TryGetValue("id", out id))
            {
                int contentId;
                if (int.TryParse(id as string, out contentId))
                {
                    return _contentManager.Get(contentId);
                }
            }
            return null;
        }

        public ContentCultureSettings GetSettings()
        {
            return _cacheManager.Get("ContentCultureSettings",
                ctx =>
                {
                    ctx.Monitor(_signals.When(ContentCultureSettingsPart.CacheKey));
                    var settingsPart = _workContextAccessor.GetContext().CurrentSite.As<ContentCultureSettingsPart>();
                    return new ContentCultureSettings
                    {
                        Enabled = settingsPart.Enabled,
                        Priority = settingsPart.Priority
                    };
                });
        }
    }
}

