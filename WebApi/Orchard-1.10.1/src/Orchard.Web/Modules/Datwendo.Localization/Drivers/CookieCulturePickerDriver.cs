using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Orchard;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using Datwendo.Localization.Models;
using Orchard.Localization.Services;
using Datwendo.Localization.Services;
using Orchard.ContentManagement;
using System.Web.Mvc;
using Datwendo.Localization.ViewModels;
using Orchard.Localization.Models;
using System.Web.Routing;

namespace Datwendo.Localization.Drivers
{
    [OrchardFeature("Datwendo.Localization.CookieCultureSelector")]
    public class CookieCulturePickerDriver : ContentPartDriver<CookieCulturePickerPart> {

        public static readonly string[] Styles = new[] { "Inline List", "Dropdown" };

        protected override string Prefix { get { return "CookieCulturePickerEdit"; } }

        private readonly ICultureService _cultureService;
        private readonly IOrchardServices _orchardServices;
        private readonly IHomePageService _homePageService;
        private readonly IBrowserCultureService _browserCultureService;
        private readonly ICookieCultureService _cookieCultureService;

        public CookieCulturePickerDriver(IOrchardServices orchardServices
            , ICultureService cultureService
            , IHomePageService homePageService
            , IBrowserCultureService browserCultureService
            , ICookieCultureService cookieCultureService)
        {
            _orchardServices        = orchardServices;
            _cultureService         = cultureService;
            _homePageService        = homePageService;
            _browserCultureService  = browserCultureService;
            _cookieCultureService   = cookieCultureService;
        }

        protected override DriverResult Display(CookieCulturePickerPart part, string displayType, dynamic shapeHelper)
        {
            bool isBrowserCurrent = _cookieCultureService.isBrowserCurrent();
            string currentCulture = null;
            var cookieCultureItems = _cultureService.BuildViewModel(part.ShowBrowser, isBrowserCurrent, out currentCulture);
            
            if (part.Style == Styles[1])
            {
                return ContentShape("Parts_CookieCulturePickerWidget", () => shapeHelper.Parts_DropdownCookieCulturePicker(Cultures: cookieCultureItems, CurrentCulture: currentCulture));
            }
            else
            {
                return ContentShape("Parts_CookieCulturePickerWidget", () => shapeHelper.Parts_InlineListCookieCulturePicker(Cultures: cookieCultureItems, CurrentCulture: currentCulture));
            }
        }

        protected override DriverResult Editor(CookieCulturePickerPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_CookieCulturePicker_Edit",
                () => shapeHelper.EditorTemplate(TemplateName: "Parts/CookieCulturePicker", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(CookieCulturePickerPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}
