using System.Collections.Generic;
using Onestop.Layouts.Models;

namespace Onestop.Layouts.ViewModels {
    public class LayoutViewModel {
        public LayoutTemplatePart Layout { get; set; }
        public IEnumerable<dynamic> LayoutElementEditors { get; set; }
        public bool IsTemplate { get; set; }
        public IEnumerable<LayoutTemplatePart> Layouts { get; set; }
        public int? ParentLayoutId { get; set; }
        public IEnumerable<StylesheetDescription> Stylesheets { get; set; }
        public StylesheetDescription Stylesheet { get; set; }
        public Dictionary<string, IEnumerable<string>> CssClasses { get; set; }
        public Dictionary<string, IEnumerable<string>> Fonts { get; set; }
    }
}
