using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.ContentTree.Models {
    public enum ContentTreeAction {
        Delete,
        Unpublish,
        Publish,
        PublishDraft
    }
}