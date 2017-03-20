using System;
using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace AgileUploaderField.Settings
{
    public class AgileUploaderFieldEditorEvents : ContentDefinitionEditorEventsBase 
    {

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition)
        {
            if (definition.FieldDefinition.Name == "AgileUploaderField")
            {
                var model = definition.Settings.GetModel<AgileUploaderFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }
 
        public override IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel) {
            var model = new AgileUploaderFieldSettings();
            if (builder.FieldType != "AgileUploaderField")
            {
                yield break;
            } 
            if (updateModel.TryUpdateModel(model, "AgileUploaderFieldSettings", null, null))
            {
                builder.WithSetting("AgileUploaderFieldSettings.Hint", model.Hint);
                builder.WithSetting("AgileUploaderFieldSettings.MaxHeight", Convert.ToString(model.MaxHeight));
                builder.WithSetting("AgileUploaderFieldSettings.MaxWidth", Convert.ToString(model.MaxWidth));
                builder.WithSetting("AgileUploaderFieldSettings.AuthorCanSetAlternateText", Convert.ToString(model.AuthorCanSetAlternateText));
                builder.WithSetting("AgileUploaderFieldSettings.MediaFolder", model.MediaFolder);
                builder.WithSetting("AgileUploaderFieldSettings.FileLimit", Convert.ToString(model.FileLimit));
            }

            yield return DefinitionTemplate(model);
        }
    }
}