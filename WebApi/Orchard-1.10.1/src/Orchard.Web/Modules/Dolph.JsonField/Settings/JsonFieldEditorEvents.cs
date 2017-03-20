using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Dolph.JsonField.Settings
{
    public class JsonFieldEditorEvents : ContentDefinitionEditorEventsBase
    {

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition)
        {
            if (definition.FieldDefinition.Name == "JsonField")
            {
                var model = definition.Settings.GetModel<JsonFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel)
        {
            if (builder.FieldType != "JsonField")
            {
                yield break;
            }

            var model = new JsonFieldSettings();
            if (updateModel.TryUpdateModel(model, "JsonFieldSettings", null, null))
            {
                builder.WithSetting("JsonFieldSettings.Hint", model.Hint);
                builder.WithSetting("JsonFieldSettings.Template", model.Template);
                builder.WithSetting("JsonFieldSettings.UpdateValuesOnly", model.UpdateValuesOnly.ToString(CultureInfo.InvariantCulture));
                builder.WithSetting("JsonFieldSettings.CanEditJsonText", model.CanEditJsonText.ToString(CultureInfo.InvariantCulture));
            }

            yield return DefinitionTemplate(model);
        }
    }
}