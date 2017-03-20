using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.IO;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amba.System.Extensions;
using Microsoft.SqlServer.Server;
using Orchard.Environment.Extensions;
using Orchard.FileSystems.Media;
using PathUtils = System.IO.Path;
using Orchard.Media.Helpers;
using System.Web;

namespace Amba.AmazonS3.Services
{
    public interface IAmazonS3StorageProvider : IStorageProvider
    {
        //debug only
        List<S3Object> ListObjects(string prefix, Func<S3Object, bool> filterfFunc = null);
        void Test();
        Stream GetObjectStream(string path);
        void PublishFile(string path);
    }
    /*
     * var root = new S3DirectoryInfo(_client, _configurationService.AWSWalkerUploadBucket);
     */

    [OrchardSuppressDependency("Orchard.FileSystems.Media.FileSystemStorageProvider")]
    public class AmazonS3StorageProvider : IAmazonS3StorageProvider, IStorageProvider
    {
        private readonly IAmazonS3StorageConfiguration _amazonS3StorageConfiguration;
        private readonly IAmazonS3 _client = null;
        private readonly TransferUtility _transferUtility = null;
        
        public AmazonS3StorageProvider(IAmazonS3StorageConfiguration amazonS3StorageConfiguration)
        {
            _amazonS3StorageConfiguration = amazonS3StorageConfiguration;
            var cred = new BasicAWSCredentials(_amazonS3StorageConfiguration.AWSAccessKey, _amazonS3StorageConfiguration.AWSSecretKey);
            //TODO: aws region to config
            _client = new AmazonS3Client(cred, RegionEndpoint.USEast1);
            var config = new TransferUtilityConfig();
            _transferUtility = new TransferUtility(_client, config);
        }

        public void Test()
        {
            /*
            var x = "woof/bubu/x";
            var dir = new S3DirectoryInfo(_client, _amazonS3StorageConfiguration.AWSFileBucket, "a/b");
            dir.Create();
            Console.WriteLine("Name: " + dir.Name + ", FullName: " + dir.FullName);

            
            var dir2 = new S3DirectoryInfo(_client, _amazonS3StorageConfiguration.AWSFileBucket, "1\\2");
            dir2.Create();
            Console.WriteLine("Name: " + dir2.Name + ", FullName: " + dir2.FullName);
            var file = new S3FileInfo(_client, _amazonS3StorageConfiguration.AWSFileBucket, "1\\2\\t.txt");
            using (file.Create()) { }
            Console.WriteLine("Name: {0}, FullName: {1}, DirName: {2}", file.Name, file.FullName, file.DirectoryName);
            var file2 = new S3FileInfo(_client, _amazonS3StorageConfiguration.AWSFileBucket, "a/b/t2.txt");

            using (file2.Create()) { }
            Console.WriteLine("Name: {0}, FullName: {1}, DirName: {2}", file2.Name, file2.FullName, file2.DirectoryName);
            =*/
            using (var fsFileStream = File.OpenRead(@"E:\al\pics\adventure_time\x.jpg"))
            {
                var a3File = new S3FileInfo(_client, _amazonS3StorageConfiguration.AWSFileBucket, @"test\x2.jpg");
                using (a3File.Create()) { }
                
                using(var outStream = a3File.OpenWrite())
                {
                    fsFileStream.CopyTo(outStream);
                }
                PublishFile(a3File.FullName);
            }
        }

        public bool FileExists(string path)
        {
            path = CleanPath(path);
            var file = new S3FileInfo(_client, _amazonS3StorageConfiguration.AWSFileBucket, path);
            return file.Exists;
        }

        public string GetPublicUrl(string path)
        {
            path = CleanPath(path);
            string key = PathToKey(path);
            string url;

            if (MediaHelpers.IsPicture(null, path))
            {
                url = string.Format("{0}{1}/{2}", _amazonS3StorageConfiguration.AWSS3PublicUrl,
                          _amazonS3StorageConfiguration.AWSFileBucket,
                          path);

                url = HttpUtility.UrlPathEncode(url);
            }
            else
            {
                GetPreSignedUrlRequest preRequest = new GetPreSignedUrlRequest();
                preRequest.BucketName = _amazonS3StorageConfiguration.AWSFileBucket;
                preRequest.Key = key;
                preRequest.Expires = DateTime.Now.AddMinutes(_amazonS3StorageConfiguration.AWSExpireMinutes);
                preRequest.Protocol = Protocol.HTTPS;
                url = _client.GetPreSignedURL(preRequest);
            }

            return url.Replace("\\", "/");

        }

