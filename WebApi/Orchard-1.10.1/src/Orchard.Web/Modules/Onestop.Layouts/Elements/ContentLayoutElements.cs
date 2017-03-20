using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Onestop.Layouts.Helpers;
using Onestop.Layouts.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Descriptors;
using Orchard.Environment;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace Onestop.Layouts.Elements {
    [OrchardFeature("Onestop.Layouts")]
    public class ContentLayoutElements : ILayoutElement {
        private readonly Work<IEnumerable<IContentPartDriver>> _partDrivers;
        private readonly Work<IEnumerable<IContentFieldDriver>> _fieldDrivers;
        private readonly IWorkContextAccessor _wca;
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public ContentLayoutElements(
            IShapeFactory shapeFactory,
            Work<IEnumerable<IContentPartDriver>> partDrivers,
            Work<IEnumerable<IContentFieldDriver>> fieldDrivers,
            IWorkContextAccessor wca,
            IContentDefinitionManager contentDefinitionManager) {

            Shape = shapeFactory;
            _partDrivers = partDrivers;
            _fieldDrivers = fieldDrivers;
            _wca = wca;
            _contentDefinitionManager = contentDefinitionManager;
            T = NullLocalizer.Instance;
        }
        
        dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public dynamic BuildEditor(XElement description, XElement data, string prefix, IDictionary<string, IEnumerator<XElement>> dataEnumerators = null) {
            switch (description.Name.LocalName.ToLowerInvariant()) {
                case "part":
                    return Shape.LayoutElements_Part_Edit(
                        Name: T("Part"),
                        Title: XmlHelpers.Attr(description, "title"),
                        Prefix: prefix);
                case "field":
                    return Shape.LayoutElements_Field_Edit(
                        Name: T("Field"),
                        Title: XmlHelpers.Attr(description, "title"),
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
            switch (description.Name.LocalName.ToLowerInvariant()) {
                case "part":
                    var layoutElement = description.FindParentWithAttributes("width", "height") ?? new XElement("layout");
                    if (contentPart == null) return null;
                    var partTypeName = XmlHelpers.Attr(description, "part");
                    if (string.IsNullOrWhiteSpace(partTypeName)) return null;
                    if (contentPart.ContentItem.Parts.All(
                        p => p != null && p.GetType().Name != partTypeName)) {
                        var secondaryItem = contentPart as ISecondaryContent;
                        if (secondaryItem != null) {
                            contentPart = secondaryItem.GetPrimaryContentItem();
                            if (contentPart == null) return null;
                        }
                    }
                    var partDriver = _partDrivers.Value.FirstOrDefault(
                        h => {
                            var driverType = h.GetType().BaseType;
                            if (driverType == null ||
                                (!driverType.IsGenericType) ||
                                (driverType.Name != "ContentPartDriver`1")) return false;
                            var driverPartTypeName = driverType.GetGenericArguments()[0].Name;
                            return driverPartTypeName == partTypeName;
                        });
                    if (partDriver == null) return null;
                    var workContext = _wca.GetContext();
                    var partShapes = BuildShape(workContext, contentPart, displayType, partDriver);
                    return Shape.LayoutElements_Part(
                        Name: T("Part"),
                        PartName: partTypeName,
                        PartShapes: partShapes,
                        Top: description.AttrLength("top"),
                        Left: description.AttrLength("left"),
                        LayoutWidth: layoutElement.AttrInt("width"),
                        LayoutHeight: layoutElement.AttrInt("height"),
                        CssClass: XmlHelpers.Attr(description, "class"),
                        ElementTitle: XmlHelpers.Attr(description, "title"),
                        ContentPart: contentPart,
                        ContentItem: contentPart.ContentItem);
                case "field":
                    layoutElement = description.FindParentWithAttributes("width", "height") ?? new XElement("layout");
                    if (contentPart == null || contentPart.ContentItem == null) return null;
                    var fieldName = XmlHelpers.Attr(description, "field");
                    if (string.IsNullOrWhiteSpace(fieldName)) return null;
                    var field = GetField(contentPart, fieldName);
                    if (field == null) {
                        var secondaryItem = contentPart as ISecondaryContent;
                        if (secondaryItem != null) {
                            contentPart = secondaryItem.GetPrimaryContentItem();
                            if (contentPart == null) return null;
                            field = GetField(contentPart, fieldName);
                            if (field == null) return null;
                        }
                    }
                    var fieldTypeName = field == null ? "" : field.GetType().Name;
                    var fieldDriver = _fieldDrivers.Value.FirstOrDefault(
                        h => {
                            var driverType = h.GetType().BaseType;
                            if (driverType == null ||
                                (!driverType.IsGenericType) ||
                                (driverType.Name != "ContentFieldDriver`1")) return false;
                            var driverFieldTypeName = driverType.GetGenericArguments()[0].Name;
                            return driverFieldTypeName == fieldTypeName;
                        });
                    if (fieldDriver == null) return null;
                    workContext = _wca.GetContext();
                    var fieldShapes = BuildShape(workContext, contentPart, displayType, fieldDriver: fieldDriver, fieldName: fieldName);
                    return Shape.LayoutElements_Field(
                        Name: T("Field"),
                        FieldName: fieldName,
                        FieldShapes: fieldShapes,
                        Top: description.AttrLength("top"),
                        Left: description.AttrLength("left"),
                        LayoutWidth: layoutElement.AttrInt("width"),
                        LayoutHeight: layoutElement.AttrInt("height"),
                        CssClass: XmlHelpers.Attr(description, "class"),
                        ElementTitle: XmlHelpers.Attr(description, "title"),
                        ContentPart: contentPart,
                        ContentItem: contentPart.ContentItem);
            }
            return null;
        }

        private static ContentField GetField(IContent contentPart, string fieldName) {
            return contentPart
                .ContentItem
                .Parts
                .SelectMany(p => p.Fields.Where(f => f.Name == fieldName))
                .FirstOrDefault();
        }

        private static string GetNewZoneName(WorkContext workContext) {
            var zoneIndexObject = workContext.HttpContext.Items["onestop.layout.zone.index"];
            var zoneIndex = (zoneIndexObject == null) ? 0 : (int) zoneIndexObject;
            workContext.HttpContext.Items["onestop.layout.zone.index"] = zoneIndex + 1;
            var zoneName = "__here" + zoneIndex;
            return zoneName;
        }

        private IEnumerable<dynamic> BuildShape(
            WorkContext workContext,
            IContent contentItem,
            string displayType,
            IContentPartDriver partDriver = null,
            IContentFieldDriver fieldDriver = null,
            string fieldName = null) {

            var layout = workContext.Layout;
            var zoneName = GetNewZoneName(workContext);
            var context = new BuildDisplayContext(layout, contentItem, displayType, null, Shape) {
                Layout = layout,
                FindPlacement =
                    (partType, differentiator, defaultLocation) =>
                    new PlacementInfo {Location = zoneName}
            };
            var displayResult =
                partDriver == null
                    ? (fieldDriver == null ? null : fieldDriver.BuildDisplayShape(context))
                    : partDriver.BuildDisplay(context);
            if (displayResult == null) return null;
            var resultsAsCombined = displayResult as CombinedResult;
            if (resultsAsCombined != null && !resultsAsCombined.GetResults().Any()) return null;
            displayResult.Apply(context);
            var hereShapes = (IEnumerable<dynamic>) context.Shape[zoneName];
            if (fieldName != null) {
                return hereShapes.Where(s => s.ContentField != null && s.ContentField.Name == fieldName);
            }
            return hereShapes.Where(s => s.Metadata.DisplayType == displayType &&
                !s.Metadata.Type.EndsWith("_SummaryAdmin", StringComparison.Ordinal) &&
                !s.Metadata.Type.EndsWith("_Summary", StringComparison.Ordinal));
        }

        public IEnumerable<dynamic> BuildLayoutEditors() {
            var contentParts = _contentDefinitionManager
                .ListPartDefinitions()
                .OrderBy(p => p.Name);
            var contentFields = _contentDefinitionManager
                .ListPartDefinitions()
                .SelectMany(p => p.Fields)
                .OrderBy(f => f.DisplayName);
            return new[] {
                Shape.LayoutElements_Part_LayoutEditor(
                    Order: "10",
                    ContentParts: contentParts
                ),
                Shape.LayoutElements_Field_LayoutEditor(
                    Order: "11",
                    ContentFields: contentFields
                )
            };
        }
    }
}
