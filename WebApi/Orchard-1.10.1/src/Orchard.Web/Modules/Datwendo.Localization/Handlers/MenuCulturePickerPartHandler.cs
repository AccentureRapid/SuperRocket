using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Datwendo.Localization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Datwendo.Localization.Handlers
{
    [OrchardFeature("Datwendo.Localization.MenuCulturePicker")]
    public class MenuCulturePickerPartHandler : ContentHandler
    {
        public Localizer T { get; set; }

        public MenuCulturePickerPartHandler(IRepository<MenuCulturePickerPartRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
            T = NullLocalizer.Instance;
        }
    }
}
