using Orchard.UI.Resources;

namespace Admvol.CityProblemTracking
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest.DefineScript("jQuery.Migrate").SetUrl("jquery-migrate-1.2.1.min.js").SetDependencies("jQuery");            
        }
    }
}