using System.Collections.Generic;
using System.Linq;
using Onestop.Layouts.Models;
using Onestop.Layouts.Services;
using Onestop.Layouts.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace Onestop.Layouts.Drivers {
    [OrchardFeature("Onestop.Layouts")]
    public class LayoutTemplatePartDriver : ContentPartDriver<LayoutTemplatePart> {
        private readonly ILayoutService _layoutService;
        private readonly IStylesheetService _stylesheetService;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IContentManager _contentManager;

        public LayoutTemplatePartDriver(
            ILayoutService layoutService, 
            IStylesheetService stylesheetService,
            IContentDefinitionManager contentDefinitionManager,
            IContentManager contentManager) {

            _layoutService = layoutService;
            _stylesheetService = stylesheetService;
            _contentDefinitionManager = contentDefinitionManager;
            _contentManager = contentManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        protected override string Prefix {
            get { return "OnestopLayout"; }
        }

        protected override DriverResult Display(LayoutTemplatePart part, string displayType, dynamic shapeHelper) {
            return ContentShape(
                "Parts_Layout",
                () => shapeHelper.Parts_SlideLayout(
                    ContentItem: part.ContentItem,
                    LayoutDescription: part.LayoutDescription));
        }

        protected override DriverResult Editor(LayoutTemplatePart part, dynamic shapeHelper) {
            var isTemplate = false;
            var typeDefinition = _contentDefinitionManager
                .GetTypeDefinition(part.ContentItem.ContentType);
            if (typeDefinition != null) {
                var LayoutTemplatePart = typeDefinition
                    .Parts
                    .SingleOrDefault(p => p.PartDefinition.Name == "LayoutTemplatePart");
                if (LayoutTemplatePart != null) {
                    isTemplate = LayoutTemplatePart.Settings.ContainsKey("isTemplate");
                }
            }

            IEnumerable<LayoutTemplatePart> layouts = null;
            if (isTemplate) {
                layouts = _layoutService
                    .GetLayouts();
            }

            var stylesheets = _stylesheetService.GetAvailableStylesheets().ToList();
            var classes = stylesheets
                .ToDictionary(
                    s => s.VirtualPath,
                    s => s.GetClasses());
            var fonts = stylesheets
                .ToDictionary(
                    s => s.VirtualPath,
                    s => s.GetFonts());

            return ContentShape("Parts_Layout_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/Layout",
                    Model: new LayoutViewModel {
                        Layout = part,
                        Layouts = layouts,
                        ParentLayoutId = part.ParentLayoutId,
                        Stylesheets = stylesheets,
                        Stylesheet = stylesheets.FirstOrDefault(s => s.VirtualPath == part.StylesheetPath),
                        CssClasses = classes,
                        Fonts = fonts,
                        LayoutElementEditors = _layoutService.GetLayoutElementEditors(),
                        IsTemplate = isTemplate
                    },
                    Prefix: Prefix));
        }

        protected override DriverResult Editor(LayoutTemplatePart part, IUpdateModel updater, dynamic shapeHelper) {
            var model = new LayoutViewModel {Layout = part};
            var oldLayoutId = part.ParentLayoutId ?? -1;
            updater.TryUpdateModel(model, Prefix, null, null);
            if (model.ParentLayoutId != null && model.ParentLayoutId != 0) {
                var parentLayout = _contentManager.Get<LayoutTemplatePart>(model.ParentLayoutId.Value);
                model.Layout.ParentLayoutId = parentLayout.Id;
            }
            var descriptionDocument = part.LayoutDescriptionDocument;
            var layoutElement = descriptionDocument.Element("layout");
            var parentLayoutRecordId = model.Layout.ParentLayoutId ?? -1;
            if (layoutElement == null) {
                updater.AddModelError(
                    Prefix,
                    T("Layout description document should have layout as its top-level element."));
            }
            else if ((!layoutElement.Elements().Any() && model.ParentLayoutId != null) ||
                (oldLayoutId != parentLayoutRecordId && model.Layout.ParentLayoutId != null)) {
                    var parentLayout = _contentManager.Get<LayoutTemplatePart>(model.ParentLayoutId.Value);
                model.Layout.LayoutDescription = parentLayout != null ? parentLayout.LayoutDescription : default(string);
            }

            return Editor(part, shapeHelper);
        }

        protected override void Importing(LayoutTemplatePart part, ImportContentContext context) {
            var layoutElement = context.Data.Element("LayoutTemplatePart");
            if (layoutElement != null) {
                part.LayoutDescription = layoutElement.Value;
            }
        }

        protected override void Exporting(LayoutTemplatePart part, ExportContentContext context) {
            if (!string.IsNullOrWhiteSpace(part.LayoutDescription)) {
                var elt = context.Element("LayoutTemplatePart");
                elt.Value = part.LayoutDescription;
            }
        }
    }
}