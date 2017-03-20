﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace RM.QuickLogOn.OAuth.ViewModels
{
    /*
     * http://wiki.open.kaixin001.com/index.php?id=%E4%BD%BF%E7%94%A8Authorization_Code%E8%8E%B7%E5%8F%96Access_Token
     */
    [DataContract]
    public class KaixinAccessTokenJsonModel
    {
        [DataMember]
        public string access_token { get; set; }

        [DataMember]
        public string expires_in { get; set; }

        [DataMember]
        public string refresh_token { get; set; }

        [DataMember]
        public string scope { get; set; }

        [DataMember]
        public string error_code { get; set; }

        [DataMember]
        public string request { get; set; }

        [DataMember]
        public string error { get; set; }
    }
}
