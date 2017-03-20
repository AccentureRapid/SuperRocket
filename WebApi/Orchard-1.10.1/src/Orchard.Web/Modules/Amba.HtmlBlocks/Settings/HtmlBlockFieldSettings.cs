using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Amba.HtmlBlocks.Settings
{
    public class HtmlBlockFieldSettings
    {
        [DisplayName("Html block height in px")]        
        public int Height { get; set; }
        [DisplayName("Html block height in px")]
        public string HelpText { get; set; }
    }
}