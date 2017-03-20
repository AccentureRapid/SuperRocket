using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace RM.QuickLogOn.OAuth.Models
{
    [OrchardFeature("RM.QuickLogOn.OAuth.Twitter")]
    public class TwitterSettingsPartRecord : ContentPartRecord
    {
        public virtual string ConsumerKey { get; set; }
        public virtual string EncryptedConsumerSecret { get; set; }
        public virtual string AccessToken { get; set; }
    }
}
