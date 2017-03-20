using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Localization.Models;
using Datwendo.Localization.Models;

namespace Datwendo.Localization.ViewModels
{
    public class AdminCultureViewModel  
    {
        public AdminCultureSettings AdminCulture { get; set; }
        public string SelectedCulture { get; set; }
        public IEnumerable<string> SiteCultures { get; set; }
    }
}