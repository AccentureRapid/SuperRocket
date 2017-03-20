using Orchard.DoTNetX.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.DoTNetX.ViewModels
{
    public class BlogPostViewModel
    {
        public IEnumerable<BlogPostModel> Entities { get; set; }

        public BlogPostSearch Search { get; set; }
        public BlogPostSort Sort { get; set; }

        public dynamic Pager { get; set; }
        public dynamic Table { get; set; }
        public dynamic SearchDialog { get; set; }
    }
}