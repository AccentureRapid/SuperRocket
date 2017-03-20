using System.Collections.Generic;
using System.Xml.Linq;
using Orchard;
using Orchard.ContentManagement;

namespace Onestop.Layouts.Elements {
    public interface ILayoutElement : IDependency {
        dynamic BuildDisplay(XElement description, XElement data, IContent contentPart, string displayType, IDictionary<string, IEnumerator<XElement>> dataEnumerators = null);
        dynamic BuildEditor(XElement description, XElement data, string prefix, IDictionary<string, IEnumerator<XElement>> dataEnumerators = null);
        void HandleEdits(IDictionary<string, string> dictionary, XElement description, IUpdateModel updater, string prefix);
        IEnumerable<dynamic> BuildLayoutEditors();
    }
}
