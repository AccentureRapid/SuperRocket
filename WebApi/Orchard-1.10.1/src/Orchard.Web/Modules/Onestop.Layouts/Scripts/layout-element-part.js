(window.layoutPreviewers = window.layoutPreviewers || { })
    .part = function(description, target) {
        var part = $("<div></div>", {
                src: description.attr("src"),
                title: description.attr("title")
            })
            .addClass(description.attr("class") || "")
            .html((description.attr("title") ? description.attr("title") + ": " : "") + (description.attr("part") || "Part"));
        if (typeof description.attr("left") !== "undefined" || typeof description.attr("top") !== "undefined") {
            part.css({
                position: "absolute",
                top: description.attr("top"),
                left: description.attr("left")
            });
        }
        target.append(part);
        return part;
    };