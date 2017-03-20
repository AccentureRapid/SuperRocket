using Orchard.ContentManagement;

namespace Onestop.Layouts.Models {
    public interface ISecondaryContent {
        IContent GetPrimaryContentItem();
    }
}
