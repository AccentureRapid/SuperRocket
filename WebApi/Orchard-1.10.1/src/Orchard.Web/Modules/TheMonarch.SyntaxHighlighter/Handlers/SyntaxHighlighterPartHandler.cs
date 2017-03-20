using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using JetBrains.Annotations;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Blogs.Models;
using TheMonarch.SyntaxHighlighter.Models;

namespace TheMonarch.SyntaxHighlighter.Handlers {
    //[UsedImplicitly]
    public class SyntaxHighlighterPartHandler : ContentHandler {
        public SyntaxHighlighterPartHandler() {
            Filters.Add(new ActivatingFilter<SyntaxHighlighterPart>("BlogPost"));
        }
    }
}
