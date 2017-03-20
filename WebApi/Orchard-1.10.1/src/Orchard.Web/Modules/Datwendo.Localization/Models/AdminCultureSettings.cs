using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage.InfosetStorage;
using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datwendo.Localization.Models
{
    public class AdminCultureSettings
    {
        public string AdminCulture { get; set; }
        public bool Enabled { get; set; }
        public int Priority { get; set; }
    }

    [OrchardFeature("Datwendo.Localization.AdminCultureSelector")]
    public class AdminCultureSettingsPart : ContentPart
    {
        public const string CacheKey = "AdminCultureSettingsPart";
        public string AdminCulture
        {
            get 
            {
                return this.As<InfosetPart>().Get<AdminCultureSettingsPart>("AdminCulture");
            }
            set 
            {
                this.As<InfosetPart>().Set<AdminCultureSettingsPart>("AdminCulture", value.ToString());
            }
        }

        public bool Enabled
        {
            get
            {
                var attributeValue = this.As<InfosetPart>().Get<AdminCultureSettingsPart>("Enabled");
                return !String.IsNullOrWhiteSpace(attributeValue) && Convert.ToBoolean(attributeValue);
            }
            set { this.As<InfosetPart>().Set<AdminCultureSettingsPart>("Enabled", value.ToString()); }
        }

        public int Priority
        {
            get
            {
                var attributeValue = this.As<InfosetPart>().Get<AdminCultureSettingsPart>("Priority");
                return String.IsNullOrWhiteSpace(attributeValue) ? int.MinValue : Convert.ToInt32(attributeValue);
            }
            set { this.As<InfosetPart>().Set<AdminCultureSettingsPart>("Priority", value.ToString()); }
        }
      }
}
