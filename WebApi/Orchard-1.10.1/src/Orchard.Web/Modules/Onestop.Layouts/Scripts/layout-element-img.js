(window.layoutPreviewers = window.layoutPreviewers || { })
    .img = function(description, target, scale) {
        var width = description.attr("width") || "100px",
            height = description.attr("height") || "70px",
            isInPixels = width.slice(-2) === "px" && height.slice(-2) === "px",
            pixelWidth = parseInt(width, 10),
            pixelHeight = parseInt(height, 10),
            useImg = description.attr("default") || !isInPixels,
            preview = (useImg ?
                $("<img/>", {
                    src: window.layout.makeAbsolute(description.attr("default")),
                    alt: description.attr("defaultalt"),
                    title: description.attr("title")
                }) :
                $("<canvas width=\"" + pixelWidth + "\" height=\"" + pixelHeight + "\"></canvas>"))
                .css("background-color", "white")
                .addClass(description.attr("class") || "");
        if (typeof description.attr("left") !== "undefined" || typeof description.attr("top") !== "undefined") {
            preview.css({
                position: "absolute",
                top: description.attr("top"),
                left: description.attr("left")
            });
        }
        if (useImg) {
            if (description.attr("width")) preview.css({ width: width });
            if (description.attr("height")) preview.css({ height: height });
        } else {
            var ctx = preview[0].getContext("2d");
            ctx.lineWidth = Math.max(1, 1.2 / scale);
            ctx.moveTo(0, 0);
            ctx.lineTo(pixelWidth, 0);
            ctx.lineTo(pixelWidth, pixelHeight);
            ctx.lineTo(0, pixelHeight);
            ctx.lineTo(0, 0);
            ctx.lineTo(pixelWidth, pixelHeight);
            ctx.moveTo(pixelWidth, 0);
            ctx.lineTo(0, pixelHeight);
            ctx.stroke();
        }
        target.append(preview);
        return preview;
    };
