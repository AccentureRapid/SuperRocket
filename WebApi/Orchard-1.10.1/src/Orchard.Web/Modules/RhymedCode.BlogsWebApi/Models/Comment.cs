using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RhymedCode.BlogsWebApi.Models
{
    public class Comment
    {
        public string Author { get; set; }
        public DateTime? CommentDateUtc { get; set; }
        public string CommentText { get; set; }
        public int Id { get; internal set; }
    }
}