using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheMonarch.SyntaxHighlighter.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Localization;
//using JetBrains.Annotations;

namespace TheMonarch.SyntaxHighlighter.Drivers {
   // [UsedImplicitly]
    public class SyntaxHighlighterSettingsPartDriver : ContentPartDriver<SyntaxHighlighterSettingsPart> {

        public Localizer T { get; set; }
        protected override string Prefix { get { return "SyntaxHighlighterSettings"; } }
        private const string TemplateName = "Parts/SyntaxHighlighterSettings";

        public SyntaxHighlighterSettingsPartDriver() {
            T = NullLocalizer.Instance;
        }


        protected override DriverResult Editor(SyntaxHighlighterSettingsPart part, dynamic shapeHelper) {
            var model = new SyntaxHighlighterSettingsViewModel { Theme = part.Theme, Themes = GetThemes() }; 
            return ContentShape("Parts_SyntaxHighlighterSettings_Edit",
                    () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: model, Prefix: Prefix))
                    .OnGroup("syntax-highlighter");
        }

        protected override DriverResult Editor(SyntaxHighlighterSettingsPart part, IUpdateModel updater, dynamic shapeHelper) {
            return ContentShape("Parts_SyntaxHighlighterSettings_Edit", () => {
                var model = new SyntaxHighlighterSettingsViewModel { Theme = part.Theme, Themes = GetThemes() }; 
                updater.TryUpdateModel(part, Prefix, null, null);

                return shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: model, Prefix: Prefix);
            })
                .OnGroup("syntax-highlighter");
        }


        private IEnumerable<string> GetThemes() {
            return new string[] {
                "prettify"
                , "desert"
                , "github"
                , "hemisu-dark"
                , "hemisu-light"
                , "son-of-obsidian"
                , "sunburst"
                , "vibrant-ink"
                , "tomorrow"
            }; 
        }

    }
}
