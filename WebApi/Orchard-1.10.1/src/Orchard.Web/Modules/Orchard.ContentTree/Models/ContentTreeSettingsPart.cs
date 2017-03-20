using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace Orchard.ContentTree.Models {
    public class ContentTreeSettingsPart : ContentPart<ContentTreeSettingsRecord> {
        public string[] IncludedTypes {
            get {
                if (String.IsNullOrEmpty(Record.IncludedTypes)) {
                    return new string[0];
                }
                else {
                    return Record.IncludedTypes.Split(';');
                }

            }
            set{
                Record.IncludedTypes = String.Join(";", value);
            }
        }
    }

    public class ContentTreeSettingsRecord : ContentPartRecord {
        public virtual string IncludedTypes { get; set; }
    }
}