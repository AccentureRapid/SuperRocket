using System;
using Orchard.Logging;
using Orchard.Recipes.Models;
using Orchard.Recipes.Services;
using SH.Robots.Services;

namespace SH.Robots.ImportExport {
    public class RobotsRecipeHandler : IRecipeHandler {
        private readonly IRobotsService _robotsService;

        public RobotsRecipeHandler(IRobotsService robotsService) {
            _robotsService = robotsService;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public void ExecuteRecipeStep(RecipeContext recipeContext) {
            if (!String.Equals(recipeContext.RecipeStep.Name, RobotsCustomExportStep.ExportStep, StringComparison.OrdinalIgnoreCase)) {
                return;
            }

            var stepElement = recipeContext.RecipeStep.Step;

            _robotsService.Save(stepElement.Value);

            recipeContext.Executed = true;
        }
    }
}