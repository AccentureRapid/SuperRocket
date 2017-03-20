using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RM.QuickLogOn.OAuth.ViewModels
{
    [DataContract]
    public class GoogleEmailAddressJsonViewModel
    {
        [DataMember]
        public string email { get; set; }

        [DataMember]
        public bool verified_email { get; set; }
    }
}
