using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datwendo.Localization.ViewModels
{
    public class AddLocalizationViewModel
    {
        public dynamic Content { get; set; }
        public int Id { get; set; }
        public string SelectedCulture { get; set; }
        public IEnumerable<string> SiteCultures { get; set; }
    }
}
