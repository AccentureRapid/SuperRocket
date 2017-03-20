using System.Collections.Generic;
using Orchard.Events;

namespace SH.Robots.ImportExport {
    public interface ICustomExportStep : IEventHandler {
        void Register(IList<string> steps);
    }

    public class RobotsCustomExportStep : ICustomExportStep {
        public const string ExportStep = "Robots";

        public void Register(IList<string> steps) {
            steps.Add(ExportStep);
        }
    }
}