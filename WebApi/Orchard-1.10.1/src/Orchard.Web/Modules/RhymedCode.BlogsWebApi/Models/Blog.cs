using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RhymedCode.BlogsWebApi.Models
{
    public class Blog
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public int PostCount { get; set; }
        public int Id { get; internal set; }
    }

    public class BlogPost
    {
        public string CreatedBy { get; internal set; }
        public int Id { get; set; }
        public DateTime? PublishedUtc { get; set; }

        public string Text { get; set; }

        public string Title { get; set; }
    }
}