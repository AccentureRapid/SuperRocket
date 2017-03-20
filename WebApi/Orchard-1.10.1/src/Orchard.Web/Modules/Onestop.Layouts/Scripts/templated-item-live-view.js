$(window).load(function () {
    var toClientUrl = function(serverUrl) {
        return (serverUrl[0] === '~' && serverUrl[1] === '/' && window.pathFormatString) ?
            window.pathFormatString.replace("__0__", serverUrl.substr(2)) :
            serverUrl;
    };
    $(".templated-live-field")
        .on("keyup change", function() {
            var field = $(this),
                target = $("." + this.id + "_live"),
                targetProperty = target.data("live-property-" + this.id),
                val = field.val() || field.attr("data-default-value");
            if (targetProperty === "src" || targetProperty === "href") {
                target.attr(targetProperty, toClientUrl(val));
            } else if (targetProperty === "html") {
                target.html(val);
            } else {
                if (target.data("live-property-" + this.id + "-is-style")) {
                    if (target.data("live-property-" + this.id + "-is-url")) {
                        val = "url(" + toClientUrl(val) + ")";
                    }
                    target.css(targetProperty, val);
                }
                target.attr(targetProperty, val);
            }
        });
    $(".layout-element")
        .each(function() {
            var slideElement = $(this),
                index = slideElement.data("index"),
                position = slideElement.position();
            if (isNaN(index)) return;
            var scale = slideElement.closest(".scaled").data("scale") || 1;
            $("<div></div>")
                .css({
                    "font-face": "arial",
                    "font-size": "4em",
                    "text-shadow": "0 0 4px white",
                    position: "absolute",
                    left: (position.left  + 2)/ scale + "px",
                    top: (position.top + 6) / scale + "px"
                })
                .addClass("layout-element-index")
                .html(index)
                .appendTo(slideElement.offsetParent());
        });
});