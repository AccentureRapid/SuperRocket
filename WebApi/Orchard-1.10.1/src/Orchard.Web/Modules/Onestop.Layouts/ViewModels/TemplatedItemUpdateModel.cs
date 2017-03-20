using System.Collections.Generic;

namespace Onestop.Layouts.ViewModels {
    public class TemplatedItemUpdateModel {
        public int LayoutId { get; set; }
        public IList<IDictionary<string, string>> TemplatedItemData { get; set; }
    }
}
