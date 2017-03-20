using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Datwendo.Localization.Models
{
    [OrchardFeature("Datwendo.Localization.CookieCultureSelector")]
    public class CookieCulturePickerPart : ContentPart<CookieCulturePickerPartRecord>
    {
        public string Style 
        { 
            get { return Retrieve(x => x.Style); }
            set { Store(x => x.Style, value); }
        }
        public bool ShowBrowser 
        {
            get { return Retrieve(x => x.ShowBrowser); }
            set { Store(x => x.ShowBrowser, value); }
        }
    }
}
