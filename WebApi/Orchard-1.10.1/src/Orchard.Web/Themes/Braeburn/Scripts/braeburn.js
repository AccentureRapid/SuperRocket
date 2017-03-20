$(function () {
    // TO TOP BUTTON
    $(window).scroll(function () {
        if ($(this).scrollTop() != 0) {
            $("#toTop").fadeIn()
        }
        else {
            $("#toTop").fadeOut()
        }
    });

    $("#toTop").click(function () {
        $("body,html").animate({ scrollTop: 0 }, 2e3)
    })

    // PAGINATION
    $('#pagination ul').removeClass('pager').addClass('pagination');

    // Validation Summary (borrowed from: http://stackoverflow.com/a/14651140/316989)
    $("span.field-validation-valid, span.field-validation-error").addClass('help-block');
    $("div.form-group").has("span.field-validation-error").addClass('has-error');
    $("div.validation-summary-errors").has("li:visible").addClass("alert alert-block alert-danger");

    // any validation summary items should be encapsulated by a class alert and alert-danger
    $('.validation-summary-errors').each(function () {
        $(this).addClass('alert');
        $(this).addClass('alert-danger');
    });

    // update validation fields on submission of form
    if ($.fn.validate !== undefined) {
        $('form').submit(function () {
            if ($(this).valid === 'function')
                if ($(this).valid()) {
                    $(this).find('div.control-group').each(function () {
                        if ($(this).find('span.field-validation-error').length == 0) {
                            $(this).removeClass('has-error');
                            $(this).addClass('has-success');
                        }
                    });
                }
                else {
                    $(this).find('div.control-group').each(function () {
                        if ($(this).find('span.field-validation-error').length > 0) {
                            $(this).removeClass('has-success');
                            $(this).addClass('has-error');
                        }
                    });
                    $('.validation-summary-errors').each(function () {
                        if ($(this).hasClass('alert-danger') == false) {
                            $(this).addClass('alert');
                            $(this).addClass('alert-danger');
                        }
                    });
                }
            $('.validation-summary-errors').each(function () {
                $(this).addClass('alert');
                $(this).addClass('alert-danger');
            });
            $('.validation-summary-valid').each(function () {
                $(this).removeClass('alert');
                $(this).removeClass('alert-danger');
            });
        });
    }

    // check each form-group for errors on ready
    $('form').each(function () {
        $(this).find('div.form-group').each(function () {
            if ($(this).find('span.field-validation-error').length > 0) {
                $(this).addClass('has-error');
            }
        });
    });
});


function slide(type, key) {
    $("#" + type + "-ask-" + key).width("0")
    $("#" + type + "-yes-" + key).width("28%");
    $("#" + type + "-no-" + key).width("72%");
    return false;
};

function slideCancel(type, key) {
    $("#" + type + "-ask-" + key).width("100%")
    $("#" + type + "-yes-" + key).width("0");
    $("#" + type + "-no-" + key).width("0");
    return false;
};

// FORM VALIDATION CLASSES
// Borrowed from: http://stackoverflow.com/a/19842032/316989
if ($.validator !== undefined) {
    $.validator.setDefaults({
        highlight: function (element) {
            $(element).closest(".form-group").addClass("has-error");
        },
        unhighlight: function (element) {
            $(element).closest(".form-group").removeClass("has-error");
        }
    });
}

$(document).ready(function () {
    var tallestCard = 200;
    var widestCard = 0;
    $('.vcard, .ccard').each(function () {
        if ($(this).height() > tallestCard) {
            tallestCard = $(this).height();
        }
        if ($(this).width() > widestCard) {
            widestCard = $(this).width();
        }
    });

    $('.vcard, .ccard').height(tallestCard);
    $('.vcard, .ccard').width(widestCard);

    $('.menu-checkout-chain li a').attr("href", "#");

    $('.widget-aside-second .dropdown.active').each(function () {
        if ($(this).find('.active').length > 0){
            $(this).addClass("open");
            $(this).children('.dropdown-bar').children('.expand-arrow').children('.fa-angle-down, .fa-angle-right').toggleClass('fa-angle-down fa-angle-right');
        }
    });

    $('.menu-main-menu .expand-arrow-companion a').css("pointer-events", "none");
    $('.menu-main-menu .expand-arrow-companion').addClass('dropdown-toggle');
    $('.menu-main-menu .expand-arrow-companion').css("cursor", "pointer");
    $('.menu-main-menu .expand-arrow-companion').attr("data-toggle", "dropdown");

    var nowTemp = new Date();
    var now = new Date(nowTemp.getFullYear(), nowTemp.getMonth(), nowTemp.getDate(), 0, 0, 0, 0);

    if (window['getCartSummaryUrl'] != undefined) {
        $.getJSON(getCartSummaryUrl, function (data) {
            var quantity = data.hasOwnProperty('CartQuantity') ? data.CartQuantity : "";
            var total = data.hasOwnProperty('CartTotal') ? data.CartTotal : "";

            $('.cart-button').html(quantity + '<i class="fa fa-shopping-cart fa-lg"></i>');
            $('.cart-money').html(total);
        });
    }

    if (window['getUserNameUrl'] != undefined) {
        $.getJSON(getUserNameUrl, function (data) {
            if (data.hasOwnProperty('Welcome') && data.hasOwnProperty("Logout")) {
                $('.login-widget').html(
                    '<i class="fa fa-user"></i>' +
                    '<span class="user-welcome"> ' + data.Welcome.Text + '</span>' +
                    '<span class="user-logout"><a href="' + logOffUrl + '">' + data.Logout.Text + '</a></span>'
                );
            }
            else {
                $('.login-widget').html('<a href="' + logOnUrl + '"><i class="fa fa-user"></i> ' + logInText + '</a>');
            }
        });
    }

    $('.datepicker').each(function () {
        
        var chosenDate = $(this).datepicker({
            onRender: function (date) {
                return '';
            }
        }).on('changeDate', function (ev) {
            chosenDate.hide();
        }).data('datepicker');
    });
});

