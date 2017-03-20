using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RaisingStudio.ModuleGenerator.ViewModels
{
    public class GeneratorViewModel
    {
        public string ModuleName { get; set; }
        public string TemplateName { get; set; }
        public IEnumerable<ContentType> ContentTypes { get; set; }
        public IEnumerable<string> ReferenceTemplates { get; set; }
        public string ModuleTemplatesPathName { get; set; }
    }
}