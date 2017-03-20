using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement.Records;
using Orchard.ContentManagement;

namespace TheMonarch.SyntaxHighlighter.Models {
    public class SyntaxHighlighterSettingsPartRecord : ContentPartRecord {
        public virtual string Theme { get; set; }
    }

    public class SyntaxHighlighterSettingsPart : ContentPart<SyntaxHighlighterSettingsPartRecord> {
        public string Theme {
            get { return Record.Theme; }
            set { Record.Theme = value; }
        }
    }
    
    /// TODO: Move to /ViewModels folder: 
    public class SyntaxHighlighterSettingsViewModel {
        public string Theme { get; set; }
        public IEnumerable<string> Themes { get; set; }
    }
}
