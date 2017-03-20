using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RM.QuickLogOn.OAuth.ViewModels
{
    public class LiveIDOAuthAuthViewModel
    {
        public string Code { get; set; }
        public string Error { get; set; }
        public string ReturnUrl { get; set; }
    }
}
