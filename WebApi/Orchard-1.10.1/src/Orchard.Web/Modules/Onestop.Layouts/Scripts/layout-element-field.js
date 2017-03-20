(window.layoutPreviewers = window.layoutPreviewers || { })
    .field = function(description, target) {
        var field = $("<div></div>", {
                src: description.attr("src"),
                title: description.attr("title")
            })
            .addClass(description.attr("class") || "")
            .html((description.attr("title") ? description.attr("title") + ": " : "") + (description.attr("field") || "Field"));
        if (typeof description.attr("left") !== "undefined" || typeof description.attr("top") !== "undefined") {
            field.css({
                position: "absolute",
                top: description.attr("top"),
                left: description.attr("left")
            });
        }
        target.append(field);
        return field;
    };