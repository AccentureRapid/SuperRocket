using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RaisingStudio.ModuleGenerator.TemplateModels
{
    public class Entity
    {
        public string TypeName { get; set; }
        public IEnumerable<Property> Properties { get; set; }

        public string DisplayName { get; set; }
        public Module Module { get; set; }

        public string ParameterName { get; set; }
        public string IndentityName { get; set; }
    }
}