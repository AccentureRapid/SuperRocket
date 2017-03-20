using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Laughlin.ErrorLog.Models
{
    public class LogItem
    {
        public string Text { get; set; }
        public string Date { get; set; }
        public string Preview {
            get { return Text.Substring(0, 75) + "..."; }
        }
    }
}