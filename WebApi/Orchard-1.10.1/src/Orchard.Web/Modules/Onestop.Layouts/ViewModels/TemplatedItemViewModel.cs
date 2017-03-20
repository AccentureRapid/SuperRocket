using System.Collections.Generic;
using Onestop.Layouts.Models;

namespace Onestop.Layouts.ViewModels {
    public class TemplatedItemViewModel {
        public string Prefix { get; set; }
        public TemplatedItemPart TemplatedItem { get; set; }
        public LayoutTemplatePart Template { get; set; }
        public bool CanChangeTemplate { get; set; }
        public IEnumerable<LayoutTemplatePart> Templates { get; set; }
        public IEnumerable<dynamic> EditorShapes { get; set; }
        public dynamic TemplatedItemPreviewShape { get; set; }
    }
}
