using System;
using System.Web;
using System.Web.Mvc;
using Amba.HtmlBlocks.Services;
using Orchard;

namespace Amba.HtmlBlocks.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlString HtmlBlock(this HtmlHelper helper, string htmlBlockKey)
        {
            var workContext = helper.ViewContext.RequestContext.GetWorkContext();

            if (workContext == null)
                throw new ApplicationException("The WorkContext cannot be found for the request");

            var html = workContext.Resolve<IHtmlBlockService>().GetHtmlBlockByKey(htmlBlockKey);
            return helper.Raw(html);
        }
    }
}