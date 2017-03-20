using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Orchard;
using Orchard.Environment.Extensions;

namespace RM.QuickLogOn.Providers
{
    [OrchardFeature("RM.QuickLogOn.TestForm")]
    public class DummyLogOnProvider : IQuickLogOnProvider
    {
        public string Name
        {
            get { return "Test LogOn"; }
        }

        public string Description
        {
            get { return "DON'T USE IT ON REAL SITES!!!!"; }
        }

        public string GetLogOnUrl(WorkContext context)
        {
            var urlHelper = new UrlHelper(context.HttpContext.Request.RequestContext);
            return urlHelper.Action("Dummy", "LogOn", new { Area = "RM.QuickLogOn", ReturnUrl = context.HttpContext.Request.Url.ToString() });
        }
    }
}
