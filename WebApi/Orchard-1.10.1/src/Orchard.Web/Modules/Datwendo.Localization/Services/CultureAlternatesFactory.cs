using Orchard.DisplayManagement.Implementation;
using Orchard.Environment.Extensions;
using Orchard.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Datwendo.Localization.Services
{
    [OrchardFeature("Datwendo.Localization.Alternates")]
    public class CultureAlternatesFactory : ShapeDisplayEvents 
    {
        private readonly ICultureService _cultureService;
        private readonly Lazy<string> _HomePageAlternates;
        private readonly IHttpContextAccessor _httpContextAccessor;


         public CultureAlternatesFactory(ICultureService cultureService,IHttpContextAccessor httpContextAccessor) 
         {
            _httpContextAccessor = httpContextAccessor;
            _cultureService = cultureService;
            _HomePageAlternates = new Lazy<string>(() => {
                var httpContext = _httpContextAccessor.Current();

                if (httpContext == null) {
                    return null;
                }

                var request = httpContext.Request;

                if (request == null) {
                    return null;
                }
                string culture          = _cultureService.GetCurrentCulture();
                // extract each segment of the url
                var urlSegments = VirtualPathUtility.ToAppRelative(request.Path.ToLower())
                    .Split(new char[]{'/'},StringSplitOptions.RemoveEmptyEntries)
                    .Skip(1) // ignore the heading ~ segment 
                    .ToArray();
                int cnt = urlSegments.Count();
                if ( cnt == 0 ||  ( ( cnt == 1 ) && string.Equals(urlSegments[0],culture,StringComparison.InvariantCultureIgnoreCase)) ) 
                {
                    return "__CultureHomepage";
                }

                return null;
            });

        }

        public override void Displaying(ShapeDisplayingContext context) 
        {            
            context.ShapeMetadata.OnDisplaying(displayedContext => 
            {
                string culture          = _cultureService.GetCurrentCulture();
                if (string.IsNullOrEmpty(culture))
                    return;

                int idx                 = culture.IndexOf('-');
                var parentCulture       = (idx > 0) ? culture.Substring(0, idx) : null;
                var countryCulture      = (idx > 0) ? culture.Substring(idx + 1) : null;                
                culture                 = culture.Replace("-", string.Empty);
                var cultureTag          = "__"+culture;
             
                // prevent applying alternate again, c.f. http://orchard.codeplex.com/workitem/18298
                if ( displayedContext.ShapeMetadata.Alternates.Any(x => x.Contains(cultureTag))) 
                    return;
                var countryTag              = (countryCulture != null) ? "__" + countryCulture : null;
                var parentTag               = (parentCulture != null) ?  "__"+ parentCulture : null;
   

                List<string> _cultureAlternates = new List<string>( new string[]{ cultureTag, countryTag,parentTag});

                // appends culture alternates to current ones
                displayedContext.ShapeMetadata.Alternates = displayedContext.ShapeMetadata.Alternates.SelectMany(
                    alternate => new [] { alternate }.Union(_cultureAlternates.Select(a => 
                                { 
                                    int id = alternate.IndexOf("__"); 
                                    if ( id > 0 )
                                        return alternate.Substring(0,id)+a+alternate.Substring(id);
                                    return alternate + a;
                                }))).ToList();

                // appends [ShapeType]__culture__[Culture] alternates
                displayedContext.ShapeMetadata.Alternates = _cultureAlternates.Select(cult => displayedContext.ShapeMetadata.Type + "__culture" + cult)
                    .Union(displayedContext.ShapeMetadata.Alternates)
                    .ToList();

                if (_HomePageAlternates.Value != null)
                {
                    _cultureAlternates.Clear();
                    string homeTag      = _HomePageAlternates.Value;
                    _cultureAlternates.Add(homeTag);
                    _cultureAlternates.Add(homeTag+cultureTag);
                    if (countryCulture != null)
                        _cultureAlternates.Add(homeTag + countryTag);
                    if (parentCulture != null)
                        _cultureAlternates.Add(homeTag+parentTag );

                    // appends [ShapeType]__CultureHomepage__[Culture] alternates
                    displayedContext.ShapeMetadata.Alternates = _cultureAlternates.Select(cult => displayedContext.ShapeMetadata.Type + cult)
                        .Union(displayedContext.ShapeMetadata.Alternates)
                        .ToList();
                }
            });

        }
    }
}