using System.Collections.Generic;
using System.Globalization;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace Dolph.SmartContentPicker.Settings
{
    public class SmartContentPickerFieldEditorEvents : ContentDefinitionEditorEventsBase {

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition) {
            if (definition.FieldDefinition.Name == "SmartContentPickerField") {
                var model = definition.Settings.GetModel<SmartContentPickerFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel) {
            if (builder.FieldType != "SmartContentPickerField") {
                yield break;
            }

            var model = new SmartContentPickerFieldSettings();
            if (updateModel.TryUpdateModel(model, "SmartContentPickerFieldSettings", null, null)) {
                builder.WithSetting("SmartContentPickerFieldSettings.Hint", model.Hint);
                builder.WithSetting("SmartContentPickerFieldSettings.Required", model.Required.ToString(CultureInfo.InvariantCulture));
                builder.WithSetting("SmartContentPickerFieldSettings.Multiple", model.Multiple.ToString(CultureInfo.InvariantCulture));                
                builder.WithSetting("SmartContentPickerFieldSettings.DisplayedContentTypes", model.DisplayedContentTypes);
            }

            yield return DefinitionTemplate(model);
        }
    }
}