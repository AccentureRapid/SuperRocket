using Orchard.UI.Resources;

namespace Mello.ImageGallery {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();
            //Gallery
            manifest.DefineStyle("ImageGalleryAdmin").SetUrl("image-gallery-admin.css");
            manifest.DefineStyle("ImageGallery").SetUrl("image-gallery.css");

            manifest.DefineScript("jQueryMultiFile").SetDependencies("jquery").SetUrl("jquery.MultiFile.pack.js");
            //masonry
            manifest.DefineScript("masonry").SetUrl("masonry.pkgd.min.js", "masonry.pkgd.js").SetVersion("4.1.1").SetDependencies("jQuery");
            manifest.DefineScript("imagesloaded").SetUrl("imagesloaded.pkgd.min.js", "imagesloaded.pkgd.js").SetVersion("4.1.1");
            manifest.DefineScript("infinitescroll").SetUrl("jquery.infinitescroll.min.js", "jquery.infinitescroll.js").SetVersion("2.1.0").SetDependencies("jQuery");

            //app
            manifest.DefineScript("app").SetUrl("app.js").SetVersion("1.0.0").SetDependencies("AngularJS");
            manifest.DefineScript("view1").SetUrl("view1/view1.js").SetVersion("1.0.0").SetDependencies("AngularJS");
        }
    }
}