using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RM.QuickLogOn.OAuth.ViewModels
{
    [DataContract]
    public class LiveIDEmailAddressJsonViewModel
    {
        [DataContract]
        public class EmailAddresses
        {
            [DataMember]
            public string preferred { get; set; }

            [DataMember]
            public string account { get; set; }

            [DataMember]
            public string personal { get; set; }

            [DataMember]
            public string business { get; set; }
        }

        [DataMember]
        public EmailAddresses emails { get; set; }
    }
}
