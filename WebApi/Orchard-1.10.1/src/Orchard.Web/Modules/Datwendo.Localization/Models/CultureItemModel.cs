using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Datwendo.Localization.Models
{
    public class CultureItemModel
    {
        public string Culture { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public string LocalizedName { get; set; }
    }
}
