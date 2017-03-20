using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RaisingStudio.ModuleGenerator.Interfaces
{
    public interface IZipFileService : IDependency
    {
        void CreateFromDirectory(string pathName, string fileName);
        void ExtractToDirectory(string fileName, string pathName);
        bool IsZipFile(string fileName);
    }
}
