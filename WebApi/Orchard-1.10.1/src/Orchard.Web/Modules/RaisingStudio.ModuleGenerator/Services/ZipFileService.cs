using Ionic.Zip;
using RaisingStudio.ModuleGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RaisingStudio.ModuleGenerator.Services
{
    public class ZipFileService : IZipFileService
    {
        public void CreateFromDirectory(string pathName, string fileName)
        {
            ZipFile zipFile = new ZipFile();
            zipFile.AddDirectory(pathName);
            zipFile.Save(fileName);
        }

        public void ExtractToDirectory(string fileName, string pathName)
        {
            ZipFile zipFile = new ZipFile(fileName);
            zipFile.ExtractAll(pathName);
        }
        
        public bool IsZipFile(string fileName)
        {
            return ZipFile.IsZipFile(fileName);
        }
    }
}