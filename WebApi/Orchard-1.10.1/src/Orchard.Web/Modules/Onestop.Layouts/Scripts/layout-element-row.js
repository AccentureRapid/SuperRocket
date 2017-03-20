(window.layoutPreviewers = window.layoutPreviewers || { })
    .row = function(description, target, scale) {

        var row = $("<div></div>")
            .addClass("layout-element-row row-fluid row " + (description.attr("class") || ""))
            .css({
                border: Math.max(1.2, 1 / scale) + "px solid #6FA041",
                "background-color": "#f0ffff",
                position: "relative"
            })
            .data("handles", false),
            height = description.attr("height");
        if (typeof height !== "undefined") {
            row.css({height: height});
        }
        target.append(row);
        return row;
    };

(window.layoutEditorScripts = window.layoutEditorScripts || {})
    .row = function (editor, layoutElement, layout) {
        var columns = layoutElement.children("column"),
            getHandlePositions = function(cols) {
                for (var i = 0, accumulator = 0, positions = []; i < cols.length - 1; i++) {
                    var column = $(cols[i]),
                        width = (+(column.attr("span") || 1)) + (+(column.attr("offset") || 0));
                    accumulator += width;
                    positions[i] = accumulator;
                }
                return positions;
            };
        if (columns.length > 1) {
            // Create a multiple handle slider
            var slider = $("<div></div>")
                .addClass("layout-multi-column-sizer layout-range-slider");
            slider.multislider({
                values: getHandlePositions(columns),
                animate: true,
                min: 0,
                max: 12,
                slide: function (event, ui) {
                    var previous = 0,
                        values = ui.values.slice(0);
                    values.push(12);
                    for (var j = 0; j < values.length; j++) {
                        var value = values[j],
                            prev = j > 0 ? values[j - 1] : 0,
                            next = j < values.length - 1 ? values[j + 1] : Number.POSITIVE_INFINITY,
                            width = value - previous,
                            column = $(columns[j]),
                            offset = +(column.attr("offset") || 0),
                            span = Math.max(width - offset, 1),
                            offsetEditor = column.data("editor").find(".layout-offset-editor"),
                            spanEditor = column.data("editor").find(".layout-span-editor");
                        if (prev >= value || next <= value) return false;
                        offset = Math.max(width - span, 0);
                        if (offset) {
                            column.attr("offset", offset);
                            offsetEditor.val(offset);
                        } else {
                            column.removeAttr("offset");
                            offsetEditor.val("");
                        }
                        column.attr("span", span);
                        spanEditor.val(span);
                        previous = value;
                    }
                    layout.syncLayoutXml();
                    return true;
                }
            })
                .on("layout-column-change", function() {
                    var cols = layoutElement.children("column");
                    editor
                        .find(".layout-multi-column-sizer")
                        .multislider({ values: getHandlePositions(cols) });
                });
            editor.append(slider);
        }
    };