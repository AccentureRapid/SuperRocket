using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RaisingStudio.ModuleGenerator.TemplateModels
{
    public class Property
    {
        public string PropertyName { get; set; }
        public string PropertyType { get; set; }

        public string DisplayName { get; set; }
        public bool Required { get; set; }
    }
}