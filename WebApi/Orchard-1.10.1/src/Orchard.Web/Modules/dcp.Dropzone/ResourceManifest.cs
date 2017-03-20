using Orchard.UI.Resources;

namespace dcp.Dropzone
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();
            
            #region scripts

            manifest.DefineScript("dropzone")
                .SetUrl("dropzone/dropzone.js");

            #endregion

            #region styles
            
            manifest.DefineStyle("dropzone.base")
                .SetBasePath("~/Modules/dcp.Dropzone/Scripts/")
                .SetUrl("dropzone/basic.css");

            manifest.DefineStyle("dropzone.default")
                .SetBasePath("~/Modules/dcp.Dropzone/Scripts/")
                .SetUrl("dropzone/dropzone.css")
                .SetDependencies("dropzone.base");

            #endregion
        }
    }
}