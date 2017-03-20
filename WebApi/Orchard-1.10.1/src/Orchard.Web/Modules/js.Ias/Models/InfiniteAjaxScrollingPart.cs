using System;
using Orchard.ContentManagement;

namespace js.Ias.Models
{
    public class InfiniteAjaxScrollingPart : ContentPart<InfiniteAjaxScrollingPartRecord>
    {
        public string Container { get; set; }
        public bool UseHistory { get; set; }
        public string Item { get; set; }
        public string Pagination { get; set; }
        public string NextAnchor { get; set; }
        public string Loader { get; set; }
        public string OnPageChange { get; set; }
        public string BeforePageChange { get; set; }
        public string OnLoadItems { get; set; }
        public string OnRenderComplete { get; set; }
    }
}