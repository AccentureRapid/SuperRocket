﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace RM.QuickLogOn.OAuth.Models
{
    [OrchardFeature("RM.QuickLogOn.OAuth.Taobao")]
    public class TaobaoSettingsPart : ContentPart<TaobaoSettingsPartRecord>
    {
        [Required(ErrorMessage = "appid(oauth_consumer_key/client_id)是必须的")]
        public string ClientId { get { return Record.ClientId; } set { Record.ClientId = value; } }
        /// <summary>
        /// appkey(auth_consumer_secret/client_secret)
        /// </summary>
        public string ClientSecret { get; set; }
        /// <summary>
        /// 附加参数
        /// </summary>
        public string Additional { get { return Record.Additional; } set { Record.Additional = value; } }
    }
}
