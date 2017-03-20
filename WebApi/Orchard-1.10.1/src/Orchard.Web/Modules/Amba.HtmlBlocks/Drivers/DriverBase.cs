using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;

namespace Amba.HtmlBlocks.Drivers
{
    public abstract class FieldDriverBase<TContentField> : ContentFieldDriver<TContentField>
        where TContentField : ContentField, new()
    {
        public IOrchardServices Services { get; set; }

        protected static string GetPrefix(ContentField field, ContentPart part)
        {
            return part.PartDefinition.Name + "." + field.Name;
        }

        protected string GetDifferentiator(TContentField field, ContentPart part)
        {
            return field.Name;
        }
    }
}