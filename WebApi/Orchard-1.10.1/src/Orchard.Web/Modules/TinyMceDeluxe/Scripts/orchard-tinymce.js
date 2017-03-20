////
//// How to customize the TinyMce editor via TinyMceDeluxe: 
//// 1. Copy this file to your module's Scripts folder (e.g., /Modules/YOUR_MODULE/Scripts/orchard-tinymce.js)
//// 2. Declare it in your ResourceManifest.cs (use a version # higher than the default one in TinyMceDeluxe's ResourceManifest, which is 1.0). 
////        Example:
////            manifest.DefineScript("OrchardTinyMceDeluxe").SetUrl("orchard-tinymce.js").SetDependencies("TinyMceDeluxe").SetVersion("2.0");
//// 3. Make sure you declare TinyMceDeluxe as a dependency in your module's /Module.txt file. 
//// 4. This will override the copy in Orchard's default TinyMce module. Now you can customize the init options below. 


//// Check the tinymce core plugins documentation to find out which toolbar controls are available for each plugin: http://www.tinymce.com/wiki.php/Plugins

$(document).ready(function () {
    tinyMceDeluxe = new TinyMceDeluxe.Orchard();

    // 1st arg is an array of plugin names. See plugin link above for full list of available plugins
    // 2nd arg is an options object; also see TinyMce documentation for details on all available options. 
    tinyMceDeluxe.init(['pagebreak', 'paste', 'table', 'template', 'syntaxhl'], {
        theme: "advanced",
        mode: "specific_textareas",
        editor_selector: "tinymce",
        plugins: "fullscreen,autoresize,searchreplace,mediapicker,inlinepopups,-table,-pagebreak,-template,-paste,-syntaxhl",
        theme_advanced_toolbar_location: "top",
        theme_advanced_toolbar_align: "left",
        theme_advanced_statusbar_location: "bottom",
        theme_advanced_resizing: "true",
        theme_advanced_buttons1: "search,replace,|,cut,copy,paste,|,undo,redo,|,link,unlink,charmap,emoticon,codeblock,|,bold,italic,|,numlist,bullist,formatselect,|,styleselect,|,syntaxhl,code,fullscreen",
        theme_advanced_buttons2: "mediapicker,|,tablecontrols,|,hr,removeformat,visualaid,|,visualchars,template,blockquote,pagebreak",
        theme_advanced_buttons3: "",
        convert_urls: false,
        template_external_list_url: "/modules/tinymcedeluxe/scripts/samples/tinymce_template_list.js",
        // content_css sets the path to your site's main .css file. The styles from this file are imported into a droplist in the TinyMce editor. 
        // TinyMceDeluxe sets this path automatically to the /Styles/custom.css file in your site's theme, but you can override the path by declaring it here:
        //content_css: '/path/to/your/stylesheet.css',
        valid_elements: "*[*]",
        // shouldn't be needed due to the valid_elements setting, but TinyMCE would strip script.src without it.
        extended_valid_elements: "script[type|defer|src|language]"
    });
});