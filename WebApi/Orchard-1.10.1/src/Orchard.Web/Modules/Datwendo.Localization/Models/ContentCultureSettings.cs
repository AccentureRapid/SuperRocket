using System;
using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage.InfosetStorage;
using Orchard.Environment.Extensions;

namespace Datwendo.Localization.Models
{
    public class ContentCultureSettings {
        public bool Enabled { get; set; }
        public int Priority { get; set; }
    }

    [OrchardFeature("Datwendo.Localization.ContentCultureSelector")]
    public class ContentCultureSettingsPart : ContentPart
    {
        public const string CacheKey = "ContentCultureSettingsPart";


        public bool Enabled {
            get {
                var attributeValue = this.As<InfosetPart>().Get<ContentCultureSettingsPart>("Enabled");
                return !String.IsNullOrWhiteSpace(attributeValue) && Convert.ToBoolean(attributeValue);
            }
            set { this.As<InfosetPart>().Set<ContentCultureSettingsPart>("Enabled", value.ToString()); }
        }

        public int Priority
        {
            get
            {
                var attributeValue = this.As<InfosetPart>().Get<ContentCultureSettingsPart>("Priority");
                return String.IsNullOrWhiteSpace(attributeValue) ? int.MinValue : Convert.ToInt32(attributeValue);
            }
            set { this.As<InfosetPart>().Set<ContentCultureSettingsPart>("Priority", value.ToString()); }
        }
    }
}