function showEdit(editButton, removeSliderName, key) {
    var expireGroup = $(editButton).parents('.edit-remove-group').siblings('.expire');
    expireGroup.children(".expire-date").hide();
    expireGroup.children(".edit-expiration").show();
    slideCancel(removeSliderName, key);
    expireGroup.removeClass("has-error");
}

// Cancel editing a credit card
function cancelEdit(sliderName, key) {
    $("#expire-date-" + key).show();
    $("#edit-expiration-" + key).hide();
    slideCancel(sliderName, key);
    return false;
};

// Save editing a a credit card
// saveButton - the HTML save button that's clicked
// actionUrl - the URL of the action to perform, which should be Action(epiId, newMonth, newYear)
// sliderName - the name of the slider used for the button
function saveEdit(saveButton, actionUrl, sliderName) {
    var id = $(saveButton).data('edit-id');
    var expireGroup = $(saveButton).parents('.edit-remove-group').siblings('.expire');
    var expireDate = expireGroup.children(".expire-date");
    var editExpiration = expireGroup.children(".edit-expiration");

    var newMonth = $("#select-month-" + id).val();
    var newYear = $("#select-year-" + id).val();

    if (newMonth == "" || newYear == "") {
        $("#select-year-" + id).parents(".expire").addClass("has-error");
    }
    else {
        $.post(
            actionUrl,
            AddAntiForgeryToken({
                id: id,
                month: newMonth,
                year: newYear
            })).done(function () {
                expireDate.text(newMonth + "/" + newYear);
                expireDate.show();
                editExpiration.hide();
                slideCancel(sliderName, id);
            }).fail(function (data) {
                alert('Error updating expiration date.');
            });
    }

    return false;
}

// Removes the credit card
// removeButton - the HTML remove card button that's clicked
// actionUrl - the URL of the action to perform, which should be Action(epiId, billingOrganization, currency)
function removePaymentMethod(removeButton, actionUrl, billingOrganization, currency) {
    var deleteLink = $(removeButton);

    $.post(
        actionUrl,
        AddAntiForgeryToken({
            epiId: deleteLink.data('remove-id'),
            billingOrganization: billingOrganization,
            currency: currency
        })).done(function () {
            deleteLink.parents(".ccard").remove();
        }).fail(function (data) {
            alert('@T("Error removing payment method.").Text');
        });

    return false;
}

// Change the price for a product when the promotion choice changes.
// The dropdown options should have a data-price attribute for the price display associated with the choice.
function updateProductChoiceInformation(dropdown, itemNumber) {
    var choice = $(dropdown.options[dropdown.selectedIndex]);

    var priceDisplay = $('#' + itemNumber + '-price');
    priceDisplay.text(choice.data('price'));

    var narrativeDisplay = $('#' + itemNumber + '-narrative');
    narrativeDisplay.text(choice.data('narrative'));

    var savingsDisplay = $('#' + itemNumber + '-savings small');
    savingsDisplay.text(choice.data('savings-text'));
}

function showShippingMethodUpdateForm(orderNumber) {
    $('#shipping-options-' + orderNumber).hide();
    $('#shipping-options-' + orderNumber + '-form').show();
}

function hideShippingMethodUpdateForm(orderNumber) {
    $('#shipping-options-' + orderNumber).show();
    $('#shipping-options-' + orderNumber + '-form').hide();
}

function showOrderLineUpdateForm(orderNumber, lineNumber) {
    $('#quantity-' + orderNumber + '-' + lineNumber).hide();
    $('#quantity-' + orderNumber + '-' + lineNumber + '-form').show();
}

function hideOrderLineUpdateForm(orderNumber, lineNumber) {
    $('#quantity-' + orderNumber + '-' + lineNumber).show();
    $('#quantity-' + orderNumber + '-' + lineNumber + '-form').hide();
}

