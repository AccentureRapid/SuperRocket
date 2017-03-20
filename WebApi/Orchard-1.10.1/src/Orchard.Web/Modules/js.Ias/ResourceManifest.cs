using Orchard.UI.Resources;

namespace js.Ias {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();
            manifest.DefineScript("jQuery_Ias")
                .SetVersion("0.1.6")
                .SetDependencies("jQuery")
                .SetUrl("jquery.ias.js", "jquery.ias.js");                

            manifest.DefineStyle("jQuery_Ias")
                .SetVersion("0.1.6")
                .SetUrl("jquery.ias.css", "jquery.ias.css");
        }
    }
}
