window.namespace = (target, name, block) ->
  [target, name, block] = [(if typeof exports isnt 'undefined' then exports else window), arguments...] if arguments.length < 3
  top    = target
  target = target[item] or= {} for item in name.split '.'
  block target, top

namespace 'TinyMceDeluxe', (exports) -> 
    exports.Orchard = class 
        
        ThemePath: ''
        
        PluginBaseUrl: ''

        constructor: () -> 

        init: (plugins, options) -> 
            @loadPlugins(plugins)
            if !options.content_css? && TinyMceDeluxe.Orchard.ThemePath>''
                options.content_css = TinyMceDeluxe.Orchard.ThemePath
            console.log 'content_css: ' + options.content_css
            tinyMCE.init options

        loadPlugins: (plugins)-> 
            baseUrl = TinyMceDeluxe.Orchard.PluginBaseUrl
            (tinymce.PluginManager.load plugin, baseUrl.concat('/').concat(plugin).concat('/editor_plugin.js')) for plugin in plugins 
            return
