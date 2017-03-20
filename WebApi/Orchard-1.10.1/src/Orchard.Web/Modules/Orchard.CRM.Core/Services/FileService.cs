namespace Orchard.CRM.Core.Services
{
    using Orchard.ContentManagement.MetaData;
    using Orchard.MediaLibrary.Services;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using Orchard.CRM.Core.Settings;
    using Orchard.Localization;

    public class FileService : IFileService
    {
        private readonly IMediaLibraryService mediaService;
        private readonly IContentDefinitionManager contentDefinitionManager;
        private static readonly object folderCreationLock = new object();

        public FileService(
            IContentDefinitionManager contentDefinitionManager,
            IMediaLibraryService mediaService)
        {
            this.mediaService = mediaService;
            this.contentDefinitionManager = contentDefinitionManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public bool AddFile(string fileName, Stream stream, string contentTypeForFile, Guid guid, Dictionary<string, string> errors)
        {
            int filesCount = 0;
            return this.AddFile(fileName, stream, contentTypeForFile, guid, errors, out filesCount);
        }
        
        public bool AddFile(string fileName, Stream stream, string contentTypeForFile, Guid guid, Dictionary<string, string> errors, out int filesCount)
        {
            filesCount = 0;
            var definition = this.contentDefinitionManager.GetTypeDefinition(contentTypeForFile);
            var settings = definition.Parts.Single(x => x.PartDefinition.Name == "FileUploadPart").GetFileUploadPartSettings();
            if (stream.Length >= settings.FileSizeLimit * 1024 * 1024)
            {
                errors.Add("FileSize", T("File exceeds maximum size allowed").Text);
                return false;
            }

            var path = EnsureFolder("Uploads", guid.ToString());
            var files = this.mediaService.GetMediaFiles(path);

            filesCount = files.Count();
            if (files.Count() >= settings.FileCountLimit)
            {
                errors.Add("FileCount", T("Maximum number of files allowed has been reached").Text);
                return false;
            }

            if (files.Any(x => x.Name == fileName))
            {
                this.mediaService.DeleteFile(path, fileName);
            }

            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, (int)stream.Length);

            this.mediaService.UploadMediaFile(path, fileName, bytes);

            return true;
        }

        private string EnsureFolder(string relativePath, string name)
        {
            lock (folderCreationLock)
            {
                var folders = this.mediaService.GetMediaFolders(relativePath);
                if (!folders.Any(x => x.Name == name))
                {
                    this.mediaService.CreateFolder(relativePath, name);
                }
            }

            return Path.Combine(relativePath, name);
        }
    }
}