function checkQuantityChanged(orderNumber, lineNumber, oldQuantity) {
    var newQuantity = $('#quantity-' + orderNumber + '-' + lineNumber + '-update').find('.order-line-quantity').val();
    var hasChanged = newQuantity != oldQuantity;
    if (hasChanged) {
        showOrderReviewSpinner();
    }
    else {
        hideOrderLineUpdateForm(orderNumber, lineNumber);
    }
    return hasChanged;
}

function showOrderReviewSpinner() {
    document.getElementById("order-summary").className = "csspinner ringed";
}

// Authentication for POSTing via javascript
function AddAntiForgeryToken(data) {
    data.__RequestVerificationToken = $('input[name=__RequestVerificationToken]').val();
    return data;
};

+function ($) {
    "use strict";

    // DROPDOWN CLASS DEFINITION
    // =========================

    var backdrop = '.dropdown-backdrop'
    var toggle = '[data-toggle=dropdown]'
    var Dropdown = function (element) {
        $(element).on('click.bs.dropdown', this.toggle)
    }

    Dropdown.prototype.toggle = function (e) {
        var $this = $(this)

        if ($this.is('.disabled, :disabled')) return

        var $parent = getParent(getParent($this))
        var isActive = $parent.hasClass('open')

        closeThisMenu($this)

        if (!isActive) {
            if ('ontouchstart' in document.documentElement && !$parent.closest('.navbar-nav').length) {
                //if mobile we use a backdrop because click events don't delegate
                $('<div class="dropdown-backdrop"/>').insertAfter($(this)).on('click', clearMenus)
            }

            $parent.trigger(e = $.Event('show.bs.dropdown'))

            if (e.isDefaultPrevented()) return

            $parent
              .toggleClass('open')
              .trigger('shown.bs.dropdown')

            $this.focus()
        }

        return false
    }

    Dropdown.prototype.keydown = function (e) {
        if (!/(38|40|27)/.test(e.keyCode)) return

        var $this = $(this)

        e.preventDefault()
        e.stopPropagation()

        if ($this.is('.disabled, :disabled')) return

        var $parent = getParent(getParent($this))
        var isActive = $parent.hasClass('open')

        if (!isActive || (isActive && e.keyCode == 27)) {
            if (e.which == 27) $parent.find(toggle).focus()
            return $this.click()
        }

        var $items = $('[role=menu] li:not(.divider):visible a', $parent)

        if (!$items.length) return

        var index = $items.index($items.filter(':focus'))

        if (e.keyCode == 38 && index > 0) index--                        // up
        if (e.keyCode == 40 && index < $items.length - 1) index++                        // down
        if (!~index) index = 0

        $items.eq(index).focus()
    }

    function clearMenus() {
        $(backdrop).remove()
        $(toggle).each(function (e) {
            var $parent = getParent($(this))
            if (!$parent.hasClass('open')) return
            $parent.trigger(e = $.Event('hide.bs.dropdown'))
            if (e.isDefaultPrevented()) return
            $parent.removeClass('open').trigger('hidden.bs.dropdown')
        })
    }

    function closeThisMenu(menuId) {
        $(backdrop).remove()

        var $parent = getParent(menuId)
        $parent.children('.expand-arrow').children('.fa-angle-down, .fa-angle-right').toggleClass('fa-angle-down fa-angle-right');
        $parent = getParent($parent);
        $parent.removeClass('open').trigger('hidden.bs.dropdown')
    }

    function getParent($this) {
        var selector = $this.attr('data-target')

        if (!selector) {
            selector = $this.attr('href')
            selector = selector && /#/.test(selector) && selector.replace(/.*(?=#[^\s]*$)/, '') //strip for ie7
        }

        var $parent = selector && $(selector)

        return $parent && $parent.length ? $parent : $this.parent()
    }


    // DROPDOWN PLUGIN DEFINITION
    // ==========================

    var old = $.fn.dropdown

    $.fn.dropdown = function (option) {
        return this.each(function () {
            var $this = $(this)
            var data = $this.data('bs.dropdown')

            if (!data) $this.data('bs.dropdown', (data = new Dropdown(this)))
            if (typeof option == 'string') data[option].call($this)
        })
    }

    $.fn.dropdown.Constructor = Dropdown


    // DROPDOWN NO CONFLICT
    // ====================

    $.fn.dropdown.noConflict = function () {
        $.fn.dropdown = old
        return this
    }


    // APPLY TO STANDARD DROPDOWN ELEMENTS
    // ===================================

    $(document)
      .on('click.bs.dropdown.data-api', '.dropdown form', function (e) { e.stopPropagation() })
      .on('click.bs.dropdown.data-api', toggle, Dropdown.prototype.toggle)
      .on('keydown.bs.dropdown.data-api', toggle + ', [role=menu]', Dropdown.prototype.keydown)
}(jQuery);