        public string GetStoragePath(string url)
        {
            Uri uri = new Uri(url);
            string localPath = PathToKey(uri.LocalPath);

            if (localPath.StartsWith(_amazonS3StorageConfiguration.AWSFileBucket, StringComparison.OrdinalIgnoreCase))
            {
                localPath = localPath.Substring(_amazonS3StorageConfiguration.AWSFileBucket.Length);
            }

            localPath = PathToKey(localPath);
            return localPath;

            //var rootPath = string.Format("{0}{1}", 
            //    _amazonS3StorageConfiguration.AWSS3PublicUrl,
            //    _amazonS3StorageConfiguration.AWSFileBucket);
            //if (string.IsNullOrWhiteSpace(url) || url.Length < rootPath.Length)
            //    return rootPath;

            //return url.Substring(rootPath.Length);
        }

        public IStorageFile GetFile(string path)
        {
            path = CleanPath(path);
            var file = new S3FileInfo(_client, _amazonS3StorageConfiguration.AWSFileBucket, path);
            return new AmazonS3StorageFile(file, this);
        }

        public static string CleanPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;
            path = path.RegexRemove(@"^[^:]+\:").Replace("/", "\\").RegexRemove(@"^[\\]+");
            return path;
        }

        public static string PathToKey(string path)
        {
            var result = path.RegexRemove(@"^[^:]+\:").Replace("\\", "/").RegexRemove(@"^[\/]+");
            return result;
        }

        public IEnumerable<IStorageFile> ListFiles(string path)
        {
            path = CleanPath(path);
            var dir = new S3DirectoryInfo(_client, _amazonS3StorageConfiguration.AWSFileBucket, path);
            return dir.GetFiles().Where(x => !x.Name.EndsWith("_$folder$")).Select(x => new AmazonS3StorageFile(x, this)).ToList();
        }

        private string CleanFolderPath(string path)
        {
            path = CleanPath(path).RegexRemove(@"[\\]$") + "\\";
            if (path == "\\")
                return "";
            return path;

        }

        private string GetFolderKey(string path)
        {
            path = CleanPath(path);
            path = path.RegexRemove("/$");
            var folderName = PathUtils.GetFileName(path);
            return path + "/" + folderName + "_$folder$";
        }

        public bool FolderExists(string path)
        {
            path = CleanPath(path);
            var dir = new S3DirectoryInfo(_client, path);
            return dir.Exists;
        }

        public IEnumerable<IStorageFolder> ListFolders(string path)
        {
            path = CleanPath(path);
            var dir = new S3DirectoryInfo(_client, _amazonS3StorageConfiguration.AWSFileBucket, path);
            var folders = dir.GetDirectories("*", SearchOption.TopDirectoryOnly).Select(x => new AmazonS3StorageFolder(x)).ToList();
            return folders;
        }

        public bool TryCreateFolder(string path)
        {
            try
            {
                path = CleanPath(path);
                var dir = new S3DirectoryInfo(_client, _amazonS3StorageConfiguration.AWSFileBucket, path);
                dir.Create();
            }
            catch 
            {
                return false;
            }
            return true;
        }

        public void CreateFolder(string path)
        {
            path = CleanPath(path);
            var dir = new S3DirectoryInfo(_client, _amazonS3StorageConfiguration.AWSFileBucket, path);
            dir.Create();
        }

        public void DeleteFolder(string path)
        {
            path = CleanPath(path);
            var dir = new S3DirectoryInfo(_client, _amazonS3StorageConfiguration.AWSFileBucket, path);
            dir.Delete(true);
        }

        public void RenameFolder(string oldPath, string newPath)
        {
            oldPath = CleanPath(oldPath);
            newPath = CleanPath(newPath);
            var oldDir = new S3DirectoryInfo(_client, _amazonS3StorageConfiguration.AWSFileBucket, oldPath);
            var newDir = new S3DirectoryInfo(_client, _amazonS3StorageConfiguration.AWSFileBucket, newPath);
            oldDir.MoveTo(newDir);

        }

        public void DeleteFile(string path)
        {
            path = CleanPath(path);
            var file = new S3FileInfo(_client, _amazonS3StorageConfiguration.AWSFileBucket, path);
            file.Delete();
            
        }

