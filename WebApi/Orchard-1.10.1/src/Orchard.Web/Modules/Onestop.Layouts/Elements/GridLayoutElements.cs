using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Onestop.Layouts.Helpers;
using Onestop.Layouts.Models;
using Onestop.Layouts.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Environment;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace Onestop.Layouts.Elements {
    [OrchardFeature("Onestop.Layouts")]
    public class GridLayoutElements : ILayoutElement {
        private readonly Work<ITemplateService> _templateService;
        private readonly IWorkContextAccessor _wca;

        public GridLayoutElements(IShapeFactory shapeFactory, Work<ITemplateService> templateService, IWorkContextAccessor wca) {
            Shape = shapeFactory;
            _templateService = templateService;
            _wca = wca;
            T = NullLocalizer.Instance;
        }
        
        dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public dynamic BuildEditor(XElement description, XElement data, string prefix, IDictionary<string, IEnumerator<XElement>> dataEnumerators = null) {
            switch (description.Name.LocalName.ToLowerInvariant()) {
                case "row":
                    return Shape.LayoutElements_Row_Edit(
                        Name: T("Row"),
                        Title: XmlHelpers.Attr(description, "title"),
                        Elements: _templateService.Value.GetLayoutElementEditors(
                            description, data, prefix, dataEnumerators).ToList(),
                        Prefix: prefix);
                case "column":
                    return Shape.LayoutElements_Column_Edit(
                        Name: T("Column"),
                        Span: description.AttrInt("span"),
                        Offset: description.AttrInt("offset"),
                        Title: XmlHelpers.Attr(description, "title"),
                        Elements: _templateService.Value.GetLayoutElementEditors(
                            description, data, prefix, dataEnumerators).ToList(),
                        Prefix: prefix);
            }
            return null;
        }

        public void HandleEdits(
            IDictionary<string, string> dictionary,
            XElement description,
            IUpdateModel updater, string prefix) {
        }

        public dynamic BuildDisplay(XElement description, XElement data, IContent contentPart, string displayType, IDictionary<string, IEnumerator<XElement>> dataEnumerators = null) {
            var settings = _wca.GetContext().CurrentSite.As<LayoutSettingsPart>();
            switch (description.Name.LocalName.ToLowerInvariant()) {
                case "row":
                    return Shape.LayoutElements_Row(
                        Name: T("Row"),
                        Elements: _templateService.Value.GetLayoutElementDisplays(
                            description, data, contentPart, displayType, dataEnumerators).ToList(),
                        CssClass: XmlHelpers.Attr(description, "class"),
                        ElementTitle: XmlHelpers.Attr(description, "title"),
                        Height: description.AttrLength("height"),
                        ContentPart: contentPart,
                        ContentItem: contentPart == null ? null : contentPart.ContentItem);
                case "column":
                    var breakOn = XmlHelpers.Attr(description.Parent, "break");
                    if (string.IsNullOrWhiteSpace(breakOn)) {
                        breakOn = string.IsNullOrWhiteSpace(settings.DefaultColumnBreak) ? "md" : settings.DefaultColumnBreak;
                    }
                    return Shape.LayoutElements_Column(
                        Name: T("Column"),
                        Span: description.AttrInt("span"),
                        Offset: description.AttrInt("offset"),
                        BreakOn: breakOn,
                        Elements: _templateService.Value.GetLayoutElementDisplays(
                            description, data, contentPart, displayType, dataEnumerators).ToList(),
                        CssClass: XmlHelpers.Attr(description, "class"),
                        ElementTitle: XmlHelpers.Attr(description, "title"),
                        Width: description.AttrLength("width"),
                        ContentPart: contentPart,
                        ContentItem: contentPart == null ? null : contentPart.ContentItem);
            }
            return null;
        }

        public IEnumerable<dynamic> BuildLayoutEditors() {
            return new[] {
                Shape.LayoutElements_Row_LayoutEditor(Order: "01"),
                Shape.LayoutElements_Column_LayoutEditor(Order: "02")
            };
        }
    }
}
