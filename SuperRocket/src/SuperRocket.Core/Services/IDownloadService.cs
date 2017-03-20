using System.Collections.Generic;
using SuperRocket.Core.Model;

namespace SuperRocket.Core.Services
{
    public interface IDownloadService
    {
        void Download(string sourcePath,string destinationPath);
    }
}
