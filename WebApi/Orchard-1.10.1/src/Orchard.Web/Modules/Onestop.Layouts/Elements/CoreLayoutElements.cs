using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Onestop.Layouts.Helpers;
using Onestop.Layouts.Models;
using Onestop.Layouts.Services;
using Orchard.Autoroute.Models;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Environment;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.MediaLibrary.Models;

namespace Onestop.Layouts.Elements {
    [OrchardFeature("Onestop.Layouts")]
    public class CoreLayoutElements : ILayoutElement {
        private readonly Work<ITemplateService> _templateService;
        private readonly IContentManager _contentManager;

        public CoreLayoutElements(
            IContentManager contentManager,
            IShapeFactory shapeFactory, 
            Work<ITemplateService> templateService) {

            _contentManager = contentManager;
            Shape = shapeFactory;
            _templateService = templateService;
            T = NullLocalizer.Instance;
        }
        
        dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public dynamic BuildEditor(XElement description, XElement data, string prefix, IDictionary<string, IEnumerator<XElement>> dataEnumerators = null) {
            switch (description.Name.LocalName.ToLowerInvariant()) {
                case "img":
                    var src = XmlHelpers.Attr(data, "src");
                    var defaultUrl = XmlHelpers.Attr(description, "default");
                    var defaultAlt = XmlHelpers.Attr(description, "defaultalt");
                    return Shape.LayoutElements_Image_Edit(
                        Name: T("Image"),
                        Url: src,
                        DefaultUrl: defaultUrl,
                        AlternateText: XmlHelpers.Attr(data, "alt"),
                        DefaultAlt: defaultAlt,
                        Title: XmlHelpers.Attr(description, "title"),
                        Prefix: prefix,
                        UsesIndex: true);
                case "text":
                    var text = XmlHelpers.Attr(data, "text");
                    var defaultText = XmlHelpers.Attr(description, "default");
                    return Shape.LayoutElements_Text_Edit(
                        Name: T("Text"),
                        Text: text,
                        DefaultText: defaultText,
                        Title: XmlHelpers.Attr(description, "title"),
                        Prefix: prefix,
                        UsesIndex: true);
                case "link":
                    var linkText = XmlHelpers.Attr(data, "text");
                    var defaultLinkText = XmlHelpers.Attr(description, "default");
                    var href = XmlHelpers.Attr(data, "href");
                    var defaultHref = XmlHelpers.Attr(description, "defaulturl");
                    return Shape.LayoutElements_Link_Edit(
                        Name: T("Link"),
                        Text: linkText,
                        DefaultText: defaultLinkText,
                        Url: href,
                        DefaultUrl: defaultHref,
                        Title: XmlHelpers.Attr(description, "title"),
                        Prefix: prefix,
                        UsesIndex: true);
                case "container":
                    var hasLink = description.AttrBool("haslink");
                    var hasContext = description.AttrBool("hascontext");
                    var hasBackground = description.AttrBool("hasbackground");
                    var background = XmlHelpers.Attr(data, "background");
                    var defaultBackground = XmlHelpers.Attr(description, "defaultbackground");
                    IContent context = null;
                    if (hasContext) {
                        var contextId = data.AttrInt("context");
                        context = _contentManager.Get(contextId);
                    }
                    return Shape.LayoutElements_Container_Edit(
                        Name: T("Container"),
                        Title: XmlHelpers.Attr(description, "title"),
                        HasTargetUrl: hasLink,
                        TargetUrl: XmlHelpers.Attr(data, "href"),
                        HasBackground: hasBackground,
                        Background: background,
                        DefaultBackground: defaultBackground,
                        Elements: _templateService.Value.GetLayoutElementEditors(
                            description, data, prefix, dataEnumerators).ToList(),
                        UsesIndex: hasLink || hasBackground || hasContext,
                        HasContext: hasContext,
                        Context: context,
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
            var layoutElement = description.FindParentWithAttributes("width", "height") ?? new XElement("layout");
            switch (description.Name.LocalName.ToLowerInvariant()) {
                case "img":
                    var src = XmlHelpers.Attr(data, "src");
                    var defaultUrl = XmlHelpers.Attr(description, "default");
                    var alt = XmlHelpers.Attr(data, "alt");
                    var defaultAlt = XmlHelpers.Attr(description, "defaultalt");
                    return Shape.LayoutElements_Image(
                        Name: T("Image"),
                        Url: string.IsNullOrWhiteSpace(src) ? defaultUrl : src,
                        Thumbnail: src,
                        AlternateText: string.IsNullOrWhiteSpace(alt) ? defaultAlt : alt,
                        Width: description.AttrLength("width"),
                        Height: description.AttrLength("height"),
                        Top: description.AttrLength("top"),
                        Left: description.AttrLength("left"),
                        LayoutWidth: layoutElement.AttrInt("width"),
                        LayoutHeight: layoutElement.AttrInt("height"),
                        CssClass: XmlHelpers.Attr(description, "class"),
                        ElementTitle: XmlHelpers.Attr(description, "title"),
                        UsesIndex: true,
                        ContentPart: contentPart,
                        ContentItem: contentPart == null ? null : contentPart.ContentItem);
                case "text":
                    var text = XmlHelpers.Attr(data, "text");
                    var defaultText = XmlHelpers.Attr(description, "default");
                    return Shape.LayoutElements_Text(
                        Name: T("Text"),
                        Text: string.IsNullOrWhiteSpace(text) ? defaultText : text,
                        Top: description.AttrLength("top"),
                        Left: description.AttrLength("left"),
                        LayoutWidth: layoutElement.AttrInt("width"),
                        LayoutHeight: layoutElement.AttrInt("height"),
                        Font: XmlHelpers.Attr(description, "font"),
                        Size: XmlHelpers.Attr(description, "size"),
                        CssClass: XmlHelpers.Attr(description, "class"),
                        ElementTitle: XmlHelpers.Attr(description, "title"),
                        Tag: XmlHelpers.Attr(description, "tag"),
                        UsesIndex: true,
                        ContentPart: contentPart,
                        ContentItem: contentPart == null ? null : contentPart.ContentItem);
                case "link":
                    var linkText = XmlHelpers.Attr(data, "text");
                    var defaultLinkText = XmlHelpers.Attr(description, "default");
                    var href = XmlHelpers.Attr(data, "href");
                    // If href is empty, revert to pointing to the current context
                    if (string.IsNullOrWhiteSpace(href) && contentPart != null) {
                        var secondaryItem = contentPart as ISecondaryContent;
                        if (secondaryItem != null) {
                            var primaryItem = secondaryItem.GetPrimaryContentItem();
                            if (primaryItem != null) {
                                var autoroutePart = primaryItem.As<AutoroutePart>();
                                if (autoroutePart != null) {
                                    href = autoroutePart.DisplayAlias;
                                }
                            }
                        }
                    }
                    var defaultHref = XmlHelpers.Attr(description, "defaulturl");
                    return Shape.LayoutElements_Link(
                        Name: T("Link"),
                        Text: string.IsNullOrWhiteSpace(linkText) ? defaultLinkText : linkText,
                        Url: string.IsNullOrWhiteSpace(href) ? defaultHref : href,
                        Top: description.AttrLength("top"),
                        Left: description.AttrLength("left"),
                        LayoutWidth: layoutElement.AttrInt("width"),
                        LayoutHeight: layoutElement.AttrInt("height"),
                        Font: XmlHelpers.Attr(description, "font"),
                        Size: XmlHelpers.Attr(description, "size"),
                        CssClass: XmlHelpers.Attr(description, "class"),
                        ElementTitle: XmlHelpers.Attr(description, "title"),
                        UsesIndex: true,
                        ContentPart: contentPart,
                        ContentItem: contentPart == null ? null : contentPart.ContentItem);
                case "container":
                    var hasLink = description.AttrBool("haslink");
                    var hasBackground = description.AttrBool("hasbackground");
                    var background = XmlHelpers.Attr(data, "background");
                    var defaultBackground = XmlHelpers.Attr(description, "defaultbackground");
                    var targetUrl = XmlHelpers.Attr(data, "href");
                    var hasContext = description.AttrBool("hascontext");
                    IContent context = null;
                    if (hasContext) {
                        var contextId = data.AttrInt("context");
                        context = _contentManager.Get(contextId);
                        if (hasLink && context != null && string.IsNullOrWhiteSpace(targetUrl)) {
                            // Make it a link to the context
                            var autoroutePart = context.As<AutoroutePart>();
                            if (autoroutePart != null) targetUrl = autoroutePart.DisplayAlias;
                        }
                    }
                    if (hasBackground && (context != null || contentPart != null) &&
                        string.IsNullOrWhiteSpace(background)) {
                        // Find an image or media field on the context
                        var contextItem = (context ?? contentPart).ContentItem;
                        if (contextItem != null) {
                            var mediaPickerField = FindMediaPickerField(contextItem);
                            if (mediaPickerField == null) {
                                var secondaryItem = contentPart as ISecondaryContent;
                                if (secondaryItem != null) {
                                    mediaPickerField = FindMediaPickerField(secondaryItem.GetPrimaryContentItem());
                                }
                            }
                            if (mediaPickerField != null) {
                                var dynamicField = (dynamic) mediaPickerField;
                                background = dynamicField.Url;
                                if (background == null && mediaPickerField.GetType().Name == "MediaLibraryPickerField") {
                                    var mediaParts = dynamicField.MediaParts as IEnumerable<MediaPart>;
                                    if (mediaParts != null) {
                                        var mediaPartList = mediaParts.ToList();
                                        if (mediaPartList.Any()) {
                                            var firstMedia = mediaPartList.First();
                                            if (firstMedia != null) {
                                                background = firstMedia.MediaUrl;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    var usesIndex = hasLink || hasBackground || hasContext;
                    return Shape.LayoutElements_Container(
                        Name: T("Container"),
                        TargetUrl: targetUrl,
                        Background: hasBackground ? (string.IsNullOrWhiteSpace(background) ? defaultBackground : background) : "",
                        Width: description.AttrLength("width"),
                        Height: description.AttrLength("height"),
                        Top: description.AttrLength("top"),
                        Left: description.AttrLength("left"),
                        LayoutWidth: layoutElement.AttrInt("width"),
                        LayoutHeight: layoutElement.AttrInt("height"),
                        Center: description.AttrBool("center"),
                        CssClass: XmlHelpers.Attr(description, "class"),
                        ElementTitle: XmlHelpers.Attr(description, "title"),
                        Elements: _templateService.Value.GetLayoutElementDisplays(
                            description, data, hasContext && context != null ? context : contentPart, displayType, dataEnumerators).ToList(),
                        HasContext: hasContext,
                        Context: context,
                        UsesIndex: usesIndex,
                        ContentPart: contentPart,
                        ContentItem: contentPart == null ? null : contentPart.ContentItem);
            }
            return null;
        }

        private static ContentField FindMediaPickerField(IContent contextItem) {
            return contextItem
                .ContentItem
                .Parts
                .SelectMany(
                    p =>
                    p.Fields.Where(
                        f =>
                        f.GetType().Name == "MediaPickerField"
                        || f.GetType().Name == "MediaLibraryField"))
                .FirstOrDefault();
        }

        public IEnumerable<dynamic> BuildLayoutEditors() {
            return new[] {
                Shape.LayoutElements_Image_LayoutEditor(Order: "20"),
                Shape.LayoutElements_Text_LayoutEditor(Order: "24"),
                Shape.LayoutElements_Link_LayoutEditor(Order: "26"),
                Shape.LayoutElements_Container_LayoutEditor(Order: "28")
            };
        }
    }
}
