using Orchard.DisplayManagement.Descriptors;
using Orchard.Environment.Extensions;

namespace dcp.Dropzone.Providers
{
    [OrchardFeature("dcp.Dropzone.AgileUploaderField")]
    public class DropzoneShapeTableProvider : IShapeTableProvider
    {
        public void Discover(ShapeTableBuilder builder)
        {
            builder.Describe("EditorTemplate").OnDisplaying(context =>
            {
                if (context.Shape.TemplateName != "Fields/AgileUploader")
                    return;

                context.Shape.TemplateName = "Fields/AgileUploaderDropbox";
            });
        }
    }
}