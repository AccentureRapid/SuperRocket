using System.Web.Mvc;
using Amba.HtmlBlocks.Models;
using Amba.HtmlBlocks.Services;
using Orchard;
using Orchard.Logging;
using Orchard.UI.Admin;

namespace Amba.HtmlBlocks.Controllers
{
    [Admin]
    [ValidateInput(false)]
    public class AdminController : Controller
    {
        private readonly IHtmlBlockService _htmlBlockService;
        private readonly IOrchardServices _services;
     
        public ILogger Logger { get; set; }

        public AdminController(
            
            IHtmlBlockService htmlBlockService,
            IOrchardServices services
         
            )
        {
            _services = services;
            _htmlBlockService = htmlBlockService;
            
            Logger = NullLogger.Instance;
        }

        public ActionResult List()
        {
            var blockList = _htmlBlockService.GetAllBlocks();
            return View(blockList);    
        }

        public ActionResult Edit(int id = 0)
        {
            var record = _htmlBlockService.GetHtmlBlock(id);
            if (record == null)
            {
                record = new HtmlBlockRecord();
            }
            return View(record);
        }

        [HttpPost]
        public ActionResult Edit(HtmlBlockRecord record)
        {
            record.BlockKey = record.BlockKey.Trim();
            if (_htmlBlockService.BlockExists(record.BlockKey) && record.Id <= 0)
            {
                ModelState.AddModelError("BlockKey", "Html block with this key already exists");

                return View(record);
            }
            _htmlBlockService.SaveHtmlBlock(record);
            return RedirectToAction("List");
        }

        public ActionResult Delete(int id = 0)
        {
            _htmlBlockService.Delete(id);
            return RedirectToAction("List");
        }
    }
}