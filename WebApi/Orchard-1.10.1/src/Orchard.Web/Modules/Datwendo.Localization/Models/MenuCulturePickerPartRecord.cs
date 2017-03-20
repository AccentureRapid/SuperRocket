using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.ContentManagement.Records;

namespace Datwendo.Localization.Models
{
    [OrchardFeature("Datwendo.Localization.MenuCulturePicker")]
    public class MenuCulturePickerPartRecord : ContentPartRecord
    {
        public virtual bool ShowBrowser { get; set; }
    }
}
