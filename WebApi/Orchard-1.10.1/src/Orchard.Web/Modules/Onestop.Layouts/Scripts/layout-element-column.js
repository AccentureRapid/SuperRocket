(function () {
    // Preview
    (window.layoutPreviewers = window.layoutPreviewers || { })
        .column = function(description, target, scale) {
            var span = description.attr("span"),
                offset = description.attr("offset"),
                breakOn = description.parent().attr("break") || "md",
                column = $("<div></div>"),
                width = description.attr("width");
            if (typeof width !== "undefined") {
                column.css({width: width});
            }
            if (offset > 0) column.addClass("offset" + offset + " col-" + breakOn + "-offset-" + offset);
            if (span) column.addClass("span" + span + " col-" + breakOn + "-" + span);
            column.addClass("layout-element-column");
            var preview =
                    $("<div></div>")
                        .css({
                            border: Math.max(1, 1.2 / scale) + "px solid #0080f0",
                            "background-color": "#f0fff0",
                            padding: "5px",
                            position: "relative",
                            height: "100%"
                        })
                        .addClass(description.attr("class") || "");
            target.append(column.append(preview));
            return preview;
        };

    // Special handlers after adding or deleting a column, to keep all columns in the row evenly sized
    var setColumnSpan = function(column, span) {
        var offset = Math.min(+(column.attr("offset") || 0), span - 1);
        if (offset === 0) {
            column.removeAttr("offset");
        } else {
            column.attr("offset", offset);
        }
        column.attr("span", span - offset);
    },
        resetColumnSpans = function(row) {
            var columns = row.find("column"),
                columnCount = columns.length,
                lowWidth = Math.floor(12 / columnCount),
                howManyHaveLowWidth = columns.length - (12 % columnCount);
            $.each(columns.slice(0, howManyHaveLowWidth), function(index, el) {
                setColumnSpan($(el), lowWidth);
            });
            $.each(columns.slice(howManyHaveLowWidth), function(index, el) {
                setColumnSpan($(el), lowWidth + 1);
            });
        };

    // On new column added
    (window.layoutOnAdded = window.layoutOnAdded || { })
        .column = function(added) {
            resetColumnSpans(added.parent());
        };

    // On column removed
    (window.layoutOnDeleted = window.layoutOnDeleted || { })
        .column = function(deleted, parent) {
            resetColumnSpans(parent);
            parent.data("editor").find(".layout-multi-column-sizer").remove();
            window.layoutEditorScripts.row(parent.data("editor"), parent);
        };

    // Custom editor behavior
    (window.layoutEditorScripts = window.layoutEditorScripts || { })
        .column = function(editor, layoutElement, layout) {
            var offsetEditor = editor.find(".layout-offset-editor"),
                spanEditor = editor.find(".layout-span-editor");
            offsetEditor.on("change keyup", function() {
                var oldOffset = +(layoutElement.attr("offset") || 0),
                    newOffset = +offsetEditor.val(),
                    oldSpan = +(layoutElement.attr("span") || 1);
                if (isNaN(newOffset)) {
                    layoutElement.removeAttr("offset");
                }
                else {
                    var newSpan = (oldSpan + oldOffset) - newOffset;
                    if (newSpan < 1) {
                        newOffset = oldSpan + oldOffset - 1;
                        newSpan = 1;
                        offsetEditor.val(newOffset);
                    }
                    layoutElement.attr({
                        offset: newOffset,
                        span: newSpan
                    });
                    spanEditor.val(newSpan);
                }
                layoutElement.parent().data("editor").find(".layout-multi-column-sizer")
                    .trigger("layout-column-change");
                layout.syncLayoutXml();
            });
            spanEditor.on("change keyup", function() {
                var oldSpan = +(layoutElement.attr("span") || 1),
                    newSpan = +(spanEditor.val() || 1);
                if (newSpan < 1) newSpan = 1;
                var spanDiff = newSpan - oldSpan,
                    canSiblingCompensate = function () {
                        var sibling = $(this),
                            siblingSpan = +(sibling.attr("span")),
                            newSiblingSpan = siblingSpan - spanDiff;
                        return !isNaN(newSiblingSpan) && newSiblingSpan > 0 && newSiblingSpan < 12;
                    },
                    possibleSiblings = layoutElement
                        .nextAll()
                        .add(layoutElement.prevAll())
                        .filter(canSiblingCompensate);
                if (possibleSiblings.length === 0) {
                    newSpan = oldSpan;
                }
                else {
                    var chosenSibling = possibleSiblings.first(),
                        chosenSiblingSpan = +(chosenSibling.attr("span") || 1) - spanDiff;
                    chosenSibling
                        .attr("span", chosenSiblingSpan)
                        .data("editor")
                        .find(".layout-span-editor")
                        .val(chosenSiblingSpan);
                }
                layoutElement.attr("span", newSpan);
                spanEditor.val(newSpan);
                layoutElement.parent().data("editor").find(".layout-multi-column-sizer")
                    .trigger("layout-column-change");
                layout.syncLayoutXml();
            });
        };
})();