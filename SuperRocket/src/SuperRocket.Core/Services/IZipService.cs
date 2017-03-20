using System.Collections.Generic;
using SuperRocket.Core.Model;

namespace SuperRocket.Core.Services
{
    public interface IZipService
    {
        void UnZip(string fileToUnzip,string destinationPath);
    }
}
