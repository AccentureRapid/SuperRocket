﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using Contrib.MediaFolder.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Configuration;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Validation;
using Orchard.FileSystems.Media;

namespace Contrib.MediaFolder.Services {
    [OrchardSuppressDependency("Orchard.FileSystems.Media.FileSystemStorageProvider")]
    public class RemoteStorageProvider : IStorageProvider {
        private readonly ShellSettings _settings;
        private readonly IOrchardServices _orchardServices;

        public RemoteStorageProvider(ShellSettings settings, IOrchardServices orchardServices) {
            _settings = settings;
            _orchardServices = orchardServices;


            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        private string __storagePath;
        private string _storagePath {
            get {
                if (__storagePath == null) {
                    InitializePaths();
                }
                return __storagePath;
            }
        }

        private string __publicPath;
        private string _publicPath {
            get {
                if (__publicPath == null) {
                    InitializePaths();
                }
                return __publicPath;
            }
        }

        private void InitializePaths() {
            var configuration = _orchardServices.WorkContext.CurrentSite.As<RemoteStorageSettingsPart>().Record;
            var mediaPath = configuration.MediaLocation;

            __storagePath = Path.Combine(mediaPath, _settings.Name);

            var appPath = "";
            if (HostingEnvironment.IsHosted) {
                appPath = HostingEnvironment.ApplicationVirtualPath;
            }
            if (!appPath.EndsWith("/"))
                appPath = appPath + '/';
            if (!appPath.StartsWith("/"))
                appPath = '/' + appPath;

            __publicPath = configuration.EnableDirectRoute
                ? configuration.DirectRoute + _settings.Name + (configuration.DirectRoute.EndsWith("/") ? "" : "/")
                : HostingEnvironment.VirtualPathProvider.CombineVirtualPaths(appPath, "Media/") + _settings.Name + "/";
        }

        /// <summary>
        /// Maps a relative path into the storage path.
        /// </summary>
        /// <param name="path">The relative path to be mapped.</param>
        /// <returns>The relative path combined with the storage path.</returns>
        private string MapStorage(string path) {
            if (!string.IsNullOrEmpty(path))
            {
                if (!path.StartsWith("\\"))
                {
                    path = "\\" + path;
                } 
            }
            string mappedPath = string.IsNullOrEmpty(path) ? _storagePath : _storagePath + path;
            return PathValidation.ValidatePath(_storagePath, mappedPath);
        }

        /// <summary>
        /// Maps a relative path into the public path.
        /// </summary>
        /// <param name="path">The relative path to be mapped.</param>
        /// <returns>The relative path combined with the public path in an URL friendly format ('/' character for directory separator).</returns>
        private string MapPublic(string path) {
            return string.IsNullOrEmpty(path) ? _publicPath : Path.Combine(_publicPath, path).Replace(Path.DirectorySeparatorChar, '/');
        }

        private static string Fix(string path) {
            return string.IsNullOrEmpty(path)
                       ? ""
                       : Path.DirectorySeparatorChar != '/'
                             ? path.Replace('/', Path.DirectorySeparatorChar)
                             : path;
        }

        #region Implementation of IStorageProvider

        /// <summary>
        /// Retrieves the public URL for a given file within the storage provider.
        /// </summary>
        /// <param name="path">The relative path within the storage provider.</param>
        /// <returns>The public URL.</returns>
        public string GetPublicUrl(string path) {
            return MapPublic(path);
        }

        /// <summary>
        /// Retrieves a file within the storage provider.
        /// </summary>
        /// <param name="path">The relative path to the file within the storage provider.</param>
        /// <returns>The file.</returns>
        /// <exception cref="ArgumentException">If the file is not found.</exception>
        public IStorageFile GetFile(string path) {
            var fileInfo = new FileInfo(MapStorage(path));
            if (!fileInfo.Exists) {
                throw new ArgumentException(T("File {0} does not exist", path).ToString());
            }

            return new FileSystemStorageFile(Fix(path), fileInfo);
        }

        /// <summary>
        /// Lists the files within a storage provider's path.
        /// </summary>
        /// <param name="path">The relative path to the folder which files to list.</param>
        /// <returns>The list of files in the folder.</returns>
        public IEnumerable<IStorageFile> ListFiles(string path) {
            var directoryInfo = new DirectoryInfo(MapStorage(path));
            if (!directoryInfo.Exists) {
                throw new ArgumentException(T("Directory {0} does not exist", path).ToString());
            }

            return directoryInfo
                .GetFiles()
                .Where(fi => !IsHidden(fi))
                .Select<FileInfo, IStorageFile>(fi => new FileSystemStorageFile(Path.Combine(Fix(path), fi.Name), fi))
                .ToList();
        }

        /// <summary>
        /// Lists the folders within a storage provider's path.
        /// </summary>
        /// <param name="path">The relative path to the folder which folders to list.</param>
        /// <returns>The list of folders in the folder.</returns>
        public IEnumerable<IStorageFolder> ListFolders(string path) {
            var directoryInfo = new DirectoryInfo(MapStorage(path));
            if (!directoryInfo.Exists) {
                try {
                    directoryInfo.Create();
                }
                catch (Exception ex) {
                    throw new ArgumentException(T("The folder could not be created at path: {0}. {1}", path, ex).ToString());
                }
            }

            return directoryInfo
                .GetDirectories()
                .Where(di => !IsHidden(di))
                .Select<DirectoryInfo, IStorageFolder>(di => new FileSystemStorageFolder(Path.Combine(Fix(path), di.Name), di))
                .ToList();
        }

        /// <summary>
        /// Tries to create a folder in the storage provider.
        /// </summary>
        /// <param name="path">The relative path to the folder to be created.</param>
        /// <returns>True if success; False otherwise.</returns>
        public bool TryCreateFolder(string path) {
            try {
                // prevent unnecessary exception
                var directoryInfo = new DirectoryInfo(MapStorage(path));
                if (directoryInfo.Exists) {
                    return false;
                }

                CreateFolder(path);
            }
            catch {
                return false; 
            }

            return true;
        }

        /// <summary>
        /// Creates a folder in the storage provider.
        /// </summary>
        /// <param name="path">The relative path to the folder to be created.</param>
        /// <exception cref="ArgumentException">If the folder already exists.</exception>
        public void CreateFolder(string path) {
            var directoryInfo = new DirectoryInfo(MapStorage(path));
            if (directoryInfo.Exists) {
                throw new ArgumentException(T("Directory {0} already exists", path).ToString());
            }

            Directory.CreateDirectory(directoryInfo.FullName);
        }

        /// <summary>
        /// Deletes a folder in the storage provider.
        /// </summary>
        /// <param name="path">The relative path to the folder to be deleted.</param>
        /// <exception cref="ArgumentException">If the folder doesn't exist.</exception>
        public void DeleteFolder(string path) {
            var directoryInfo = new DirectoryInfo(MapStorage(path));
            if (!directoryInfo.Exists) {
                throw new ArgumentException(T("Directory {0} does not exist", path).ToString());
            }

            directoryInfo.Delete(true);
        }

        /// <summary>
        /// Renames a folder in the storage provider.
        /// </summary>
        /// <param name="oldPath">The relative path to the folder to be renamed.</param>
        /// <param name="newPath">The relative path to the new folder.</param>
        public void RenameFolder(string oldPath, string newPath) {
            var sourceDirectory = new DirectoryInfo(MapStorage(oldPath));
            if (!sourceDirectory.Exists) {
                throw new ArgumentException(T("Directory {0} does not exist", oldPath).ToString());
            }

            var targetDirectory = new DirectoryInfo(MapStorage(newPath));
            if (targetDirectory.Exists) {
                throw new ArgumentException(T("Directory {0} already exists", newPath).ToString());
            }

            Directory.Move(sourceDirectory.FullName, targetDirectory.FullName);
        }

        /// <summary>
        /// Deletes a file in the storage provider.
        /// </summary>
        /// <param name="path">The relative path to the file to be deleted.</param>
        /// <exception cref="ArgumentException">If the file doesn't exist.</exception>
        public void DeleteFile(string path) {
            var fileInfo = new FileInfo(MapStorage(path));
            if (!fileInfo.Exists) {
                throw new ArgumentException(T("File {0} does not exist", path).ToString());
            }

            fileInfo.Delete();
        }

        /// <summary>
        /// Renames a file in the storage provider.
        /// </summary>
        /// <param name="oldPath">The relative path to the file to be renamed.</param>
        /// <param name="newPath">The relative path to the new file.</param>
        public void RenameFile(string oldPath, string newPath) {
            var sourceFileInfo = new FileInfo(MapStorage(oldPath));
            if (!sourceFileInfo.Exists) {
                throw new ArgumentException(T("File {0} does not exist", oldPath).ToString());
            }

            var targetFileInfo = new FileInfo(MapStorage(newPath));
            if (targetFileInfo.Exists) {
                throw new ArgumentException(T("File {0} already exists", newPath).ToString());
            }

            File.Move(sourceFileInfo.FullName, targetFileInfo.FullName);
        }

        /// <summary>
        /// Creates a file in the storage provider.
        /// </summary>
        /// <param name="path">The relative path to the file to be created.</param>
        /// <exception cref="ArgumentException">If the file already exists.</exception>
        /// <returns>The created file.</returns>
        public IStorageFile CreateFile(string path) {
            var fileInfo = new FileInfo(MapStorage(path));
            if (fileInfo.Exists) {
                throw new ArgumentException(T("File {0} already exists", fileInfo.Name).ToString());
            }

            // ensure the directory exists
            var dirName = Path.GetDirectoryName(fileInfo.FullName);
            if (!Directory.Exists(dirName)) {
                Directory.CreateDirectory(dirName);
            }
            File.WriteAllBytes(fileInfo.FullName, new byte[0]);

            return new FileSystemStorageFile(Fix(path), fileInfo);
        }

        /// <summary>
        /// Tries to save a stream in the storage provider.
        /// </summary>
        /// <param name="path">The relative path to the file to be created.</param>
        /// <param name="inputStream">The stream to be saved.</param>
        /// <returns>True if success; False otherwise.</returns>
        public bool TrySaveStream(string path, Stream inputStream) {
            try {
                SaveStream(path, inputStream);
            }
            catch {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Saves a stream in the storage provider.
        /// </summary>
        /// <param name="path">The relative path to the file to be created.</param>
        /// <param name="inputStream">The stream to be saved.</param>
        /// <exception cref="ArgumentException">If the stream can't be saved due to access permissions.</exception>
        public void SaveStream(string path, Stream inputStream) {
            // Create the file.
            // The CreateFile method will map the still relative path
            var file = CreateFile(path);

            var outputStream = file.OpenWrite();
            var buffer = new byte[8192];
            for (;;) {

                var length = inputStream.Read(buffer, 0, buffer.Length);
                if (length <= 0)
                    break;
                outputStream.Write(buffer, 0, length);
            }
            outputStream.Dispose();
        }

        /// <summary>
        /// Combines to paths.
        /// </summary>
        /// <param name="path1">The parent path.</param>
        /// <param name="path2">The child path.</param>
        /// <returns>The combined path.</returns>
        public string Combine(string path1, string path2) {
            return Path.Combine(path1, path2);
        }

        private static bool IsHidden(FileSystemInfo di) {
            return (di.Attributes & FileAttributes.Hidden) != 0;
        }

        public bool FileExists(string path)
        {
            var abPath = MapStorage(path);
            return File.Exists(abPath);
        }

        public string GetStoragePath(string url)
        {
            if (url.StartsWith(_publicPath))
            {
                return url.Substring(_publicPath.Length).Replace('/', Path.DirectorySeparatorChar).Replace("%20", " ");
            }

            return null;
        }

        public bool FolderExists(string path)
        {
            return new DirectoryInfo(MapStorage(path)).Exists;
        }

        public void CopyFile(string originalPath, string duplicatePath)
        {
            FileInfo sourceFileInfo = new FileInfo(MapStorage(originalPath));
            if (!sourceFileInfo.Exists)
            {
                throw new ArgumentException(T("File {0} does not exist", originalPath).ToString());
            }

            FileInfo targetFileInfo = new FileInfo(MapStorage(duplicatePath));
            if (targetFileInfo.Exists)
            {
                throw new ArgumentException(T("File {0} already exists", duplicatePath).ToString());
            }

            File.Copy(sourceFileInfo.FullName, targetFileInfo.FullName);
        }

        #endregion

        private class FileSystemStorageFile : IStorageFile {
            private readonly string _path;
            private readonly FileInfo _fileInfo;

            public FileSystemStorageFile(string path, FileInfo fileInfo) {
                _path = path;
                _fileInfo = fileInfo;
            }

            #region Implementation of IStorageFile

            public string GetPath() {
                return _path;
            }

            public string GetName() {
                return _fileInfo.Name;
            }

            public long GetSize() {
                return _fileInfo.Length;
            }

            public DateTime GetLastUpdated() {
                return _fileInfo.LastWriteTime;
            }

            public string GetFileType() {
                return _fileInfo.Extension;
            }

            public Stream OpenRead() {
                return new FileStream(_fileInfo.FullName, FileMode.Open, FileAccess.Read);
            }

            public Stream OpenWrite() {
                return new FileStream(_fileInfo.FullName, FileMode.Open, FileAccess.ReadWrite);
            }

            public Stream CreateFile() {
                return new FileStream(_fileInfo.FullName, FileMode.Truncate, FileAccess.ReadWrite);
            }

            #endregion
        }

        private class FileSystemStorageFolder : IStorageFolder {
            private readonly string _path;
            private readonly DirectoryInfo _directoryInfo;

            public FileSystemStorageFolder(string path, DirectoryInfo directoryInfo) {
                _path = path;
                _directoryInfo = directoryInfo;

                T = NullLocalizer.Instance;
            }

            public Localizer T { get; set; }

            #region Implementation of IStorageFolder

            public string GetPath() {
                return _path;
            }

            public string GetName() {
                return _directoryInfo.Name;
            }

            public DateTime GetLastUpdated() {
                return _directoryInfo.LastWriteTime;
            }

            public long GetSize() {
                return GetDirectorySize(_directoryInfo);
            }

            public IStorageFolder GetParent() {
                if (_directoryInfo.Parent != null) {
                    return new FileSystemStorageFolder(Path.GetDirectoryName(_path), _directoryInfo.Parent);
                }
                throw new ArgumentException(T("Directory {0} does not have a parent directory", _directoryInfo.Name).ToString());
            }

            #endregion

            private static long GetDirectorySize(DirectoryInfo directoryInfo) {
                long size = 0;

                FileInfo[] fileInfos = directoryInfo.GetFiles();
                foreach (FileInfo fileInfo in fileInfos) {
                    if (!IsHidden(fileInfo)) {
                        size += fileInfo.Length;
                    }
                }
                DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
                foreach (DirectoryInfo dInfo in directoryInfos) {
                    if (!IsHidden(dInfo)) {
                        size += GetDirectorySize(dInfo);
                    }
                }

                return size;
            }
        }
    }
}
