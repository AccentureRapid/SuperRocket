using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Onestop.Layouts.Models {
    public class StylesheetDescription {
        private readonly Regex _fontExpression =
            new Regex(@"^\s*font-family:(?<font>[^;]*)\s*;",
            RegexOptions.ExplicitCapture | RegexOptions.Multiline);
        private readonly Regex _classExpression =
            new Regex(@"^\s*\.(?<class>-?[_a-zA-Z]+[_a-zA-Z0-9-]*)\s+",
            RegexOptions.ExplicitCapture | RegexOptions.Multiline);

        private string _contents;
        private List<string> _classes;
        private List<string> _fonts; 

        public StylesheetDescription(string name, string virtualPath, string physicalPath, string themeName) {
            Name = name;
            VirtualPath = virtualPath;
            PhysicalPath = physicalPath;
            ThemeName = themeName;
        }
        public string Name { get; private set; }
        public string VirtualPath { get; private set; }
        public string PhysicalPath { get; private set; }
        public string ThemeName { get; private set; }

        private void ReadContents() {
            if (_contents == null) {
                _contents = File.ReadAllText(PhysicalPath);
            }
        }
        public IEnumerable<string> GetClasses() {
            if (_classes == null) {
                ReadContents();
                _classes = _classExpression
                    .Matches(_contents)
                    .Cast<Match>()
                    .Select(m => m.Groups["class"].Value)
                    .Distinct()
                    .ToList();

            }
            return _classes;
        }
        public IEnumerable<string> GetFonts() {
            if (_fonts == null) {
                ReadContents();
                _fonts = _fontExpression
                    .Matches(_contents)
                    .Cast<Match>()
                    .SelectMany(m => m
                        .Groups["font"]
                        .Value
                        .Split(',')
                        .Select(f => f.Trim(' ', '"', '\'')))
                    .Distinct()
                    .ToList();
            }
            return _fonts;
        }
    }
}
