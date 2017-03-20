using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RM.QuickLogOn.OAuth.Models
{
    public class AccessTokenModel
    {
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
        public string UserId { get; set; }
        public string ScreenName { get; set; }
        public string Error { get; set; }

        public string User 
        { 
            get { return string.Format("{0}@{1}.twitter.com", ScreenName.Replace(" ", "_"), UserId); } 
        }
    }
}