        public void RenameFile(string oldPath, string newPath)
        {
            oldPath = CleanPath(oldPath);
            newPath = CleanPath(newPath);
            var file = new S3FileInfo(_client, _amazonS3StorageConfiguration.AWSFileBucket, oldPath);
            var newFile = new S3FileInfo(_client, _amazonS3StorageConfiguration.AWSFileBucket, newPath);
            file.MoveTo(newFile);
            PublishFile(newPath);
            
        }

        public void CopyFile(string originalPath, string duplicatePath)
        {
            originalPath = CleanPath(originalPath);
            duplicatePath = CleanPath(duplicatePath);
            var file = new S3FileInfo(_client, _amazonS3StorageConfiguration.AWSFileBucket, originalPath);
            var newFile = new S3FileInfo(_client, _amazonS3StorageConfiguration.AWSFileBucket, duplicatePath);
            file.CopyTo(newFile);
            PublishFile(duplicatePath);
        }

        public void PublishFile(string path)
        {
            var key = PathToKey(path);
            Console.WriteLine("Publish key:" + key);
            _client.PutACL(new PutACLRequest
            {
                BucketName = _amazonS3StorageConfiguration.AWSFileBucket,
                Key = key,
                CannedACL = MediaHelpers.IsPicture(null, path) ?
                    S3CannedACL.PublicRead : S3CannedACL.AuthenticatedRead
            });
        }

        
        public IStorageFile CreateFile(string path)
        {
            path = CleanPath(path);
            var file = new S3FileInfo(_client, _amazonS3StorageConfiguration.AWSFileBucket, path);
            using (file.Create()) { }
            PublishFile(path);
            return new AmazonS3StorageFile(file, this);
        }

        

        public bool TrySaveStream(string path, Stream inputStream)
        {
            try
            {
                path = CleanPath(path);
                SaveStream(path, inputStream);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void SaveStream(string path, Stream inputStream)
        {
            path = CleanPath(path);
            var file = new S3FileInfo(_client, _amazonS3StorageConfiguration.AWSFileBucket, path);
            var isNew = !file.Exists;
            using (var stream = file.Exists ? file.OpenWrite() : file.Create())
            {
                inputStream.CopyTo(stream);
            }
            if (isNew)
            {
                PublishFile(path);
            }
        }


        public string Combine(string path1, string path2)
        {
            return CleanPath(Path.Combine(path1, path2));
        }

        public List<S3Object> ListObjects(string prefix, Func<S3Object, bool> filterfFunc = null)
        {
            try
            {
                if (filterfFunc == null)
                {
                    filterfFunc = (obj) => { return true; };
                }
                prefix = CleanPath(prefix);

                List<S3Object> result = new List<S3Object>();
                ListObjectsRequest request = new ListObjectsRequest
                {
                    BucketName = _amazonS3StorageConfiguration.AWSFileBucket,
                    Prefix = prefix,
                    MaxKeys = 1000
                };

                do
                {
                    ListObjectsResponse response = _client.ListObjects(request);
                    result.AddRange(response.S3Objects.Where(x => filterfFunc(x)));

                    if (response.IsTruncated && response.S3Objects.Any())
                    {
                        request.Marker = response.NextMarker;
                    }
                    else
                    {
                        break;
                    }
                } while (request != null);
                return result;
            }
            catch (AmazonS3Exception ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                    return new List<S3Object>();
                throw;
            }
        }

        public Stream GetObjectStream(string path)
        {
            path = CleanPath(path);
            var file = new S3FileInfo(_client, _amazonS3StorageConfiguration.AWSFileBucket, path);

            return file.OpenRead();
            //return Download(path);
        }

        private Stream Download(string key)
        {
            var stream = _transferUtility.OpenStream(new TransferUtilityOpenStreamRequest()
            {
                BucketName = _amazonS3StorageConfiguration.AWSFileBucket,
                Key = key,
            });
            return stream;
        }


        private bool Upload(Stream stream, string fileKey, bool asPublic = false, bool closeStream = false)
        {
            
            try
            {
                var request = new TransferUtilityUploadRequest()
                {
                    BucketName = _amazonS3StorageConfiguration.AWSFileBucket,
                    Key = fileKey,
                    InputStream = stream,
                    AutoCloseStream = closeStream,
                    AutoResetStreamPosition = true,
                };
                if (asPublic)
                {
                    request.CannedACL = S3CannedACL.PublicRead;
                }
                _transferUtility.Upload(request);
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}