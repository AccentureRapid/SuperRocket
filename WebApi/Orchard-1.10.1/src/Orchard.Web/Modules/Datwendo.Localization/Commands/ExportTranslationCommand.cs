using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using Orchard.Commands;
using Orchard.Environment.Extensions;
using Datwendo.Localization.Services;
using System.IO;

namespace Datwendo.Localization.Commands
{
    [OrchardFeature("Datwendo.Localization")]
    public class ExportTranslationCommand : DefaultOrchardCommandHandler
    {
        private readonly IExportTranslationService _service = null;

        [OrchardSwitch]
        public string Output { get; set; }

        public ExportTranslationCommand(IExportTranslationService service)
        {
            _service = service;
        }

        [CommandName("export translation")]
        [CommandHelp(@"export translation <culture>
    Export the existing translation files for modules, themes and etc.
    <culture> should be in format 'xx-YY' or 'all' to export all translations")]
        [OrchardSwitches("Output")]
        public void ExportTranslation(string culture)
        {
            var siteRoot = HostingEnvironment.ApplicationPhysicalPath;
            var files = _service.GatherPOFiles(siteRoot, culture);
            var zipPath = Path.GetFileName(siteRoot.TrimEnd('\\'));
            zipPath = Path.Combine(Output ?? string.Empty, string.Format("{0}.{1}.po.zip", zipPath, culture));
            _service.Zip(zipPath, siteRoot, files);
        }
    }
}
