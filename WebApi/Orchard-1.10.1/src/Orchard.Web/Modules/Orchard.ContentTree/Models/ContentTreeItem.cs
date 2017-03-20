using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;

namespace Orchard.ContentTree.Models {
    public class ContentTreeItem {
        public string Path { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }
        public ContentPart Content { get; set; }

        public List<ContentTreeItem> Children { get; set; }
    }
}