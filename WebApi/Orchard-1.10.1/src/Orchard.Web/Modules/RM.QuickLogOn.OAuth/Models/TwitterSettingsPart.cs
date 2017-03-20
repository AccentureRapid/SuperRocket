using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace RM.QuickLogOn.OAuth.Models
{
    [OrchardFeature("RM.QuickLogOn.OAuth.Twitter")]
    public class TwitterSettingsPart : ContentPart<TwitterSettingsPartRecord>
    {
        [Required(ErrorMessage = "Twitter ConsumerKey is required")]
        public string ConsumerKey { get { return Record.ConsumerKey; } set { Record.ConsumerKey = value; } }

        [Required(ErrorMessage = "Twitter AccessToken is required")]
        public string AccessToken { get { return Record.AccessToken; } set { Record.AccessToken = value; } }

        public string ConsumerSecret { get; set; }
    }
}
