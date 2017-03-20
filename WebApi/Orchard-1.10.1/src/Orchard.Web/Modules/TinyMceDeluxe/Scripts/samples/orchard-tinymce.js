////
////
//// Copy this file to your module's Scripts folder, and declare it in your ResourceManifest.cs. This will override the copy in Orchard's default TinyMce module.

////
////
//// Un-comment-out the tinymce.PluginManager.load commands for the plugins you want to use: 
////tinymce.PluginManager.load('advhr', '/Modules/TinyMceDeluxe/Scripts/plugins/advhr/editor_plugin.js');
////tinymce.PluginManager.load('advimage', '/Modules/TinyMceDeluxe/Scripts/plugins/advimage/editor_plugin.js');
////tinymce.PluginManager.load('advlink', '/Modules/TinyMceDeluxe/Scripts/plugins/advlink/editor_plugin.js');
////tinymce.PluginManager.load('advlist', '/Modules/TinyMceDeluxe/Scripts/plugins/advlist/editor_plugin.js');
////tinymce.PluginManager.load('autolink', '/Modules/TinyMceDeluxe/Scripts/plugins/autolink/editor_plugin.js');
////tinymce.PluginManager.load('autosave', '/Modules/TinyMceDeluxe/Scripts/plugins/autosave/editor_plugin.js');
////tinymce.PluginManager.load('bbcode', '/Modules/TinyMceDeluxe/Scripts/plugins/bbcode/editor_plugin.js');
////tinymce.PluginManager.load('contextmenu', '/Modules/TinyMceDeluxe/Scripts/plugins/contextmenu/editor_plugin.js');
////tinymce.PluginManager.load('directionality', '/Modules/TinyMceDeluxe/Scripts/plugins/directionality/editor_plugin.js');
////tinymce.PluginManager.load('emotions', '/Modules/TinyMceDeluxe/Scripts/plugins/emotions/editor_plugin.js');
////tinymce.PluginManager.load('example', '/Modules/TinyMceDeluxe/Scripts/plugins/example/editor_plugin.js');
////tinymce.PluginManager.load('fullpage', '/Modules/TinyMceDeluxe/Scripts/plugins/fullpage/editor_plugin.js');
////tinymce.PluginManager.load('iespell', '/Modules/TinyMceDeluxe/Scripts/plugins/iespell/editor_plugin.js');
////tinymce.PluginManager.load('inlinepopups', '/Modules/TinyMceDeluxe/Scripts/plugins/inlinepopups/editor_plugin.js');
////tinymce.PluginManager.load('insertdatetime', '/Modules/TinyMceDeluxe/Scripts/plugins/insertdatetime/editor_plugin.js');
////tinymce.PluginManager.load('layer', '/Modules/TinyMceDeluxe/Scripts/plugins/layer/editor_plugin.js');
////tinymce.PluginManager.load('legacyoutput', '/Modules/TinyMceDeluxe/Scripts/plugins/legacyoutput/editor_plugin.js');
////tinymce.PluginManager.load('lists', '/Modules/TinyMceDeluxe/Scripts/plugins/lists/editor_plugin.js');
////tinymce.PluginManager.load('media', '/Modules/TinyMceDeluxe/Scripts/plugins/media/editor_plugin.js');
////tinymce.PluginManager.load('nonbreaking', '/Modules/TinyMceDeluxe/Scripts/plugins/nonbreaking/editor_plugin.js');
////tinymce.PluginManager.load('noneditable', '/Modules/TinyMceDeluxe/Scripts/plugins/noneditable/editor_plugin.js');
////tinymce.PluginManager.load('pagebreak', '/Modules/TinyMceDeluxe/Scripts/plugins/pagebreak/editor_plugin.js');
////tinymce.PluginManager.load('paste', '/Modules/TinyMceDeluxe/Scripts/plugins/paste/editor_plugin.js');
////tinymce.PluginManager.load('preview', '/Modules/TinyMceDeluxe/Scripts/plugins/preview/editor_plugin.js');
////tinymce.PluginManager.load('print', '/Modules/TinyMceDeluxe/Scripts/plugins/print/editor_plugin.js');
////tinymce.PluginManager.load('save', '/Modules/TinyMceDeluxe/Scripts/plugins/save/editor_plugin.js');
////tinymce.PluginManager.load('spellchecker', '/Modules/TinyMceDeluxe/Scripts/plugins/spellchecker/editor_plugin.js');
////tinymce.PluginManager.load('style', '/Modules/TinyMceDeluxe/Scripts/plugins/style/editor_plugin.js');
////tinymce.PluginManager.load('tabfocus', '/Modules/TinyMceDeluxe/Scripts/plugins/tabfocus/editor_plugin.js');
////tinymce.PluginManager.load('table', '/Modules/TinyMceDeluxe/Scripts/plugins/table/editor_plugin.js');
////tinymce.PluginManager.load('template', '/Modules/TinyMceDeluxe/Scripts/plugins/template/editor_plugin.js');
////tinymce.PluginManager.load('visualchars', '/Modules/TinyMceDeluxe/Scripts/plugins/visualchars/editor_plugin.js');
////tinymce.PluginManager.load('wordcount', '/Modules/TinyMceDeluxe/Scripts/plugins/wordcount/editor_plugin.js');
////tinymce.PluginManager.load('xhtmlxtras', '/Modules/TinyMceDeluxe/Scripts/plugins/xhtmlxtras/editor_plugin.js');

////
////
//// list the plug-ins you want to use in the plugins: property (and make sure to prefix them with a dash; e.g. to load template plugin you would do: plugins: "-template",
//// update the value of the content_css: property to point to your site's .css file. TinyMce will load the styles from that file into a droplist in the TinyMce editor, so your content editors can apply them.

//// Check the tinymce core plugins documentation to find out which toolbar controls are available for each plugin: http://www.tinymce.com/wiki.php/Plugins

//tinyMCE.init({
//    theme: "advanced",
//    mode: "specific_textareas",
//    editor_selector: "tinymce",
//    plugins: "fullscreen,autoresize,searchreplace,mediapicker,-table,-pagebreak,-template,-paste",
//    theme_advanced_toolbar_location: "top",
//    theme_advanced_toolbar_align: "left",
//    theme_advanced_statusbar_location: "bottom",
//    theme_advanced_resizing: "true",
//    theme_advanced_buttons1: "search,replace,|,cut,copy,paste,|,undo,redo,|,link,unlink,charmap,emoticon,codeblock,|,bold,italic,|,numlist,bullist,formatselect,|,styleselect,|,code,fullscreen",
//    theme_advanced_buttons2: "mediapicker,|,tablecontrols,|,hr,removeformat,visualaid,|,visualchars,template,blockquote,pagebreak",
//    theme_advanced_buttons3: "",
//    convert_urls: false,
//    template_external_list_url : "/Modules/TinyMceDeluxe/Scripts/samples/tinymce_template_list.js",
//    content_css: "/Themes/MyTheme/Styles/Site.css",
//    valid_elements: "*[*]",
//    // shouldn't be needed due to the valid_elements setting, but TinyMCE would strip script.src without it.
//    extended_valid_elements: "script[type|defer|src|language]"
//});
