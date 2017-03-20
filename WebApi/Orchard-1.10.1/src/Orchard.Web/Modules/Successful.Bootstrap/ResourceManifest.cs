using Orchard.UI.Resources;

namespace Successful.Bootstrap {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            
            var manifest = builder.Add();

            manifest.DefineScript("Bootstrap")
                .SetUrl("bootstrap.min.js", "bootstrap.js").SetVersion("3.3.1")
                .SetCdn("//maxcdn.bootstrapcdn.com/bootstrap/3.3.1/js/bootstrap.min.js",
                    "//maxcdn.bootstrapcdn.com/bootstrap/3.3.1/js/bootstrap.js", true)
                .SetDependencies("jQuery");

            manifest.DefineStyle("Bootstrap")
                .SetUrl("bootstrap.min.css", "bootstrap.css")
                .SetVersion("3.3.1")
                .SetCdn("//maxcdn.bootstrapcdn.com/bootstrap/3.3.1/css/bootstrap.min.css", "//maxcdn.bootstrapcdn.com/bootstrap/3.3.1/css/bootstrap.css", true);

            manifest.DefineStyle("BootstrapTheme")
                .SetUrl("bootstrap-theme.min.css", "bootstrap-theme.css")
                .SetVersion("3.3.1")
                .SetCdn("//maxcdn.bootstrapcdn.com/bootstrap/3.3.1/css/bootstrap-theme.min.css",
                    "//maxcdn.bootstrapcdn.com/bootstrap/3.3.1/css/bootstrap-theme.css", true);

        }
    }
}
