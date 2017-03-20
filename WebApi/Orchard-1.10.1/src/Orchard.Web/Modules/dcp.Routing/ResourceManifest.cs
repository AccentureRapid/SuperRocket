using Orchard.Environment.Extensions;
using Orchard.UI.Resources;

namespace dcp.Routing
{
    [OrchardFeature("dcp.Routing.Redirects")]
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            #region Libs

            manifest.DefineScript("expressive.annotations.validate")
                .SetUrl("expressive.annotations.validate.min.js", "expressive.annotations.validate.js")
                .SetVersion("2.7.0")
                .SetDependencies("jquery.validate.unobtrusive");
            
            #endregion
        }
    }
}