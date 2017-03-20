using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Onestop.Layouts.Models;
using Onestop.Layouts.Services;
using Onestop.Layouts.Settings;
using Onestop.Layouts.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace Onestop.Layouts.Drivers {
    public interface ITemplatedItemPartDriver : IDependency {
        dynamic GetDisplayShape(TemplatedItemPart part, string displayType, dynamic shapeHelper, IEnumerable<LayoutTemplatePart> layouts = null);
    }

    [OrchardFeature("Onestop.Layouts")]
    public class TemplatedItemPartDriver : ContentPartDriver<TemplatedItemPart>, ITemplatedItemPartDriver {
        private readonly ITemplateService _templateService;
        private readonly IContentManager _contentManager;

        public TemplatedItemPartDriver(ITemplateService templateService, IContentManager contentManager, IOrchardServices services) {
            _templateService = templateService;
            _contentManager = contentManager;
            Services = services;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public IOrchardServices Services { get; set; }

        protected override string Prefix {
            get { return "TemplatedItem"; }
        }

        protected override DriverResult Display(TemplatedItemPart part, string displayType, dynamic shapeHelper) {
            return ContentShape(
                "Parts_TemplatedItem",
                () => GetDisplayShape(part, displayType, shapeHelper));
        }

        public dynamic GetDisplayShape(TemplatedItemPart part, string displayType, dynamic shapeHelper, IEnumerable<LayoutTemplatePart> layouts = null) {
            var template = GetLayoutTemplatePart(part, layouts);
            var elementShapes = _templateService.GetLayoutElementDisplays(part, template, displayType).ToList();
            var templatedItemShape = shapeHelper.Parts_TemplatedItem(
                ContentItem: part.ContentItem,
                Elements: elementShapes,
                LayoutTemplate: template,
                Id: part.ContentItem == null ? 0 : part.ContentItem.Id,
                Data: part.Data);
            var thumbnailElement = elementShapes.FirstOrDefault(s => s.Thumbnail != null);
            if (thumbnailElement != null) {
                templatedItemShape.Thumbnail = thumbnailElement.Thumbnail;
            }
            templatedItemShape.Metadata.DisplayType = displayType;
            return templatedItemShape;
        }

        protected override DriverResult Editor(TemplatedItemPart part, dynamic shapeHelper) {
            var template = GetLayoutTemplatePart(part);
            var templates = _templateService.GetTemplates();
            var model = new TemplatedItemViewModel {
                Prefix = Prefix,
                TemplatedItem = part,
                Template = template,
                Templates = templates,
                CanChangeTemplate = part.Settings.GetModel<TemplatedItemPartSettings>().AllowTemplateChoice,
                EditorShapes = _templateService.GetLayoutElementEditors(part, template, Prefix).ToList(),
                TemplatedItemPreviewShape = GetDisplayShape(part, "Preview", shapeHelper)
            };
            return ContentShape("Parts_TemplatedItem_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/TemplatedItem",
                    Model: model,
                    Prefix: Prefix));
        }

        protected override DriverResult Editor(TemplatedItemPart part, IUpdateModel updater, dynamic shapeHelper) {
            var model = new TemplatedItemUpdateModel();
            updater.TryUpdateModel(model, Prefix, null, null);
            if (model.TemplatedItemData != null)
            {
            var layout = _contentManager.Get<LayoutTemplatePart>(model.LayoutId);
            part.LayoutId = (layout != null) ? layout.Id : default(int?);
                _templateService.PersistLayoutElement(
                    model.TemplatedItemData, part, layout,
                    updater, Prefix);
            }
            return Editor(part, shapeHelper);
        
        }

        protected override void Importing(TemplatedItemPart part, ImportContentContext context) {
            var layoutElement = context.Data.Element("TemplatedItemPart");
            if (layoutElement == null) return;
            part.Data = layoutElement.Value;
            var layoutId = layoutElement.Attr("layout");
            if (String.IsNullOrEmpty(layoutId)) return;
            var layout = context.GetItemFromSession(layoutId);
            if (layout != null) {
                part.LayoutId = layout.Id;
            }
        }

        protected override void Exporting(TemplatedItemPart part, ExportContentContext context) {
            var elt = context.Element("TemplatedItemPart");
            if (!string.IsNullOrWhiteSpace(part.Data)) {
                elt.Value = part.Data;
            }
            if (part.LayoutId == null || part.LayoutId == 0) return;
            var layout = _contentManager.Get<LayoutTemplatePart>(part.LayoutId.Value);
            if (layout == null) return;
            var layoutId = _contentManager.GetItemMetadata(layout).Identity.ToString();
            elt.Add(new XAttribute("layout", layoutId));
        }

        private LayoutTemplatePart GetLayoutTemplatePart(TemplatedItemPart part, IEnumerable<LayoutTemplatePart> layouts = null) {
            if (part.LayoutId == null) return null;
            return layouts == null
                ? _contentManager.Get<LayoutTemplatePart>(part.LayoutId.GetValueOrDefault())
                : layouts.FirstOrDefault(l => l.Id == part.LayoutId);
        }
    }
}