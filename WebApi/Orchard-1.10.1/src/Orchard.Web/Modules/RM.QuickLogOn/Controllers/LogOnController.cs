using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Orchard;
using Orchard.Mvc;
using Orchard.Mvc.Extensions;
using Orchard.UI.Notify;
using RM.QuickLogOn.Providers;
using RM.QuickLogOn.Services;
using RM.QuickLogOn.ViewModels;
using Orchard.Themes;
using Orchard.Environment.Extensions;

namespace RM.QuickLogOn.Controllers
{
    [HandleError, Themed]
    [OrchardFeature("RM.QuickLogOn.TestForm")]
    public class LogOnController : Controller
    {
        private readonly IOrchardServices _services;
        private readonly IQuickLogOnService _quickLogOnService;

        public LogOnController(IOrchardServices services, IQuickLogOnService quickLogOnService)
        {
            _services = services;
            _quickLogOnService = quickLogOnService;
        }

        [HttpGet]
        public ActionResult Dummy(string returnUrl)
        {
            return View("LogOnDummy", new LogOnDummyModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public ActionResult Dummy(LogOnDummyModel model)
        {
            if (ModelState.IsValid)
            {
                var response = _quickLogOnService.LogOn(new QuickLogOnRequest
                                                            {
                                                                UserName = model.Login,
                                                                Email = model.Login,
                                                                RememberMe = false,
                                                                ReturnUrl = model.ReturnUrl
                                                            });

                if (response.Error != null)
                {
                    _services.Notifier.Add(NotifyType.Error, response.Error);
                }

                return this.RedirectLocal(response.ReturnUrl);
            }

            return View("LogOnDummy", model);
        }
    }
}
