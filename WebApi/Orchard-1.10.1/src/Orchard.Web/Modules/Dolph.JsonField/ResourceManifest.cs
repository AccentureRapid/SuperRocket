using Orchard.UI.Resources;

namespace Dolph.JsonField
{
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();

            manifest.DefineScript("JsonEditor")
                    .SetUrl("jsoneditor.min.js", "jsoneditor.js");

            manifest.DefineScript("JsonFieldEdit")
                    .SetUrl("json-field-edit.js")
                    .SetDependencies("JsonEditor", "jQuery");

            manifest.DefineScript("JsonFieldSettingsEdit")
                    .SetUrl("json-field-settings-edit.js")
                    .SetDependencies("JsonEditor", "jQuery");

            manifest.DefineScript("JsonEditor")
                    .SetUrl("jsoneditor.min.js", "jsoneditor.js");

            manifest.DefineStyle("JsonEditorStyle")
                    .SetUrl("jsoneditor.min.css", "jsoneditor.css");
           
        }
    }
}
