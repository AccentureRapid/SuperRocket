using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Onestop.Layouts.Elements;
using Onestop.Layouts.Helpers;
using Onestop.Layouts.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Contents.ViewModels;
using Orchard.Core.Title.Models;
using Orchard.Environment.Extensions;

namespace Onestop.Layouts.Services {
    [OrchardFeature("Onestop.Layouts")]
    public class TemplateService : ITemplateService {
        private readonly IEnumerable<ILayoutElement> _layoutElements; 
        private readonly IContentManager _contentManager;
        private readonly IWorkContextAccessor _wca;

        public TemplateService(
            IEnumerable<ILayoutElement> layoutElements,
            IContentManager contentManager,
            IWorkContextAccessor wca) {

            _layoutElements = layoutElements;
            _contentManager = contentManager;
            _wca = wca;
        }

        public IEnumerable<LayoutTemplatePart> GetTemplates(ContentsOrder orderBy = ContentsOrder.Published) {
            var query = _contentManager
                .Query<LayoutTemplatePart>("OSTemplate")
                .ForVersion(VersionOptions.Latest)
                .WithQueryHints(new QueryHints().ExpandParts<TitlePart>());

            switch (orderBy) {
                case ContentsOrder.Modified:
                    query.OrderByDescending<CommonPartRecord>(cr => cr.ModifiedUtc);
                    break;
                case ContentsOrder.Published:
                    query.OrderByDescending<CommonPartRecord>(cr => cr.PublishedUtc);
                    break;
                case ContentsOrder.Created:
                    query.OrderByDescending<CommonPartRecord>(cr => cr.CreatedUtc);
                    break;
            }
            return query.List();
        }

        public IEnumerable<dynamic> GetLayoutElementEditors() {
            return _layoutElements
                .SelectMany(
                    le => le.BuildLayoutEditors()
                )
                .OrderBy(el => el.Order);
        }
        public IEnumerable<dynamic> GetLayoutElementDisplays(
            TemplatedItemPart templatedItem, 
            LayoutTemplatePart layout, 
            string displayType) {
            var contentItem = templatedItem.ContentItem;
            return GetLayoutElementDisplays(
                GetDefinitions(layout, templatedItem.DataDocument),
                templatedItem.RootElement,
                contentItem,
                displayType);
        }

        public IEnumerable<dynamic> GetLayoutElementDisplays(
            XElement definitions,
            XElement data,
            IContent contentItem,
            string displayType,
            IDictionary<string, IEnumerator<XElement>> dataEnumerators = null) {

            // First, flatten the data structure
            var elements = data.Descendants().ToList();
            // We'll have one enumerator by element name, so we can advance the cursor for each element type separately
            var nameToEnumerator = BuildNameToEnumerator(dataEnumerators, elements);

            var displays = new List<dynamic>();
            foreach (var definition in definitions.Elements()) {
                var name = definition.Name.ToString();
                // Create a new empty enumerator per absent element type, as needed
                IEnumerator<XElement> elementEnumerator;
                if (!nameToEnumerator.TryGetValue(name, out elementEnumerator)) {
                    elementEnumerator = ((IEnumerable<XElement>)new XElement[0]).GetEnumerator();
                    nameToEnumerator.Add(name, elementEnumerator);
                }
                // Move next looking for the data for this new element, until data is exhausted
                var eof = !elementEnumerator.MoveNext();
                XElement element = null;
                while (!eof && (element = elementEnumerator.Current).Name != name) {
                    eof = !elementEnumerator.MoveNext();
                }
                if (element != null && element.Name != name) element = null;
                // Ask each element to display itself if it recognizes the element type
                foreach (var layoutElement in _layoutElements) {
                    var display = layoutElement.BuildDisplay(definition, element ?? new XElement(name), contentItem, displayType, nameToEnumerator);
                    if (display != null) {
                        display.Metadata.DisplayType = displayType;
                        var nameAlternate = display.ElementTitle == null || string.IsNullOrWhiteSpace(display.ElementTitle)
                            ? null : EncodeAlternateElement(display.ElementTitle);
                        var hasNameAlternate = !string.IsNullOrWhiteSpace(nameAlternate);
                        if (hasNameAlternate) {
                            display.Metadata.Alternates.Add(display.Metadata.Type + "__" + nameAlternate);
                        }
                        if (contentItem != null && contentItem.ContentItem != null) {
                            display.Metadata.Alternates.Add(
                                display.Metadata.Type + "_" + contentItem.ContentItem.ContentType);
                            if (hasNameAlternate) {
                                display.Metadata.Alternates.Add(
                                    display.Metadata.Type + "_" + contentItem.ContentItem.ContentType + "__" +
                                    nameAlternate);
                            }
                        }
                        displays.Add(display);
                        break;
                    }
                }
            }
            return displays;
        }

        private static IDictionary<string, IEnumerator<XElement>> BuildNameToEnumerator(IDictionary<string, IEnumerator<XElement>> dataEnumerators, List<XElement> elements) {
            var nameToEnumerator =
                dataEnumerators ??
                elements
                    .Select(el => el.Name.LocalName)
                    .Distinct()
                    .ToDictionary(
                        n => n,
                        n => elements
                                 .Where(el => el.Name.LocalName == n)
                                 .GetEnumerator());
            return nameToEnumerator;
        }

        private string EncodeAlternateElement(string alternateElement) {
            return alternateElement.Replace(" ", "").Replace("-", "__").Replace(".", "_");
        }

