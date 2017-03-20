(window.layoutPreviewers = window.layoutPreviewers || { })
    .container = function(description, target) {
        var container = $("<div></div>", {
                src: description.attr("src"),
                title: description.attr("title")
            }).css({
                border: "solid 1px black"
            })
            .addClass(description.attr("class") || ""),
            width = description.attr("width"),
            height = description.attr("height"),
            defaultBackground = description.attr("defaultbackground");
        if (defaultBackground) {
            container.css({
                background: "no-repeat center top url(" + window.layout.makeAbsolute(defaultBackground) + ")",
                backgroundSize: "cover"
            });
        }
        if (typeof width !== "undefined" || typeof height !== "undefined") {
            container.css({
                width: width,
                height: height
            });
        }
        if (typeof description.attr("left") !== "undefined" || typeof description.attr("top") !== "undefined") {
            container.css({
                position: "absolute",
                top: description.attr("top"),
                left: description.attr("left")
            });
        }
        if (description.attr("center") === "center") {
            container.css({
                "text-align": "center"
            });
            var centeredBlock = $("<div></div>").css("display", "inline-block");
            var table = $("<div></div>").css({
                position: "relative",
                display: "table"
            });
            var cell = $("<div></div>").css({
                display: "table-cell",
                "vertical-align": "middle"
            });
            if (height) {
                cell.css("height", height);
            }
            target.append(container.append(centeredBlock.append(table.append(cell))));
            return cell;
        }
        target.append(container);
        return container;
    };