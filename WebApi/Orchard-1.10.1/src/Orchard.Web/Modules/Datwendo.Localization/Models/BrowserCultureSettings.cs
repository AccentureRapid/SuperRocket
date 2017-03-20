using System;
using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage.InfosetStorage;
using Orchard.Environment.Extensions;

namespace Datwendo.Localization.Models
{
    public class BrowserCultureSettings {
        public bool Enabled { get; set; }
        public int Priority { get; set; }
    }

    [OrchardFeature("Datwendo.Localization.BrowserCultureSelector")]
    public class BrowserCultureSettingsPart : ContentPart
    {
        public const string CacheKey = "BrowserCultureSettingsPart";

        public bool Enabled 
        {
            get 
            {
                var attributeValue = this.As<InfosetPart>().Get<BrowserCultureSettingsPart>("Enabled");
                return !String.IsNullOrWhiteSpace(attributeValue) && Convert.ToBoolean(attributeValue);
            }
            set { this.As<InfosetPart>().Set<BrowserCultureSettingsPart>("Enabled", value.ToString()); }
        }

        public int Priority
        {
            get
            {
                var attributeValue = this.As<InfosetPart>().Get<BrowserCultureSettingsPart>("Priority");
                return String.IsNullOrWhiteSpace(attributeValue) ? int.MinValue : Convert.ToInt32(attributeValue);
            }
            set { this.As<InfosetPart>().Set<BrowserCultureSettingsPart>("Priority", value.ToString()); }
        }
    }
}