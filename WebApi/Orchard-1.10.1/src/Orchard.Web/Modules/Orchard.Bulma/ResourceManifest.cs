using Orchard.UI.Resources;

namespace Orchard.Bulma
{
    public class ResourceManifest : IResourceManifestProvider
    {
        private const string StableVersion = "0.3.0";

        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest.DefineStyle("bulma")
                .SetUrl("bulma.min.css", "bulma.css")
                .SetVersion(StableVersion)
                .SetCdn(
                    string.Format("//cdnjs.cloudflare.com/ajax/libs/bulma/{0}/css/bulma.css", StableVersion),
                    string.Format("//cdnjs.cloudflare.com/ajax/libs/bulma/{0}/css/bulma.min.css", StableVersion),
                    true
                );
        }
    }
}
