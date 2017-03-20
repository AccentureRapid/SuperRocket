using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Orchard;
using Orchard.Environment.Extensions;
using Ionic.Zip;

namespace Datwendo.Localization.Services
{
    public interface IExportTranslationService : IDependency
    {
        List<string> GatherPOFiles(string siteRoot, string culture);
        List<string> GatherModulePOFiles(string moduleRoot, string culture);
        void Zip(string zipPath, string rootPath, List<string> files);
    }

    [OrchardFeature("Datwendo.Localization")]
    public class ExportTranslationService : IExportTranslationService
    {
        private static readonly string[] Modules = new string[] { "Core", "Modules\\*", "Themes\\*" };

        public List<string> GatherPOFiles(string siteRoot, string culture)
        {
            var files = new List<string>();
            foreach (var modulePath in Modules)
            {
                foreach (var directory in Directory.GetDirectories(siteRoot, modulePath, SearchOption.TopDirectoryOnly))
                {
                    files.AddRange(GatherModulePOFiles(directory, culture));
                }
            }
            return files;
        }

        public List<string> GatherModulePOFiles(string moduleRoot, string culture)
        {
            var isAll = string.Compare("all", culture, StringComparison.InvariantCultureIgnoreCase) == 0;
            var path2Look = Path.Combine(moduleRoot, isAll ? "App_Data\\Localization" : string.Format("App_Data\\Localization\\{0}", culture));
            if (!Directory.Exists(path2Look)) return new List<string>();
            var directories = isAll ?   Directory.EnumerateDirectories(path2Look, "*", SearchOption.TopDirectoryOnly) : 
                                        Directory.EnumerateDirectories(Path.Combine(path2Look, ".."), culture, SearchOption.TopDirectoryOnly);
            return directories.SelectMany(x => Directory.EnumerateFiles(x, "*.po", SearchOption.TopDirectoryOnly)).Select(Path.GetFullPath).ToList();
        }

        public void Zip(string zipPath, string rootPath, List<string> files)
        {
            using (var zip = new ZipFile())
            {
                foreach (var f in files)
                {
                    if (f.IndexOf(rootPath, StringComparison.InvariantCultureIgnoreCase) != 0) continue;
                    var relativePath = Path.GetDirectoryName(f.Substring(rootPath.Length));
                    zip.AddFile(f, relativePath);
                }
                zip.Save(zipPath);
            }
        }
    }
}
