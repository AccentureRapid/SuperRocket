using Orchard.UI.Resources;

namespace Orchard.ContentTree
{
    public class Resources : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest.DefineScript("Orchard_ContentTree").SetUrl("orchard.contentree.js").SetDependencies("jQuery", "jQueryEffects_Slide");
            manifest.DefineStyle("Orchard_ContentTree").SetUrl("orchard.contentree.css");
        }
    }
}