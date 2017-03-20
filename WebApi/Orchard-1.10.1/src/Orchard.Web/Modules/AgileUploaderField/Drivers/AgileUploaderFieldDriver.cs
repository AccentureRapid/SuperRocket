using System;
using AgileUploaderField.Settings;
using AgileUploaderField.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;

namespace AgileUploaderField.Drivers
{
    public class AgileUploaderFieldDriver : ContentFieldDriver<Fields.AgileUploaderField>
    {
        private readonly IOrchardServices _orchardServices;

        public AgileUploaderFieldDriver(IOrchardServices orchardServices)
        {
            _orchardServices = orchardServices;
        }

        private const string TemplateName = "Fields/AgileUploader"; 
        private const string TokenContentType = "{content-type}";
        private const string TokenFieldName = "{field-name}";
        private const string TokenContentItemId = "{content-item-id}";
        private const string TokenUserId = "{user-id}";

        private static string GetPrefix(ContentField field, ContentPart part)
        {
            return part.PartDefinition.Name + "." + field.Name;
        }

        private static string GetDifferentiator(Fields.AgileUploaderField field, ContentPart part)
        {
            return field.Name;
        }

        protected override DriverResult Display(ContentPart part, Fields.AgileUploaderField field, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Fields_AgileUploader", GetDifferentiator(field, part),
                () =>
                    shapeHelper.Fields_AgileUploader( // this is the actual Shape which will be resolved (Fields/AgileUploader.cshtml)
                        ContentPart: part, // it will allow to access the content item
                        ContentField: field
                        )
                    );
        }

        protected override DriverResult Editor(ContentPart part, Fields.AgileUploaderField field, dynamic shapeHelper)
        {
            var settings = field.PartFieldDefinition.Settings.GetModel<AgileUploaderFieldSettings>();
            var agileUploaderMediaFolder = GetAgileUploaderMediaFolder(part, field, settings);

            var viewModel = new AgileUploaderFieldViewModel
            {
                Settings = settings,
                AgileUploaderMediaFolder = agileUploaderMediaFolder,
                Field = field
            };

            return ContentShape("Fields_AgileUploader_Edit", GetDifferentiator(field, part),
                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: viewModel, Prefix: GetPrefix(field, part)));
        }

        protected override DriverResult Editor(ContentPart part, Fields.AgileUploaderField field, IUpdateModel updater, dynamic shapeHelper)
        {
            var viewModel = new AgileUploaderFieldViewModel
            {
                Field = field
            };
            updater.TryUpdateModel(viewModel, GetPrefix(field, part), null, null);
            return Editor(part, field, shapeHelper);
        }

        private string GetAgileUploaderMediaFolder(IContent part, ContentField field, AgileUploaderFieldSettings settings)
        {
            var agileUploaderMediaFolder = settings.MediaFolder;
            if (String.IsNullOrWhiteSpace(agileUploaderMediaFolder))
            {
                agileUploaderMediaFolder = TokenContentType + "/" + TokenFieldName;
            }

            agileUploaderMediaFolder = agileUploaderMediaFolder
                .Replace(TokenContentType, part.ContentItem.ContentType)
                .Replace(TokenFieldName, field.Name)
                .Replace(TokenContentItemId, Convert.ToString(part.ContentItem.Id));
            if (!string.IsNullOrEmpty(TokenUserId))
            {
                var idUser = "anonymousUser";
                if (_orchardServices.WorkContext.CurrentUser != null)
                {
                    idUser = Convert.ToString(_orchardServices.WorkContext.CurrentUser.Id);
                }
                agileUploaderMediaFolder = agileUploaderMediaFolder.Replace(TokenUserId, idUser);
            }
            return agileUploaderMediaFolder;
        }

        protected override void Exporting(ContentPart part, Fields.AgileUploaderField field, ExportContentContext context)
        {
            context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("AlternateText", field.AlternateText);
            context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("FileName", field.FileNames);
        }

        protected override void Importing(ContentPart part, Fields.AgileUploaderField field, ImportContentContext context)
        {
            field.AlternateText = context.Attribute(field.FieldDefinition.Name + "." + field.Name, "AlternateText");
            field.FileNames = context.Attribute(field.FieldDefinition.Name + "." + field.Name, "FileNames");
        }

    }
}
