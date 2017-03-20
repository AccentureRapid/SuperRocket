using dcp.Routing.Models;
using Orchard.Autoroute.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;

namespace dcp.Routing.Services
{
    [OrchardFeature("dcp.Routing.UrlUpdating")]
    public class UrlContentHandler : ContentHandler
    {
        private string _sourceUrl;

        public UrlContentHandler(IRoutingAppService routingAppService)
        {
            OnUpdating<AutoroutePart>((ctx, part) =>
            {
                _sourceUrl = part.DisplayAlias;
            });

            OnUpdated<AutoroutePart>((ctx, part) =>
            {
                if (string.IsNullOrEmpty(_sourceUrl) || string.IsNullOrEmpty(part.DisplayAlias))
                    return;

                if (_sourceUrl.TrimStart('/') == part.DisplayAlias.TrimStart('/'))
                    return;

                routingAppService.Add(new RedirectRule
                {
                    SourceUrl = _sourceUrl,
                    DestinationUrl = part.DisplayAlias,
                    IsPermanent = true
                });
            });
        }
    }
    
}