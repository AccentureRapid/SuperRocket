using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage.InfosetStorage;
using Orchard.Environment.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Datwendo.Localization.Models
{
    public enum CultureFallbackMode 
    {
        [Display (Name= "Fallback to Site Culture")]
        FallbackToSite = 0,
        [Display(Name = "Fallback to First Existing Translation")]
        FallbackToFirstExisting,
        [Display(Name = "Show Existing Translations to select")]
        ShowExistingTranslations,
        [Display(Name = "Use Regex to find fallback")]
        UseRegex
    };

    public enum MenuFilterMode
    {
        [Display(Name = "Non localized Menus are Site Culture")]
        NonLocalizedAreSite = 0,
        [Display(Name = "Non localized Menus are all cultures")]
        NonLocalizedAreAll
    };

    [OrchardFeature("Datwendo.Localization")]
    public class HomePageSettings {
        public bool Enabled { get; set; }
        public bool AllPages { get; set; }
        public CultureFallbackMode FallBackMode { get; set; }
        public MenuFilterMode MenuMode { get; set; }
        public string FallBackRegex { get; set; }
    }

    public class HomePageSettingsPart : ContentPart
    {
        public const string CacheKey = "HomePageSettingsPart";

        public bool Enabled 
        {
            get 
            {
                var attributeValue = this.As<InfosetPart>().Get<HomePageSettingsPart>("Enabled");
                return !String.IsNullOrWhiteSpace(attributeValue) && Convert.ToBoolean(attributeValue);
            }
            set 
            { 
                this.As<InfosetPart>().Set<HomePageSettingsPart>("Enabled", value.ToString()); 
            }
        }

        public bool AllPages
        {
            get
            {
                var attributeValue = this.As<InfosetPart>().Get<HomePageSettingsPart>("AllPages");
                return !String.IsNullOrWhiteSpace(attributeValue) && Convert.ToBoolean(attributeValue);
            }
            set
            {
                this.As<InfosetPart>().Set<HomePageSettingsPart>("AllPages", value.ToString());
            }
        }
        
        public CultureFallbackMode FallBackMode
        {
            get
            {
                var attributeValue = this.As<InfosetPart>().Get<HomePageSettingsPart>("FallBackMode");
                int md = 0;
                return !String.IsNullOrWhiteSpace(attributeValue) && int.TryParse(attributeValue,out md) ? (CultureFallbackMode)md : CultureFallbackMode.FallbackToSite ;
            }
            set 
            {
                this.As<InfosetPart>().Set<HomePageSettingsPart>("FallBackMode", ((int)value).ToString()); 
            }
        }

        public MenuFilterMode MenuMode
        {
            get
            {
                var attributeValue = this.As<InfosetPart>().Get<HomePageSettingsPart>("MenuMode");
                int md = 0;
                return !String.IsNullOrWhiteSpace(attributeValue) && int.TryParse(attributeValue, out md) ? (MenuFilterMode)md : MenuFilterMode.NonLocalizedAreSite;
            }
            set
            {
                this.As<InfosetPart>().Set<HomePageSettingsPart>("MenuMode", ((int)value).ToString());
            }
        }

        public string FallBackRegex
        {
            get
            {
                return this.As<InfosetPart>().Get<HomePageSettingsPart>("FallBackRegex");
            }
            set
            {
                this.As<InfosetPart>().Set<HomePageSettingsPart>("FallBackRegex", value);
            }
        }
    }
}