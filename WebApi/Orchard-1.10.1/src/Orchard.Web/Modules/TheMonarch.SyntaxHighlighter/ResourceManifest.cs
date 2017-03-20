using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;
using Orchard.UI.Resources;

namespace TheMonarch.SyntaxHighlighter {

    public class ResourceManifest : IResourceManifestProvider {

        #region helpers

        private static string ThemeFolder = @"~/Themes/TheMonarch.SyntaxHighlighter/";
        private static string ModuleFolder = @"~/Modules/TheMonarch.SyntaxHighlighter/";
        private enum ResourceType { Scripts, Styles }
        private enum ResourceLocation { Module, Theme }
        
        private static string ModuleStyle(string path) {
            return GetResourcePath(path, ResourceType.Styles, ResourceLocation.Module);
        }

        private static string ModuleScript(string path) {
            return GetResourcePath(path, ResourceType.Scripts, ResourceLocation.Module);
        }

        private static string GetResourcePath(string path, ResourceType type, ResourceLocation loc) {
            if (path.StartsWith("/")) {
                path = path.Substring(1);
            }
            return string.Format("{0}{1}/{2}"
                , loc == ResourceLocation.Module ? ModuleFolder : ThemeFolder
                , type.ToString()
                , path
            );
        }

        #endregion helpers

        private readonly IOrchardServices _services;

        public ResourceManifest(IOrchardServices services) {
            _services = services;
        }

        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();

            //Styles
            manifest.DefineStyle("TheMonarch.prettify-prettify").SetUrl(ModuleStyle("pretty/prettify.css")).SetVersion("1.0");
            manifest.DefineStyle("TheMonarch.prettify-desert").SetUrl(ModuleStyle("pretty/desert.css")).SetVersion("1.0");
            manifest.DefineStyle("TheMonarch.prettify-github").SetUrl(ModuleStyle("pretty/github.css")).SetVersion("1.0");
            manifest.DefineStyle("TheMonarch.prettify-hemisu-dark").SetUrl(ModuleStyle("pretty/hemisu-dark.css")).SetVersion("1.0");
            manifest.DefineStyle("TheMonarch.prettify-hemisu-light").SetUrl(ModuleStyle("pretty/hemisu-light.css")).SetVersion("1.0");
            manifest.DefineStyle("TheMonarch.prettify-son-of-obsidian").SetUrl(ModuleStyle("pretty/son-of-obsidian.css")).SetVersion("1.0");
            manifest.DefineStyle("TheMonarch.prettify-sunburst").SetUrl(ModuleStyle("pretty/sunburst.css")).SetVersion("1.0");
            manifest.DefineStyle("TheMonarch.prettify-vibrant-ink").SetUrl(ModuleStyle("pretty/vibrant-ink.css")).SetVersion("1.0");
            manifest.DefineStyle("TheMonarch.prettify-tomorrow").SetUrl(ModuleStyle("pretty/tomorrow.css")).SetVersion("1.0");


            // Scripts
            manifest.DefineScript("TheMonarch.prettify").SetUrl(ModuleScript("pretty/prettify.js")).SetDependencies("jQuery");
            manifest.DefineScript("TheMonarch.prettify-css").SetUrl(ModuleScript("pretty/prettify-css.js")).SetDependencies("TheMonarch.prettify");
            manifest.DefineScript("TheMonarch.prettify-sql").SetUrl(ModuleScript("pretty/prettify-sql.js")).SetDependencies("TheMonarch.prettify");
        }
    }
}
