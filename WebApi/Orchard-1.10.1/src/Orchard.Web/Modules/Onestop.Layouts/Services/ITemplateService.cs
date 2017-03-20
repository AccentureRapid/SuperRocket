using System.Collections.Generic;
using System.Xml.Linq;
using Onestop.Layouts.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Contents.ViewModels;

namespace Onestop.Layouts.Services {
    public interface ITemplateService : IDependency {
        IEnumerable<LayoutTemplatePart> GetTemplates(ContentsOrder orderBy = ContentsOrder.Published);
        IEnumerable<dynamic> GetLayoutElementEditors();
        IEnumerable<dynamic> GetLayoutElementEditors(TemplatedItemPart templatedItem, LayoutTemplatePart layout, string prefix);
        IEnumerable<dynamic> GetLayoutElementEditors(XElement definitions, XElement data, string prefix, IDictionary<string, IEnumerator<XElement>> dataEnumerators = null);
        IEnumerable<dynamic> GetLayoutElementDisplays(TemplatedItemPart templatedItem, LayoutTemplatePart layout, string displayType);
        IEnumerable<dynamic> GetLayoutElementDisplays(XElement definitions, XElement data, IContent contentItem, string displayType, IDictionary<string, IEnumerator<XElement>> dataEnumerators = null);
        void PersistLayoutElement(
            IList<IDictionary<string, string>> layoutData,
            TemplatedItemPart templatedItem,
            LayoutTemplatePart layout,
            IUpdateModel updater,
            string prefix);
    }
}
