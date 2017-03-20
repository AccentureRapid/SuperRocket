using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RaisingStudio.ModuleGenerator.Models
{
    public class TemplateConfig
    {
        public class TemplateFile
        {
            public string Template { get; set; }
            public string Output { get; set; }
            public bool IsStatic { get; set; }

            public string Content { get; set; }
        }
        public class TemplateSetting
        {
            public string Name { get; set; }
            public string DefaultValue { get; set; }
            public string Value { get; set; }
        }
        public string Name { get; set; }
        public TemplateSetting[] Settings { get; set; }
        public string[] Folders { get; set; }
        public string[] Files { get; set; }
        public TemplateFile[] Module { get; set; }
        public TemplateFile[] Entity { get; set; }
    }
}