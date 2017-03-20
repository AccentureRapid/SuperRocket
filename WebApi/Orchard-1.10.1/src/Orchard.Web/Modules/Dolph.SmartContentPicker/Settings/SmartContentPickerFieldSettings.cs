using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dolph.SmartContentPicker.Settings
{
    public class SmartContentPickerFieldSettings {
        public SmartContentPickerFieldSettings()
        {
            
        }

        public string Hint { get; set; }
        public bool Required { get; set; }
        public bool Multiple { get; set; }        
        public string DisplayedContentTypes { get; set; }
    }
}