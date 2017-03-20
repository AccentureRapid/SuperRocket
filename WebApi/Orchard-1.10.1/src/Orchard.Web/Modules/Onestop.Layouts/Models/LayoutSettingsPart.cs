using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace Onestop.Layouts.Models {
    [OrchardFeature("Onestop.Layouts")]
    public class LayoutSettingsPart : ContentPart {
        public string DefaultColumnBreak {
            get { return this.Retrieve(x => x.DefaultColumnBreak); }
            set { this.Store(x => x.DefaultColumnBreak, value); }
        }
    }
}