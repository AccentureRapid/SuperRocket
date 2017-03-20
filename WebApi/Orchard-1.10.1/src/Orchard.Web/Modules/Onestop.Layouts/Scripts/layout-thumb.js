$(window)
    .load(function() {
        var applyCssScale = function(element, scale, translate) {
            var browserPrefixes = ["", "-ms-", "-moz-", "-webkit-", "-o-"],
                offset = ((1 - scale) * 50) + "%",
                scaleString = (translate !== false ? "translate(-" + offset + ", -" + offset + ") " : "") + "scale(" + scale + "," + scale + ")";
            $(browserPrefixes).each(function() {
                element
                    .css(this + "transform", scaleString);
            });
            element
                .data({ scale: scale })
                .addClass("scaled");
        };

        $(".templated-item.summary-admin, .templated-item-editor, .slide-show-slides.summary-admin, .templated-item-preview")
            .css("display", "block")
            .each(function() {
                var slideshow = $(this),
                    slide = slideshow.find(".templated-item"),
                    parent = slide.parent(),
                    width = +slideshow.data("width") || slide.width() || slide.find("img").width(),
                    height = +slideshow.data("height") || slide.height() || slide.find("img").height(),
                    boundingDimension = slideshow.data("size") || 150,
                    slideStyle = slide.attr("style");
                if (!slideshow.data("width") && (slideStyle != null && slideStyle.indexOf("width:") == -1)) width = 1024;
                if (!slideshow.data("height") && (slideStyle != null && slideStyle.indexOf("height:") == -1)) height = 768;
                slide
                    .css({
                        width: width + "px",
                        height: height + "px",
                        position: "absolute"
                    });
                var scaledForWidth = width > height,
                    largestDimension = (scaledForWidth ? width : height),
                    scale = boundingDimension / largestDimension;
                parent.css({
                    width: Math.floor(width * scale) + "px",
                    height: Math.floor(height * scale) + "px",
                    position: "relative",
                    overflow: "hidden"
                });
                applyCssScale(slide, scale);
                slideshow.parent(".primary").css("overflow", "visible");
                slide.closest(".slide-row").height(height * scale);
                slide.parents().show();
            });
    });