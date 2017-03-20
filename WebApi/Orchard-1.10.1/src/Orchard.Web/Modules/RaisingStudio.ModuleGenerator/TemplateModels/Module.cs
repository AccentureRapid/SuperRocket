using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RaisingStudio.ModuleGenerator.TemplateModels
{
    public class Module
    {
        public IDictionary<string, string> Settings { get; set; }

        public string ModuleName { get; set; }
        public string Guid { get; set; }
        public IEnumerable<Entity> Entities { get; set; }
    }
}