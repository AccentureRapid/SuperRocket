using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Onestop.Layouts.Models;
using Orchard;
using Orchard.Environment.Descriptor.Models;
using Orchard.Environment.Extensions;
using Orchard.Environment.Extensions.Models;

namespace Onestop.Layouts.Services {
    [OrchardFeature("Onestop.Layouts")]
    public class StylesheetService : IStylesheetService {
        private readonly IExtensionManager _extensionManager;
        private readonly IWorkContextAccessor _wca;
        private readonly ShellDescriptor _shellDescriptor;

        public StylesheetService(
            IExtensionManager extensionManager,
            IWorkContextAccessor wca,
            ShellDescriptor shellDescriptor) {

            _extensionManager = extensionManager;
            _wca = wca;
            _shellDescriptor = shellDescriptor;
        }

        public IEnumerable<StylesheetDescription> GetAvailableStylesheets() {
            var themes = _extensionManager
                .AvailableExtensions()
                .Where(ex => DefaultExtensionTypes.IsTheme(ex.ExtensionType) && _shellDescriptor.Features.Any(sf => sf.Name == ex.Id));
            var server = _wca.GetContext().HttpContext.Server;
            return themes
                .SelectMany(
                    t => {
                        var themeStylePath = t.Location + "/" + t.Id.Replace(" ", "") + "/Styles/";
                        var files = Directory.EnumerateFiles(server.MapPath(themeStylePath), "*.css");
                        return files
                            .Select(
                                p => new StylesheetDescription(
                                    name: Path.GetFileNameWithoutExtension(p),
                                    virtualPath: VirtualPathUtility.Combine(themeStylePath, Path.GetFileName(p)),
                                    physicalPath: p,
                                    themeName: t.Name));
                    });
        }
    }
}
