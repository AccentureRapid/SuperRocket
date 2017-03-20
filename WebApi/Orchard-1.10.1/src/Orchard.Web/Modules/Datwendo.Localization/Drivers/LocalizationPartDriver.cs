using System.Collections.Generic;
using System.Linq;
//using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Localization.Models;
using Orchard.Localization.Services;
using Orchard.Localization.ViewModels;
using Datwendo.Localization.Services;
using Orchard.Environment.Extensions;
using Orchard.ContentManagement.Handlers;

namespace Datwendo.Localization.Drivers
{
    //[UsedImplicitly]
    [OrchardFeature("Datwendo.Localization")]
    [OrchardSuppressDependency("Orchard.Localization.Drivers.LocalizationPartDriver")]
    public class LocalizationPartDriver : ContentPartDriver<LocalizationPart>
    {
        private const string TemplatePrefix = "Localization";
        private readonly ICultureService _cultureService;
        private readonly Orchard.Localization.Drivers.LocalizationPartDriver _orgDriver;
        private readonly ICultureManager _cultureManager;
        private readonly ILocalizationService _localizationService;
        private readonly IContentManager _contentManager;

        public LocalizationPartDriver(ICultureService cultureService
            ,ICultureManager cultureManager, 
                ILocalizationService localizationService, 
                IContentManager contentManager) 
        {
            _cultureService         = cultureService;
            _cultureManager         = cultureManager;
            _localizationService    = localizationService;
            _contentManager         = contentManager;
            _orgDriver              = new Orchard.Localization.Drivers.LocalizationPartDriver(cultureManager,localizationService, contentManager);
        }

        protected override DriverResult Display(LocalizationPart part, string displayType, dynamic shapeHelper) 
        {
            var masterId = part.MasterContentItem != null
                   ? part.MasterContentItem.Id
                   : part.Id;
            var localizations   = _cultureService.GetLocalizations(part, VersionOptions.Latest).ToList();
            var siteCulture     = _cultureService.GetSiteCulture();
            var selectedCulture = part.Culture != null ? part.Culture.Culture : (part.Id == 0 ? siteCulture : null);
            var sC              = _cultureService.ListCultures();
            var siteCultures    = sC.Where(c => c.Culture != null)
                                    .Select(c => c.Culture)
                                    .Where(s => !localizations.Select(l => l.Culture.Culture).Contains(s));
            return Combined(
                ContentShape("Parts_ExLocalization_ContentTranslations",
                             () => shapeHelper.Parts_ExLocalization_ContentTranslations(MasterId: masterId, Localizations: localizations.Where(c => c.Culture.Culture != selectedCulture))),
                ContentShape("Parts_ExLocalization_ContentTranslations_Summary",
                             () => shapeHelper.Parts_ExLocalization_ContentTranslations_Summary(MasterId: masterId, 
                                                                                        ShowAddTranslation: _cultureService.ListCultures().Select(c => c.Culture).Where(s => s != siteCulture && !localizations.Select(l => l.Culture.Culture).Contains(s)).Any(),
                                                                                        SelectedCulture: selectedCulture,
                                                                                        ContentItem: part.ContentItem,
                                                                                        SiteCultures: siteCultures,
                                                                                        Localizations: localizations.Where(c => c.Culture.Culture != selectedCulture))),
                ContentShape("Parts_ExLocalization_ContentTranslations_SummaryAdmin",
                             () => shapeHelper.Parts_ExLocalization_ContentTranslations_SummaryAdmin(MasterId: masterId,
                                                                                        MasterContentItem: part.MasterContentItem,
                                                                                        ShowAddTranslation: _cultureService.ListCultures().Select(c => c.Culture).Where(s => s != siteCulture && !localizations.Select(l => l.Culture.Culture).Contains(s)).Any(),
                                                                                        SelectedCulture: selectedCulture,
                                                                                        SiteCultures: siteCultures,
                                                                                        ContentItem : part.ContentItem,
                                                                                        Localizations: localizations.Where(c => c.Culture.Culture != selectedCulture)))
                                                                                        );
        }


        protected override DriverResult Editor(LocalizationPart part, dynamic shapeHelper) 
        {
            var lPs             = _cultureService.GetLocalizations(part, VersionOptions.Latest).ToList();
            var localizations   = lPs.Where(l => l.Culture.Culture != null).ToList();
            var siteCulture     = _cultureService.GetSiteCulture();
            var selectedCulture = part.Culture != null ? part.Culture.Culture : (part.Id == 0 ? siteCulture : null);
            var sC              = _cultureService.ListCultures();
            var siteCultures    = sC.Where(c => c.Culture != null)
                                    .Select(c => c.Culture)
                                    .Where(s => !localizations.Select(l => l.Culture.Culture).Contains(s));
            var model = new EditLocalizationViewModel
            {
                SelectedCulture         = selectedCulture,
                SiteCultures            = siteCultures,
                ContentItem             = part,
                MasterContentItem       = part.MasterContentItem,
                ContentLocalizations    = new ContentLocalizationsViewModel(part) { Localizations = localizations.Where(c => c.Culture.Culture != selectedCulture) }
            };

            return ContentShape("Parts_ExLocalization_ContentTranslations_Edit",
                () => shapeHelper.EditorTemplate(TemplateName: "Parts/ExLocalization.ContentTranslations", Model: model, Prefix: TemplatePrefix));
        }

        protected override DriverResult Editor(LocalizationPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            var model = new EditLocalizationViewModel();
            if (updater != null && updater.TryUpdateModel(model, TemplatePrefix, null, null))
            {
                _localizationService.SetContentCulture(part, model.SelectedCulture);
            }

            return Editor(part, shapeHelper);
        }


        protected override void Importing(LocalizationPart part, ImportContentContext context)
        {
            var masterContentItem = context.Attribute(part.PartDefinition.Name, "MasterContentItem");
            if (masterContentItem != null)
            {
                var contentItem = context.GetItemFromSession(masterContentItem);
                if (contentItem != null)
                {
                    part.MasterContentItem = contentItem;
                }
            }

            var culture = context.Attribute(part.PartDefinition.Name, "Culture");
            if (culture != null)
            {
                var targetCulture = _cultureManager.GetCultureByName(culture);
                // Add Culture.
                if (targetCulture == null && _cultureManager.IsValidCulture(culture))
                {
                    _cultureManager.AddCulture(culture);
                    targetCulture = _cultureManager.GetCultureByName(culture);
                }
                part.Culture = targetCulture;
            }
        }

        protected override void Exporting(LocalizationPart part,ExportContentContext context)
        {
            if (part.MasterContentItem != null)
            {
                var masterContentItemIdentity = _contentManager.GetItemMetadata(part.MasterContentItem).Identity;
                context.Element(part.PartDefinition.Name).SetAttributeValue("MasterContentItem", masterContentItemIdentity.ToString());
            }

            if (part.Culture != null)
            {
                context.Element(part.PartDefinition.Name).SetAttributeValue("Culture", part.Culture.Culture);
            }
        }
    }
}