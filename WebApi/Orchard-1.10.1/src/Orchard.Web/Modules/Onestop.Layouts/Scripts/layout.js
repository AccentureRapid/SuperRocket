var layout = function(options) {
    var res =
            options.resources || {
                del: "Delete",
                cancel: "Cancel",
                confirm: "Confirm",
                expandAll: "Expand all",
                collapseAll: "Collapse all"
            },
        applicationPath = options.applicationPath,
        isTemplate = !!options.isTemplate,
        sourceField = options.sourceField,
        generalEditor = options.generalEditor,
        elementTemplates = options.elementTemplates,
        editorContainer = options.editorContainer,
        previewContainer = options.previewContainer,
        widthField = options.widthField,
        heightField = options.heightField,
        classField = options.classField,
        pageClassField = options.pageClassField,
        stylesheetField = options.stylesheetField,
        stylesheet = "",
        cssClasses = options.cssClasses,
        fonts = options.fonts,
        layoutDocument = null,
        layoutRootElement = null,
        zoom = options.zoom,
        previewers = window.layoutPreviewers || {},
        onAdded = window.layoutOnAdded || {},
        onDeleted = window.layoutOnDeleted || {},
        editorScripts = window.layoutEditorScripts || {},
        scale = 1,
        templates = {},
        newElementButtons = [],
        currentlyFocused,
        dragged,
        editorsCollapsed = true,
        toggleLink;

    // Build the layout object that will get returned
    $.extend(layout, {
        _applyScale: function(container) {
            var browserPrefixes = ["", "-ms-", "-moz-", "-webkit-", "-o-"],
                offset = ((scale - 1) * 50) + "%",
                scaleString =
                    "translate(" + offset + ", " + offset + ") " +
                        "scale(" + scale + "," + scale + ")";
            $(browserPrefixes).each(function() {
                container
                    .css(this + "transform", scaleString);
            });
            return layout;
        },

        _formatXml: function(xml) {
            // See http://stackoverflow.com/questions/376373/pretty-printing-xml-with-javascript
            var reg = /(>)(<)(\/*)/g;
            var wsexp = / *(.*) +\n/g;
            var contexp = /(<.+>)(.+\n)/g;
            xml = xml.replace(reg, "$1\n$2$3").replace(wsexp, "$1\n").replace(contexp, "$1\n$2");
            var formatted = "";
            var lines = xml.split("\n");
            var indent = 0;
            var lastType = "?";
            // 4 types of tags - single, closing, opening, other (text, doctype, comment) - 4*4 = 16 transitions 
            var transitions = {
                "ss": 0,
                "sc": -1,
                "so": 0,
                "s?": 0,
                "cs": 0,
                "cc": -1,
                "co": 0,
                "c?": 0,
                "os": 1,
                "oc": 0,
                "oo": 1,
                "o?": 1,
                "?s": 0,
                "?c": -1,
                "?o": 0,
                "??": 0
            };

            for (var i = 0; i < lines.length; i++) {
                var ln = $.trim(lines[i]);
                if (!ln) continue;
                var single = !!ln.match(/<.+\/>/), // is this line a single tag? ex. <br />
                    closing = !!ln.match(/<\/.+>/), // is this a closing tag? ex. </a>
                    opening = !!ln.match(/<[^!].*>/), // is this even a tag (that's not <!something>)
                    type = single ? "s" : closing ? "c" : opening ? "o" : "?",
                    fromTo = lastType + type;
                lastType = type;
                var padding = "";

                indent += transitions[fromTo];
                for (var j = 0; j < indent; j++) {
                    padding += "    ";
                }

                formatted += padding + ln + "\n";
            }

            return formatted.replace(/ xmlns="http:\/\/www.w3.org\/1999\/xhtml"/g, "");
        },

        _serialize: function(jQueryXmlDocument, pretty) {
            // ReSharper disable InconsistentNaming
            var xmlString = new XMLSerializer().serializeToString(jQueryXmlDocument[0]);
            // ReSharper restore InconsistentNaming
            return pretty === false ? xmlString : layout._formatXml(xmlString);
        },

        _setDimensions: function(source, destination) {
            var width = source.width,
                height = source.height;
            if (width) destination.css("width", width + "px");
            if (height) destination.css("height", height + "px");
            return layout;
        },

        _syncXml: function() {
            // Parse the XML from the text area
            try {
                layoutDocument = $($.parseXML(sourceField.val()));
                layoutRootElement = layoutDocument.find("layout");
                // Also sync general property editors
                widthField.val(layoutRootElement.attr("width"));
                heightField.val(layoutRootElement.attr("height"));
                classField.val(layoutRootElement.attr("class"));
                pageClassField.val(layoutRootElement.attr("pageclass"));
                stylesheetField.val(stylesheet = layoutRootElement.attr("stylesheet"));
            } catch(ex) {
            }
            return layout;
        },

        addElement: function(template, templateName, defaultxml) {
            if (template.data("disable")) return;
            // Append a new xml element to the layout document,
            // or to the currently focused element if it can support it
            var newElement = $(defaultxml || ("<" + templateName + "/>")),
                container = currentlyFocused &&
                    layout.canBeNested(currentlyFocused.data("editor"), template) ?
                    currentlyFocused : layoutDocument.find("layout");
            container.append(newElement);
            // Trigger add events
            if (onAdded[templateName]) {
                onAdded[templateName](newElement);
            }
            // Refresh everything
            layout
                .syncLayoutXml()
                .buildEditors();
            var editor = newElement.data("editor");
            // Focus on the newly-created element if it wants to
            if (template.data("focus-when-creating")) {
                layout.focusOn(editor, true);
            }
                // Otherwise, just scroll it into view
            else {
                layout.scrollIntoView(editor, true);
            }
        },

        autoComplete: function(textField, stylesheetToValues, separator) {
            textField
                .on("keydown", function(event) {
                    if (event.keyCode === $.ui.keyCode.TAB &&
                        $(this).data("autocomplete").menu.active) {
                        event.preventDefault();
                    }
                })
                .autocomplete({
                    minLength: 0,
                    source: function(request, response) {
                        response($.ui.autocomplete.filter(
                            stylesheet ? stylesheetToValues[stylesheet] : [], request.term.split(" ").pop()));
                    },
                    focus: function() {
                        return false;
                    },
                    select: function(event, ui) {
                        separator = separator || " ";
                        var autoCompleted = $(this);
                        var terms = $.map(autoCompleted.val().split(separator), $.trim);
                        terms.pop();
                        terms.push(ui.item.value);
                        terms.push("");
                        var completedValue = $.trim(terms.join(separator + (separator === " " ? "" : " ")));
                        if (completedValue.substr(-1) === separator) {
                            completedValue = completedValue.substr(0, completedValue.length - 1);
                        }
                        autoCompleted
                            .val(completedValue)
                            .change();
                        return false;
                    }
                });
            return layout;
        },

        bindEditor: function(editor, layoutElement, idMap) {
            editor
                .find("*")
                .each(function() {
                    var templateElement = $(this);
                    $.each(this.attributes, function() {
                        var attr = this.name,
                            val = templateElement.attr(attr);
                        if (val && val.substr(0, 2) === "{{" && val.substr(val.length - 2, 2) === "}}") {
                            var bindName = val.substr(2, val.length - 4);
                            if (attr === "value" || attr === "data-value") {
                                // value and data-value attributes get replaced with the property value specified in {{ }} and get set as the value
                                // For example, data-value="{{top}}" with top="4" in the XML results in value="4"
                                var templateElementTag = templateElement[0].tagName.toLowerCase(),
                                    isCheckbox = templateElementTag === "input" &&
                                        templateElement.attr("type") === "checkbox",
                                    isLength = templateElementTag === "input" &&
                                        templateElement.attr("type") === "number" &&
                                        templateElement.hasClass("length"),
                                    stringval = layoutElement.attr(bindName),
                                    value = (isCheckbox ?
                                        stringval === templateElement.data("attribute-value") :
                                        isLength ?
                                            parseInt(stringval, 10) :
                                            stringval) || templateElement.attr("data-default-value");
                                if (isLength) {
                                    var unitDropdown = $("<select><option>px</option><option>%</option><option>em</option><option>pt</option></select>")
                                        .find("option")
                                        .each(function() {
                                            var option = $(this),
                                                optionval = option.val();
                                            if (stringval && stringval.slice(-optionval.length) === optionval) {
                                                option.attr("selected", "selected");
                                            }
                                        })
                                        .end()
                                        .on("change", function() {
                                            var newValue = parseInt($.trim(templateElement.val()), 10);
                                            if (!isNaN(newValue)) {
                                                layoutElement.attr(bindName, newValue + unitDropdown.val());
                                            }
                                            layout.syncLayoutXml();
                                        });
                                    templateElement
                                        // Add unit drop-down next to the number input
                                        .after(unitDropdown)
                                        // and handle change
                                        .on("change keyup", function() {
                                            // On change, the value of the input is sent to the corresponding attribute in the XML
                                            var newValue = $.trim(templateElement.val());
                                            if (newValue === "") {
                                                layoutElement.removeAttr(bindName);
                                            } else {
                                                layoutElement.attr(bindName, newValue + unitDropdown.val());
                                            }
                                            layout.syncLayoutXml();
                                        })
                                        .val(value);
                                } else if (isCheckbox) {
                                    templateElement.prop("checked", value);
                                } else {
                                    templateElement.val(value);
                                }
                                if (!templateElement.data("custom-handling")) {
                                    if (templateElementTag === "select") {
                                        // Handle <select>
                                        // Select the right option
                                        templateElement.find("option[value=\"" + value + "\"]").prop("selected", true);
                                        // Handle change
                                        templateElement.change(function() {
                                            var newValue = $(this).find("option:selected").val();
                                            layoutElement.attr(bindName, newValue);
                                            layout.syncLayoutXml();
                                        });
                                    } else if (isCheckbox) {
                                        // Handle checkboxes
                                        templateElement.change(function() {
                                            if (templateElement.prop("checked")) {
                                                layoutElement.attr(bindName, templateElement.data("attribute-value"));
                                            } else {
                                                layoutElement.removeAttr(bindName);
                                            }
                                            layout.syncLayoutXml();
                                        });
                                    } else if (!isLength) {
                                        templateElement
                                            .on("change keyup", function() {
                                                // On change, the value of the input is sent to the corresponding attribute in the XML
                                                var newValue = $.trim(templateElement.val());
                                                if (newValue === "") {
                                                    layoutElement.removeAttr(bindName);
                                                } else {
                                                    layoutElement.attr(bindName, newValue);
                                                }
                                                layout.syncLayoutXml();
                                            });
                                    }
                                }
                            } else {
                                // Other attributes get replaced with a unique id generated for the property.
                                if (!idMap[bindName]) {
                                    idMap[bindName] = "layout_element_" + (window.layoutElementIndex++);
                                }
                                templateElement.attr(attr, idMap[bindName]);
                            }
                        }
                    });
                    // Set-up range editors
                    layout.buildRangeEditor(templateElement, editor);
                });
            // Add custom script
            var elementName = layoutElement[0].tagName;
            if (editorScripts[elementName]) {
                editorScripts[elementName](editor, layoutElement, layout);
            }
            return layout;
        },

        buildDeleteButton: function(container, deleteCallback) {
            var deleteButton = $("<a/>", {
                href: "#",
                text: res.del,
                click: function(e) {
                    e.preventDefault();
                    deleteButton.hide();
                    confirmButton.show();
                    cancelButton.show();
                }
            }),
                confirmButton = $("<a/>", {
                    href: "#",
                    text: res.confirm,
                    click: function(e) {
                        e.preventDefault();
                        deleteCallback();
                        deleteButton.show();
                        confirmButton.hide();
                        cancelButton.hide();
                    }
                }),
                cancelButton = $("<a/>", {
                    href: "#",
                    text: res.cancel,
                    click: function(e) {
                        e.preventDefault();
                        deleteButton.show();
                        confirmButton.hide();
                        cancelButton.hide();
                    }
                });
            container.append(
                $("<div></div>")
                    .addClass("layout-editor-element-actions")
                    .append(deleteButton)
                    .append(confirmButton.hide())
                    .append(cancelButton.hide())
            );
            return layout;
        },

        buildEditor: function(index, layoutElement, container) {
            container = container || editorContainer;
            // Method may be called from an iterator
            layoutElement = layoutElement || this;
            var name = layoutElement.tagName,
                // Clone the template
                elementEditor = templates[name]
                    .clone()
                    .removeClass("layout-editor-element-template")
                    .addClass("layout-editor-element"),
                idMap = {};
            layoutElement = $(layoutElement);
            if (elementEditor.data("for-templates") === isTemplate) {
                // Create the delete, cancel and confirm buttons
                layout.buildDeleteButton(elementEditor, function() {
                    var templateName = layoutElement[0].tagName,
                        parent = layoutElement.parent();
                    layoutElement.remove();
                    elementEditor.remove();
                    // Trigger deleted events
                    if (onDeleted[templateName]) {
                        onDeleted[templateName](layoutElement, parent);
                    }
                    // Refresh 
                    layout.syncLayoutXml();
                    // Unfocus all
                    layout.focusOn(null, true);
                });

                // Substitute {{ }} placeholders with values from the XML layout element
                layout.bindEditor(elementEditor, layoutElement, idMap);
            } else {
                // Elements of this type are not editable in this context, remove all contents other than the legend
                elementEditor.find(":not(legend)").remove();
            }
            // Attempt to add the editor's icon to its legend
            var icon = elementEditor.data("icon"),
                legend = elementEditor.find("legend");
            if (icon && legend) {
                legend.prepend(
                    $("<img/>")
                        .attr({ src: icon, alt: name })
                        .css({ marginRight: "6px", position: "relative", top: "2px" }));
            }
            // Append the new editor
            var li = $("<li></li>")
                .data("xml-element", layoutElement)
                .append(elementEditor);
            container.append(li);
            layoutElement.data("editor", elementEditor);

            // Recurse into children
            if (elementEditor.data("iscontainer")) {
                var subcontainer = $("<ol></ol>")
                    .data("iscontainer", true);
                // Build child editors
                layoutElement.children()
                    .each(function() {
                        layout.buildEditor(index, this, subcontainer);
                    });
                li.append(subcontainer);
            } else {
                li.addClass("no-nest");
            }

            return layout;
        },

        buildEditors: function() {
            // Start from scratch
            editorContainer.empty();
            previewContainer.empty();

            // Create an editor element for each XML layout element
            layoutRootElement
                .children()
                .each(layout.buildEditor);

            // Collapse or expand all depending on current state
            layout.toggleAll(!editorsCollapsed);

            // Set-up class and font editors
            layout
                .autoComplete(editorContainer.find(".css-class-editor"), cssClasses)
                .autoComplete(editorContainer.find(".css-font-editor"), fonts, ",");

            // focus on click
            editorContainer.find(".layout-editor-element legend").click(function() {
                layout.focusOn($(this).closest(".layout-editor-element"));
            });

            // Generate the preview
            layout.syncPreview();

            return layout;
        },

        buildNewElementButton: function(icon, description, templateName, defaultxml) {
            var template = templates[templateName],
                newButton =
                    $("<a></a>", {
                        href: "#",
                        click: function(e) {
                            e.preventDefault();
                            layout.addElement(template, templateName, defaultxml);
                        }
                    })
                        .append(
                            // Create the icon
                            $("<img/>", {
                                src: icon,
                                title: description,
                                alt: description
                            })
                        )
                        .data("template", template);
            toolbar.append($("<li></li>").append(newButton));
            return newButton;
        },

        buildRangeEditor: function(templateElement, editor) {
            if (templateElement.hasClass("layout-range-slider")) {
                var minEditor = editor.find("#" + templateElement.data("min")),
                    maxEditor = editor.find("#" + templateElement.data("max"));
                templateElement.slider({
                    range: true,
                    values: [+minEditor.val(), +minEditor.val() + (+maxEditor.val())],
                    animate: true,
                    min: +minEditor.attr("min"),
                    max: +maxEditor.attr("max"),
                    slide: function(event, ui) {
                        var span = ui.values[1] - ui.values[0];
                        if (span < +maxEditor.attr("min")) {
                            event.preventDefault();
                        } else {
                            minEditor.val(ui.values[0]);
                            maxEditor.val(span);
                            // Trigger change event
                            minEditor.add(maxEditor).change();
                        }
                    }
                });
                minEditor.add(maxEditor).on("change keyup", function(e) {
                    var loEnd = +minEditor.val(),
                        dist = +maxEditor.val(),
                        minMin = +minEditor.attr("min"),
                        maxMin = +maxEditor.attr("min"),
                        maxMax = +maxEditor.attr("max"),
                        hiEnd = loEnd + dist;
                    if (loEnd < minMin || dist < maxMin) {
                        e.preventDefault();
                    }
                    if (hiEnd > maxMax) {
                        dist = maxMax - loEnd;
                        maxEditor.val(dist).change();
                    }
                    templateElement.slider({
                        values: [loEnd, hiEnd]
                    });
                });
            }
            return layout;
        },

        buildShowHideButton: function() {
            toolbar.append(
                $("<li></li>").append(
                    toggleLink = $("<a href='#'>+</a>")
                        .attr("title", res.expandAll)
                        .click(function(e) {
                            e.preventDefault();
                            layout.toggleAll();
                        })
                ).css("text-align", "center"));
            return layout;
        },

        buildToolbar: function() {
            // Create a new button for each available element
            elementTemplates
                .each(function() {
                    var template = $(this),
                        icon = template.data("icon"),
                        description = template.data("description"),
                        templateName = template.data("template"),
                        defaultxml = template.data("default-xml"),
                        forTemplates = template.data("for-templates");
                    templates[templateName] = template;
                    if (forTemplates === isTemplate) {
                        newElementButtons.push(layout.buildNewElementButton(icon, description, templateName, defaultxml));
                    }
                });
            layout.syncNewButtons();
            // Add show/hide details button
            toolbar.append($("<li></li>").append($("<hr/>")));
            layout.buildShowHideButton();

            window.layoutElementIndex = 0;
            return layout;
        },

        canBeNested: function(parent, item) {
            //var previousTarget = currentTarget;
            //currentTarget = parent ? parent[0] : null;
            //if (currentTarget != previousTarget) {
            //    console.log("Dragging to ", currentTarget);
            //}
            if (parent && parent.data("iscontainer") !== true) return false;
            if (parent && parent.data("can-contain")) {
                var allowedContainees = parent.data("can-contain").split(",");
                if ($.inArray(item.data("template"), allowedContainees) === -1) return false;
            }
            if (item.data("can-go-under")) {
                var allowedContainers = item.data("can-go-under").split(",");
                if (!parent || $.inArray(parent.data("template"), allowedContainers) === -1) return false;
            }
            return true;
        },

        createElementPreview: function(layoutElement, previews, container) {
            container = container || previewContainer;
            var name = layoutElement[0].tagName,
                previewer = null;

            // Get the preview instance from the relevant previewer
            if (previewers[name]) {
                previews.push(previewer = previewers[name](layoutElement, container, scale));
            }

            if (previewer) {
                // Remember the element
                previewer
                    .data("layoutElement", layoutElement)
                    // set-up focus code
                    .click(function(e) {
                        e.stopPropagation();
                        e.preventDefault();
                        layout.focusOn(previewer);
                    });
                // Also help the layout element remember its preview
                layoutElement.data("previewer", previewer);

                // Recurse into children
                if (layoutElement.children().length > 0) {
                    layoutElement.children().each(function() {
                        layout.createElementPreview($(this), previews, previewer);
                    });
                }
            }
            return layout;
        },

        enable: function(el) {
            el.css({
                cursor: "auto",
                opacity: "1"
            })
                .data("template").removeData("disable");
            return layout;
        },

        disable: function(el) {
            el.css({
                cursor: "default",
                opacity: "0.4"
            })
                .data("template").data("disable", true);
            return layout;
        },

        fitPreview: function() {
            // Zoom level is computed so the whole slide fits
            var containerWidth = (previewContainer.width() || previewContainer.parent().width()) + 20,
                containerHeight = (previewContainer.height() || previewContainer.parent().height()) + 20,
                initialScalePercent = Math.floor((scale = Math.min(
                    previewContainer.parent().width() / containerWidth,
                    previewContainer.parent().height() / containerHeight
                )) * 1000) / 10;
            layout.zoomTo(initialScalePercent);
        },

        focusOn: function(previewElementOrEditor, preventAnimation) {
            editorContainer.add(previewContainer).find(".focused").removeClass("focused");

            // Collapse all if collapse button is on that state
            if (editorsCollapsed) {
                layout.toggleAll(false);
            }

            // If null was passed in, just unfocus everything and update new element buttons
            if (!previewElementOrEditor) {
                currentlyFocused = null;
                layout.syncNewButtons();
                return;
            }
            // Find layout, editor and preview
            var isEditor = previewElementOrEditor.hasClass("layout-editor-element"),
                layoutElement = isEditor ?
                    previewElementOrEditor.parent().data("xml-element") :
                    previewElementOrEditor.data("layoutElement");
            if (!layoutElement) {
                currentlyFocused = null;
                layout.syncNewButtons();
                return;
            }
            var previewElement = layoutElement.data("previewer"),
                editor = layoutElement.data("editor").parent();
            // If the new focused element is the same as before, unfocus
            if (currentlyFocused && currentlyFocused[0] === layoutElement[0]) {
                currentlyFocused = null;
                layout.syncNewButtons();
                return;
            }
            // Set currently focused to the layout element
            currentlyFocused = layoutElement;
            // Focus preview and editor
            editor.add(previewElement).addClass("focused");
            // Enable / disable buttons
            layout.syncNewButtons();
            // Expand the editor if state is currently collapsed for all
            editor.children("fieldset").children("div").show();
            // Scroll into view
            layout.scrollIntoView(editor, preventAnimation);
        },

        makeAppRelative: function (path) {
            if (!path) return "";
            var lcasePath = path.toLowerCase();
            if (lcasePath.substr(0, 7) !== "http://" &&
                lcasePath.substr(0, 8) !== "https://") {
                return "~/" + path.substr(applicationPath.length);
            }
            return path;
        },

        makeAbsolute: function(path) {
            if (!path) return "";
            if (path.substr(0, 2) === "~/") {
                return applicationPath + path.substr(2);
            }
            return path;
        },

        rebuildXmlFromEditorList: function(container, parentElement) {
            container = container || editorContainer;
            parentElement = parentElement || layoutRootElement;
            parentElement.empty();
            container
                .children("li")
                .each(function() {
                    var childEditor = $(this),
                        childList = childEditor.children("ol"),
                        childElement = childEditor.data("xml-element").empty();
                    parentElement.append(childElement);
                    if (childList.length > 0) {
                        layout.rebuildXmlFromEditorList(childList, childElement);
                    }
                });
            return layout;
        },

        scalePreview: function() {
            layout._applyScale(previewContainer);
            return layout;
        },

        scrollIntoView: function(editor, preventAnimation) {
            // Scroll editor into view, with style
            var scrollTop = editorContainer.scrollTop() + editor.offset().top - editorContainer.offset().top - 100;
            if (preventAnimation) {
                editorContainer.scrollTop(scrollTop);
            } else {
                editorContainer.animate({ scrollTop: scrollTop });
            }
        },

        sendPreviewToTopLeft: function() {
            // move the preview container to the top-left
            previewContainer
                .css({ left: "5px", top: "5px" });
        },

        syncLayoutXml: function(syncPreview) {
            var xml = layout._serialize(layoutDocument);
            sourceField.val(xml);
            if (syncPreview !== false) {
                // If the XMl needs to be synchronized, the preview probably needs it as well
                layout.syncPreview();
            }
            return layout;
        },

        syncNewButtons: function() {
            $.each(newElementButtons, function() {
                var template = this.data("template"),
                    enabled = layout.canBeNested(currentlyFocused ? currentlyFocused.data("editor") : null, template);
                if (enabled) {
                    layout.enable(this);
                } else {
                    layout.disable(this);
                }
            });
        },

        syncPreview: function() {
            // Empty the preview
            previewContainer.empty();

            // Get the XML document for the layout
            var previews = [];

            // Size the preview with the layout dimensions
            var dimensions = {
                width: layoutRootElement.attr("width") || 490,
                height: layoutRootElement.attr("height") || 390
            };
            layout._setDimensions(
                dimensions,
                previewContainer
                    .css("position", "absolute")
                    .data("previews", previews));

            // Create a preview for each XML layout element
            layoutRootElement
                .children()
                .each(function() {
                    layout.createElementPreview($(this), previews);
                });

            return layout;
        },

        toggleAll: function(expand) {
            if (typeof(expand) === "undefined") {
                expand = toggleLink.text() === "+";
            }
            toggleLink.text(expand ? "-" : "+")
                .attr("title", expand ? res.collapseAll : res.expandAll);
            $(".layout-editor-list fieldset>div").toggle(!!expand);
            editorsCollapsed = !expand;
            return layout;
        },

        zoomTo: function(percent) {
            zoomLevel.val(percent).change();
        }
    });
    // Layout, editor & XML tab switches
    var generalSwitch = options.generalSwitch
        .click(function(e) {
            e.preventDefault();
            generalEditor.show();
            editorContainer.parent().hide();
            generalSwitch.parent().addClass("selected");
            editorsSwitch.parent().removeClass("selected");
            xmlSwitch.parent().removeClass("selected");
        }),
        editorsSwitch = options.editorsSwitch
            .click(function(e) {
                e.preventDefault();
                generalEditor.hide();
                editorContainer.parent().show();
                generalSwitch.parent().removeClass("selected");
                editorsSwitch.parent().addClass("selected");
                xmlSwitch.parent().removeClass("selected");
            }),
        xmlSwitch = options.xmlSwitch
            .click(function(e) {
                e.preventDefault();
                generalEditor.hide();
                editorContainer.parent().hide();
                generalSwitch.parent().removeClass("selected");
                editorsSwitch.parent().removeClass("selected");
                xmlSwitch.parent().addClass("selected");
            });
    xmlSwitch.parent().parent().show();

    // Fix application path if necessary
    if (!/\/$/.test(applicationPath)) {
        applicationPath += '/';
    }

    // Build the XML view and document
    layout._syncXml();

    // Bind the general property fields
    widthField.on("change keyup", function() {
        layoutRootElement.attr("width", $(this).val());
        layout.syncLayoutXml();
    });
    heightField.on("change keyup", function() {
        layoutRootElement.attr("height", $(this).val());
        layout.syncLayoutXml();
    });
    classField.on("change keyup", function() {
        layoutRootElement.attr("class", $(this).val());
        layout.syncLayoutXml();
    });
    pageClassField.on("change keyup", function() {
        layoutRootElement.attr("pageclass", $(this).val());
        layout.syncLayoutXml();
    });
    stylesheetField.on("change", function() {
        stylesheet = $(this).val();
        layoutRootElement.attr("stylesheet", stylesheet);
        layout.syncLayoutXml();
    });

    // Create the ul of new element buttons
    var toolbar = options.toolbar
        .append($("<ul></ul>"))
        .find("ul"),
        // Setup the zoom slider
        zoomSlider = zoom
            .show()
            .find(".layout-preview-zoom-slider")
            .slider({
                max: 200,
                min: 10,
                value: 100,
                animate: true,
                range: "min",
                slide: function(e, sliderElement) {
                    $(sliderElement.handle)
                        .parents(".layout-preview-zoom")
                        .find(".layout-preview-zoom-value")
                        .val(sliderElement.value);
                    scale = sliderElement.value / 100;
                    layout.scalePreview();
                },
                stop: function() {
                    zoomLevel.change();
                }
            }),
        zoomLevel = zoom.find(".layout-preview-zoom-value")
            .on("change keyup", function() {
                var value = $(this).val();
                zoomSlider.slider({ value: value });
                scale = value / 100;
                layout
                    .scalePreview()
                    .syncPreview();
            });
    zoom.find(".layout-zoom-fit")
        .on("click", function(e) {
            e.preventDefault();
            layout.fitPreview();
            layout.sendPreviewToTopLeft();
        });
    zoom.find(".layout-zoom-100-percent")
        .on("click", function(e) {
            e.preventDefault();
            layout.zoomTo(100);
            layout.sendPreviewToTopLeft();
        });

    // Set-up preview draggability
    previewContainer.draggable({ scroll: false });

    layout.sendPreviewToTopLeft();
    // Show the editor container
    editorContainer
        .show()
        // Make the list sortable
        .nestedSortable({
            items: "li",
            handle: ">fieldset>legend",
            helper: "clone",
            cursor: "move",
            opacity: 1,
            placeholder: "layout-editor-placeholder",
            disableNesting: "no-nest",
            errorClass: "error",
            forcePlaceholderSize: true,
            tabSize: 24,
            scroll: true,
            tolerance: "pointer",
            toleranceElement: ">fieldset",
            revert: 50,
            start: function(event, ui) {
                dragged = ui.item.find(".layout-editor-element");
                //console.log("Starting to drag ", dragged);
            },
            isAllowed: function(parent) {
                return layout.canBeNested(parent ? parent.parent().parent().find(">.layout-editor-element") : null, dragged);
            },
            stop: function() {
                // Record the current scroll position in order to restore it after the editors are rebuilt
                var scrollPosition = editorContainer.scrollTop();
                //console.log("Dropped ", dragged, " into ", ui.item.parent().parent());
                layout
                    // Rebuild the XML document on drop from the stored elements on each editor
                    .rebuildXmlFromEditorList()
                    // Rebuild the editors themselves, as the document references have become invalid
                    .buildEditors()
                    // Rebuild the preview and the XML view
                    .syncLayoutXml();
                // Restore scroll position
                editorContainer.scrollTop(scrollPosition);
            },
        });

    // Rebuild the editor and preview if the XML changes
    sourceField.on("change keyup", function() {
        var xml = layout._serialize(layoutDocument);
        if (xml === sourceField.val()) return;
        layout
            ._syncXml()
            .buildEditors(sourceField, editorContainer, previewContainer)
            .syncPreview(editorContainer, previewContainer);
    });

    layout
        .buildToolbar()
        .buildEditors();

    return layout;
};
