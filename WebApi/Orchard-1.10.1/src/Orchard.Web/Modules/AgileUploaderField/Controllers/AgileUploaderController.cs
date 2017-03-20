using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Orchard.Media.Services;

namespace AgileUploaderField.Controllers
{
    public class AgileUploaderController : Controller
    {
        private readonly IMediaService _mediaService;

        public AgileUploaderController(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        [HttpPost]
        public string Upload(string agileUploaderMediaFolder)
        {
            var postedFiles = Request.Files;
            List<string> results = null;
            if (postedFiles.Count > 0)
            {
                results = new List<string>(postedFiles.Count);
                foreach (string keyPostedFile in postedFiles)
                {
                    var postedFile = postedFiles[keyPostedFile];
                    if (_mediaService.FileAllowed(postedFile))
                    {
                        var fileName = Path.GetFileName(postedFile.FileName);
                        var uniqueFileName = fileName;
                        try
                        {
                            // try to create the folder before uploading a file into it
                            _mediaService.CreateFolder(null, agileUploaderMediaFolder);
                        }
                        catch
                        {
                            // the folder can't be created because it already exists, continue
                        }
                        var filesInFolder = _mediaService.GetMediaFiles(agileUploaderMediaFolder).ToList();
                        var found =
                            filesInFolder.Any(
                                f => 0 == String.Compare(fileName, f.Name, StringComparison.OrdinalIgnoreCase));
                        var index = 0;
                        while (found)
                        {
                            index++;
                            uniqueFileName = String.Format("{0}-{1}{2}", Path.GetFileNameWithoutExtension(fileName),
                                                           index,
                                                           Path.GetExtension(fileName));
                            found =
                                filesInFolder.Any(
                                    f => 0 == String.Compare(uniqueFileName, f.Name, StringComparison.OrdinalIgnoreCase));
                        }
                        results.Add(_mediaService.UploadMediaFile(agileUploaderMediaFolder, uniqueFileName,
                                                                  postedFile.InputStream, false));
                    }
                }
            }
            return results != null ? string.Join(";", results) : string.Empty;
            
        }
    }
}
