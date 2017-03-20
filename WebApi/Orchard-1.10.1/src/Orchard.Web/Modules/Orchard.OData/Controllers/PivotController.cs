
namespace Orchard.OData.Controllers
{
    using Orchard.Themes;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;

    [Themed]
    public class PivotController : Controller
    {
        // GET: Pivot
        public ViewResult Index(string contentType)
        {
            ViewBag.ContentType = contentType;
            return View();
        }

        public FileResult Resource(string fileName)
        {
            string originalPath = HttpContext.Request.Path.ToLower();
            var path = originalPath.Replace("/odata/images", "/modules/orchard.odata/images");
            if (0 < originalPath.IndexOf("/$pivot/")) {
                path = originalPath.Replace("/odata/$pivot/images", "/modules/orchard.odata/images");
            }
            var mimeType = MimeTypeMap.GetMimeType(Path.GetExtension(fileName));
            return base.File(path, mimeType);
        }
    }
}