using System.IO;
using System.Web.Mvc;
using Contrib.MediaFolder.Models;
using Microsoft.Win32;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Configuration;

namespace Contrib.MediaFolder.Controllers {
    public class HomeController : Controller {
        private readonly IOrchardServices _orchardServices;
        private readonly ShellSettings _shellSettings;

        public HomeController(IOrchardServices orchardServices, ShellSettings shellSettings) {
            _orchardServices = orchardServices;
            _shellSettings = shellSettings;
        }

        public ActionResult Index(string path) {
            var mediaLocation =
                _orchardServices.WorkContext.CurrentSite.As<RemoteStorageSettingsPart>().Record.MediaLocation;
            var filePath = Path.Combine(mediaLocation,_shellSettings.Name, path);
            return new FilePathResult(filePath, GetMimeType(new FileInfo(filePath)));
        }

        static string GetMimeType(FileInfo fileInfo) {
            var mimeType = "application/unknown";
            var regKey = Registry.ClassesRoot.OpenSubKey(fileInfo.Extension.ToLower());
            if (regKey != null) {
                var contentType = regKey.GetValue("Content Type");
                if (contentType != null) mimeType = contentType.ToString();
            }
            return mimeType;
        }
    }
}