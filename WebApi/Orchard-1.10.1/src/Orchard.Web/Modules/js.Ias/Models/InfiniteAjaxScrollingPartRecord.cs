using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement.Records;
using Orchard.Data.Conventions;

namespace js.Ias.Models
{
    public class InfiniteAjaxScrollingPartRecord : ContentPartRecord
    {
        public virtual string Container { get; set; }
        public virtual bool UseHistory { get; set; }
        public virtual string Item { get; set; }
        public virtual string Pagination { get; set; }
        public virtual string NextAnchor { get; set; }
        public virtual string Loader { get; set; }
        [StringLengthMax]
        public virtual string OnPageChange { get; set; }
        [StringLengthMax]
        public virtual string BeforePageChange { get; set; }
        [StringLengthMax]
        public virtual string OnLoadItems { get; set; }
        [StringLengthMax]
        public virtual string OnRenderComplete { get; set; }
    }
}