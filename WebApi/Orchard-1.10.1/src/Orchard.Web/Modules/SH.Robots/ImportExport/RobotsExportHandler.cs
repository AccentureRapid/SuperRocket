using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Orchard.Events;
using SH.Robots.Services;

namespace SH.Robots.ImportExport {
    public interface IExportEventHandler : IEventHandler {
        void Exporting(dynamic context);
        void Exported(dynamic context);
    }

    public class RobotsExportHandler : IExportEventHandler {
        private readonly IRobotsService _robotsService;

        public RobotsExportHandler(IRobotsService robotsService) {
            _robotsService = robotsService;
        }

        public void Exporting(dynamic context) {}

        public void Exported(dynamic context) {
            var customSteps = (IEnumerable<string>) context.ExportOptions.CustomSteps;
            if (!customSteps.Contains(RobotsCustomExportStep.ExportStep)) {
                return;
            }

            var robotsFileRecord = _robotsService.Get();

            if (robotsFileRecord == null) {
                return;
            }

            if (string.IsNullOrWhiteSpace(robotsFileRecord.FileContent)) {
                return;
            }

            var element = new XElement(RobotsCustomExportStep.ExportStep) {
                Value = robotsFileRecord.FileContent
            };

            context.Document.Element("Orchard").Add(element);
        }
    }
}