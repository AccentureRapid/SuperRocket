using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using TheMonarch.SyntaxHighlighter.Models; 

namespace TheMonarch.SyntaxHighlighter.Drivers {

    /// <summary>
    /// This class intentionnaly has no Display method to prevent external access to this information through standard 
    /// Content Item display methods.
    /// </summary>
    public class SyntaxHighlighterPartDriver : ContentPartDriver<SyntaxHighlighterPart> {
        private readonly IOrchardServices _services; 

        public SyntaxHighlighterPartDriver(IOrchardServices services) {
            _services = services; 
        }

        protected override DriverResult Display(SyntaxHighlighterPart part, string displayType, dynamic shapeHelper) {
            var settings = _services.WorkContext.CurrentSite.As<SyntaxHighlighterSettingsPart>();
            var themeName = settings.Theme; 

            return ContentShape(
                "SyntaxHighlighter"
                , () => shapeHelper.SyntaxHighlighter(ThemeName: themeName)
            );
        }
    }
}