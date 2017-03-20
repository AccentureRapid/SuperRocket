using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Environment.Configuration;
using Orchard.Environment.ShellBuilders.Models;
using Orchard.FileSystems.AppData;
using Orchard.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RaisingStudio.Contents.RepositoryFactory.Services
{
    public class ContentsRepositoryFactory : IContentsRepositoryFactory
    {
        private readonly ShellSettings _shellSettings;
        private readonly ShellBlueprint _shellBlueprint;
        private readonly IAppDataFolder _appDataFolder;
        private readonly ISessionLocator _sessionLocator;
        private readonly IOrchardServices _orchardServices;
        private readonly IContentManager _contentManager;

        public ILogger Logger { get; set; }

        public ContentsRepositoryFactory(
            ShellSettings shellSettings,
            ShellBlueprint shellBlueprint,
            IAppDataFolder appDataFolder,
            ISessionLocator sessionLocator,
            IOrchardServices orchardServices,
            IContentManager contentManager)
        {
            _shellSettings = shellSettings;
            _shellBlueprint = shellBlueprint;
            _appDataFolder = appDataFolder;
            _sessionLocator = sessionLocator;
            _orchardServices = orchardServices;
            _contentManager = contentManager;

            Logger = NullLogger.Instance;
        }

        public IContentsRepository<T> GetRepository<T>(string contentTypeName = null) where T : class, new()
        {
            return new ContentsRepository<T>(_orchardServices, _contentManager, contentTypeName);
        }
    }
}