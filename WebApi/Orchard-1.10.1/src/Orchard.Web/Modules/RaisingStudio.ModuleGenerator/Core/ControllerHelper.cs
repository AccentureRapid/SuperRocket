using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace RaisingStudio.ModuleGenerator.Core
{
    public static class ControllerHelper
    {
        public static bool IsIE(this Controller controller)
        {
            Regex regex = new Regex("MSIE [6-9]");
            return regex.IsMatch(controller.Request.UserAgent);
        }

        public static bool AcceptText(this Controller controller)
        {
            bool acceptText = true;
            var acceptTypes = controller.Request.AcceptTypes;
            if (acceptTypes != null)
            {
                acceptText = acceptTypes.Contains("text/html") && !acceptTypes.Contains("application/json");
            }
            if(!acceptText)
            {
                string contentType = controller.Request.ContentType;
                if (!string.IsNullOrEmpty(contentType))
                {
                    if(contentType.Contains("multipart/form-data"))
                    {
                        acceptText = true;
                    }
                }
            }
            return acceptText;
        }
    }
}