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
    public class MenuCulturePickerDriver : ContentPartDriver<MenuCulturePickerPart> 
    {
        protected override string Prefix { get { return "MenuCulturePicker"; } }

        private readonly ICultureService _cultureService;
        //private readonly IBrowserCultureService _browserCultureService;
        private readonly ICookieCultureService _cookieCultureService;

        public MenuCulturePickerDriver(ICultureService cultureService
            //, IBrowserCultureService browserCultureService
            , ICookieCultureService cookieCultureService)
        {
            _cultureService         = cultureService;
            //_browserCultureService  = browserCultureService;
            _cookieCultureService   = cookieCultureService;
        }

        protected override DriverResult Display(MenuCulturePickerPart part, string displayType, dynamic shapeHelper)
        {
            bool isBrowserCurrent           = _cookieCultureService.isBrowserCurrent();
            string currentCulture           = null;
            var cookieCultureItems          = _cultureService.BuildViewModel(part.ShowBrowser, isBrowserCurrent, out currentCulture);
            return ContentShape("Parts_CookieCulturePickerMenu", () => shapeHelper.Parts_CookieCulturePickerMenu(Cultures: cookieCultureItems, CurrentCulture: currentCulture));
        }


        protected override DriverResult Editor(MenuCulturePickerPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_MenuCulturePicker_Edit",
                () => shapeHelper.EditorTemplate(TemplateName: "Parts/MenuCulturePicker", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(MenuCulturePickerPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}
