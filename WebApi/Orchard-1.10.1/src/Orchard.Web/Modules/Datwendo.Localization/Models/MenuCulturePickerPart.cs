using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Datwendo.Localization.Models
{
    [OrchardFeature("Datwendo.Localization.MenuCulturePicker")]
    public class MenuCulturePickerPart : ContentPart<MenuCulturePickerPartRecord>
    {
        public bool ShowBrowser 
        {
            get { return Retrieve(x => x.ShowBrowser); }
            set { Store(x => x.ShowBrowser, value); }
        }
    }
}