        public IEnumerable<dynamic> GetLayoutElementEditors(TemplatedItemPart templatedItem, LayoutTemplatePart layout, string prefix) {
            var definitions = GetDefinitions(layout, templatedItem.DataDocument);
            return GetLayoutElementEditors(definitions, templatedItem.RootElement, prefix);
        }

        private static XElement GetDefinitions(LayoutTemplatePart layout, XDocument dataDocument) {
            if (layout != null) return layout.RootElement;
            var firstElement = dataDocument == null ? null : 
                dataDocument.Root.Elements().FirstOrDefault();
            return new XElement("layout", new XElement(
                firstElement == null ? "img" : firstElement.Name),
                new XElement("link", new XAttribute("Title", "Caption")));
        }

        public IEnumerable<dynamic> GetLayoutElementEditors(
            XElement definitions,
            XElement data,
            string prefix,
            IDictionary<string, IEnumerator<XElement>> dataEnumerators = null) {

            // First, flatten the data structure
            var elements = data.Descendants().ToList();
            // We'll have one enumerator by element name, so we can advance the cursor for each element type separately
            var nameToEnumerator = BuildNameToEnumerator(dataEnumerators, elements);
            var editors = new List<dynamic>();
            foreach (var definition in definitions.Elements()) {
                var name = definition.Name.ToString();
                // Create a new empty enumerator per absent element type, as needed
                IEnumerator<XElement> elementEnumerator;
                if (!nameToEnumerator.TryGetValue(name, out elementEnumerator)) {
                    elementEnumerator = ((IEnumerable<XElement>)new XElement[0]).GetEnumerator();
                    nameToEnumerator.Add(name, elementEnumerator);
                }
                // Move next looking for the data for this new element, until data is exhausted
                var eof = !elementEnumerator.MoveNext();
                XElement element = null;
                while (!eof && (element = elementEnumerator.Current).Name != name) {
                    eof = !elementEnumerator.MoveNext();
                }
                if (element != null && element.Name != name) element = null;
                // Ask each element to edit itself if it recognizes the element type
                foreach (var layoutElement in _layoutElements) {
                    var editor = layoutElement.BuildEditor(definition, element ?? new XElement(name), prefix, nameToEnumerator);
                    if (editor != null) {
                        editors.Add(editor);
                        break;
                    }
                }
            }
            return editors;
        }

        public void PersistLayoutElement(
            IList<IDictionary<string, string>> layoutData,
            TemplatedItemPart templatedItem,
            LayoutTemplatePart layout,
            IUpdateModel updater,
            string prefix) {

            templatedItem.Data = PersistLayoutElementInternal(layoutData, templatedItem, layout, updater, prefix);
        }

        private string PersistLayoutElementInternal(
            IList<IDictionary<string, string>> layoutData,
            TemplatedItemPart templatedItem,
            LayoutTemplatePart layout,
            IUpdateModel updater,
            string prefix) {

            var doc = XDocument.Parse("<data></data>");
            var layoutRoot = GetDefinitions(layout, templatedItem.DataDocument);
            var elementDescriptions = layoutRoot.Elements().ToList();
            // Form field names are of the form 
            // TemplatedItemData[element index][property index].Key
            // and TemplatedItemData[element index][property index].Value,
            // and get automatically bound to an IList<IDictionary<string, string>>
            // where the list is over the elements
            // and the dictionary is over the properties of that element.
            // Persist form values onto the XML description for the templated item,
            // walking the tree in the same order that the display was generated
            var docRoot = doc.Element("data");
            Debug.Assert(docRoot != null, "The document should always have a root called data.");
            var editors =
                GetLayoutElementEditors(
                    layoutRoot,
                    new XElement("data"), "").ToList();
            var context = _wca.GetContext().HttpContext;
            HtmlHelpers.ResetIndex(context, "LayoutEditorIndex");
            PersistLayoutElementPrivate(
                context, layoutData, elementDescriptions, docRoot, editors,
                updater, prefix);
            return doc.ToString(SaveOptions.DisableFormatting);
        }

        private void PersistLayoutElementPrivate(
            HttpContextBase context,
            IList<IDictionary<string, string>> layoutData,
            IEnumerable<XElement> elementDescriptions,
            XElement root,
            IEnumerable<dynamic> editors,
            IUpdateModel updater,
            string prefix) {

            var editorList = editors.ToList();
            var i = 0;
            foreach (var elementDescription in elementDescriptions) {
                var el = new XElement(elementDescription.Name);
                var editor = editorList[i];
                var usesIndex = editorList.Count > i && editor.UsesIndex == true;
                if (usesIndex) {
                    var j = HtmlHelpers.GetIndex(context, "LayoutEditorIndex", 0);
                    if (layoutData.ElementAtOrDefault(j) != null) {
                        // Let the element chime into edit handling
                        foreach (var layoutElement in _layoutElements) {
                            layoutElement.HandleEdits(
                                layoutData[j], elementDescription, updater, prefix);
                        }
                        // Use the posted data to populate the element attributes
                        foreach (var key in layoutData[j].Keys) {
                            el.SetAttributeValue(key, layoutData[j][key]);
                        }
                    }
                }
                if (elementDescription.HasElements) {
                    PersistLayoutElementPrivate(
                        context, layoutData, elementDescription.Elements(), el, editor.Elements,
                        updater, prefix);
                }
                root.Add(el);
                i++;
            }
        }
    }
}
