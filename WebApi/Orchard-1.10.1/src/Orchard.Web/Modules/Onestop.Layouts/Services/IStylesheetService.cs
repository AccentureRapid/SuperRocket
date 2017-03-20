using System.Collections.Generic;
using Onestop.Layouts.Models;
using Orchard;

namespace Onestop.Layouts.Services {
    public interface IStylesheetService : IDependency {
        IEnumerable<StylesheetDescription> GetAvailableStylesheets();
    }
}
