using Datwendo.Localization.Services;
using Orchard.Environment.Extensions;
using Orchard.Localization.Services;
using System.Web;

namespace Datwendo.Localization.Providers
{
    [OrchardFeature("Datwendo.Localization.ContentCultureSelector")]
    public class ContentCultureSelector : ICultureSelector 
    {
        private readonly IContentCultureService _contentCultureService;

        public ContentCultureSelector(IContentCultureService contentCultureService)
        {
            _contentCultureService = contentCultureService;
        }

        public CultureSelectorResult GetCulture(HttpContextBase context)
        {
            string cultureName = _contentCultureService.GetCulture(context);
            return cultureName == null ? null : new CultureSelectorResult { Priority = _contentCultureService.GetSettings().Priority, CultureName = cultureName };
        }
    }
}

