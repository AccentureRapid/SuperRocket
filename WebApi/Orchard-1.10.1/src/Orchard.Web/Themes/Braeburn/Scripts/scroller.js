var scrollerInterval = 10;

$(document).ready(function () {
    setEnablementOfArrows($('.item-picker.scrolling'));

    $(window).on('resize', function () {
        setEnablementOfArrows($('.item-picker.scrolling'));
    });

    $(".itemCell-link").bind("tap", function () {
        document.location.href = $(this).data('url');
    });
    $(".itemCell-link").mousedown(function () {
        $(this).data('start-time', new Date().getTime());
    }).bind('mouseup', function () {
        var endTime = new Date().getTime();
        if (endTime - $(this).data('start-time') < 250) {
            document.location.href = $(this).data('url');
        }
    });

    $('.itemPickerWidget').each(function () {
        var scrollerFullWidth = 0;
        var itemHeight = 200;
        $(this).find('.itemCell.scrolling').each(function () {
            scrollerFullWidth += $(this).outerWidth(true);
            itemHeight = $(this).outerHeight(true);
        });
        $(this).data('scrollerFullWidth', scrollerFullWidth);
        $(this).find('.item-picker.scrolling').each(function () {
            if (itemHeight < 180) {
                itemHeight = 180;
            }
            $(this).css("max-height", itemHeight);
        });
    });

    $('.item-picker.scrolling').mousedown(function (event) {
        $(this)
            .data('down', true)
            .data('x', event.clientX)
            .data('scrollLeft', this.scrollLeft);
        return false;
    }).mouseup(function (event) {
        $(this).data('down', false);
    }).mouseleave(function (event) {
        $(this).data('down', false);
    }).mousemove(function (event) {
        if ($(this).data('down') === true) {
            this.scrollLeft = $(this).data('scrollLeft') + $(this).data('x') - event.clientX;
            setEnablementOfArrows($(this));
        }
    }).css({
        'overflow': 'hidden',
        'cursor': '-moz-grab'
    });

    $(".item-picker.scrolling").each(function () {
        var scroller = $(this)[0];
        scroller.addEventListener('touchstart', function (e) {
            e.preventDefault();
            var touch = e.touches[0];

            if (scroller.data('down') != true) {
                scroller
                    .data('down', true)
                    .data('x', touch.pageX)
                    .data('scrollLeft', scroller.scrollLeft());
            }
            return false;
        });

        scroller.addEventListener('touchend', function (e) {
            scroller.data('down', false);
        });

        scroller.addEventListener('touchcancel', function (e) {
            scroller.data('down', false);
        });

        scroller.addEventListener('touchmove', function (e) {
            e.preventDefault();
            var touch = e.touches[0];

            if (scroller.data('down') === true) {
                this.scrollLeft = scroller.data('scrollLeft') + scroller.data('x') - touch.pageX;
                setEnablementOfArrows(scroller);
            }
        });
    });
});

function setEnablementOfArrows(scroller) {
    if (scroller.scrollLeft() == 0) {
        scroller.siblings('.leftArrow.scrolling').addClass("disabled");
    }
    else {
        scroller.siblings('.leftArrow.scrolling').removeClass('disabled');
    }

    var widget = scroller.closest('.itemPickerWidget');

    if ((scroller.scrollLeft() + scroller.width()) >= (widget.data('scrollerFullWidth'))) {
        scroller.siblings('.rightArrow.scrolling').addClass("disabled");
    }
    else {
        scroller.siblings('.rightArrow.scrolling').removeClass('disabled');
    }
}

function scrollSlider(arrow, interval) {

    var widget = arrow.closest('.itemPickerWidget');
    var timeoutId = widget.data('timeoutId');
    if (timeoutId !== undefined && timeoutId != 0) {
        clearInterval(timeoutId);
        widget.data('timeoutId', 0);
    } else {
        var scroller = arrow.siblings('.item-picker.scrolling');
        timeoutId = setInterval(function () {
            scroller.scrollLeft(scroller.scrollLeft() + interval);
            setEnablementOfArrows(scroller);
        }, 10);
        widget.data('timeoutId', timeoutId);
        if (scroller.scrollLeft() == 0) {
            scroller.siblings('.leftArrow.scrolling').addClass("disabled");
        }
        else {
            scroller.siblings('.leftArrow.scrolling').removeClass('disabled');
        }
    }
}

function cancelScroll(arrow) {
    var widget = arrow.closest('.itemPickerWidget');
    clearInterval(widget.data('timeoutId'));
    widget.data('timeoutId', 0);
}

$('.rightArrow').mousedown(function () {
    scrollSlider($(this), scrollerInterval);
    setEnablementOfArrows($(this));
}).bind('mouseup mouseleave', function () {
    cancelScroll($(this));
});

$('.leftArrow').mousedown(function () {
    scrollSlider($(this), -scrollerInterval);
}).bind('mouseup mouseleave', function () {
    cancelScroll($(this));
});

$('.rightArrow').on({
    'touchstart': function () {
        scrollSlider($(this), scrollerInterval);
    }
}).bind('touchend touchcancel', function () {
    cancelScroll($(this));
});

$('.leftArrow').on({
    'touchstart': function () {
        scrollSlider($(this), scrollerInterval);
    }
}).bind('touchend touchcancel', function () {
    cancelScroll($(this));
});

