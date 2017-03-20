using System.Collections.Generic;
using System.Globalization;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace Amba.HtmlBlocks.Settings
{
    public class HtmlBlockFieldFieldEditorEvents : ContentDefinitionEditorEventsBase
    {
        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition)
        {
            if (definition.FieldDefinition.Name == "HtmlBlockField")
            {
                var model = definition.Settings.GetModel<HtmlBlockFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel)
        {
            if (builder.FieldType != "HtmlBlockField")
            {
                yield break;
            }

            var model = new HtmlBlockFieldSettings();
            if (updateModel.TryUpdateModel(model, "HtmlBlockFieldSettings", null, null))
            {
                builder.WithSetting("HtmlBlockFieldSettings.Height", model.Height.ToString());
                builder.WithSetting("HtmlBlockFieldSettings.Helptext", model.HelpText);                
            }
            yield return DefinitionTemplate(model);
        }
    }
}