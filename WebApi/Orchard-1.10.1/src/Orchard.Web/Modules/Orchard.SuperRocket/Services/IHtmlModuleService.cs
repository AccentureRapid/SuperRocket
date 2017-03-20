using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using System.Collections;

namespace Orchard.SuperRocket.Services
{
    public interface IHtmlModuleService : IDependency
    {
        dynamic GetAvailableHtmlModules();
    }
}