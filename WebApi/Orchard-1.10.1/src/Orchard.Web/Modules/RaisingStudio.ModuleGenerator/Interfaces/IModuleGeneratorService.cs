using Orchard;
using RaisingStudio.ModuleGenerator.TemplateModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RaisingStudio.ModuleGenerator.Interfaces
{
    public interface IModuleGeneratorService : IDependency
    {
        bool GenerateModule(string templateName, Module module);
    }
}
