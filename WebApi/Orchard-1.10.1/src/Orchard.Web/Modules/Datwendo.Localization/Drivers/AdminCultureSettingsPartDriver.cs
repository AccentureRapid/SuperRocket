using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using Datwendo.Localization.Models;
using Datwendo.Localization.ViewModels;
using Datwendo.Localization.Services;

namespace Datwendo.Localization.Drivers
{
    public class AdminCultureSettingsPartDriver : ContentPartDriver<AdminCultureSettingsPart> 
    {
        private readonly ISignals _signals;
        private readonly ICultureService _cultureService;
        private const string TemplateName = "Parts/AdminCultureSettings";

        public AdminCultureSettingsPartDriver(ISignals signals,ICultureService cultureService) 
        {
            _signals        = signals;
            T               = NullLocalizer.Instance;
            _cultureService = cultureService;
        }

        public Localizer T { get; set; }

        protected override string Prefix {
            get { return "AdminCultureSettings"; }
        }

        protected override DriverResult Editor(AdminCultureSettingsPart part, dynamic shapeHelper) 
        {
            var siteCultures            = _cultureService.SiteCultureNames();
            AdminCultureViewModel model = new AdminCultureViewModel
            {
                AdminCulture = new AdminCultureSettings { AdminCulture = part.AdminCulture, Enabled = part.Enabled, Priority = part.Priority },
                SiteCultures = siteCultures,
                SelectedCulture = part.AdminCulture
            };
            return ContentShape("Parts_AdminCultureSettings_Edit",
                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: model, Prefix: Prefix))
                .OnGroup("Localization");
        }

        protected override DriverResult Editor(AdminCultureSettingsPart part, IUpdateModel updater, dynamic shapeHelper) 
        {
            AdminCultureViewModel model = new AdminCultureViewModel();
            if (updater.TryUpdateModel(model, Prefix, null, null)) 
            {
                bool signal             = false;
                if (model.AdminCulture != null)
                {
                    part.Priority       = model.AdminCulture.Priority;
                    part.Enabled        = model.AdminCulture.Enabled;
                    signal              = true;
                }
                if (model.SelectedCulture != null )
                { 
                    part.AdminCulture   = model.SelectedCulture;
                    signal              = true;
                }
                if ( signal )
                    _signals.Trigger(AdminCultureSettingsPart.CacheKey);
            }
            return Editor(part, shapeHelper);
        }

        protected override void Importing(AdminCultureSettingsPart part, ImportContentContext context) 
        {
            var elementName     = part.PartDefinition.Name;
            part.Enabled        = bool.Parse(context.Attribute(elementName, "Enabled") ?? "false");
            part.Priority       = int.Parse(context.Attribute(elementName, "Priority") ?? "0");
            part.AdminCulture   = context.Attribute(elementName, "AdminCulture");
            _signals.Trigger(AdminCultureSettingsPart.CacheKey);
        }

        protected override void Exporting(AdminCultureSettingsPart part, ExportContentContext context) 
        {
            var el = context.Element(part.PartDefinition.Name);
            el.SetAttributeValue("Enabled", part.Enabled);
            el.SetAttributeValue("Priority", part.Priority.ToString());
            el.SetAttributeValue("AdminCulture", part.AdminCulture);
        }
    }
}