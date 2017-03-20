using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dolph.JsonField.Fields;
using Orchard.ContentManagement.Handlers;

namespace Dolph.JsonField.Drivers
{
    public class JsonFieldDriver : ContentFieldDriver<Dolph.JsonField.Fields.JsonField>
    {
        public IOrchardServices Services { get; set; }
        private const string TemplateName = "Fields/Json.Edit";

        public JsonFieldDriver(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        private static string GetPrefix(ContentField field, ContentPart part) {
            return part.PartDefinition.Name + "." + field.Name;
        }

        private static string GetDifferentiator(Dolph.JsonField.Fields.JsonField field, ContentPart part)
        {
            return field.Name;
        }

        protected override DriverResult Display(ContentPart part, Dolph.JsonField.Fields.JsonField field, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Fields_Json", GetDifferentiator(field, part), () => {
                var settings = field.PartFieldDefinition.Settings.GetModel<Dolph.JsonField.Settings.JsonFieldSettings>();
                return shapeHelper.Fields_Input().Settings(settings);
            });
        }

        protected override DriverResult Editor(ContentPart part, Dolph.JsonField.Fields.JsonField field, dynamic shapeHelper)
        {
            return ContentShape("Fields_Json_Edit", GetDifferentiator(field, part),
                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: field, Prefix: GetPrefix(field, part)));
        }

        protected override DriverResult Editor(ContentPart part, Dolph.JsonField.Fields.JsonField field, IUpdateModel updater, dynamic shapeHelper)
        {
            if (updater.TryUpdateModel(field, GetPrefix(field, part), null, null)) {
                var settings = field.PartFieldDefinition.Settings.GetModel<Dolph.JsonField.Settings.JsonFieldSettings>();
            }

            return Editor(part, field, shapeHelper);
        }

        protected override void Importing(ContentPart part, Dolph.JsonField.Fields.JsonField field, ImportContentContext context)
        {
            context.ImportAttribute(field.FieldDefinition.Name + "." + field.Name, "Value", v => field.Value = v);
        }

        protected override void Exporting(ContentPart part, Dolph.JsonField.Fields.JsonField field, ExportContentContext context)
        {
            context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("Value", field.Value);
        }

        protected override void Describe(DescribeMembersContext context) {
            context
                .Member(null, typeof(string), T("Value"), T("The value of the field."))
                .Enumerate<Dolph.JsonField.Fields.JsonField>(() => field => new[] { field.Value });
        }
    }
}