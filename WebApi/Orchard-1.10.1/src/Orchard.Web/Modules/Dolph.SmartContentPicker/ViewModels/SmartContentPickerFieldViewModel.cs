using Dolph.SmartContentPicker.Fields;
using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dolph.SmartContentPicker.ViewModels
{
    public class SmartContentPickerFieldViewModel
    {
        public ICollection<ContentItem> ContentItems { get; set; }
        public IEnumerable<ContentItem> AvailableContentItems { get; set; }
        public string[] SelectedIds { get; set; }
        public SmartContentPickerField Field { get; set; }
        public ContentPart Part { get; set; }
    }
}