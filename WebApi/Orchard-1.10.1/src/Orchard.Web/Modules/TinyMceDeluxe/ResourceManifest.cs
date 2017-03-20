using Orchard.UI.Resources;

namespace TinyMceDeluxe {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();
            manifest.DefineScript("TinyMce").SetUrl("tiny_mce.js", "tiny_mce_src.js").SetVersion("3.5.7").SetDependencies("jQuery");
            manifest.DefineScript("TinyMceDeluxe").SetUrl("tinymcedeluxe.orchard.js").SetDependencies("TinyMce");
            manifest.DefineScript("OrchardTinyMceDeluxe").SetUrl("orchard-tinymce.js").SetDependencies("TinyMceDeluxe").SetVersion("1.0");
        }

    }
}