﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RM.QuickLogOn.OAuth.Models
{
    public class RequestTokenModel
    {
        public string Token { get; set; }
        public string TokenSecret { get; set; }
        public string Error { get; set; }
    }
}
