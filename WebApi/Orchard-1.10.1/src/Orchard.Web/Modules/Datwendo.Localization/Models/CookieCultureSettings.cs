using System;
using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage.InfosetStorage;
using Orchard.Environment.Extensions;

namespace Datwendo.Localization.Models
{
    public class CookieCultureSettings {
        public bool Enabled { get; set; }
        public int Priority { get; set; }
        public bool EnforceCookieUrl { get; set; }
    }

    [OrchardFeature("Datwendo.Localization.CookieCultureSelector")]
    public class CookieCultureSettingsPart : ContentPart
    {
        public const string CacheKey = "CookieCultureSettingsPart";


        public bool Enabled 
        {
            get 
            {
                var attributeValue = this.As<InfosetPart>().Get<CookieCultureSettingsPart>("Enabled");
                return !String.IsNullOrWhiteSpace(attributeValue) && Convert.ToBoolean(attributeValue);
            }
            set { this.As<InfosetPart>().Set<CookieCultureSettingsPart>("Enabled", value.ToString()); }
        }

        public int Priority
        {
            get
            {
                var attributeValue = this.As<InfosetPart>().Get<CookieCultureSettingsPart>("Priority");
                return String.IsNullOrWhiteSpace(attributeValue) ? int.MinValue : Convert.ToInt32(attributeValue);
            }
            set { this.As<InfosetPart>().Set<CookieCultureSettingsPart>("Priority", value.ToString()); }
        }

        public bool EnforceCookieUrl
        {
            get
            {
                var attributeValue = this.As<InfosetPart>().Get<CookieCultureSettingsPart>("EnforceCookieUrl");
                return !String.IsNullOrWhiteSpace(attributeValue) && Convert.ToBoolean(attributeValue);
            }
            set { this.As<InfosetPart>().Set<CookieCultureSettingsPart>("EnforceCookieUrl", value.ToString()); }
        }
    }
}