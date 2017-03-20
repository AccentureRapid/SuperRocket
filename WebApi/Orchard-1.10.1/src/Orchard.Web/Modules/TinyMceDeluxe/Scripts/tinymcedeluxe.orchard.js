(function() {
  var __slice = [].slice;

  window.namespace = function(target, name, block) {
    var item, top, _i, _len, _ref, _ref1;
    if (arguments.length < 3) {
      _ref = [(typeof exports !== 'undefined' ? exports : window)].concat(__slice.call(arguments)), target = _ref[0], name = _ref[1], block = _ref[2];
    }
    top = target;
    _ref1 = name.split('.');
    for (_i = 0, _len = _ref1.length; _i < _len; _i++) {
      item = _ref1[_i];
      target = target[item] || (target[item] = {});
    }
    return block(target, top);
  };

  namespace('TinyMceDeluxe', function(exports) {
    return exports.Orchard = (function() {

      _Class.prototype.ThemePath = '';

      _Class.prototype.PluginBaseUrl = '';

      function _Class() {}

      _Class.prototype.init = function(plugins, options) {
        this.loadPlugins(plugins);
        if (!(options.content_css != null) && TinyMceDeluxe.Orchard.ThemePath > '') {
          options.content_css = TinyMceDeluxe.Orchard.ThemePath;
        }
        console.log('content_css: ' + options.content_css);
        return tinyMCE.init(options);
      };

      _Class.prototype.loadPlugins = function(plugins) {
        var baseUrl, plugin, _i, _len;
        baseUrl = TinyMceDeluxe.Orchard.PluginBaseUrl;
        for (_i = 0, _len = plugins.length; _i < _len; _i++) {
          plugin = plugins[_i];
          tinymce.PluginManager.load(plugin, baseUrl.concat('/').concat(plugin).concat('/editor_plugin.js'));
        }
      };

      return _Class;

    })();
  });

}).call(this);
