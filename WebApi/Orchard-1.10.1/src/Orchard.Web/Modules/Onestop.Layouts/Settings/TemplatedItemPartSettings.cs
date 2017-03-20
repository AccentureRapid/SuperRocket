using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace Onestop.Layouts.Settings {
    public class TemplatedItemPartSettings {
        public bool AllowTemplateChoice { get; set; }
    }

    public class TemplatedItemPartSettingsHooks : ContentDefinitionEditorEventsBase {
        public override IEnumerable<TemplateViewModel> TypePartEditor(
            ContentTypePartDefinition definition) {

            if (definition.PartDefinition.Name != "TemplatedItemPart")
                yield break;

            var model = definition.Settings.GetModel<TemplatedItemPartSettings>();

            yield return DefinitionTemplate(model);
        }

        public override IEnumerable<TemplateViewModel> TypePartEditorUpdate(
            ContentTypePartDefinitionBuilder builder,
            IUpdateModel updateModel) {

            if (builder.Name != "TemplatedItemPart")
                yield break;

            var model = new TemplatedItemPartSettings();
            updateModel.TryUpdateModel(model, "TemplatedItemPartSettings", null, null);
            builder.WithSetting("TemplatedItemPartSettings.AllowTemplateChoice",
                model.AllowTemplateChoice.ToString());
            yield return DefinitionTemplate(model);
        }
    }
}