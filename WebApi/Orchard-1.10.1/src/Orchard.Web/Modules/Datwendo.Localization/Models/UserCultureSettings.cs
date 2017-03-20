using System;
using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage.InfosetStorage;
using Orchard.Environment.Extensions;

namespace Datwendo.Localization.Models
{
    public class UserCultureSettings {
        public bool Enabled { get; set; }
        public int Priority { get; set; }
    }

    [OrchardFeature("Datwendo.Localization.UserCultureSelector")]
    public class UserCultureSettingsPart : ContentPart
    {
        public const string CacheKey = "UserCultureSettingsPart";

        public bool Enabled 
        {
            get 
            {
                var attributeValue = this.As<InfosetPart>().Get<UserCultureSettingsPart>("Enabled");
                return !String.IsNullOrWhiteSpace(attributeValue) && Convert.ToBoolean(attributeValue);
            }
            set { this.As<InfosetPart>().Set<UserCultureSettingsPart>("Enabled", value.ToString()); }
        }

        public int Priority
        {
            get
            {
                var attributeValue = this.As<InfosetPart>().Get<UserCultureSettingsPart>("Priority");
                return String.IsNullOrWhiteSpace(attributeValue) ? int.MinValue : Convert.ToInt32(attributeValue);
            }
            set { this.As<InfosetPart>().Set<UserCultureSettingsPart>("Priority", value.ToString()); }
        }
    }
}