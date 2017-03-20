using Orchard.UI.Resources;

namespace Dolph.SmartContentPicker
{
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();
            manifest.DefineStyle("Chosen")
                            .SetUrl("chosen.css");

            manifest.DefineScript("Chosen")
                .SetUrl("chosen.jquery.min.js", "chosen.jquery.js")
                .SetDependencies("jQuery");

            manifest.DefineScript("SmartContentPicker")
                .SetUrl("smart-content-picker.min.js", "smart-content-picker.js")
                .SetDependencies("jQuery", "Chosen");
        }
    }
}
