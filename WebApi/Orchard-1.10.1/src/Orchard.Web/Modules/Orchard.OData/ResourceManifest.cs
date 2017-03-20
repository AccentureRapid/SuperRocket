
namespace Orchard.OData
{
    using Orchard.UI.Resources;

    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder) {
            builder.Add().DefineStyle("leaflet").SetUrl("~/Modules/Orchard.OData/scripts/leaflet/leaflet.css");
            builder.Add().DefineStyle("pivotViewer").SetUrl("styles/pivotviewer.css").SetDependencies("leaflet");

            builder.Add().DefineScript("simile").SetUrl("simile-timeline/timeline_ajax/simile-ajax-api.js");
            builder.Add().DefineScript("timeline").SetUrl("simile-timeline/timeline_js/timeline-api.js").SetDependencies("simile");

            builder.Add().DefineScript("jqueryMousewheel").SetUrl("jquery.mousewheel/jquery.mousewheel.min.js");
            builder.Add().DefineScript("jqueryTooltipster").SetUrl("jquery.tooltipster/jquery.tooltipster.min.js");
            //builder.Add().DefineScript("jQueryPivot").SetUrl("jquery/jquery-1.11.0.min.js");
            //builder.Add().DefineScript("jQueryUiPivot").SetUrl("jquery-ui/jquery-ui-1.10.4.custom.min.js").SetDependencies("jQueryPivot", "jqueryMousewheelPivot", "jqueryTooltipsterPivot");

            builder.Add().DefineScript("leaflet").SetUrl("leaflet/leaflet-0.6.4.js");
            builder.Add().DefineScript("wicket").SetUrl("wicket/wicket.min.js");
            builder.Add().DefineScript("wicketLeaflet").SetUrl("wicket/wicket-leaflet.min.js").SetDependencies("wicket", "leaflet");

            builder.Add().DefineScript("modernizr").SetUrl("modernizr/modernizr.custom.93916.js");
            builder.Add().DefineScript("easing").SetUrl("easing/easing.js");
            builder.Add().DefineScript("colResizable").SetUrl("colResizable/colResizable-1.3.min.js").SetDependencies("jQuery");
            builder.Add().DefineScript("purl").SetUrl("purl-master/purl.js");

            builder.Add().DefineScript("pivotViewer").SetUrl("pivotviewer.min.js").SetDependencies("timeline", "modernizr", "jQueryUI", "jqueryMousewheel", "jqueryTooltipster", "easing", "colResizable", "purl", "wicketLeaflet");
        }
    }
}
