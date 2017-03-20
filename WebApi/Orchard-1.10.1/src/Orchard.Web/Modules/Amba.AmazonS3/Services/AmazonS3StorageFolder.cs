using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Amazon.S3.IO;
using Amba.System.Extensions;
using Orchard.FileSystems.Media;
using PathUtils = System.IO.Path;

namespace Amba.AmazonS3.Services
{
    public class AmazonS3StorageFolder : IStorageFolder
    {
        private readonly S3DirectoryInfo _s3DirectoryInfo;

        public AmazonS3StorageFolder(S3DirectoryInfo s3DirectoryInfo)
        {
            _s3DirectoryInfo = s3DirectoryInfo;
        }

        public string GetPath()
        {
            var path = _s3DirectoryInfo.FullName.RegexRemove(@"^[^:]*:");
            var temp = CleanPath(path);
            return temp;
        }

        public string GetName()
        {
            return _s3DirectoryInfo.Name;
        }

        public static string CleanPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;
            path = path.RegexRemove(@"^[^:]+\:").Replace("/", "\\").RegexRemove(@"^[\\]+");
            return path;
        }
        public long GetSize()
        {
            return 1;
        }

        public DateTime GetLastUpdated()
        {
            return DateTime.MinValue;
        }

        public IStorageFolder GetParent()
        {
            return new AmazonS3StorageFolder(_s3DirectoryInfo.Parent);
        }
    }
}