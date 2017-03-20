using System;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using Orchard.Utility.Extensions;
using Dolph.SmartContentPicker.ViewModels;
using Dolph.SmartContentPicker.Settings;
using System.Collections.Generic;
using Orchard.ContentManagement.MetaData.Models;
using Orchard;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Common.Models;
using Orchard.Core.Contents.Settings;
using Orchard.Security;
using Orchard.Core.Title.Models;

using Orchard.Roles.Models;
using Orchard.ContentPermissions.Models;

namespace Dolph.SmartContentPicker.Drivers
{    
    public class SmartContentPickerFieldDriver : ContentFieldDriver<Fields.SmartContentPickerField> {
        private readonly IContentManager _contentManager;
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public SmartContentPickerFieldDriver(IContentManager contentManager,
                                             IOrchardServices orchardServices,
                                             IContentDefinitionManager contentDefinitionManager)
        {

            _contentManager = contentManager;
            _contentDefinitionManager = contentDefinitionManager;
            Services = orchardServices;            
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; set; }

        public Localizer T { get; set; }

        private static string GetPrefix(Fields.SmartContentPickerField field, ContentPart part) {
            return part.PartDefinition.Name + "." + field.Name;
        }

        private static string GetDifferentiator(Fields.SmartContentPickerField field, ContentPart part) {
            return field.Name;
        }

        protected override DriverResult Display(ContentPart part, Fields.SmartContentPickerField field, string displayType, dynamic shapeHelper) {
            return Combined(
                ContentShape("Fields_SmartContentPicker", GetDifferentiator(field, part), () => shapeHelper.Fields_SmartContentPicker()),
                ContentShape("Fields_SmartContentPicker_SummaryAdmin", GetDifferentiator(field, part), () => shapeHelper.Fields_SmartContentPicker_SummaryAdmin())
            );
        }

        protected override DriverResult Editor(ContentPart part, Fields.SmartContentPickerField field, dynamic shapeHelper) {
            return ContentShape("Fields_SmartContentPicker_Edit", GetDifferentiator(field, part),
                () => {
                    var settings = field.PartFieldDefinition.Settings.GetModel<SmartContentPickerFieldSettings>();
                    IEnumerable<ContentTypeDefinition> contentTypes;
                    if (!String.IsNullOrEmpty(settings.DisplayedContentTypes))
                    {
                        var rawTypes = settings.DisplayedContentTypes.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        contentTypes = _contentDefinitionManager
                            .ListTypeDefinitions()
                            .Where(x => x.Parts.Any(p => rawTypes.Contains(p.PartDefinition.Name)) || rawTypes.Contains(x.Name))
                            .ToArray();
                    }
                    else
                    {
                        contentTypes = GetCreatableTypes(false).ToList();
                    }

                    var contentItems = Services.ContentManager.Query(VersionOptions.Latest, contentTypes.Select(ctd => ctd.Name).ToArray())
                                                                            .OrderBy<TitlePartRecord>(cr => cr.Title)
                                                                            //.OrderByDescending<CommonPartRecord>(cr => cr.ModifiedUtc)
                                                                            .List();

                    var contentType = contentTypes.Select(ctd => ctd.Name).ToArray();

                    if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner))
                    {
                        if (contentType.Contains("App"))
                        {
                            //App content item permission filtering by role
                            var roles = (Services.WorkContext.CurrentUser.ContentItem).As<UserRolesPart>().Roles;
                            contentItems = contentItems.Where(ci => roles.Select(x => x).Intersect((ci.As<ContentPermissionsPart>().ViewContent == null ? "0" : ci.As<ContentPermissionsPart>().ViewContent).Split(',')).Any());
                        }
                        else 
                        {
                            //filter by owned items
                            //contentItems = contentItems.Where(cr => cr.As<CommonPart>().Owner.Id == Services.WorkContext.CurrentUser.Id);
                            contentItems = contentItems.Where(cr => Services.WorkContext.CurrentUser.Id == (cr.As<CommonPart>().Owner == null ? 0 : cr.As<CommonPart>().Owner.Id));
                        }
                    }

                    var model = new SmartContentPickerFieldViewModel
                    {
                        Field = field,
                        Part = part,
                        ContentItems = _contentManager.GetMany<ContentItem>(field.Ids, VersionOptions.Published, QueryHints.Empty).ToList(),
                        AvailableContentItems = contentItems
                    };

                    //model.SelectedIds = string.Concat(",", field.Ids);

                    return shapeHelper.EditorTemplate(TemplateName: "Fields/SmartContentPicker.Edit", Model: model, Prefix: GetPrefix(field, part));
                });
        }

        protected override DriverResult Editor(ContentPart part, Fields.SmartContentPickerField field, IUpdateModel updater, dynamic shapeHelper) {
            var model = new SmartContentPickerFieldViewModel();

            updater.TryUpdateModel(model, GetPrefix(field, part), null, null);

            var settings = field.PartFieldDefinition.Settings.GetModel<SmartContentPickerFieldSettings>();

            if (model.SelectedIds == null || model.SelectedIds.Length == 0 || (model.SelectedIds.Length == 1 && model.SelectedIds[0] == string.Empty))
            {
                field.Ids = new int[0];
            }
            else {
                field.Ids = Array.ConvertAll(model.SelectedIds, int.Parse);
            }

            if (settings.Required && field.Ids.Length == 0) {
                updater.AddModelError("Id", T("The field {0} is mandatory", field.Name.CamelFriendly()));
            }

            return Editor(part, field, shapeHelper);
        }

        protected override void Importing(ContentPart part, Fields.SmartContentPickerField field, ImportContentContext context) {
            var contentItemIds = context.Attribute(field.FieldDefinition.Name + "." + field.Name, "ContentItems");
            if (contentItemIds != null) {
                field.Ids = contentItemIds.Split(',')
                    .Select(context.GetItemFromSession)
                    .Select(contentItem => contentItem.Id).ToArray();
            }
            else {
                field.Ids = new int[0];
            }
        }

        protected override void Exporting(ContentPart part, Fields.SmartContentPickerField field, ExportContentContext context) {
            if (field.Ids.Any()) {
                var contentItemIds = field.Ids
                    .Select(x => _contentManager.Get(x))
                    .Select(x => _contentManager.GetItemMetadata(x).Identity.ToString())
                    .ToArray();

                context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("ContentItems", string.Join(",", contentItemIds));
            }
        }

        protected override void Describe(DescribeMembersContext context) {
            context
                .Member(null, typeof(string), T("Ids"), T("A formatted list of the ids, e.g., {1},{42}"));
        }

        private IEnumerable<ContentTypeDefinition> GetCreatableTypes(bool andContainable)
        {
            return _contentDefinitionManager.ListTypeDefinitions().Where(ctd => ctd.Settings.GetModel<ContentTypeSettings>().Creatable && (!andContainable || ctd.Parts.Any(p => p.PartDefinition.Name == "ContainablePart")));
        }
    }
}