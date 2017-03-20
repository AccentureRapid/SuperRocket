using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dolph.JsonField.Settings
{
    public class JsonFieldSettings
    {
        public string Hint { get; set; }
        public string Template { get; set;}
        public bool UpdateValuesOnly { get; set; }
        public bool CanEditJsonText { get; set; }
    }
}