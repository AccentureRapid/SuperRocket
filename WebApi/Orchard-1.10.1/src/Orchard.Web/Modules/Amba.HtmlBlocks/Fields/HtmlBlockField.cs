using Orchard.ContentManagement;
using System.Collections.Generic;

namespace Amba.HtmlBlocks.Fields
{
    public class HtmlBlockField : ContentField
    {
        public string HTML
        {
            get
            {
                return Storage.Get<string>("HTML");
            }
            set
            {
                Storage.Set<string>("HTML", value);
            }
        }
    }
}