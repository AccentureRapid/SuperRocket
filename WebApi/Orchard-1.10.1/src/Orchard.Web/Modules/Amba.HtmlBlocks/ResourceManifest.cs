using Orchard.UI.Resources;

namespace Amba.HtmlBlocks
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();
            manifest.DefineScript("HB_CodeMirror").SetUrl("codemirror/lib/codemirror.js");

            manifest.DefineScript("HB_CodeMirror_Search").SetUrl("codemirror/addon/search/search.js").SetDependencies("HB_CodeMirror");
            manifest.DefineScript("HB_CodeMirror_Search_Cursor").SetUrl("codemirror/addon/search/searchcursor.js").SetDependencies("HB_CodeMirror", "HB_CodeMirror_Search");
            manifest.DefineScript("HB_CodeMirror_Search_MatchHighLight").SetUrl("codemirror/addon/search/match-highlighter.js").SetDependencies("HB_CodeMirror", "HB_CodeMirror_Search");
            manifest.DefineScript("HB_CodeMirror_Search_MatchScrollbar").SetUrl("codemirror/addon/search/matchesonscrollbar.js").SetDependencies("HB_CodeMirror");
            manifest.DefineScript("HB_CodeMirror_Search_Dialog").SetUrl("codemirror/addon/dialog/dialog.js").SetDependencies("HB_CodeMirror");

            manifest.DefineScript("HB_CodeMirror_Lint").SetUrl("codemirror/addon/lint/lint.js").SetDependencies("HB_CodeMirror");
            
            manifest.DefineScript("HB_CodeMirror_XML").SetUrl("codemirror/mode/xml/xml.js").SetDependencies("HB_CodeMirror");
            manifest.DefineScript("HB_CodeMirror_CSS").SetUrl("codemirror/mode/css/css.js").SetDependencies("HB_CodeMirror");
            manifest.DefineScript("HB_CodeMirror_JS").SetUrl("codemirror/mode/javascript/javascript.js").SetDependencies("HB_CodeMirror");
            manifest.DefineScript("HB_CodeMirror_CLike").SetUrl("codemirror/mode/clike/clike.js").SetDependencies("HB_CodeMirror");

            manifest.DefineScript("HB_CodeMirror_HtmlMixed").SetUrl("codemirror/mode/htmlmixed/htmlmixed.js ")
                .SetDependencies("HB_CodeMirror", "HB_CodeMirror_XML", "HB_CodeMirror_CSS", "HB_CodeMirror_JS");


            manifest.DefineStyle("HB_CodeMirror").SetUrl("~/modules/Amba.HtmlBlocks/Scripts/codemirror/lib/codemirror.css");
            manifest.DefineStyle("HB_CodeMirror_Lint").SetUrl("~/modules/Amba.HtmlBlocks/Scripts/codemirror/addon/lint/lint.css").SetDependencies("HB_CodeMirror");
            manifest.DefineStyle("HB_CodeMirror_MatchScrollbar").SetUrl("~/modules/Amba.HtmlBlocks/Scripts/codemirror/addon/search/matchesonscrollbar.css").SetDependencies("HB_CodeMirror");
            manifest.DefineStyle("HB_CodeMirror_Dialog").SetUrl("~/modules/Amba.HtmlBlocks/Scripts/codemirror/addon/dialog/dialog.css").SetDependencies("HB_CodeMirror");

            manifest.DefineScript("HB_Highlightjs").SetUrl("~/modules/Amba.HtmlBlocks/Scripts/highlightjs/highlight.pack.js");
            manifest.DefineStyle("HB_Highlightjs").SetUrl("~/modules/Amba.HtmlBlocks/Scripts/highlightjs/styles/vs.css");
            manifest.DefineScript("HB_Highlightjs_Init").SetUrl("~/modules/Amba.HtmlBlocks/Scripts/HighlightjsInit.js").SetDependencies("HB_Highlightjs");
        }
    }
}