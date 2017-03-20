using System.Collections.Generic;
using Onestop.Layouts.Models;
using Orchard;
using Orchard.Core.Contents.ViewModels;

namespace Onestop.Layouts.Services {
    public interface ILayoutService : IDependency {
        IEnumerable<LayoutTemplatePart> GetLayouts(ContentsOrder orderBy = ContentsOrder.Published);
        IEnumerable<dynamic> GetLayoutElementEditors();
    }
}
